(window.webpackJsonp=window.webpackJsonp||[]).push([[33],{ctQO:function(t,n,e){"use strict";e.r(n),e.d(n,"ExportAppComponent",(function(){return l}));var o=e("1C3z"),i=e("BLjT"),p=e("WZuw"),c=e("8AiQ"),r=e("mPmy"),a=e("ZSGP"),s=e("9HSk");function b(t,n){if(1&t){var e=o.Xb();o.Wb(0,"div"),o.Wb(1,"p",5),o.Pc(2,"Specs"),o.Vb(),o.Wb(3,"ul",6),o.Wb(4,"li"),o.Pc(5),o.Vb(),o.Wb(6,"li"),o.Pc(7),o.Vb(),o.Wb(8,"li"),o.Pc(9),o.Vb(),o.Vb(),o.Wb(10,"p",5),o.Pc(11,"Contains"),o.Vb(),o.Wb(12,"ul",6),o.Wb(13,"li"),o.Pc(14),o.Vb(),o.Wb(15,"li"),o.Pc(16),o.Vb(),o.Wb(17,"li"),o.Pc(18),o.Vb(),o.Wb(19,"li"),o.Pc(20),o.Vb(),o.Wb(21,"li"),o.Pc(22),o.Vb(),o.Vb(),o.Wb(23,"div",7),o.Wb(24,"div",8),o.Wb(25,"mat-checkbox",9),o.ec("ngModelChange",(function(t){return o.Ec(e),o.ic().includeContentGroups=t})),o.Wb(26,"span",10),o.Pc(27," Include all content-groups to re-import the app in an exact copy of this site. Only select this option when you copy an entire DNN site. "),o.Vb(),o.Vb(),o.Vb(),o.Wb(28,"div",8),o.Wb(29,"mat-checkbox",9),o.ec("ngModelChange",(function(t){return o.Ec(e),o.ic().resetAppGuid=t})),o.Wb(30,"span",10),o.Pc(31," Reset the app GUID to zero. You only need this option for special tutorial apps, and usually must not select it. "),o.Vb(),o.Vb(),o.Vb(),o.Vb(),o.Wb(32,"div",11),o.Wb(33,"button",12),o.ec("click",(function(){return o.Ec(e),o.ic().closeDialog()})),o.Pc(34,"Cancel"),o.Vb(),o.Wb(35,"button",12),o.ec("click",(function(){return o.Ec(e),o.ic().exportGit()})),o.Pc(36,"Export Data for Github versioning"),o.Vb(),o.Wb(37,"button",13),o.ec("click",(function(){return o.Ec(e),o.ic().exportApp()})),o.Pc(38,"Export App"),o.Vb(),o.Vb(),o.Vb()}if(2&t){var i=o.ic();o.Bb(5),o.Rc("Name: ",i.appInfo.Name,""),o.Bb(2),o.Rc("Guid: ",i.appInfo.Guid,""),o.Bb(2),o.Rc("Version: ",i.appInfo.Version,""),o.Bb(5),o.Rc("",i.appInfo.EntitiesCount," entities"),o.Bb(2),o.Rc("",i.appInfo.LanguagesCount," languages"),o.Bb(2),o.Tc(" ",i.appInfo.TemplatesCount," templates (Token: ",i.appInfo.HasTokenTemplates,", Razor: ",i.appInfo.HasRazorTemplates,") "),o.Bb(2),o.Rc("",i.appInfo.TransferableFilesCount," files to export"),o.Bb(2),o.Rc("",i.appInfo.FilesCount," files in the app folder totally"),o.Bb(3),o.qc("ngModel",i.includeContentGroups)("disabled",i.resetAppGuid),o.Bb(4),o.qc("ngModel",i.resetAppGuid)("disabled",i.includeContentGroups),o.Bb(4),o.qc("disabled",i.isExporting),o.Bb(2),o.qc("disabled",i.isExporting),o.Bb(2),o.qc("disabled",i.isExporting)}}var l=function(){function t(t,n){this.dialogRef=t,this.exportAppService=n,this.includeContentGroups=!1,this.resetAppGuid=!1,this.isExporting=!1}return t.prototype.ngOnInit=function(){var t=this;this.exportAppService.getAppInfo().subscribe((function(n){t.appInfo=n}))},t.prototype.exportApp=function(){this.isExporting=!0,this.exportAppService.exportApp(this.includeContentGroups,this.resetAppGuid),this.isExporting=!1},t.prototype.exportGit=function(){var t=this;this.isExporting=!0,this.exportAppService.exportForVersionControl(this.includeContentGroups,this.resetAppGuid).subscribe({next:function(n){t.isExporting=!1,alert("Done - please check your '.data' folder")},error:function(n){t.isExporting=!1}})},t.prototype.closeDialog=function(){this.dialogRef.close()},t.\u0275fac=function(n){return new(n||t)(o.Qb(i.h),o.Qb(p.a))},t.\u0275cmp=o.Kb({type:t,selectors:[["app-export-app"]],decls:12,vars:1,consts:[["mat-dialog-title",""],[1,"dialog-title-box"],[1,"dialog-description"],["href","http://2sxc.org/en/help?tag=export-app","target","_blank"],[4,"ngIf"],[1,"app-info__title"],[1,"app-info__content"],[1,"options-wrapper"],[1,"option-box"],[3,"ngModel","disabled","ngModelChange"],[1,"option-box__text"],[1,"dialog-actions-box"],["mat-raised-button","",3,"disabled","click"],["mat-raised-button","","color","accent",3,"disabled","click"]],template:function(t,n){1&t&&(o.Wb(0,"div",0),o.Wb(1,"div",1),o.Pc(2,"Export app"),o.Vb(),o.Vb(),o.Wb(3,"p",2),o.Pc(4," Pack the entire app to a "),o.Wb(5,"em"),o.Pc(6,"zip"),o.Vb(),o.Pc(7," folder which can be imported again to another site. For further help visit "),o.Wb(8,"a",3),o.Pc(9,"2sxc Help"),o.Vb(),o.Pc(10,".\n"),o.Vb(),o.Nc(11,b,39,17,"div",4)),2&t&&(o.Bb(11),o.qc("ngIf",n.appInfo))},directives:[i.i,c.t,r.a,a.r,a.u,s.b],styles:[".app-info__title[_ngcontent-%COMP%]{font-size:18px;margin:24px 0 0;font-weight:700}.app-info__content[_ngcontent-%COMP%]{font-size:14px}.app-info__content[_ngcontent-%COMP%]   li[_ngcontent-%COMP%]{padding:2px 0}.options-wrapper[_ngcontent-%COMP%]{margin:24px 0}.option-box[_ngcontent-%COMP%]{margin:2px 0}.option-box__text[_ngcontent-%COMP%]{white-space:normal;font-size:14px}"]}),t}()}}]);
//# sourceMappingURL=33.js.map