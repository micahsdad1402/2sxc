﻿using DotNetNuke.Entities.Users;

namespace ToSic.Sxc.Dnn.Web.ClientInfos
{
    public class ClientInfosUser
    {
        public bool CanDesign;
        public bool CanDevelop;

        public ClientInfosUser(UserInfo uinfo)
        {
            CanDesign = DnnSecurity.IsInSexyContentDesignersGroup(uinfo);
            CanDevelop = uinfo.IsSuperUser;
        }
    }
}
