﻿using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.ContentBlocks
{
    internal sealed class ModuleContentBlock: ContentBlockBase
    {
        public IInstanceInfo InstanceInfo;

        public override ContentGroupReferenceManagerBase Manager => new ModuleContentGroupReferenceManager(SxcInstance);

        public override bool ParentIsEntity => false;


        public override ViewDataSource Data => _dataSource 
            ?? (_dataSource = ViewDataSource.ForContentGroupInSxc(SxcInstance, Template, Configuration, Log, InstanceInfo.Id));

        private readonly IEnumerable<KeyValuePair<string, string>> _urlParams;

        /// <summary>
        /// Create a module-content block
        /// </summary>
        /// <param name="instanceInfo">the dnn module-info</param>
        /// <param name="parentLog">a parent-log; can be null but where possible you should wire one up</param>
        /// <param name="tennant"></param>
        /// <param name="overrideParams">optional override parameters</param>
        public ModuleContentBlock(IInstanceInfo instanceInfo, Log parentLog, ITennant tennant = null, IEnumerable<KeyValuePair<string, string>> overrideParams = null): base(parentLog, "CB.Mod")
        {
            InstanceInfo = instanceInfo ?? throw new Exception("Need valid Instance/ModuleInfo / ModuleConfiguration of runtime");
            ParentId = instanceInfo.Id;
            ContentBlockId = ParentId;

            // url-params
            _urlParams = overrideParams ?? DnnWebForms.Helpers.SystemWeb.GetUrlParams();

            // Ensure we know what portal the stuff is coming from
            // PortalSettings is null, when in search mode
            Tennant = tennant;
            //new DnnTennant(PortalSettings.Current == null || _dnnModule.OwnerPortalID != _dnnModule.PortalID
            //    ? new PortalSettings(_dnnModule.OwnerPortalID)
            //    : PortalSettings.Current);


            // important: don't use the SxcInstance.Environment, as it would try to init the Sxc-object before the app is known, causing various side-effects
            var tempEnv = Factory.Resolve<IEnvironmentFactory>().Environment(parentLog);
            ZoneId = tempEnv.ZoneMapper.GetZoneId(tennant.Id); // use tennant as reference, as it can be different from instance.TennantId
            
            AppId = Factory.Resolve<IMapAppToInstance>().GetAppIdFromInstance(instanceInfo, ZoneId) ?? 0;// fallback/undefined YET

            Log.Add($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

            if (AppId == Settings.DataIsMissingInDb)
            {
                _dataIsMissing = true;
                Log.Add("data is missing, will stop here");
                return;
            }

            if (AppId != 0)
            {
                Log.Add("real app, will load data");
                // try to load the app - if possible
                App = new App(ZoneId, AppId, /*PortalSettings*/ Tennant, parentLog: Log);

                Configuration = ConfigurationProvider.GetConfigProviderForModule(InstanceInfo.Id, App, SxcInstance);

                // maybe ensure that App.Data is ready
                var userMayEdit = Factory.Resolve<IPermissions>().UserMayEditContent(SxcInstance.InstanceInfo);
                App.InitData(userMayEdit,// SxcInstance.Environment.Permissions.UserMayEditContent,
                    SxcInstance.Environment.PagePublishing.IsEnabled(InstanceInfo.Id), 
                    Configuration);

                var res = App.ContentGroupManager.GetInstanceContentGroup(instanceInfo.Id, instanceInfo.PageId);
                var contentGroupGuid = res.Item1;
                var previewTemplateGuid = res.Item2;
                ContentGroup = App.ContentGroupManager.GetContentGroupOrGeneratePreview(contentGroupGuid, previewTemplateGuid);

                if (ContentGroup.DataIsMissing)
                {
                    _dataIsMissing = true;
                    App = null;
                    return;
                }

                SxcInstance.SetTemplateOrOverrideFromUrl(ContentGroup.Template);                
            }
        }


        public override SxcInstance SxcInstance
            => _sxcInstance ?? (_sxcInstance = new SxcInstance(this, InstanceInfo, _urlParams, parentLog: Log));

        public override bool IsContentApp => InstanceInfo.IsPrimary;

    }
}