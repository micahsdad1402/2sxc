﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav.ImportExport.Interfaces;
using ToSic.Eav.ImportExport.Logging;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class ImportExportEnvironment : IImportExportEnvironment
    {
        public List<ExportImportMessage> Messages { get; } = new List<ExportImportMessage>();

        //private IFileManager _dnnFileManager;
        //private IFolderManager _dnnFolderManager;
        //private int _portalId;

        /// <summary>
        /// Copy all files from SourceFolder to DestinationFolder
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinationFolder">The portal-relative path where the files should be copied to</param>
        ///// <param name="overwriteFiles"></param>
        ///// <param name="messages"></param>
        ///// <param name="fileIdMapping">The fileIdMapping is needed to re-assign the existing "File:" parameters while importing the content</param>
        public void TransferFilesToTennant(string sourceFolder, string destinationFolder)//, Boolean overwriteFiles, List<ExportImportMessage> messages)
        {
            var messages = Messages;
            var files = Directory.GetFiles(sourceFolder, "*.*");

            var dnnFileManager = DotNetNuke.Services.FileSystem.FileManager.Instance;
            var dnnFolderManager = FolderManager.Instance;
            var portalId = PortalSettings.Current.PortalId;

            if (!dnnFolderManager.FolderExists(portalId, destinationFolder))
                dnnFolderManager.AddFolder(portalId, destinationFolder);
            var folderInfo = dnnFolderManager.GetFolder(portalId, destinationFolder);

            foreach (var sourceFilePath in files)
            {
                var destinationFileName = Path.GetFileName(sourceFilePath);

                if (!dnnFileManager.FileExists(folderInfo, destinationFileName))
                {
                    try
                    {
                        using (var stream = File.OpenRead(sourceFilePath))
                            dnnFileManager.AddFile(folderInfo, destinationFileName, stream, false);
                    }
                    catch (InvalidFileExtensionException e)
                    {
                        messages.Add(
                            new ExportImportMessage(
                                "File '" + destinationFileName +
                                "' not copied because the file extension is not allowed.",
                                ExportImportMessage.MessageTypes.Error));
                        Exceptions.LogException(e);
                    }
                    catch (Exception e)
                    {
                        messages.Add(new ExportImportMessage("Can't copy file '" + destinationFileName + "' because of an unkown error. It's likely that your files and folders are not in sync with DNN, usually re-syncing will fix the issue.", ExportImportMessage.MessageTypes.Warning));
                        Exceptions.LogException(e);
                    }
                }
                else
                    messages.Add(new ExportImportMessage("File '" + destinationFileName + "' not copied because it already exists", ExportImportMessage.MessageTypes.Warning));
            }

            // Call the method recursively to handle subdirectories
            foreach (var sourceFolderPath in Directory.GetDirectories(sourceFolder))
            {
                var newDestinationFolder = Path.Combine(destinationFolder, sourceFolderPath.Replace(sourceFolder, "").TrimStart('\\')).Replace('\\', '/');
                TransferFilesToTennant(sourceFolderPath, newDestinationFolder);//, overwriteFiles, messages);
            }
        }

        public Version TennantVersion => typeof(PortalSettings).Assembly.GetName().Version;

        public string DefaultLanguage => PortalSettings.Current.DefaultLanguage;

        public string TemplatesRoot(int zoneId, int appId)
        {
            var app = new App(zoneId, appId, PortalSettings.Current, false);

            // Copy all files in 2sexy folder to (portal file system) 2sexy folder
            var templateRoot =  HttpContext.Current.Server.MapPath(Internal.TemplateHelpers.GetTemplatePathRoot(Settings.TemplateLocations.PortalFileSystem, app));
            return templateRoot;
        }

        public string TargetPath(string folder)
        {
            var appPath = Path.Combine(AppHelpers.AppBasePath(null), folder);

            return HttpContext.Current.Server.MapPath(appPath);
        }

        #region stuff we need for Import

        public void MapExistingFilesToImportSet(Dictionary<int, string> filesAndPaths, Dictionary<int, int> fileIdMap)
        {
            var maybePortalId = PortalSettings.Current?.PortalId;

            if (!maybePortalId.HasValue)
                return;

            var portalId = maybePortalId.Value;
            var fileManager = FileManager.Instance;
            var folderManager = FolderManager.Instance;

            foreach (var file in filesAndPaths)
            {
                var fileId = file.Key;
                var relativePath = file.Value;

                var fileName = Path.GetFileName(relativePath);
                var directory = Path.GetDirectoryName(relativePath)?.Replace('\\', '/');
                if (directory == null) continue;

                if (!folderManager.FolderExists(portalId, directory))
                    continue;

                var folderInfo = folderManager.GetFolder(portalId, directory);

                if (!fileManager.FileExists(folderInfo, fileName))
                    continue;

                var fileInfo = fileManager.GetFile(folderInfo, fileName);
                fileIdMap.Add(fileId, fileInfo.FileId);
            }
        }

        public void CreateFoldersAndMapToImportIds(Dictionary<int, string> foldersAndPath, Dictionary<int, int> folderIdCorrectionList, List<ExportImportMessage> importLog)
        {
            var maybePortalId = PortalSettings.Current?.PortalId;

            if (!maybePortalId.HasValue)
                return;
            var portalId = maybePortalId.Value;
            var folderManager = FolderManager.Instance;

            foreach (var file in foldersAndPath) // portalFiles)
            {
                //var origId = int.Parse(portalFile.Attribute(XmlConstants.FolderNodeId).Value);
                //var relativePath = portalFile.Attribute(XmlConstants.FolderNodePath).Value;
                try
                {
                    if (String.IsNullOrEmpty(file.Value)) continue;
                    var directory = Path.GetDirectoryName(file.Value)?.Replace('\\', '/');
                    if (directory == null) continue;
                    // if not exist, create - important because we need for metadata assignment
                    var folderInfo = (!folderManager.FolderExists(portalId, directory))
                        ? folderManager.AddFolder(portalId, directory)
                        : folderManager.GetFolder(portalId, directory);

                    folderIdCorrectionList.Add(file.Key, folderInfo.FolderID);
                }
                catch (Exception)
                {
                    importLog.Add(
                        new ExportImportMessage(
                            "Had a problem with folder id '" + file.Key + "' path '" + file.Value +
                            "' - you'll have to figure out yourself it this is a problem",
                            ExportImportMessage.MessageTypes.Warning));
                }
            }
        }

        public string ModuleVersion => Settings.ModuleVersion;

        public string FallbackContentTypeScope => Settings.AttributeSetScope;

        #endregion
    }
}