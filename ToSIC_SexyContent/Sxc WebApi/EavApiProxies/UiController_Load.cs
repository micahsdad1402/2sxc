﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.Format;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.Permissions;
using ToSic.Sxc.Compatibility;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    public partial class UiController
    {

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public AllInOne Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            // Security check
            var wraplog = Log.Call("Load", $"load many a#{appId}, items⋮{items.Count}");

            // do early permission check - but at this time it may be that we don't have the types yet
            // because they may be group/id combinations, without type information which we'll look up afterwards
            var appForSecurityChecks = App.LightWithoutData(new DnnTenant(PortalSettings), appId, Log);
            items = new SaveHelpers.ContentGroupList(SxcInstance, Log).ConvertListIndexToId(items, appForSecurityChecks);

            // now look up the types, and repeat security check with type-names
            var permCheck = new MultiPermissionsTypes(SxcInstance, appId, items, Log);
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, out var exception))
                throw exception;

            // load items - similar
            var result = new AllInOne();
            var entityApi = new EntityApi(appId, Log);
            var typeRead = entityApi.AppManager.Read.ContentTypes;
            var list = entityApi.GetEntitiesForEditing(appId, items);
            var jsonSerializer = new JsonSerializer();
            result.Items = list.Select(e => new BundleWithHeader<JsonEntity>
            {
                Header = e.Header,
                Entity = GetSerializeAndMdAssignJsonEntity(appId, e, jsonSerializer, typeRead)
            }).ToList();

            // set published if some data already exists
            if(list.Any())
            {
                result.IsPublished = list.First().Entity?.IsPublished ?? true; // Entity could be null (new), then true
                // only set draft-should-branch if this draft already has a published item
                if (!result.IsPublished) result.DraftShouldBranch = list.First().Entity?.GetPublished() != null;
            }

            // since we're retrieving data - make sure we're allowed to
            // this is to ensure that if public forms only have create permissions, they can't access existing data
            // important, this code is shared/duplicated in the EntitiesController.GetManyForEditing
            if (list.Any(set => set.Entity != null))
                if (!permCheck.EnsureAll(GrantSets.ReadSomething, out exception))
                    throw exception;

            // load content-types
            var types = UsedTypes(list, typeRead);
            result.ContentTypes = types
                .Select(ct => JsonSerializer.ToJson(ct, true))
                .ToList();

            // Fix not-supported input-type names; map to correct name
            result.ContentTypes
                .ForEach(jt => jt.Attributes
                    .ForEach(at => at.InputType = InputTypes.MapInputTypeV10(at.InputType)));

            // load input-field configurations
            result.InputTypes = GetNecessaryInputTypes(result.ContentTypes/*types*/, typeRead);

            // also include UI features
            result.Features = SystemController.FeatureListWithPermissionCheck(appId, permCheck).ToList();

            // done - return
            wraplog($"ready, sending items:{result.Items.Count}, " +
                    $"types:{result.ContentTypes.Count}, " +
                    $"inputs:{result.InputTypes.Count}, " +
                    $"feats:{result.Features.Count}");
            return result;
        }

        /// <summary>
        /// Get Serialized entity or create a new one, and assign metadata
        /// based on the header (if none already existed)
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="bundle"></param>
        /// <param name="jsonSerializer"></param>
        /// <param name="typeRead"></param>
        /// <returns></returns>
        private static JsonEntity GetSerializeAndMdAssignJsonEntity(int appId, BundleIEntity bundle, JsonSerializer jsonSerializer,
            ContentTypeRuntime typeRead)
        {
            // attach original metadata assignment when creating a new one
            JsonEntity ent;
            if (bundle.Entity != null)
                ent = jsonSerializer.ToJson(bundle.Entity);
            else
            {
                ent = jsonSerializer.ToJson(ConstructEmptyEntity(appId, bundle.Header, typeRead));

                // only attach metadata, if no metadata already exists
                if (ent.For == null)
                {
                    if (bundle.Header?.For != null)
                        ent.For = bundle.Header.For;

                    // if we have "old" Metadata headers, attach these
                    else if (bundle.Header?.Metadata != null)
                    {
                        var md = bundle.Header.Metadata;
                        ent.For = new JsonMetadataFor
                        {
                            Guid = md.KeyGuid,
                            String = md.KeyString,
                            Number = md.KeyNumber,
                            Target = jsonSerializer.MetadataProvider.GetType(md.TargetType)
                        };
                    }
                }
            }

            // new UI doesn't use this any more, reset it
            if (bundle.Header != null)
            {
                bundle.Header.Metadata = null;
                bundle.Header.For = null;
            }
            return ent;
        }

        private static List<IContentType> UsedTypes(List<BundleIEntity> list, ContentTypeRuntime typeRead)
            => list.Select(i
                    // try to get the entity type, but if there is none (new), look it up according to the header
                    => i.Entity?.Type
                       ?? typeRead.Get(i.Header.ContentTypeName))
                .ToList();

        private static List<InputTypeInfo> GetNecessaryInputTypes(List<JsonContentType> types, ContentTypeRuntime typeRead)
        {
            var fields = types
                .SelectMany(t => t.Attributes)
                .Select(a => a.InputType)//.InputTypeTempBetterForNewUi)
                .Distinct();
            return typeRead.GetInputTypes()
                .Where(it => fields.Contains(it.Type))
                .ToList();
        }

        private static IEntity ConstructEmptyEntity(int appId, ItemIdentifier header, ContentTypeRuntime typeRead)
        {
            var type = typeRead.Get(header.ContentTypeName);
            var ent = EntityBuilder.EntityWithAttributes(appId, header.Guid, header.EntityId, 0, type);
            return ent;
        }

    }
}