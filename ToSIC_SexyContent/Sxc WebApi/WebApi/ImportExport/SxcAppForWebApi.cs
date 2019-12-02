﻿using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.SxcTemp;
using Factory = ToSic.SexyContent.Environment.Dnn7.Factory;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class SxcAppForWebApi
    {
        private IApp App { get; }

        internal static IApp AppBasedOnUserPermissions(int zoneId, int appId, UserInfo user)
            => GetBasedOnUserPermissions(zoneId, appId, user).App;


        private static SxcAppForWebApi GetBasedOnUserPermissions(int zoneId, int appId, UserInfo user)
        {
            var appWrapper = user.IsSuperUser
                ? new SxcAppForWebApi(zoneId, appId) // only super-user may switch to another zone for export
                : new SxcAppForWebApi(appId, false);
            return appWrapper;
        }

        private SxcAppForWebApi(int appId, bool versioningEnabled)
        {
            App = Factory.App(appId, versioningEnabled);
        }

        private SxcAppForWebApi(int zoneId, int appId)
        {
            App = GetApp.LightWithoutData(new Tenant(PortalSettings.Current), zoneId, appId, false, null);
        }

    }
}