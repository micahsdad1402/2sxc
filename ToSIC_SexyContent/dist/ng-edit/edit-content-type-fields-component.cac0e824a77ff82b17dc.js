(window.webpackJsonp=window.webpackJsonp||[]).push([[27],{"87pQ":function(t,e,n){"use strict";n.r(e);var i=n("D57K"),o=n("DOM6"),c={Boolean:{label:"Boolean (yes/no)",description:"Yes/no or true/false values"},Custom:{label:"Custom - ui-tools or custom types",description:"Use for things like gps-pickers (which writes into multiple fields) or for custom-data which serializes something exotic into the db like an array, a custom json or anything"},DateTime:{label:"Date and/or time",description:"For date, time or combined values"},Empty:{label:"Empty - for form-titles etc.",description:"Use to structure your form"},Entity:{label:"Entity (other content-items)",description:"One or more other content-items"},Hyperlink:{label:"Link / file reference",description:"Hyperlink or reference to a picture / file"},Number:{label:"Number",description:"Any kind of number"},String:{label:"Text / string",description:"Any kind of text"}},r=n("WwKs"),a=n("1C3z"),s=n("BLjT"),p=n("5/c3"),l=n("/NRo"),d=n("/Foi"),u=n("2pW/"),b=n("8AiQ"),f=n("ZSGP"),h=n("9HSk"),y=n("hOvr"),m=n("TDrE"),g=n("LuBX"),T=n("r4gC"),v=n("Uk43"),S=n("OeRG");function I(t,e){1&t&&(a.Ub(0),a.Pc(1,"Fields"),a.Tb())}function O(t,e){1&t&&(a.Ub(0),a.Pc(1,"Edit Field"),a.Tb())}function W(t,e){1&t&&(a.Ub(0),a.Pc(1,"Add Fields"),a.Tb())}function C(t,e){if(1&t&&(a.Wb(0,"app-field-hint",21),a.Pc(1),a.Vb()),2&t){var n=a.ic(4);a.qc("isError",!0),a.Bb(1),a.Qc(n.contentTypeNameError)}}function w(t,e){if(1&t&&(a.Ub(0),a.Nc(1,C,2,2,"app-field-hint",20),a.Tb()),2&t){a.ic();var n=a.Cc(6);a.Bb(1),a.qc("ngIf",n.errors.pattern)}}function B(t,e){if(1&t&&(a.Wb(0,"mat-option",22),a.Wb(1,"mat-icon"),a.Pc(2),a.Vb(),a.Wb(3,"span"),a.Pc(4),a.Vb(),a.Vb()),2&t){var n=e.$implicit;a.qc("value",n.name),a.Bb(2),a.Qc(n.icon),a.Bb(2),a.Qc(n.label)}}function V(t,e){if(1&t&&(a.Wb(0,"mat-option",22),a.Pc(1),a.Vb()),2&t){var n=e.$implicit;a.qc("value",n.inputType),a.Bb(1),a.Rc(" ",n.label,"")}}function P(t,e){if(1&t){var n=a.Xb();a.Wb(0,"div",10),a.Wb(1,"div",11),a.Wb(2,"mat-form-field",12),a.Wb(3,"mat-label"),a.Pc(4,"Name"),a.Vb(),a.Wb(5,"input",13,14),a.ec("ngModelChange",(function(t){return a.Ec(n),e.$implicit.StaticName=t})),a.Vb(),a.Vb(),a.Nc(7,w,2,1,"ng-container",15),a.Vb(),a.Wb(8,"div",11),a.Wb(9,"mat-form-field",12),a.Wb(10,"mat-label"),a.Pc(11,"Data Type"),a.Vb(),a.Wb(12,"mat-select",16),a.ec("selectionChange",(function(){a.Ec(n);var t=e.index,i=a.ic(2);return i.resetInputType(t),i.calculateInputTypeOptions(t),i.calculateHints(t)}))("ngModelChange",(function(t){return a.Ec(n),e.$implicit.Type=t})),a.Wb(13,"mat-select-trigger"),a.Wb(14,"mat-icon",17),a.Pc(15),a.Vb(),a.Wb(16,"span"),a.Pc(17),a.Vb(),a.Vb(),a.Nc(18,B,5,3,"mat-option",18),a.Vb(),a.Vb(),a.Wb(19,"app-field-hint"),a.Pc(20),a.Vb(),a.Vb(),a.Wb(21,"div",11),a.Wb(22,"mat-form-field",12),a.Wb(23,"mat-label"),a.Pc(24,"Input Type"),a.Vb(),a.Wb(25,"mat-select",19),a.ec("selectionChange",(function(){a.Ec(n);var t=e.index;return a.ic(2).calculateHints(t)}))("ngModelChange",(function(t){return a.Ec(n),e.$implicit.InputType=t})),a.Nc(26,V,2,2,"mat-option",18),a.Vb(),a.Vb(),a.Wb(27,"app-field-hint"),a.Pc(28),a.Vb(),a.Vb(),a.Vb()}if(2&t){var i=e.$implicit,o=e.index,c=a.Cc(6),r=a.ic(2);a.Bb(5),a.qc("pattern",r.contentTypeNamePattern)("ngModel",i.StaticName)("name","StaticName"+o)("disabled",r.editMode),a.Bb(2),a.qc("ngIf",c.touched&&c.errors),a.Bb(5),a.qc("ngModel",i.Type)("name","Type"+o)("disabled",r.editMode),a.Bb(3),a.Qc(r.findIcon(i.Type)),a.Bb(2),a.Qc(i.Type),a.Bb(1),a.qc("ngForOf",r.dataTypes),a.Bb(2),a.Qc(r.dataTypeHints[o]),a.Bb(5),a.qc("ngModel",i.InputType)("name","InputType"+o),a.Bb(1),a.qc("ngForOf",r.filteredInputTypeOptions[o]),a.Bb(2),a.Qc(r.inputTypeHints[o])}}function N(t,e){if(1&t){var n=a.Xb();a.Wb(0,"form",4,5),a.ec("ngSubmit",(function(){return a.Ec(n),a.ic().onSubmit()})),a.Nc(2,P,29,16,"div",6),a.Wb(3,"div",7),a.Wb(4,"button",8),a.Pc(5,"Cancel"),a.Vb(),a.Wb(6,"button",9),a.Pc(7,"Save"),a.Vb(),a.Vb(),a.Vb()}if(2&t){var i=a.Cc(1),o=a.ic();a.Bb(2),a.qc("ngForOf",o.fields),a.Bb(4),a.qc("disabled",!i.form.valid)}}n.d(e,"EditContentTypeFieldsComponent",(function(){return M}));var M=function(){function t(t,e,n,i,o){this.dialogRef=t,this.route=e,this.contentTypesService=n,this.contentTypesFieldsService=i,this.snackBar=o,this.filteredInputTypeOptions=[],this.dataTypeHints=[],this.inputTypeHints=[],this.contentTypeNamePattern=r.b,this.contentTypeNameError=r.a,this.contentTypeStaticName=this.route.snapshot.paramMap.get("contentTypeStaticName")}return t.prototype.ngOnInit=function(){return Object(i.b)(this,void 0,void 0,(function(){var t,e,n,r,a,s,p;return Object(i.e)(this,(function(l){switch(l.label){case 0:return t=this,[4,this.contentTypesService.retrieveContentType(this.contentTypeStaticName).toPromise()];case 1:return t.contentType=l.sent(),[4,this.contentTypesFieldsService.getFields(this.contentType).toPromise()];case 2:return e=l.sent(),n=this.route.snapshot.paramMap.get("id")?parseInt(this.route.snapshot.paramMap.get("id"),10):null,this.editMode=null!==n,[4,this.contentTypesFieldsService.typeListRetrieve().toPromise()];case 3:return r=l.sent(),this.dataTypes=function(t){var e,n,r=[];try{for(var a=Object(i.i)(t),s=a.next();!s.done;s=a.next()){var p=s.value;r.push({name:p,label:c[p].label,icon:Object(o.a)(p),description:c[p].description})}}catch(l){e={error:l}}finally{try{s&&!s.done&&(n=a.return)&&n.call(a)}finally{if(e)throw e.error}}return r}(r),a=this,[4,this.contentTypesFieldsService.getInputTypesList().toPromise()];case 4:if(a.inputTypeOptions=l.sent(),this.editMode)s=e.find((function(t){return t.Id===n})),this.fields=[s];else for(this.fields=[],p=1;p<=8;p++)this.fields.push({Id:0,Type:"String",InputType:"string-default",StaticName:"",IsTitle:0===e.length,SortOrder:e.length+p});for(p=0;p<this.fields.length;p++)this.calculateInputTypeOptions(p),this.calculateHints(p);return[2]}}))}))},t.prototype.closeDialog=function(){this.dialogRef.close()},t.prototype.resetInputType=function(t){this.fields[t].InputType=this.fields[t].Type.toLowerCase()+"-default"},t.prototype.calculateInputTypeOptions=function(t){var e=this;this.filteredInputTypeOptions[t]=this.inputTypeOptions.filter((function(n){return n.dataType===e.fields[t].Type.toLowerCase()}))},t.prototype.calculateHints=function(t){var e=this,n=this.dataTypes.find((function(n){return n.name===e.fields[t].Type})),i=this.inputTypeOptions.find((function(n){return n.inputType===e.fields[t].InputType}));this.dataTypeHints[t]=n?n.description:"",this.inputTypeHints[t]=i?i.description:""},t.prototype.onSubmit=function(){return Object(i.b)(this,void 0,void 0,(function(){var t,e,n,o,c,r;return Object(i.e)(this,(function(a){switch(a.label){case 0:return this.snackBar.open("Saving..."),this.editMode?[4,this.contentTypesFieldsService.updateInputType(this.fields[0].Id,this.fields[0].StaticName,this.fields[0].InputType).toPromise()]:[3,2];case 1:return a.sent(),[3,10];case 2:t=this.fields.filter((function(t){return t.StaticName})),a.label=3;case 3:a.trys.push([3,8,9,10]),e=Object(i.i)(t),n=e.next(),a.label=4;case 4:return n.done?[3,7]:[4,this.contentTypesFieldsService.add(n.value,this.contentType.Id).toPromise()];case 5:a.sent(),a.label=6;case 6:return n=e.next(),[3,4];case 7:return[3,10];case 8:return o=a.sent(),c={error:o},[3,10];case 9:try{n&&!n.done&&(r=e.return)&&r.call(e)}finally{if(c)throw c.error}return[7];case 10:return this.snackBar.open("Saved",null,{duration:2e3}),this.closeDialog(),[2]}}))}))},t.prototype.findIcon=function(t){return Object(o.a)(t)},t.\u0275fac=function(e){return new(e||t)(a.Qb(s.h),a.Qb(p.a),a.Qb(l.a),a.Qb(d.a),a.Qb(u.b))},t.\u0275cmp=a.Kb({type:t,selectors:[["app-edit-content-type-fields"]],decls:6,vars:5,consts:[["mat-dialog-title",""],[1,"dialog-title-box",3,"ngSwitch"],[4,"ngSwitchCase"],[3,"ngSubmit",4,"ngIf"],[3,"ngSubmit"],["ngForm","ngForm"],["class","row-container",4,"ngFor","ngForOf"],[1,"dialog-actions-box"],["mat-raised-button","","type","button"],["mat-raised-button","","color","accent","type","submit",3,"disabled"],[1,"row-container"],[1,"edit-input"],["appearance","standard","color","accent"],["matInput","",3,"pattern","ngModel","name","disabled","ngModelChange"],["staticName","ngModel"],[4,"ngIf"],[3,"ngModel","name","disabled","selectionChange","ngModelChange"],[1,"type-icon"],[3,"value",4,"ngFor","ngForOf"],[3,"ngModel","name","selectionChange","ngModelChange"],[3,"isError",4,"ngIf"],[3,"isError"],[3,"value"]],template:function(t,e){1&t&&(a.Wb(0,"div",0),a.Wb(1,"div",1),a.Nc(2,I,2,0,"ng-container",2),a.Nc(3,O,2,0,"ng-container",2),a.Nc(4,W,2,0,"ng-container",2),a.Vb(),a.Vb(),a.Nc(5,N,8,2,"form",3)),2&t&&(a.Bb(1),a.qc("ngSwitch",e.editMode),a.Bb(1),a.qc("ngSwitchCase",void 0),a.Bb(1),a.qc("ngSwitchCase",!0),a.Bb(1),a.qc("ngSwitchCase",!1),a.Bb(1),a.qc("ngIf",e.fields))},directives:[s.i,b.x,b.y,b.t,f.G,f.s,f.t,b.s,h.b,y.c,y.g,m.b,f.c,f.x,f.r,f.u,g.a,g.c,T.a,v.a,S.l],styles:[".row-container[_ngcontent-%COMP%]{display:flex;justify-content:space-between}.edit-input[_ngcontent-%COMP%]{padding-bottom:8px;width:30%}.type-icon[_ngcontent-%COMP%]{width:inherit;height:inherit;margin-right:8px;font-size:inherit;vertical-align:top;line-height:inherit}"]}),t}()}}]);
//# sourceMappingURL=edit-content-type-fields-component.cac0e824a77ff82b17dc.js.map