﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.File;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnAppFileSystemLoader : HasLog, IAppFileSystemLoader
    {
        public const string FieldFolderPrefix = "field-";
        public const string JsFile = "index.js";

        public int AppId { get;  }

        public string Path { get; set; }

        protected readonly PortalSettings PortalSettings;

        public DnnAppFileSystemLoader(int appId, string path, PortalSettings portalSettings, ILog log): base("Dnn.AppStf", log)
        {
            var wrapLog = Log.Call($"{appId}, {path}, ...");
            AppId = appId;
            PortalSettings = portalSettings;

            try
            {
                var tenant = new DnnTenant(portalSettings);
                var fullPath = tenant.SxcPath + "/" + path + "/" + Eav.Constants.FolderAppExtensions;
                Path = HostingEnvironment.MapPath(fullPath);
                Log.Add("System path:" + Path);
            }
            catch (Exception e)
            {
                // ignore
                wrapLog("error: " + e.Message);
                return;
            }

            wrapLog(null);
        }

        public List<InputTypeInfo> InputTypes()
        {
            var wrapLog = Log.Call<List<InputTypeInfo>>();
            var di = new DirectoryInfo(Path);
            if (!di.Exists) return wrapLog("directory not found", new List<InputTypeInfo>());
            var inputFolders = di.GetDirectories(FieldFolderPrefix + "*");
            Log.Add($"found {inputFolders.Length} field-directories");

            var withIndexJs = inputFolders
                .Where(fld => fld.GetFiles(JsFile).Any())
                .Select(fld => fld.Name).ToArray();
            Log.Add($"found {withIndexJs.Length} folders with {JsFile}");

            var types = withIndexJs.Select(name =>
                {
                    var fullName = name.Substring(FieldFolderPrefix.Length);
                    var niceName = NiceName(name);
                    // TODO: use metadata information if available
                    return new InputTypeInfo(fullName, niceName, "Extension Field", "", false,
                        $"[App:Path]/{Eav.Constants.FolderAppExtensions}/{name}/{JsFile}", "", false);
                })
                .ToList();
            return wrapLog(null, types);
        }

        private static string NiceName(string name)
        {
            var nameStack = name.Split('-');
            if (nameStack.Length < 3) return "[Bad Name Format]";
            // drop "field-" and "string-" or whatever type name is used
            nameStack = nameStack.Skip(2).ToArray();
            var caps = nameStack.Select(n =>
            {
                if (string.IsNullOrWhiteSpace(n)) return "";
                if (n.Length <= 1) return n;
                return char.ToUpper(n[0]) + n.Substring(1);
            });

            var niceName = string.Join(" ", caps);
            return niceName;
        }

        public IList<IContentType> ContentTypes(IEntitiesSource entitiesSource)
        {
            var wrapLog = Log.Call<IList<IContentType>>();
            try
            {
                var extPaths = ExtensionPaths();
                Log.Add($"Found {extPaths.Count} extensions with .data folder");
                var allTypes = extPaths.SelectMany(p => LoadTypesFromOneExtensionPath(p, entitiesSource))
                    .Distinct(new EqualityComparer_ContentType())
                    .ToList();
                return wrapLog("ok", allTypes);
            }
            catch (Exception e)
            {
                Log.Add("error " + e.Message);
            }

            return wrapLog("error", new List<IContentType>());
        }

        private IEnumerable<IContentType> LoadTypesFromOneExtensionPath(string extensionPath, IEntitiesSource entitiesSource)
        {
            var wrapLog = Log.Call<IList<IContentType>>(extensionPath);
            var fsLoader = new FileSystemLoader(extensionPath, RepositoryTypes.Folder, true, entitiesSource, Log);
            var types = fsLoader.ContentTypes();
            return wrapLog("ok", types);
        }

        private List<string> ExtensionPaths()
        {
            var dir = new DirectoryInfo(Path);
            if(!dir.Exists) return new List<string>();
            var sub = dir.GetDirectories();
            var subDirs = sub.SelectMany(s => s.GetDirectories(Eav.Constants.FolderData));
            var paths = subDirs.Select(s => s.FullName).ToList();
            return paths;
        }
    }
}
