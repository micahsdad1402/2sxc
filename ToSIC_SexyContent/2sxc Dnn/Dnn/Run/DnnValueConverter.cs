﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// The DNN implementation of the <see cref="IValueConverter"/> which converts "file:22" or "page:5" to the url,
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class DnnValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public string ToReference(string value)
            => TryToResolveOneLinkToInternalDnnCode(value);

        /// <inheritdoc />
        public string ToValue(string reference, Guid itemGuid = default)
            => TryToResolveDnnCodeToLink(itemGuid, reference);

        /// <summary>
        /// Will take a link like http:\\... to a file or page and try to return a DNN-style info like
        /// Page:35 or File:43003
        /// </summary>
        /// <param name="potentialFilePath"></param>
        /// <returns></returns>
        private static string TryToResolveOneLinkToInternalDnnCode(string potentialFilePath)
        {
            // note: this can always use the current context, because this should happen
            // when saving etc. - which is always expected to happen in the owning portal
            var portalInfo = PortalSettings.Current;

            // Try file reference
            var fileInfo = FileManager.Instance.GetFile(portalInfo.PortalId, potentialFilePath);
            if (fileInfo != null)
                return "file:" + fileInfo.FileId;

            // Try page / tab ID
            var tabController = new TabController();
            var tabCollection = tabController.GetTabsByPortal(portalInfo.PortalId);
            var tabInfo = tabCollection.Select(tab => tab.Value)
                                       .FirstOrDefault(tab => tab.TabPath == potentialFilePath);

            if (tabInfo != null)
                return "page:" + tabInfo.TabID;

            return potentialFilePath;
        }

        /// <summary>
        /// Will take a link like "File:17" and convert to "Faq/screenshot1.jpg"
        /// It will always deliver a relative path to the portal root
        /// </summary>
        /// <param name="itemGuid">the item we're in - important for the feature which checks if the file is in this items ADAM</param>
        /// <param name="originalValue"></param>
        /// <returns></returns>
        private string TryToResolveDnnCodeToLink(Guid itemGuid, string originalValue)
        {
            // new
            var resultString = originalValue;
            var regularExpression = Regex.Match(resultString, @"^(?<type>(file|page)):(?<id>[0-9]+)(?<params>(\?|\#).*)?$", RegexOptions.IgnoreCase);

            if (!regularExpression.Success)
                return originalValue;

            var linkType = regularExpression.Groups["type"].Value.ToLower();
            var linkId = int.Parse(regularExpression.Groups["id"].Value);
            var urlParams = regularExpression.Groups["params"].Value ?? "";

            var isPageLookup = linkType == "page";
            try
            {
                var result = (isPageLookup
                                 ? ResolvePageLink(linkId)
                                 : ResolveFileLink(linkId, itemGuid))
                             ?? originalValue;

                return result + (result == originalValue ? "" : urlParams);
            }
            catch (Exception e)
            {
                var wrappedEx = new Exception("Error when trying to lookup a friendly url of \"" + originalValue + "\"", e);
                Exceptions.LogException(wrappedEx);
                return originalValue;
            }

        }

        private static string ResolveFileLink(int linkId, Guid itemGuid)
        {
            var fileInfo = FileManager.Instance.GetFile(linkId);
            if (fileInfo == null)
                return null;

            #region special handling of issues in case something in the background is broken
            // there are cases where the PortalSettings will be null or something, and in these cases the serializer would break down
            // so this is to just ensure that if it can't be converted, it'll just fall back to default
            try
            {
                var filePath = Path.Combine(new PortalSettings(fileInfo.PortalId)?.HomeDirectory ?? "", fileInfo?.RelativePath ?? "");

                // return linkclick url for secure and other not standard folder locations
                var result = (fileInfo.StorageLocation == 0) ? filePath : FileLinkClickController.Instance.GetFileLinkClick(fileInfo);

                // optionally do extra security checks (new in 10.02)
                if (!Features.Enabled(FeatureIds.BlockFileIdLookupIfNotInSameApp)) return result;

                // check if it's in this item. We won't check the field, just the item, so the field is ""
                return !Sxc.Adam.Security.PathIsInItemAdam(itemGuid, "", filePath)
                    ? null
                    : result;
            }
            catch
            {
                return null;
            }
            #endregion
        }

        private static string ResolvePageLink(int id)
        {
            var tabController = new TabController();

            var tabInfo = tabController.GetTab(id, 0);
            if (tabInfo == null) return null;

            var portalSettings = PortalSettings.Current;

            // Get full PortalSettings (with portal alias) if module sharing is active
            if (PortalSettings.Current != null && PortalSettings.Current.PortalId != tabInfo.PortalID)
                portalSettings = new PortalSettings(tabInfo.PortalID);

            if (portalSettings == null) return null;

            if (tabInfo.CultureCode != "" && PortalSettings.Current != null && tabInfo.CultureCode != PortalSettings.Current.CultureCode)
            {
                var cultureTabInfo = tabController
                    .GetTabByCulture(tabInfo.TabID, tabInfo.PortalID,
                        LocaleController.Instance.GetLocale(PortalSettings.Current.CultureCode));

                if (cultureTabInfo != null)
                    tabInfo = cultureTabInfo;
            }

            // Exception in AdvancedURLProvider because ownerPortalSettings.PortalAlias is null
            return Globals.NavigateURL(tabInfo.TabID, portalSettings, "", new string[] { });
        }
    }




}