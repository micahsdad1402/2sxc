﻿using System;
using System.Web;
using Newtonsoft.Json;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public partial class InPageEditingHelper 
    {
        //[PrivateApi]
        //protected  IBlockBuilder BlockBuilder;

        #region Context Attributes

        /// <inheritdoc/>
        public HtmlString ContextAttributes(IDynamicEntity target,
            string noParameterOrder = Eav.Constants.RandomProtectionParameter, 
            string field = null,
            string contentType = null, 
            Guid? newGuid = null,
            string apps = null,
            int max = 100)
        {
            Log.Add("ctx attribs - enabled:{Enabled}");
            if (!Enabled) return null;
            Eav.Constants.ProtectAgainstMissingParameterNames(noParameterOrder, nameof(ContextAttributes), $"{nameof(field)},{nameof(contentType)},{nameof(newGuid)}");

            if (field == null) throw new Exception("need parameter 'field'");

            var serialized = JsonConvert.SerializeObject(new 
            {
                apps,
                field,
                guid = newGuid.ToString(),
                max,
                parent = target.EntityId,
                type = contentType ?? Settings.AttributeSetStaticNameContentBlockTypeName,
            });

            return new HtmlString(innerContentAttribute + "='" + serialized + "'");

            //return new HtmlString(string.Format(
            //    _jsonTemplate,
            //    target.EntityId,
            //    field,
            //    contentType ?? Settings.AttributeSetStaticNameContentBlockTypeName,
            //    newGuid));
        }

        /// <inheritdoc/>
        [PrivateApi]
        public HtmlString WrapInContext(object content,
            string noParameterOrder = Eav.Constants.RandomProtectionParameter,
            string tag = Constants.DefaultContextTag,
            bool full = false,
            bool? enableEdit = null,
            int instanceId = 0,
            int contentBlockId = 0
        )
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(noParameterOrder, nameof(WrapInContext), $"{nameof(tag)},{nameof(full)},{nameof(enableEdit)},{nameof(instanceId)},{nameof(contentBlockId)}");

            return new HtmlString(
                ((BlockBuilder)BlockBuilder).RenderingHelper.WrapInContext(content.ToString(),
                    instanceId: instanceId > 0
                        ? instanceId
                        : BlockBuilder?.Block?.ParentId ?? 0,
                    contentBlockId: contentBlockId > 0
                        ? contentBlockId
                        : BlockBuilder?.Block?.ContentBlockId ?? 0,
                    editContext: enableEdit ?? Enabled)
            );
        }

        #endregion Context Attributes


    }
}