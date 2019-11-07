!function(e){var t={};function n(r){if(t[r])return t[r].exports;var i=t[r]={i:r,l:!1,exports:{}};return e[r].call(i.exports,i,i.exports,n),i.l=!0,i.exports}n.m=e,n.c=t,n.d=function(e,t,r){n.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:r})},n.r=function(e){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},n.t=function(e,t){if(1&t&&(e=n(e)),8&t)return e;if(4&t&&"object"==typeof e&&e&&e.__esModule)return e;var r=Object.create(null);if(n.r(r),Object.defineProperty(r,"default",{enumerable:!0,value:e}),2&t&&"string"!=typeof e)for(var i in e)n.d(r,i,function(t){return e[t]}.bind(null,i));return r},n.n=function(e){var t=e&&e.__esModule?function(){return e.default}:function(){return e};return n.d(t,"a",t),t},n.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},n.p="",n(n.s=3)}([function(e,t,n){"use strict";var r,i=this&&this.__extends||(r=function(e,t){return(r=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(e,t){e.__proto__=t}||function(e,t){for(var n in t)t.hasOwnProperty(n)&&(e[n]=t[n])})(e,t)},function(e,t){function n(){this.constructor=e}r(e,t),e.prototype=null===t?Object.create(t):(n.prototype=t.prototype,new n)});Object.defineProperty(t,"__esModule",{value:!0});var o=function(e){function t(){return null!==e&&e.apply(this,arguments)||this}return i(t,e),t}(HTMLElement);t.EavCustomInputField=o;var a=function(e){function t(){return null!==e&&e.apply(this,arguments)||this}return i(t,e),t}(o);t.EavCustomInputFieldObservable=a},function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var r=n(6);t.customGpsIcons={mapMarker:r},t.buildTemplate=function(e,t){return e+"<style>\n"+t+"\n</style>"},t.parseLatLng=function(e){return JSON.parse(e.replace("latitude","lat").replace("longitude","lng"))},t.stringifyLatLng=function(e){return JSON.stringify(e).replace("lat","latitude").replace("lng","longitude")}},function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.defaultCoordinates={lat:47.17465989999999,lng:9.469142499999975};t.mapsParameters={mapApiUrl:"https://maps.googleapis.com/maps/api/js?key=AIzaSyDPhnNKpEg8FmY8nooE7Zwnue6SusxEnHE"}},function(e,t,n){n(4),e.exports=n(10)},function(e,t,n){"use strict";var r,i=this&&this.__extends||(r=function(e,t){return(r=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(e,t){e.__proto__=t}||function(e,t){for(var n in t)t.hasOwnProperty(n)&&(e[n]=t[n])})(e,t)},function(e,t){function n(){this.constructor=e}r(e,t),e.prototype=null===t?Object.create(t):(n.prototype=t.prototype,new n)});Object.defineProperty(t,"__esModule",{value:!0});var o=n(5),a=n(1),s=n(2),l=n(7),c=n(8),p=n(9),d=function(e){function t(){var t=e.call(this)||this;return t.mapApiUrl=s.mapsParameters.mapApiUrl,console.log("FieldCustomGps constructor called"),t.fieldInitialized=!1,t.eventListeners=[],t}return i(t,e),t.prototype.connectedCallback=function(){if(console.log("FieldCustomGps connectedCallback called"),!this.fieldInitialized){this.fieldInitialized=!0,this.innerHTML=a.buildTemplate(l,c),this.latInput=this.querySelector("#lat"),this.lngInput=this.querySelector("#lng");var e=this.querySelector("#address-mask-container");this.iconSearch=this.querySelector("#icon-search");var t=this.querySelector("#formatted-address-container");this.mapContainer=this.querySelector("#map");var n=this.experimental.allInputTypeNames.map(function(e){return e.name});-1!==n.indexOf(this.connector.field.settings.LatField)&&(this.latFieldName=this.connector.field.settings.LatField),-1!==n.indexOf(this.connector.field.settings.LongField)&&(this.lngFieldName=this.connector.field.settings.LongField);var r=this.connector.field.settings.AddressMask||this.connector.field.settings["Address Mask"];if(this.addressMaskService=new p.FieldMaskService(r,this.experimental.formGroup.controls,null,null),console.log("FieldCustomGps addressMask:",r),r&&(e.classList.remove("hidden"),t.innerText=this.addressMaskService.resolve()),!!window.google)this.mapScriptLoaded();else{var i=document.createElement("script");i.src=this.mapApiUrl,i.onload=this.mapScriptLoaded.bind(this),this.appendChild(i)}}},t.prototype.mapScriptLoaded=function(){console.log("FieldCustomGps mapScriptLoaded called"),this.map=new google.maps.Map(this.mapContainer,{zoom:15,center:s.defaultCoordinates,gestureHandling:"greedy"}),this.marker=new google.maps.Marker({position:s.defaultCoordinates,map:this.map,draggable:!0}),this.geocoder=new google.maps.Geocoder,this.connector.data.value?this.updateHtml(a.parseLatLng(this.connector.data.value)):this.updateHtml(s.defaultCoordinates);var e=this.onLatLngInputChange.bind(this);this.latInput.addEventListener("change",e),this.lngInput.addEventListener("change",e);var t=this.autoSelect.bind(this);this.iconSearch.addEventListener("click",t),this.eventListeners.push({element:this.latInput,type:"change",listener:e},{element:this.lngInput,type:"change",listener:e},{element:this.iconSearch,type:"click",listener:t}),this.marker.addListener("dragend",this.onMarkerDragend.bind(this))},t.prototype.updateHtml=function(e){this.latInput.value=e.lat?e.lat.toString():"",this.lngInput.value=e.lng?e.lng.toString():"",this.map.setCenter(e),this.marker.setPosition(e)},t.prototype.updateForm=function(e){this.connector.data.update(a.stringifyLatLng(e)),this.latFieldName&&this.experimental.updateField(this.latFieldName,e.lat),this.lngFieldName&&this.experimental.updateField(this.lngFieldName,e.lng)},t.prototype.onLatLngInputChange=function(){console.log("FieldCustomGps input changed");var e={lat:this.latInput.value.length>0?parseFloat(this.latInput.value):null,lng:this.lngInput.value.length>0?parseFloat(this.lngInput.value):null};this.updateHtml(e),this.updateForm(e)},t.prototype.autoSelect=function(){var e=this;console.log("FieldCustomGps geocoder called");var t=this.addressMaskService.resolve();this.geocoder.geocode({address:t},function(n,r){if(r===google.maps.GeocoderStatus.OK){var i=n[0].geometry.location,o={lat:i.lat(),lng:i.lng()};e.updateHtml(o),e.updateForm(o)}else alert("Could not locate address: "+t)})},t.prototype.onMarkerDragend=function(e){console.log("FieldCustomGps marker changed");var t={lat:e.latLng.lat(),lng:e.latLng.lng()};this.updateHtml(t),this.updateForm(t)},t.prototype.disconnectedCallback=function(){console.log("FieldCustomGps disconnectedCallback called"),window.google&&(google.maps.event.clearInstanceListeners(this.marker),google.maps.event.clearInstanceListeners(this.map)),this.eventListeners.forEach(function(e){var t=e.element,n=e.type,r=e.listener;t.removeEventListener(n,r)})},t}(o.EavExperimentalInputField);customElements.define("field-custom-gps",d)},function(e,t,n){"use strict";var r,i=this&&this.__extends||(r=function(e,t){return(r=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(e,t){e.__proto__=t}||function(e,t){for(var n in t)t.hasOwnProperty(n)&&(e[n]=t[n])})(e,t)},function(e,t){function n(){this.constructor=e}r(e,t),e.prototype=null===t?Object.create(t):(n.prototype=t.prototype,new n)});Object.defineProperty(t,"__esModule",{value:!0});var o=function(e){function t(){return null!==e&&e.apply(this,arguments)||this}return i(t,e),t}(n(0).EavCustomInputField);t.EavExperimentalInputField=o},function(e,t){e.exports='<svg aria-hidden="true" focusable="false" data-prefix="fas" data-icon="map-marker-alt" class="svg-inline--fa fa-map-marker-alt fa-w-12" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><path fill="currentColor" d="M172.268 501.67C26.97 291.031 0 269.413 0 192 0 85.961 85.961 0 192 0s192 85.961 192 192c0 77.413-26.97 99.031-172.268 309.67-9.535 13.774-29.93 13.773-39.464 0zM192 272c44.183 0 80-35.817 80-80s-35.817-80-80-80-80 35.817-80 80 35.817 80 80 80z"></path></svg>'},function(e,t){e.exports='<div class="custom-gps-container">\r\n  <div class="map-info">\r\n    <div class="input-component">\r\n      <label for="lat">Lat:</label>\r\n      <input id="lat" type="number" step="0.001" />\r\n    </div>\r\n    &nbsp;\r\n    <div class="input-component">\r\n      <label for="lng">Lng:</label>\r\n      <input id="lng" type="number" step="0.001" />\r\n    </div>\r\n  </div>\r\n\r\n  <div id="address-mask-container" class="map-info address-mask-container hidden">\r\n    <a id="icon-search" class="btn">\r\n      <i class="material-icons">search</i>\r\n    </a>\r\n    <span id="formatted-address-container"></span>\r\n  </div>\r\n\r\n  <div id="map" class="map-info__map"></div>\r\n</div>\r\n'},function(e,t){e.exports=".custom-gps-container {\r\n  display: flex;\r\n  flex-direction: column;\r\n  height: 100%;\r\n}\r\n\r\n.map-info {\r\n  flex: 0 0 32px;\r\n  display: flex;\r\n  flex-wrap: wrap;\r\n  align-items: center;\r\n  padding: 4px;\r\n  padding-left: 14px;\r\n  border-bottom: 1px solid #e1e1e1;\r\n  background: white;\r\n}\r\n\r\n.address-mask-container {\r\n  flex-wrap: nowrap;\r\n}\r\n\r\n.map-info label,\r\n.map-info #icon-search {\r\n  margin-right: 8px;\r\n  display: flex;\r\n  justify-content: center;\r\n  align-items: center;\r\n  font-size: 12px;\r\n  text-transform: uppercase;\r\n  padding: 4px;\r\n}\r\n\r\n.map-info #icon-search i {\r\n  font-size: 14px;\r\n  width: 14px;\r\n  height: 14px;\r\n  font-weight: bold;\r\n}\r\n\r\n.map-info input {\r\n  margin-right: 8px;\r\n  padding: 4px 16px;\r\n  border: none;\r\n  background: transparent;\r\n  outline: none !important;\r\n}\r\n\r\n.map-info__map {\r\n  flex: 1 1 auto;\r\n  width: 100%;\r\n  display: block;\r\n}\r\n\r\n.hidden {\r\n  display: none;\r\n}\r\n\r\n.btn {\r\n  border: 1px solid silver;\r\n  border-radius: 4px;\r\n}\r\n.btn:hover {\r\n  background-color: rgba(69, 79, 99, 0.08);\r\n  cursor: pointer;\r\n}\r\n\r\n.input-component {\r\n  display: flex;\r\n  background-color: rgba(69, 79, 99, 0.08);\r\n  padding: 4px;\r\n  border-radius: 4px 4px 0 0;\r\n  border-bottom: 1px solid silver;\r\n  margin: 8px 0;\r\n}\r\n.input-component:hover {\r\n  border-bottom: 1px solid #0087f4;\r\n}\r\n"},function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var r=function(){function e(e,t,n,r){this.changeEvent=n,this.fields=[],this.findFields=/\[.*?\]/gi,this.unwrapField=/[\[\]]/gi,this.subscriptions=[],this.mask=e,this.model=t,this.fields=this.fieldList(),r&&(this.preClean=r),t&&n&&this.watchAllFields()}return e.prototype.resolve=function(){var e=this,t=this.mask;return this.fields.forEach(function(n,r){var i=e.model.hasOwnProperty(n)&&e.model[n]&&e.model[n].value?e.model[n].value:"",o=e.preClean(n,i);t=t.replace("["+n+"]",o)}),t},e.prototype.fieldList=function(){var e=this,t=[];if(!this.mask)return t;var n=this.mask.match(this.findFields);return n?n.forEach(function(n,r){var i=n.replace(e.unwrapField,"");t.push(i)}):t.push(this.mask),t},e.prototype.preClean=function(e,t){return t},e.prototype.onChange=function(){console.log("FieldMaskService onChange called");var e=this.resolve();this.value!==e&&this.changeEvent(e),this.value=e},e.prototype.watchAllFields=function(){var e=this;console.log("FieldMaskService watchAllFields called"),this.fields.forEach(function(t){if(e.model[t]){var n=e.model[t].valueChanges.subscribe(function(t){return e.onChange()});e.subscriptions.push(n)}})},e.prototype.destroy=function(){this.subscriptions.forEach(function(e){return e.unsubscribe()})},e}();t.FieldMaskService=r},function(e,t,n){"use strict";var r,i=this&&this.__extends||(r=function(e,t){return(r=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(e,t){e.__proto__=t}||function(e,t){for(var n in t)t.hasOwnProperty(n)&&(e[n]=t[n])})(e,t)},function(e,t){function n(){this.constructor=e}r(e,t),e.prototype=null===t?Object.create(t):(n.prototype=t.prototype,new n)});Object.defineProperty(t,"__esModule",{value:!0});var o=n(0),a=n(1),s=n(2),l=n(11),c=n(12),p=function(e){function t(){var t=e.call(this)||this;return console.log("FieldCustomGpsPreview constructor called"),t}return i(t,e),t.prototype.connectedCallback=function(){var e=this;console.log("FieldCustomGpsPreview connectedCallback called"),this.innerHTML=a.buildTemplate(l,c),this.querySelector("#map-icon-container").innerHTML=a.customGpsIcons.mapMarker,this.latContainer=this.querySelector("#lat-container"),this.lngContainer=this.querySelector("#lng-container"),this.connector.data.value?this.updateHtml(a.parseLatLng(this.connector.data.value)):this.updateHtml(s.defaultCoordinates),this.connector.data.onValueChange(function(t){if(t){var n=a.parseLatLng(t);e.updateHtml(n)}else e.updateHtml(s.defaultCoordinates)})},t.prototype.updateHtml=function(e){this.latContainer.innerText=e.lat?e.lat.toString():"",this.lngContainer.innerText=e.lng?e.lng.toString():""},t.prototype.disconnectedCallback=function(){console.log("FieldCustomGpsPreview disconnectedCallback called")},t}(o.EavCustomInputField);customElements.define("field-custom-gps-preview",p)},function(e,t){e.exports='<div class="custom-gps-preview">\r\n    <div class="custom-gps-preview__text">\r\n        Lat: <span id="lat-container"></span>, Lng: <span id="lng-container"></span>\r\n    </div>\r\n    <div class="custom-gps-preview__spacer"></div>\r\n    <div id="map-icon-container" class="custom-gps-preview__map-icon"></div>\r\n</div>\r\n'},function(e,t){e.exports=".custom-gps-preview {\r\n  width: 100%;\r\n  display: flex;\r\n  align-items: center;\r\n  position: absolute;\r\n  top: 0;\r\n  bottom: 0;\r\n  font-size: 16px;\r\n  line-height: 20px;\r\n  margin-bottom: -2px;\r\n}\r\n\r\n.custom-gps-preview__text {\r\n  flex-shrink: 0;\r\n  max-width: 80%;\r\n  overflow: hidden;\r\n  white-space: nowrap;\r\n  text-overflow: ellipsis;\r\n}\r\n\r\n.custom-gps-preview__spacer {\r\n  flex-grow: 1;\r\n}\r\n\r\n.custom-gps-preview__map-icon {\r\n  flex-shrink: 0;\r\n  margin-right: 4px;\r\n  display: flex;\r\n  align-items: center;\r\n  justify-content: center;\r\n}\r\n\r\n.custom-gps-preview__map-icon svg {\r\n  width: 20px;\r\n  height: 20px;\r\n  line-height: 20px;\r\n}\r\n"}]);
//# sourceMappingURL=https://sources.2sxc.org/10.20.00/ng-edit/elements/field-custom-gps/gps-picker.js.map