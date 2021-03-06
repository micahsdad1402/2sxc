﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.WebApi;

namespace ToSic.Sxc.WebApi.Cms
{
	/// <summary>
	/// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [SxcWebApiExceptionHandling]
    [ValidateAntiForgeryToken]
	public class PipelineDesignerController : DnnApiControllerWithFixes, IPipelineDesignerController
    {
		private QueryController _eavCont;

	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
	        Log.Rename("Sxc.QryCnt");
			_eavCont = new QueryController(Log);
	    }

        /// <summary>
        /// Get a Pipeline with DataSources
        /// </summary>
        [HttpGet]
		public QueryDefinitionInfo GetPipeline(int appId, int? id = null) => _eavCont.GetPipeline(appId, id);

	    /// <summary>
		/// Get installed DataSources from .NET Runtime but only those with [PipelineDesigner Attribute]
		/// </summary>
		[HttpGet]
		public IEnumerable<QueryRuntime.DataSourceInfo> GetInstalledDataSources() => QueryController.GetInstalledDataSources();

	    /// <summary>
	    /// Save Pipeline
	    /// </summary>
	    /// <param name="data">JSON object { pipeline: pipeline, dataSources: dataSources }</param>
	    /// <param name="appId">AppId this Pipeline belogs to</param>
	    /// <param name="id">PipelineEntityId</param>
	    [HttpPost]
	    public QueryDefinitionInfo SavePipeline([FromBody] QueryDefinitionInfo data, int appId, int id)
	        => _eavCont.SavePipeline(data, appId, id);


	    /// <summary>
	    /// Query the Result of a Pipeline using Test-Parameters
	    /// </summary>
	    [HttpGet]
	    public dynamic QueryPipeline(int appId, int id)
	    {
	        var modId = ActiveModule?.ModuleID ?? 0;
	        var wrapLog = Log.Call($"app:{appId}, id:{id}", message: $"mid:{modId}");
	        var dnnConfigProvider = new GetDnnEngine().GetEngine(modId, Log);
            var result = _eavCont.QueryPipeline(appId, id, dnnConfigProvider);
            wrapLog(null);
            return result;
        }

        /// <summary>
	    /// Clone a Pipeline with all DataSources and their configurations
	    /// </summary>
	    [HttpGet]
	    public void ClonePipeline(int appId, int id) => _eavCont.ClonePipeline(appId, id);
	

		/// <summary>
		/// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations. Stops if the if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
		/// </summary>
		[HttpGet]
		public object DeletePipeline(int appId, int id)
		{
            Log.Add($"delete pipe:{id} on app:{appId}");
			// Stop if a Template uses this Pipeline
            //var app = GetApp.LightWithoutData(new DnnTenant(PortalSettings.Current), appId, Log);
            var cms = new CmsRuntime(appId, Log, true );
			var templatesUsingPipeline = cms.Views.GetAll()
                .Where(t => t.Query?.Id == id)
                .Select(t => t.Id)
                .ToArray();
			if (templatesUsingPipeline.Any())
				throw new Exception(
				        $"Pipeline is used by Views and cant be deleted. Pipeline EntityId: {id}. TemplateIds: {string.Join(", ", templatesUsingPipeline)}");

			return _eavCont.DeletePipeline(appId, id);
		}

	    [HttpPost]
	    public bool ImportQuery(EntityImport args) => _eavCont.ImportQuery(args);
	}
}