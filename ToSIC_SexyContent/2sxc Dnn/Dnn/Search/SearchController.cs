﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Search.Entities;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Search
{
    internal class SearchController: HasLog
    {
        public SearchController(ILog parentLog = null) : base("DNN.Search", parentLog) { }

        /// <summary>
        /// Get search info for each dnn module containing 2sxc data
        /// </summary>
        /// <returns></returns>
        public IList<SearchDocument> GetModifiedSearchDocuments(IContainer container, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();
            var dnnModule = (container as Container<ModuleInfo>)?.UnwrappedContents;
            // always log with method, to ensure errors are caught
            Log.Add($"start search for mod#{dnnModule?.ModuleID}");

            History.Add("dnn-search", Log);

            if (dnnModule == null) return searchDocuments;

            var isContentModule = dnnModule.DesktopModule.ModuleName == "2sxc";

            // New Context because PortalSettings.Current is null
            var zoneId = new DnnEnvironment(Log).ZoneMapper.GetZoneId(dnnModule.OwnerPortalID);

            var appId = !isContentModule
                ? new DnnMapAppToInstance(Log).GetAppIdFromInstance(container, zoneId)
                : new ZoneRuntime(zoneId, Log).DefaultAppId;

            if (!appId.HasValue)
                return searchDocuments;

            // As PortalSettings.Current is null, instantiate with modules' portal id
            var portalSettings = new PortalSettings(dnnModule.OwnerPortalID);

            // Ensure cache builds up with correct primary language
            var cache = Eav.Factory.GetAppsCache();
            cache.Load(new AppIdentity(zoneId, appId.Value) , portalSettings.DefaultLanguage.ToLower());
            //var cache = Eav.Factory.Resolve<IRootCache>();
            //((RootCacheBase)cache).ZoneId = zoneId;
            //((RootCacheBase)cache).AppId = appId.Value;
            //cache.PreLoadCache(portalSettings.DefaultLanguage.ToLower());

            // must find tenant through module, as the PortalSettings.Current is null in search mode
            var tenant = new DnnTenant(portalSettings);
            var mcb = new BlockFromModule(container, Log, tenant);
            var cmsBlock = mcb.CmsInstance;

            var language = dnnModule.CultureCode;

            //var cms = new CmsRuntime(cmsBlock.App, Log, false, false);
            //var contentGroup = cms.Blocks.GetInstanceContentGroup(dnnModule.ModuleID, dnnModule.TabID);

            var view = cmsBlock.View;// contentGroup.View;

            if (view == null)
                return searchDocuments;

            // This list will hold all EAV entities to be indexed
            var dataSource = cmsBlock.Block.Data;
			
            var engine = EngineFactory.CreateEngine(view);
            engine.Init(cmsBlock, Purpose.IndexingForSearch, Log);

            // see if data customization inside the cshtml works
            try
            {
                engine.CustomizeData();
            }
            catch (Exception e) // Catch errors here, because of references to Request etc.
            {
                Exceptions.LogException(new SearchIndexException(dnnModule, e));
            }

            
            var searchInfoDictionary = new Dictionary<string, List<ISearchItem>>();

            // Get DNN SearchDocuments from 2Sexy SearchInfos
            foreach (var stream in dataSource.Out.Where(p => p.Key != ViewParts.Presentation && p.Key != ViewParts.ListPresentation))
            {
                
                var entities = stream.Value.List;
                var searchInfoList = searchInfoDictionary[stream.Key] = new List<ISearchItem>();

                searchInfoList.AddRange(entities.Select(entity =>
                {
                    var searchInfo = new SearchItem
                    {
                        Entity = entity,
                        Url = "",
                        Description = "",
                        Body = GetJoinedAttributes(entity, language),
                        Title = entity.Title?[language]?.ToString() ?? "(no title)",
                        ModifiedTimeUtc = (entity.Modified == DateTime.MinValue ? DateTime.Now.Date.AddHours(DateTime.Now.Hour) : entity.Modified).ToUniversalTime(),
                        UniqueKey = "2sxc-" + dnnModule.ModuleID + "-" + (entity.EntityGuid != new Guid() ? entity.EntityGuid.ToString() : (stream.Key + "-" + entity.EntityId)),
                        IsActive = true,
                        TabId = dnnModule.TabID,
                        PortalId = dnnModule.PortalID
                    };

                    // Take the newest value (from ContentGroupItem and Entity)
                    if (entity is IHasEditingData typed)
                    {
                        var contentGroupItemModifiedUtc = typed.ContentGroupItemModified.ToUniversalTime();
                        searchInfo.ModifiedTimeUtc = searchInfo.ModifiedTimeUtc > contentGroupItemModifiedUtc
                            ? searchInfo.ModifiedTimeUtc
                            : contentGroupItemModifiedUtc;
                    }

                    return searchInfo;
                }));
            }

            // check if the cshtml has search customizations
            try
            {
                engine.CustomizeSearch(searchInfoDictionary, new DnnContainer(dnnModule), beginDate);
            }
            catch (Exception e)
            {
                Exceptions.LogException(new SearchIndexException(dnnModule, e));
            }

            // reduce load by only keeping recently modified ites
            foreach (var searchInfoList in searchInfoDictionary)
            {
                // Filter by Date - take only SearchDocuments that changed since beginDate
                var searchDocumentsToAdd = searchInfoList.Value.Where(p => p.ModifiedTimeUtc >= beginDate.ToUniversalTime()).Select(p => (SearchDocument) p);

                searchDocuments.AddRange(searchDocumentsToAdd);
            }

            return searchDocuments;
        }

        private string StripHtmlAndHtmlDecode(string text) => HttpUtility.HtmlDecode(Regex.Replace(text, "<.*?>", string.Empty));

        /// <summary>
        /// Gets a string that represents all entities joined with a comma , separator
        /// Does just include String and Number fields
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private string GetJoinedAttributes(IEntity entity, string language)
        {
            return string.Join(", ",
                entity.Attributes.Where(x => x.Value.Type == "String" || x.Value.Type == "Number").Select(x => x.Value[language])
                    .Where(a => a != null)
                    .Select(a => StripHtmlAndHtmlDecode(a.ToString()))
                    .Where(x => !String.IsNullOrEmpty(x))) + " ";
        }
    }
}