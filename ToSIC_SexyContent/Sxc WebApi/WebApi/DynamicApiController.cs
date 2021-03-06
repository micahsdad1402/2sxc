﻿using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using Factory = ToSic.Eav.Factory;
using ToSic.Sxc.Adam.WebApi;
using System.IO;
using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.WebApi
{
    /// <inheritdoc cref="SxcApiControllerBase" />
    /// <summary>
    /// This is the foundation for both the old SxcApiController and the new Dnn.ApiController.
    /// incl. the current App, DNN, Data, etc.
    /// For others, please use the SxiApiControllerBase, which doesn't have all that, and is usually then
    /// safer because it can't accidentally mix the App with a different appId in the params
    /// </summary>
    [PrivateApi]
    [SxcWebApiExceptionHandling]
    public abstract class DynamicApiController : SxcApiControllerBase, ICreateInstance, IHasDynCodeContext
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Log.Rename("Api.DynApi");
            Log.Add($"HasBlock: {BlockBuilder != null}");
            // Note that the CmsBlock is created by the BaseClass, if it's detectable. Otherwise it's null
            // if it's null, use the log of this object
            DynCode = DnnDynamicCode.Create(BlockBuilder, BlockBuilder?.Log ?? Log);

            // In case SxcBlock was null, there is no instance, but we may still need the app
            if (DynCode.App == null)
            {
                Log.Add("DynCode.App is null");
                TryToAttachAppFromUrlParams();
            }

            // must run this after creating AppAndDataHelpers
            controllerContext.Request.Properties.Add(Constants.DnnContextKey, Dnn); 

            if(controllerContext.Request.Properties.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var value))
                CreateInstancePath = value as string;
        }

        [PrivateApi]
        public DnnDynamicCode DynCode { get; private set; }

        public IDnnContext Dnn => DynCode.Dnn;

        private void TryToAttachAppFromUrlParams()
        {
            var wrapLog = Log.Call();
            var found = false;
            try
            {
                var routeAppPath = Route.AppPathOrNull(Request.GetRouteData());
                var appId = AppFinder.GetCurrentAppIdFromPath(routeAppPath).AppId;
                // Look up if page publishing is enabled - if module context is not available, always false
                var publish = Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
                var publishingEnabled = Dnn.Module != null && publish.IsEnabled(Dnn.Module.ModuleID);
                Log.Add($"AppId: {appId}, publishing:{publishingEnabled}");
                var app = Sxc.Dnn.Factory.App(appId, publishingEnabled, parentLog: Log);
                DynCode.LateAttachApp(app);
                found = true;
            } catch { /* ignore */ }

            wrapLog(found.ToString());
        }


        #region Adam - Shared Code Across the APIs (prevent duplicate code)

        /// <summary>
        /// See docs of official interface <see cref="IDynamicWebApi"/>
        /// </summary>
        public Sxc.Adam.IFile SaveInAdam(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, 
            Stream stream = null, 
            string fileName = null, 
            string contentType = null, 
            Guid? guid = null, 
            string field = null,
            string subFolder = "")
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "SaveInAdam", 
                $"{nameof(stream)},{nameof(fileName)},{nameof(contentType)},{nameof(guid)},{nameof(field)},{nameof(subFolder)} (optional)");

            if(stream == null || fileName == null || contentType == null || guid == null || field == null)
                throw new Exception();

            var feats = new[]{FeatureIds.UseAdamInWebApi, FeatureIds.PublicUpload};
            if (!Features.EnabledOrException(feats, "can't save in ADAM", out var exp))
                throw exp;

            return new AdamUploader(BlockBuilder, 
                BlockBuilder.Block.AppId, // 2019-11-09 not nullable any more ?? throw new Exception("can't save in adam - full context not available"), 
                Log)
                .UploadOne(stream, fileName, contentType, guid.Value, field, subFolder, false, true);
        }

        #endregion

        public string CreateInstancePath { get; set; }

        public dynamic CreateInstance(string virtualPath, 
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null, 
            string relativePath = null, 
            bool throwOnError = true) =>
            DynCode.CreateInstance(virtualPath, dontRelyOnParameterOrder, name,
                CreateInstancePath, throwOnError);
    }
}
