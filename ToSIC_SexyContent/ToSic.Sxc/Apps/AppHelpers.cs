﻿using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;

namespace ToSic.Sxc.Apps
{
    internal class AppHelpers
    {
        internal static int GetAppIdFromGuidName(int zoneId, string appName, bool alsoCheckFolderName = false)
        {
            // var baseCache = (RootCacheBase) DataSource.GetCache(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId);
            var baseCache = Factory.GetAppsCache();
            // ReSharper disable once UnusedVariable
            //var dummy = baseCache.CacheTimestamp;

            if (string.IsNullOrEmpty(appName))
                return 0; 

            var appId = baseCache.Zones[zoneId].Apps
                    .Where(p => p.Value == appName).Select(p => p.Key).FirstOrDefault();

            // optionally check folder names
            if (appId == 0 && alsoCheckFolderName)
            {
                var nameLower = appName.ToLower();
                foreach (var p in baseCache.Zones[zoneId].Apps)
                {
                    var mds = Factory.GetAppState(new AppIdentity(zoneId, p.Key)); // DataSource.GetMetaDataSource(zoneId, p.Key);
                    var appMetaData = mds
                        .Get(SystemRuntime.MetadataType(Eav.Constants.AppAssignmentName), p.Key,
                            AppConstants.TypeAppConfig)
                        .FirstOrDefault();
                    var folder = appMetaData?.GetBestValue("Folder").ToString();
                    if (!string.IsNullOrEmpty(folder) && folder.ToLower() == nameLower)
                        return p.Key;
                }
            }
            return appId > 0 ? appId : Settings.DataIsMissingInDb;
        }
        
    }
}