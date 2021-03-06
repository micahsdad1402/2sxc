﻿using System;
using System.IO;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnMapAppToInstance : HasLog, IMapAppToInstance
    {
        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public DnnMapAppToInstance() : base("Dnn.MapA2I") { }

        public DnnMapAppToInstance(ILog parentLog) : base("Dnn.MapA2I", parentLog) { }

        public int? GetAppIdFromInstance(IContainer instance, int zoneId)
        {
            var wrapLog = Log.Call<int?>(parameters: $"..., {zoneId}");

            var module = (instance as Container<ModuleInfo>)?.UnwrappedContents
                ?? throw new Exception("instance is not of type ModuleInfo");

            var msg = $"get appid from instance for Z:{zoneId} Mod:{module.ModuleID}";
            if (module.DesktopModule.ModuleName == "2sxc")
            {
                var appId = new ZoneRuntime(zoneId, null).DefaultAppId;
                Log.Add($"{msg} - use def app: {appId}");
                return wrapLog("default", appId);
            }

            if (module.ModuleSettings.ContainsKey(Settings.AppNameString))
            {
                var guid = module.ModuleSettings[Settings.AppNameString].ToString();
                var appId = AppHelpers.GetAppIdFromGuidName(zoneId, guid);
                Log.Add($"{msg} AppG:{guid} = app:{appId}");
                return wrapLog("ok", appId);
            }

            Log.Add($"{msg} not found = null");
            return wrapLog("not found", null);
        }


        
        public void SetAppIdForInstance(IContainer instance, IAppEnvironment env, int? appId, ILog parentLog)
        {
            Log.Add($"SetAppIdForInstance({instance.Id}, -, appid: {appId})");
            // Reset temporary template
            /*BlocksManager.*/ClearPreviewTemplate(instance.Id);

            // ToDo: Should throw exception if a real BlockConfiguration exists

            var module = (instance as Container<ModuleInfo>).UnwrappedContents;
            var zoneId = env.ZoneMapper.GetZoneId(module.OwnerPortalID);

            if (appId == 0 || !appId.HasValue)
                DnnTenantSettings.UpdateInstanceSettingForAllLanguages(instance.Id, Settings.AppNameString, null, Log);
            else
            {
                var appName = /*Factory.GetAppsCache*/Eav.Apps.State.Zones[zoneId].Apps[appId.Value];
                DnnTenantSettings.UpdateInstanceSettingForAllLanguages(instance.Id, Settings.AppNameString, appName, Log);
            }

            // Change to 1. available template if app has been set
            if (appId.HasValue)
            {
                var appIdentity = new AppIdentity(zoneId, appId.Value);
                var cms = new CmsRuntime(/*zoneId, appId.Value*/appIdentity, Log, true, env.PagePublishing.IsEnabled(instance.Id));
                var templateGuid = cms.Views.GetAll().FirstOrDefault(t => !t.IsHidden)?.Guid;
                if (templateGuid.HasValue)
                    /*BlocksManager.*/SetPreviewTemplate(instance.Id, templateGuid.Value);
            }
        }


        public void ClearPreviewTemplate(int instanceId)
        {
            Log.Add($"ClearPreviewTemplate(iid: {instanceId})");
            DnnTenantSettings.UpdateInstanceSettingForAllLanguages(instanceId, Settings.PreviewTemplateIdString, null, Log);
        }

        public void SetContentGroup(int instanceId, bool wasCreated, Guid guid)
        {
            Log.Add($"SetContentGroup(iid: {instanceId}, wasCreated: {wasCreated}, guid: {guid})");
            // Remove the previewTemplateId (because it's not needed as soon Content is inserted)
            ClearPreviewTemplate(instanceId);
            // Update blockConfiguration Guid for this module
            if (wasCreated)
                DnnTenantSettings.UpdateInstanceSettingForAllLanguages(instanceId, Settings.ContentGroupGuidString,
                    guid.ToString(), Log);
        }

        public BlockConfiguration GetInstanceContentGroup(BlocksRuntime cgm, ILog log, int instanceId, int? pageId)
        {
            var wrapLog = log.Call<BlockConfiguration>($"find content-group for mid#{instanceId} and page#{pageId}");
            var mci = ModuleController.Instance;

            var tabId = pageId ?? mci.GetTabModulesByModule(instanceId)[0].TabID;

            var settings = mci.GetModule(instanceId, tabId, false).ModuleSettings;

            var maybeGuid = settings[Settings.ContentGroupGuidString];
            Guid.TryParse(maybeGuid?.ToString(), out var groupGuid);
            var previewTemplateString = settings[Settings.PreviewTemplateIdString]?.ToString();

            var templateGuid = !string.IsNullOrEmpty(previewTemplateString)
                ? Guid.Parse(previewTemplateString)
                : new Guid();

            var found = cgm.GetContentGroupOrGeneratePreview(groupGuid, templateGuid);
            return wrapLog("ok", found);
        }

        /// <summary>
        /// Saves a temporary templateId to the module's settings
        /// This templateId will be used until a contentgroup exists
        /// </summary>
        public void SetPreviewTemplate(int instanceId, Guid previewTemplateGuid)
        {
            // todo: 2rm - I believe you are accidentally using uncached module settings access - pls check and probably change
            // todo: note: this is done ca. 3x in this class
            var moduleController = new ModuleController();
            var settings = moduleController.GetModule(instanceId).ModuleSettings;// older, deprecated api: .GetModuleSettings(instanceId);

            // Do not allow saving the temporary template id if a contentgroup exists for this module
            if (settings[Settings.ContentGroupGuidString] != null)
                throw new Exception("Preview template id cannot be set for a module that already has content.");

            DnnTenantSettings.UpdateInstanceSettingForAllLanguages(instanceId, Settings.PreviewTemplateIdString, previewTemplateGuid.ToString(), Log);
        }

        public void UpdateTitle(Sxc.Blocks.IBlockBuilder blockBuilder, IEntity titleItem)
        {
            Log.Add("update title");

            var languages = blockBuilder.Environment.ZoneMapper.CulturesWithState(blockBuilder.Container.TenantId,
                blockBuilder.Block.ZoneId); // not nullable any more 2019-11-09 // .Value);

            // Find Module for default language
            var moduleController = new ModuleController();
            var originalModule = moduleController.GetModule(blockBuilder.Container.Id);

            foreach (var dimension in languages)
            {
                if (!originalModule.IsDefaultLanguage)
                    originalModule = originalModule.DefaultLanguageModule;

                try // this can sometimes fail, like if the first item is null - https://github.com/2sic/2sxc/issues/817
                {
                    // Break if default language module is null
                    if (originalModule == null)
                        return;

                    // Get Title value of Entitiy in current language
                    var titleValue = titleItem.Title[dimension.Key].ToString();

                    // Find module for given Culture
                    var moduleByCulture = moduleController.GetModuleByCulture(originalModule.ModuleID,
                        originalModule.TabID, blockBuilder.Container.TenantId,
                        DotNetNuke.Services.Localization.LocaleController.Instance.GetLocale(dimension.Key));

                    // Break if no title module found
                    if (moduleByCulture == null || titleValue == null)
                        return;

                    moduleByCulture.ModuleTitle = titleValue;
                    moduleController.UpdateModule(moduleByCulture);
                }
                catch
                {
                    // ignored
                }
            }
        }

        // todo: remove this, replace with calls to the current tenant -> RootPath
        public static string AppBasePath() 
            => Path.Combine(PortalSettings.Current.HomeDirectory, Settings.AppsRootFolder);

    }
}