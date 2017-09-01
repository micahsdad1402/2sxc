﻿using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using ToSic.SexyContent.Installer;
using ToSic.SexyContent.Search;
using ToSic.Eav.Apps.Interfaces;
using ToSic.SexyContent.ContentBlocks;
using DotNetNuke.Common.Utilities;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using System.IO;
using ToSic.Eav.Interfaces;
using ToSic.SexyContent.EAVExtensions;
using System.Linq;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnBusinessController : ModuleSearchBase, IUpgradeable, IVersionable
    {
        #region DNN Interface Members - search, upgrade, versionable

        private IEnvironmentVersioning versioning;

        /// <summary>
        /// Constructor overload for DotNetNuke
        /// (BusinessController needs a parameterless constructor)
        /// </summary>
        public DnnBusinessController()
        {
            versioning = new Versioning();
        }

        public int GetLatestVersion(int moduleId)
        {
            return versioning.GetLatestVersion(moduleId);
        }

        public int GetPublishedVersion(int moduleId)
        {
            return versioning.GetPublishedVersion(moduleId);
        }

        public void PublishVersion(int moduleId, int version)
        {
            versioning.DoInsidePublishLatestVersion(moduleId, (args) => {
                
                // publish all entites of this content block
                var moduleInfo = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
                var cb = new ModuleContentBlock(moduleInfo);
                var appManager = new AppManager(cb.AppId);
                var list = cb.Data["Default"]?.LightList;

                var pres = cb.Data["Presentation"]?.LightList;
                if (pres != null) list = list.Concat(pres);
                var cont = cb.Data["ListContent"]?.LightList;
                if (cont != null) list = list.Concat(cont);
                var lPres = cb.Data["ListPresentation"]?.LightList;
                if (lPres != null) list = list.Concat(lPres);

                var ids = list.Where(e => !e.IsPublished).Select(e => e.EntityId).ToArray();

                // publish ContentGroup as well
                ids.ToList<int>().Add(cb.ContentGroup.ContentGroupId);

                appManager.Entities.Publish(ids);
            });
        }

        public void DeleteVersion(int moduleId, int version)
        {
            versioning.DoInsideDeleteLatestVersion(moduleId, (args) => {
                // NOTE for 2dm: If we want to support delete, reset any item in draft state of the content-block
            });
        }

        public int RollBackVersion(int moduleId, int version)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This is part of the IUpgradeable of DNN
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public string UpgradeModule(string version)
        {
            return new InstallationController().UpgradeModule(version);
        }

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            try
            {
                return new SearchController().GetModifiedSearchDocuments(moduleInfo, beginDate);
            }
            catch (Exception e)
            {
                throw new SearchIndexException(moduleInfo, e);
            }
        }

        #endregion
    }
}