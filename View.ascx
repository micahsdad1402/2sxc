﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToSic.SexyContent.View" Codebehind="View.ascx.cs" %>
<asp:Placeholder runat="server" ID="pnlTemplateChooser" Visible="false" EnableViewState="False">
    
    <%-- todo:2rm - auto-select template doesn't quite work when not inline-template --%>
    <div ng-controller="TemplateSelectorCtrl" data-moduleid="<%= ModuleId %>" class="sc-selector-wrapper" ng-include="'<%= ResolveUrl("~") %>desktopmodules/tosic_sexycontent/js/template-selector/template-selector-view.html'">
<%--
<div ng-cloak ng-show="manageInfo.templateChooserVisible" class="dnnFormMessage dnnFormInfo">
    <div class="sc-selectors">
        <select ng-show="!manageInfo.isContentApp" ng-model="appId" class="sc-selector-app" ng-options="a.AppId as a.Name for a in apps">
            <option value="" translate="TemplatePicker.AppPickerDefault"></option>
        </select>

        <select ng-show="manageInfo.isContentApp" ng-model="contentTypeId" ng-options="c.StaticName as c.Name for c in contentTypes" class="sc-selector-contenttype" ng-disabled="manageInfo.hasContent || manageInfo.isList">
            <option ng-disabled="contentTypeId != ''" value="" translate="TemplatePicker.ContentTypePickerDefault"></option>

        </select>
        <select ng-show="manageInfo.isContentApp ? contentTypeId != 0 : savedAppId != null" ng-model="templateId" class="sc-selector-template" ng-options="t.TemplateId as t.Name for t in filteredTemplates(contentTypeId)"></select>
    </div>
    <div class="sc-selector-actions">
        <a ng-show="templateId != null && savedTemplateId != templateId" ng-click="saveTemplateId();" class="sc-selector-save" title="{{ 'TemplatePicker.Save' | translate }}">{{ 'TemplatePicker.Save' | translate }}</a>
        <a ng-show="savedTemplateId != null" class="sc-selector-close" ng-click="setTemplateChooserState(false);" title="{{ 'TemplatePicker.Cancel' | translate }}">{{ 'TemplatePicker.Cancel' | translate }}</a>
    </div>
    <div class="sc-loading sc-loading-nobg" ng-show="loading"></div>
</div>
	--%>
    </div>
    
</asp:Placeholder>

<asp:Panel runat="server" Visible="False" class="dnnFormMessage dnnFormInfo" ID="pnlGetStarted"></asp:Panel>

<asp:Panel runat="server" ID="pnlZoneConfigurationMissing" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
    <asp:Label runat="server" ID="lblMissingZoneConfiguration" ResourceKey="ZoneConfigurationMissing"></asp:Label>
    <asp:HyperLink runat="server" ID="hlkConfigureZone" 
        CssClass="dnnSecondaryAction" ResourceKey="hlkConfigureZone"></asp:HyperLink>
</asp:Panel>

<asp:Panel runat="server" ID="pnlError" CssClass="dnnFormMessage dnnFormWarning" Visible="false"></asp:Panel>
<asp:Panel runat="server" ID="pnlMessage" CssClass="dnnFormMessage dnnFormInfo" Visible="false"></asp:Panel>

<div class="sc-viewport">
    <asp:PlaceHolder runat="server" ID="phOutput"></asp:PlaceHolder>
</div>