﻿using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Security
{
    internal class MultiPermissionsTypes: MultiPermissionsApp
    {
        protected IEnumerable<string> ContentTypes;

        public MultiPermissionsTypes(IBlockBuilder blockBuilder, int appId, string contentType, ILog parentLog) 
            : this(blockBuilder, appId, new []{contentType}, parentLog)
        { }

        public MultiPermissionsTypes(IBlockBuilder blockBuilder, int appId, IEnumerable<string> contentTypes, ILog parentLog) 
            : base(blockBuilder, appId, parentLog) 
            => ContentTypes = contentTypes;

        public MultiPermissionsTypes(IBlockBuilder blockBuilder, int appId, List<ItemIdentifier> items, ILog parentLog) 
            : base(blockBuilder, appId, parentLog) 
            => ContentTypes = ExtractTypeNamesFromItems(items);


        protected override Dictionary<string, IPermissionCheck> InitializePermissionChecks()
            => InitPermissionChecksForType(ContentTypes);

        protected Dictionary<string, IPermissionCheck> InitPermissionChecksForType(IEnumerable<string> contentTypes)
            => contentTypes.Distinct().ToDictionary(t => t, BuildTypePermissionChecker);

        private IEnumerable<string> ExtractTypeNamesFromItems(IEnumerable<ItemIdentifier> items)
        {
            // build list of type names
            var typeNames = items.Select(item =>
                !string.IsNullOrEmpty(item.ContentTypeName) || item.EntityId == 0
                    ? item.ContentTypeName
                    : AppRuntime.Entities.Get(item.EntityId).Type.StaticName);

            return typeNames;
        }

        protected AppRuntime AppRuntime => _appRuntime ?? (_appRuntime = new AppRuntime(App, true, Log));
        private AppRuntime _appRuntime;
        

        /// <summary>
        /// Creates a permission checker for an type in this app
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected IPermissionCheck BuildTypePermissionChecker(string typeName)
        {
            Log.Add($"BuildTypePermissionChecker({typeName})");
            // now do relevant security checks
            return BuildPermissionChecker(AppRuntime.ContentTypes.Get(typeName));
        }


    }
}
