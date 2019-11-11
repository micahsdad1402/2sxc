﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Persistence;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps.Blocks
{
    public partial class BlockConfiguration
    {
        public void UpdateEntityIfChanged(string type, int sortOrder, int? entityId, bool updatePresentation,
    int? presentationId)
        {
            var somethingChanged = false;
            if (sortOrder == -1)
                throw new Exception("Sort order is never -1 any more; deprecated");

            var listMain = ListWithNulls(type);

            // if necessary, add to end
            if (listMain.Count < sortOrder + 1)
                listMain.AddRange(Enumerable.Repeat(new int?(), (sortOrder + 1) - listMain.Count));

            if (listMain[sortOrder] != entityId)
            {
                listMain[sortOrder] = entityId;
                somethingChanged = true;
            }

            var package = PrepareSavePackage(type, listMain);

            if (updatePresentation)
            {
                var type2 = ReCapitalizePartName(type).Replace(ViewParts.Content, ViewParts.Presentation);
                var listPres = ListWithNulls(type2);
                if (listPres.Count < sortOrder + 1)
                    listPres.AddRange(Enumerable.Repeat(new int?(), (sortOrder + 1) - listPres.Count));
                if (listPres[sortOrder] != presentationId)
                {
                    listPres[sortOrder] = presentationId;
                    somethingChanged = true;
                }
                package = PrepareSavePackage(type2, listPres, package);
            }

            if (somethingChanged)
                SaveChangedLists(package);
        }

        private void SaveChangedLists(Dictionary<string, List<int?>> values)
        {
            // ensure that there are never more presentations than values
            if (values.ContainsKey(ViewParts.Presentation))
            {
                var contentCount = Content.Count;
                if (values.ContainsKey(ViewParts.Content))
                    contentCount = values[ViewParts.Content].Count;
                if (values[ViewParts.Presentation].Count > contentCount)
                    throw new Exception("Presentation may not contain more items than Content.");
            }

            var dicObj = values.ToDictionary(x => x.Key, x => x.Value as object);
            var newEnt = new Entity(AppId, 0, Entity.Type, dicObj);
            var saveOpts = SaveOptions.Build(ZoneId);
            saveOpts.PreserveUntouchedAttributes = true;

            var saveEnt = new EntitySaver(Log).CreateMergedForSaving(Entity, newEnt, saveOpts);

            if (VersioningEnabled)
            {
                // Force saving as draft if needed (if versioning is enabled)
                ((Entity)saveEnt).PlaceDraftInBranch = true;
                ((Entity)saveEnt).IsPublished = false;
            }

            var cms = new CmsManager(ZoneId, AppId, ShowDrafts, VersioningEnabled, Log);
            cms.Entities.Save(saveEnt, saveOpts);

            // Refresh content group entity (ensures content group is up to date)
            // todo continue here!

            Entity = cms.Read.Blocks // new BlocksManager(_zoneId, _appId, _showDrafts, _versioningEnabled, Log)
                .GetBlockConfig(Entity.EntityGuid).Entity;
        }

        private static Dictionary<string, List<int?>> PrepareSavePackage(string type, List<int?> entityIds,
            Dictionary<string, List<int?>> alreadyInitializedDictionary = null)
        {
            type = ReCapitalizePartName(type);
            if (alreadyInitializedDictionary == null) alreadyInitializedDictionary = new Dictionary<string, List<int?>>();
            alreadyInitializedDictionary.Add(type, entityIds.ToList());
            return alreadyInitializedDictionary;
        }



        /// <summary>
        /// Removes entities from a group. This will also remove the corresponding presentation entities.
        /// </summary>
        /// <param name="type">Should be 'Content' or "ListContent"</param>
        /// <param name="sortOrder"></param>
        public void RemoveContentAndPresentationEntities(string type, int sortOrder)
        {
            Log.Add($"remove content and pres items type:{type}, order:{sortOrder}");
            var list1 = ListWithNulls(type);
            list1.RemoveAt(sortOrder);
            var type2 = ReCapitalizePartName(type).Replace(ViewParts.Content, ViewParts.Presentation);
            var list2 = ListWithNulls(type2);
            if (list2.Count > sortOrder)    // in many cases the presentation-list is empty, then there is nothing to remove
                list2.RemoveAt(sortOrder);
            SaveChangedLists(PrepareSavePackage(type, list1, PrepareSavePackage(type2, list2)));
        }


        /// <summary>
        /// If SortOrder is not specified, adds at the end
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sortOrder"></param>
        /// <param name="contentId"></param>
        /// <param name="presentationId"></param>
        public void AddContentAndPresentationEntity(string type, int? sortOrder, int? contentId, int? presentationId)
        {
            Log.Add($"add content/pres for type:{type}, order:{sortOrder}, content#{contentId}, pres#{presentationId}");
            if (type.ToLower() != ViewParts.ContentLower)
                throw new Exception("This is only meant to work for content, not for list-content");

            if (!sortOrder.HasValue)
                sortOrder = Content.Count;

            var list1 = ListWithNulls(type);
            list1.Insert(sortOrder.Value, contentId);

            var list2 = GetPresentationIdWithSameLengthAsContent();
            list2.Insert(sortOrder.Value, presentationId);

            SaveChangedLists(PrepareSavePackage(ViewParts.Content, list1, PrepareSavePackage(ViewParts.Presentation, list2)));

        }


        public void ReorderEntities(int sortOrder, int destinationSortOrder)
        {
            Log.Add($"reorder entities before:{sortOrder} to after:{destinationSortOrder}");
            var contentIds = ListWithNulls(ViewParts.Content);
            var presentationIds = GetPresentationIdWithSameLengthAsContent();
            var contentId = contentIds[sortOrder];
            var presentationId = presentationIds[sortOrder];

            /*
             * ToDo 2017-08-28:
             * Create a DRAFT copy of the BlockConfiguration if versioning is enabled.
             */

            contentIds.RemoveAt(sortOrder);
            presentationIds.RemoveAt(sortOrder);

            contentIds.Insert(destinationSortOrder, contentId);
            presentationIds.Insert(destinationSortOrder, presentationId);

            var list = PrepareSavePackage(ViewParts.Content, contentIds);
            list = PrepareSavePackage(ViewParts.Presentation, presentationIds, list);
            SaveChangedLists(list);
        }

        public bool ReorderAll(int[] newSequence)
        {
            Log.Add(() => $"reorder all to:[{string.Join(",", newSequence)}]");
            var oldCIds = ListWithNulls(ViewParts.Content);
            var oldPIds = GetPresentationIdWithSameLengthAsContent();

            // some error checks
            if (newSequence.Length != oldCIds.Count)
                throw new Exception("Can't re-order - list length is different");

            var newContentIds = new List<int?>();
            var newPresIds = new List<int?>();

            for (var seqItem = 0; seqItem < newSequence.Length; seqItem++)
            {
                var cId = oldCIds[newSequence[seqItem]];
                newContentIds.Add(cId);

                var pId = oldPIds[newSequence[seqItem]];
                newPresIds.Add(pId);
            }

            var list = PrepareSavePackage(ViewParts.Content, newContentIds);
            list = PrepareSavePackage(ViewParts.Presentation, newPresIds, list);
            SaveChangedLists(list);

            return true;
        }

        private static string ReCapitalizePartName(string partName)
        {
            partName = partName.ToLower();
            if (partName == ViewParts.ContentLower) partName = ViewParts.Content;
            else if (partName == ViewParts.PresentationLower) partName = ViewParts.Presentation;
            else if (partName == ViewParts.ListContentLower) partName = ViewParts.ListContent;
            else if (partName == ViewParts.ListPresentationLower) partName = ViewParts.ListPresentation;
            else throw new Exception("Wanted to capitalize part name - but part name unknown: " + partName);
            return partName;
        }




        private List<int?> GetPresentationIdWithSameLengthAsContent()
        {
            var difference = Content.Count - Presentation.Count;

            // this should fix https://github.com/2sic/2sxc/issues/1178 and https://github.com/2sic/2sxc/issues/1158
            // goal is to simply remove the last presentation items, if there is a missmatch. Usually they will simply be nulls anyhow
            if (difference < 0)
                Presentation.RemoveRange(Content.Count, difference);

            var entityIds = ListWithNulls(ViewParts.Presentation);

            // extend as necessary
            if (difference != 0)
                entityIds.AddRange(Enumerable.Repeat(new int?(), difference));

            return entityIds;
        }

    }
}
