﻿using System;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.Blocks
{
    internal sealed class BlockFromEntity: BlockBase
    {
        internal const string CbPropertyApp = "App";
        internal const string CbPropertyTitle = "Title";
        internal const string CbPropertyContentGroup = "ContentGroup";
        //internal const string CbPropertyTemplate = "Template";
        internal const string CbPropertyShowChooser = "ShowTemplateChooser";

        public override BlockEditorBase Editor => new BlockEditorForEntity(BlockBuilder);
        public override bool ParentIsEntity => false;

        public override IBlockDataSource Data => _dataSource 
            ?? (_dataSource = Block.ForContentGroupInSxc(BlockBuilder, View, App?.ConfigurationProvider, Log));

        #region ContentBlock Definition Entity

        internal IEntity ContentBlockEntity;
        private string _appName;
        private Guid _contentGroupGuid;
        private Guid _previewTemplateGuid;

        private void ParseContentBlockDefinition(IEntity cbDefinition)
        {
            ContentBlockEntity = cbDefinition;
            _appName = ContentBlockEntity.GetBestValue(CbPropertyApp)?.ToString() ?? "";

            string temp = ContentBlockEntity.GetBestValue(CbPropertyContentGroup)?.ToString() ?? "";
            Guid.TryParse(temp, out _contentGroupGuid);

            temp = ContentBlockEntity.GetBestValue(ViewParts.TemplateContentType)?.ToString() ?? "";
            Guid.TryParse(temp, out _previewTemplateGuid);

            temp = ContentBlockEntity.GetBestValue(CbPropertyShowChooser)?.ToString() ?? "";
            bool show;
            if (bool.TryParse(temp, out show))
                ShowTemplateChooser = show;

        }
        #endregion

        public BlockFromEntity(IBlock parent, IEntity cbDefinition, ILog parentLog = null): base(parentLog, "CB.Ent") 
            => _constructor(parent, cbDefinition);

        public BlockFromEntity(IBlock parent, int contentBlockId, ILog parentLog) : base(parentLog, "CB.Ent")
        {
            var wrapLog = Log.Call();
            contentBlockId = Math.Abs(contentBlockId); // for various reasons this can be introduced as a negative value, make sure we neutralize that
            var cbDef = parent.BlockBuilder.App.Data.List.One(contentBlockId);  // get the content-block definition
            _constructor(parent, cbDef);
            wrapLog($"ok, id:{contentBlockId}");
        }

        private void _constructor(IBlock parent, IEntity cbDefinition)
        {
            var wrapLog = Log.Call();
            Parent = parent;
            ParseContentBlockDefinition(cbDefinition);
            ParentId = parent.ParentId;
            ContentBlockId = -cbDefinition.EntityId; 

            // Ensure we know what portal the stuff is coming from
            Tenant = Parent.App.Tenant;

            ZoneId = Parent.ZoneId;

            AppId = AppHelpers.GetAppIdFromGuidName(ZoneId, _appName); // should be 0 if unknown, must test

            if (AppId == Settings.DataIsMissingInDb)
            {
                _dataIsMissing = true;
                return;
            }

            // 2018-09-22 new, must come before the AppId == 0 check
            BlockBuilder = new BlockBuilder(parent.BlockBuilder, this, Parent.BlockBuilder.Container, Parent.BlockBuilder.Parameters, Log);

            if (AppId == 0) return;

            App = new App(Tenant, ZoneId, AppId, ConfigurationProvider.Build(BlockBuilder, false), true, Log);
            
            // 2019-11-11 2dm new, with CmsRuntime
            var cms = new CmsRuntime(App, Log, parent.BlockBuilder.UserMayEdit,
                parent.BlockBuilder.Environment.PagePublishing.IsEnabled(parent.BlockBuilder.Container.Id));

            Configuration = cms.Blocks.GetContentGroupOrGeneratePreview(_contentGroupGuid, _previewTemplateGuid);

            // handle cases where the content group is missing - usually because of incomplete import
            if (Configuration.DataIsMissing)
            {
                _dataIsMissing = true;
                App = null;
                return;
            }

            // use the content-group template, which already covers stored data + module-level stored settings
            ((BlockBuilder)BlockBuilder).SetTemplateOrOverrideFromUrl(Configuration.View);

            wrapLog("ok");
        }



        public override bool IsContentApp => _appName == Eav.Constants.DefaultAppName;

    }
}