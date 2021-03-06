﻿using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Security
{
    internal class MultiPermissionsItems: MultiPermissionsApp
    {
        protected List<IEntity> Items;

        public MultiPermissionsItems(IBlockBuilder blockBuilder, int appId, IEntity item, ILog parentLog) 
            : base(blockBuilder, appId, parentLog)
        {
            Items = new List<IEntity> {item};
        }

        protected override Dictionary<string, IPermissionCheck> InitializePermissionChecks()
            => Items.ToDictionary(i => i.EntityId.ToString(), BuildItemPermissionChecker);

        /// <summary>
        /// Creates a permission checker for an type in this app
        /// </summary>
        /// <returns></returns>
        protected IPermissionCheck BuildItemPermissionChecker(IEntity item)
        {
            Log.Call($"{item.EntityId}");
            // now do relevant security checks
            return BuildPermissionChecker(item.Type, item);
        }


    }
}
