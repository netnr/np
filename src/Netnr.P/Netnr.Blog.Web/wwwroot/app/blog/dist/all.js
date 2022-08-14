(()=>{var t={246:()=>{window.addEventListener("DOMContentLoaded",(function(){if(console){var t=[],e={msg:("","https://www.netnr.com"),style:("","background-image:-webkit-gradient( linear, left top, right top, color-stop(0, #f22), color-stop(0.15, #f2f), color-stop(0.3, #22f), color-stop(0.45, #2ff), color-stop(0.6, #25e),color-stop(0.75, #4f2), color-stop(0.9, #f2f), color-stop(1, #f22) );color:transparent;-webkit-background-clip: text;font-size:1.5em;")};t.push(e),e={msg:"",style:""};for(var o=[{name:"GitHub",link:"https://github.com/netnr"}],r=0;r<o.length;r++){var i=o[r];e.msg+="\r\n"+i.name+"：\r\n"+i.link+"\r\n"}t.push(e),"ActiveXObject"in window||t.map((function(t){console.log("%c"+t.msg,t.style)})),window.performance&&(window.funsi=setInterval((function(){var t=performance.timing;t.loadEventEnd&&(console.log(JSON.stringify({load:t.loadEventEnd-t.navigationStart,ready:t.domComplete-t.responseEnd,request:t.responseEnd-t.requestStart})),clearInterval(window.funsi))}),100))}if("04-05"===new Date((new Date).valueOf()+288e5).toISOString().substring(5,10)){var a=document.documentElement.style;a.filter="progid: DXImageTransform.Microsoft.BasicImage(grayscale = 1)",a["-webkit-filter"]="grayscale(100%)"}}),!1)},371:()=>{!function(t){var e=function(e,o,r){var i="upstreamCache";if(!(i in t))try{t[i]=JSON.parse(localStorage.getItem(i))||{}}catch(e){t[i]={}}var a=(new Date).valueOf(),s=e.join(","),n=t[i][s];if(n&&n.ok.length&&a-n.date<12e4)o(n.ok[0],n.ok);else{for(var l=[],c=0,d=0,h=e.length;d<h;){var u=e[d++];u=u.trim().toLowerCase().indexOf("//")>=0?u:"//"+u,fetch(u).then((function(t){t.ok?l.push(t.url):c++})).catch((function(){c++}))}var p=setInterval((function(){var e=!1,n=(new Date).valueOf();1==r&&l.length>0&&(e=!0);var d=n-a>(1!=r&&r?r:3e3);(l.length+c==h||d)&&(e=!0),e&&(clearInterval(p),t[i][s]={date:n,ok:l},localStorage.setItem(i,JSON.stringify(t[i])),o(l[0],l))}),1)}};t.upstream=e}(window)}},e={};function o(r){var i=e[r];if(void 0!==i)return i.exports;var a=e[r]={exports:{}};return t[r](a,a.exports,o),a.exports}(()=>{"use strict";var t,e,r=window.ShadowRoot&&(void 0===window.ShadyCSS||window.ShadyCSS.nativeShadow)&&"adoptedStyleSheets"in Document.prototype&&"replace"in CSSStyleSheet.prototype,i=Symbol(),a=new Map,s=class{constructor(t,e){if(this._$cssResult$=!0,e!==i)throw Error("CSSResult is not constructable. Use `unsafeCSS` or `css` instead.");this.cssText=t}get styleSheet(){let t=a.get(this.cssText);return r&&void 0===t&&(a.set(this.cssText,t=new CSSStyleSheet),t.replaceSync(this.cssText)),t}toString(){return this.cssText}},n=(t,...e)=>{const o=1===t.length?t[0]:e.reduce(((e,o,r)=>e+(t=>{if(!0===t._$cssResult$)return t.cssText;if("number"==typeof t)return t;throw Error("Value passed to 'css' function must be a 'css' function result: "+t+". Use 'unsafeCSS' to pass non-literal values, but take care to ensure page security.")})(o)+t[r+1]),t[0]);return new s(o,i)},l=r?t=>t:t=>t instanceof CSSStyleSheet?(t=>{let e="";for(const o of t.cssRules)e+=o.cssText;return(t=>new s("string"==typeof t?t:t+"",i))(e)})(t):t,c=window.trustedTypes,d=c?c.emptyScript:"",h=window.reactiveElementPolyfillSupport,u={toAttribute(t,e){switch(e){case Boolean:t=t?d:null;break;case Object:case Array:t=null==t?t:JSON.stringify(t)}return t},fromAttribute(t,e){let o=t;switch(e){case Boolean:o=null!==t;break;case Number:o=null===t?null:Number(t);break;case Object:case Array:try{o=JSON.parse(t)}catch(t){o=null}}return o}},p=(t,e)=>e!==t&&(e==e||t==t),f={attribute:!0,type:String,converter:u,reflect:!1,hasChanged:p},m=class extends HTMLElement{constructor(){super(),this._$Et=new Map,this.isUpdatePending=!1,this.hasUpdated=!1,this._$Ei=null,this.o()}static addInitializer(t){var e;null!==(e=this.l)&&void 0!==e||(this.l=[]),this.l.push(t)}static get observedAttributes(){this.finalize();const t=[];return this.elementProperties.forEach(((e,o)=>{const r=this._$Eh(o,e);void 0!==r&&(this._$Eu.set(r,o),t.push(r))})),t}static createProperty(t,e=f){if(e.state&&(e.attribute=!1),this.finalize(),this.elementProperties.set(t,e),!e.noAccessor&&!this.prototype.hasOwnProperty(t)){const o="symbol"==typeof t?Symbol():"__"+t,r=this.getPropertyDescriptor(t,o,e);void 0!==r&&Object.defineProperty(this.prototype,t,r)}}static getPropertyDescriptor(t,e,o){return{get(){return this[e]},set(r){const i=this[t];this[e]=r,this.requestUpdate(t,i,o)},configurable:!0,enumerable:!0}}static getPropertyOptions(t){return this.elementProperties.get(t)||f}static finalize(){if(this.hasOwnProperty("finalized"))return!1;this.finalized=!0;const t=Object.getPrototypeOf(this);if(t.finalize(),this.elementProperties=new Map(t.elementProperties),this._$Eu=new Map,this.hasOwnProperty("properties")){const t=this.properties,e=[...Object.getOwnPropertyNames(t),...Object.getOwnPropertySymbols(t)];for(const o of e)this.createProperty(o,t[o])}return this.elementStyles=this.finalizeStyles(this.styles),!0}static finalizeStyles(t){const e=[];if(Array.isArray(t)){const o=new Set(t.flat(1/0).reverse());for(const t of o)e.unshift(l(t))}else void 0!==t&&e.push(l(t));return e}static _$Eh(t,e){const o=e.attribute;return!1===o?void 0:"string"==typeof o?o:"string"==typeof t?t.toLowerCase():void 0}o(){var t;this._$Ep=new Promise((t=>this.enableUpdating=t)),this._$AL=new Map,this._$Em(),this.requestUpdate(),null===(t=this.constructor.l)||void 0===t||t.forEach((t=>t(this)))}addController(t){var e,o;(null!==(e=this._$Eg)&&void 0!==e?e:this._$Eg=[]).push(t),void 0!==this.renderRoot&&this.isConnected&&(null===(o=t.hostConnected)||void 0===o||o.call(t))}removeController(t){var e;null===(e=this._$Eg)||void 0===e||e.splice(this._$Eg.indexOf(t)>>>0,1)}_$Em(){this.constructor.elementProperties.forEach(((t,e)=>{this.hasOwnProperty(e)&&(this._$Et.set(e,this[e]),delete this[e])}))}createRenderRoot(){var t;const e=null!==(t=this.shadowRoot)&&void 0!==t?t:this.attachShadow(this.constructor.shadowRootOptions);return o=e,i=this.constructor.elementStyles,r?o.adoptedStyleSheets=i.map((t=>t instanceof CSSStyleSheet?t:t.styleSheet)):i.forEach((t=>{const e=document.createElement("style"),r=window.litNonce;void 0!==r&&e.setAttribute("nonce",r),e.textContent=t.cssText,o.appendChild(e)})),e;var o,i}connectedCallback(){var t;void 0===this.renderRoot&&(this.renderRoot=this.createRenderRoot()),this.enableUpdating(!0),null===(t=this._$Eg)||void 0===t||t.forEach((t=>{var e;return null===(e=t.hostConnected)||void 0===e?void 0:e.call(t)}))}enableUpdating(t){}disconnectedCallback(){var t;null===(t=this._$Eg)||void 0===t||t.forEach((t=>{var e;return null===(e=t.hostDisconnected)||void 0===e?void 0:e.call(t)}))}attributeChangedCallback(t,e,o){this._$AK(t,o)}_$ES(t,e,o=f){var r,i;const a=this.constructor._$Eh(t,o);if(void 0!==a&&!0===o.reflect){const s=(null!==(i=null===(r=o.converter)||void 0===r?void 0:r.toAttribute)&&void 0!==i?i:u.toAttribute)(e,o.type);this._$Ei=t,null==s?this.removeAttribute(a):this.setAttribute(a,s),this._$Ei=null}}_$AK(t,e){var o,r,i;const a=this.constructor,s=a._$Eu.get(t);if(void 0!==s&&this._$Ei!==s){const t=a.getPropertyOptions(s),n=t.converter,l=null!==(i=null!==(r=null===(o=n)||void 0===o?void 0:o.fromAttribute)&&void 0!==r?r:"function"==typeof n?n:null)&&void 0!==i?i:u.fromAttribute;this._$Ei=s,this[s]=l(e,t.type),this._$Ei=null}}requestUpdate(t,e,o){let r=!0;void 0!==t&&(((o=o||this.constructor.getPropertyOptions(t)).hasChanged||p)(this[t],e)?(this._$AL.has(t)||this._$AL.set(t,e),!0===o.reflect&&this._$Ei!==t&&(void 0===this._$EC&&(this._$EC=new Map),this._$EC.set(t,o))):r=!1),!this.isUpdatePending&&r&&(this._$Ep=this._$E_())}async _$E_(){this.isUpdatePending=!0;try{await this._$Ep}catch(t){Promise.reject(t)}const t=this.scheduleUpdate();return null!=t&&await t,!this.isUpdatePending}scheduleUpdate(){return this.performUpdate()}performUpdate(){var t;if(!this.isUpdatePending)return;this.hasUpdated,this._$Et&&(this._$Et.forEach(((t,e)=>this[e]=t)),this._$Et=void 0);let e=!1;const o=this._$AL;try{e=this.shouldUpdate(o),e?(this.willUpdate(o),null===(t=this._$Eg)||void 0===t||t.forEach((t=>{var e;return null===(e=t.hostUpdate)||void 0===e?void 0:e.call(t)})),this.update(o)):this._$EU()}catch(t){throw e=!1,this._$EU(),t}e&&this._$AE(o)}willUpdate(t){}_$AE(t){var e;null===(e=this._$Eg)||void 0===e||e.forEach((t=>{var e;return null===(e=t.hostUpdated)||void 0===e?void 0:e.call(t)})),this.hasUpdated||(this.hasUpdated=!0,this.firstUpdated(t)),this.updated(t)}_$EU(){this._$AL=new Map,this.isUpdatePending=!1}get updateComplete(){return this.getUpdateComplete()}getUpdateComplete(){return this._$Ep}shouldUpdate(t){return!0}update(t){void 0!==this._$EC&&(this._$EC.forEach(((t,e)=>this._$ES(e,this[e],t))),this._$EC=void 0),this._$EU()}updated(t){}firstUpdated(t){}};m.finalized=!0,m.elementProperties=new Map,m.elementStyles=[],m.shadowRootOptions={mode:"open"},null==h||h({ReactiveElement:m}),(null!==(t=globalThis.reactiveElementVersions)&&void 0!==t?t:globalThis.reactiveElementVersions=[]).push("1.3.2");var b=globalThis.trustedTypes,g=b?b.createPolicy("lit-html",{createHTML:t=>t}):void 0,v=`lit$${(Math.random()+"").slice(9)}$`,y="?"+v,w=`<${y}>`,_=document,x=(t="")=>_.createComment(t),k=t=>null===t||"object"!=typeof t&&"function"!=typeof t,$=Array.isArray,C=t=>{var e;return $(t)||"function"==typeof(null===(e=t)||void 0===e?void 0:e[Symbol.iterator])},z=/<(?:(!--|\/[^a-zA-Z])|(\/?[a-zA-Z][^>\s]*)|(\/?$))/g,S=/-->/g,A=/>/g,T=/>|[ 	\n\r](?:([^\s"'>=/]+)([ 	\n\r]*=[ 	\n\r]*(?:[^ 	\n\r"'`<>=]|("|')|))|$)/g,E=/'/g,D=/"/g,L=/^(?:script|style|textarea|title)$/i,M=t=>(e,...o)=>({_$litType$:t,strings:e,values:o}),O=M(1),F=M(2),I=Symbol.for("lit-noChange"),B=Symbol.for("lit-nothing"),P=new WeakMap,V=_.createTreeWalker(_,129,null,!1),R=(t,e)=>{const o=t.length-1,r=[];let i,a=2===e?"<svg>":"",s=z;for(let e=0;e<o;e++){const o=t[e];let n,l,c=-1,d=0;for(;d<o.length&&(s.lastIndex=d,l=s.exec(o),null!==l);)d=s.lastIndex,s===z?"!--"===l[1]?s=S:void 0!==l[1]?s=A:void 0!==l[2]?(L.test(l[2])&&(i=RegExp("</"+l[2],"g")),s=T):void 0!==l[3]&&(s=T):s===T?">"===l[0]?(s=null!=i?i:z,c=-1):void 0===l[1]?c=-2:(c=s.lastIndex-l[2].length,n=l[1],s=void 0===l[3]?T:'"'===l[3]?D:E):s===D||s===E?s=T:s===S||s===A?s=z:(s=T,i=void 0);const h=s===T&&t[e+1].startsWith("/>")?" ":"";a+=s===z?o+w:c>=0?(r.push(n),o.slice(0,c)+"$lit$"+o.slice(c)+v+h):o+v+(-2===c?(r.push(void 0),e):h)}const n=a+(t[o]||"<?>")+(2===e?"</svg>":"");if(!Array.isArray(t)||!t.hasOwnProperty("raw"))throw Error("invalid template strings array");return[void 0!==g?g.createHTML(n):n,r]},U=class{constructor({strings:t,_$litType$:e},o){let r;this.parts=[];let i=0,a=0;const s=t.length-1,n=this.parts,[l,c]=R(t,e);if(this.el=U.createElement(l,o),V.currentNode=this.el.content,2===e){const t=this.el.content,e=t.firstChild;e.remove(),t.append(...e.childNodes)}for(;null!==(r=V.nextNode())&&n.length<s;){if(1===r.nodeType){if(r.hasAttributes()){const t=[];for(const e of r.getAttributeNames())if(e.endsWith("$lit$")||e.startsWith(v)){const o=c[a++];if(t.push(e),void 0!==o){const t=r.getAttribute(o.toLowerCase()+"$lit$").split(v),e=/([.?@])?(.*)/.exec(o);n.push({type:1,index:i,name:e[2],strings:t,ctor:"."===e[1]?W:"?"===e[1]?G:"@"===e[1]?Z:X})}else n.push({type:6,index:i})}for(const e of t)r.removeAttribute(e)}if(L.test(r.tagName)){const t=r.textContent.split(v),e=t.length-1;if(e>0){r.textContent=b?b.emptyScript:"";for(let o=0;o<e;o++)r.append(t[o],x()),V.nextNode(),n.push({type:2,index:++i});r.append(t[e],x())}}}else if(8===r.nodeType)if(r.data===y)n.push({type:2,index:i});else{let t=-1;for(;-1!==(t=r.data.indexOf(v,t+1));)n.push({type:7,index:i}),t+=v.length-1}i++}}static createElement(t,e){const o=_.createElement("template");return o.innerHTML=t,o}};function N(t,e,o=t,r){var i,a,s,n;if(e===I)return e;let l=void 0!==r?null===(i=o._$Cl)||void 0===i?void 0:i[r]:o._$Cu;const c=k(e)?void 0:e._$litDirective$;return(null==l?void 0:l.constructor)!==c&&(null===(a=null==l?void 0:l._$AO)||void 0===a||a.call(l,!1),void 0===c?l=void 0:(l=new c(t),l._$AT(t,o,r)),void 0!==r?(null!==(s=(n=o)._$Cl)&&void 0!==s?s:n._$Cl=[])[r]=l:o._$Cu=l),void 0!==l&&(e=N(t,l._$AS(t,e.values),l,r)),e}var H,q,j=class{constructor(t,e){this.v=[],this._$AN=void 0,this._$AD=t,this._$AM=e}get parentNode(){return this._$AM.parentNode}get _$AU(){return this._$AM._$AU}p(t){var e;const{el:{content:o},parts:r}=this._$AD,i=(null!==(e=null==t?void 0:t.creationScope)&&void 0!==e?e:_).importNode(o,!0);V.currentNode=i;let a=V.nextNode(),s=0,n=0,l=r[0];for(;void 0!==l;){if(s===l.index){let e;2===l.type?e=new K(a,a.nextSibling,this,t):1===l.type?e=new l.ctor(a,l.name,l.strings,this,t):6===l.type&&(e=new Q(a,this,t)),this.v.push(e),l=r[++n]}s!==(null==l?void 0:l.index)&&(a=V.nextNode(),s++)}return i}m(t){let e=0;for(const o of this.v)void 0!==o&&(void 0!==o.strings?(o._$AI(t,o,e),e+=o.strings.length-2):o._$AI(t[e])),e++}},K=class{constructor(t,e,o,r){var i;this.type=2,this._$AH=B,this._$AN=void 0,this._$AA=t,this._$AB=e,this._$AM=o,this.options=r,this._$Cg=null===(i=null==r?void 0:r.isConnected)||void 0===i||i}get _$AU(){var t,e;return null!==(e=null===(t=this._$AM)||void 0===t?void 0:t._$AU)&&void 0!==e?e:this._$Cg}get parentNode(){let t=this._$AA.parentNode;const e=this._$AM;return void 0!==e&&11===t.nodeType&&(t=e.parentNode),t}get startNode(){return this._$AA}get endNode(){return this._$AB}_$AI(t,e=this){t=N(this,t,e),k(t)?t===B||null==t||""===t?(this._$AH!==B&&this._$AR(),this._$AH=B):t!==this._$AH&&t!==I&&this.$(t):void 0!==t._$litType$?this.T(t):void 0!==t.nodeType?this.k(t):C(t)?this.S(t):this.$(t)}M(t,e=this._$AB){return this._$AA.parentNode.insertBefore(t,e)}k(t){this._$AH!==t&&(this._$AR(),this._$AH=this.M(t))}$(t){this._$AH!==B&&k(this._$AH)?this._$AA.nextSibling.data=t:this.k(_.createTextNode(t)),this._$AH=t}T(t){var e;const{values:o,_$litType$:r}=t,i="number"==typeof r?this._$AC(t):(void 0===r.el&&(r.el=U.createElement(r.h,this.options)),r);if((null===(e=this._$AH)||void 0===e?void 0:e._$AD)===i)this._$AH.m(o);else{const t=new j(i,this),e=t.p(this.options);t.m(o),this.k(e),this._$AH=t}}_$AC(t){let e=P.get(t.strings);return void 0===e&&P.set(t.strings,e=new U(t)),e}S(t){$(this._$AH)||(this._$AH=[],this._$AR());const e=this._$AH;let o,r=0;for(const i of t)r===e.length?e.push(o=new K(this.M(x()),this.M(x()),this,this.options)):o=e[r],o._$AI(i),r++;r<e.length&&(this._$AR(o&&o._$AB.nextSibling,r),e.length=r)}_$AR(t=this._$AA.nextSibling,e){var o;for(null===(o=this._$AP)||void 0===o||o.call(this,!1,!0,e);t&&t!==this._$AB;){const e=t.nextSibling;t.remove(),t=e}}setConnected(t){var e;void 0===this._$AM&&(this._$Cg=t,null===(e=this._$AP)||void 0===e||e.call(this,t))}},X=class{constructor(t,e,o,r,i){this.type=1,this._$AH=B,this._$AN=void 0,this.element=t,this.name=e,this._$AM=r,this.options=i,o.length>2||""!==o[0]||""!==o[1]?(this._$AH=Array(o.length-1).fill(new String),this.strings=o):this._$AH=B}get tagName(){return this.element.tagName}get _$AU(){return this._$AM._$AU}_$AI(t,e=this,o,r){const i=this.strings;let a=!1;if(void 0===i)t=N(this,t,e,0),a=!k(t)||t!==this._$AH&&t!==I,a&&(this._$AH=t);else{const r=t;let s,n;for(t=i[0],s=0;s<i.length-1;s++)n=N(this,r[o+s],e,s),n===I&&(n=this._$AH[s]),a||(a=!k(n)||n!==this._$AH[s]),n===B?t=B:t!==B&&(t+=(null!=n?n:"")+i[s+1]),this._$AH[s]=n}a&&!r&&this.C(t)}C(t){t===B?this.element.removeAttribute(this.name):this.element.setAttribute(this.name,null!=t?t:"")}},W=class extends X{constructor(){super(...arguments),this.type=3}C(t){this.element[this.name]=t===B?void 0:t}},Y=b?b.emptyScript:"",G=class extends X{constructor(){super(...arguments),this.type=4}C(t){t&&t!==B?this.element.setAttribute(this.name,Y):this.element.removeAttribute(this.name)}},Z=class extends X{constructor(t,e,o,r,i){super(t,e,o,r,i),this.type=5}_$AI(t,e=this){var o;if((t=null!==(o=N(this,t,e,0))&&void 0!==o?o:B)===I)return;const r=this._$AH,i=t===B&&r!==B||t.capture!==r.capture||t.once!==r.once||t.passive!==r.passive,a=t!==B&&(r===B||i);i&&this.element.removeEventListener(this.name,this,r),a&&this.element.addEventListener(this.name,this,t),this._$AH=t}handleEvent(t){var e,o;"function"==typeof this._$AH?this._$AH.call(null!==(o=null===(e=this.options)||void 0===e?void 0:e.host)&&void 0!==o?o:this.element,t):this._$AH.handleEvent(t)}},Q=class{constructor(t,e,o){this.element=t,this.type=6,this._$AN=void 0,this._$AM=e,this.options=o}get _$AU(){return this._$AM._$AU}_$AI(t){N(this,t)}},J={L:"$lit$",P:v,V:y,I:1,N:R,R:j,j:C,D:N,H:K,F:X,O:G,W:Z,B:W,Z:Q},tt=window.litHtmlPolyfillSupport;null==tt||tt(U,K),(null!==(e=globalThis.litHtmlVersions)&&void 0!==e?e:globalThis.litHtmlVersions=[]).push("2.2.4");var et=class extends m{constructor(){super(...arguments),this.renderOptions={host:this},this._$Dt=void 0}createRenderRoot(){var t,e;const o=super.createRenderRoot();return null!==(t=(e=this.renderOptions).renderBefore)&&void 0!==t||(e.renderBefore=o.firstChild),o}update(t){const e=this.render();this.hasUpdated||(this.renderOptions.isConnected=this.isConnected),super.update(t),this._$Dt=((t,e,o)=>{var r,i;const a=null!==(r=null==o?void 0:o.renderBefore)&&void 0!==r?r:e;let s=a._$litPart$;if(void 0===s){const t=null!==(i=null==o?void 0:o.renderBefore)&&void 0!==i?i:null;a._$litPart$=s=new K(e.insertBefore(x(),t),t,void 0,null!=o?o:{})}return s._$AI(t),s})(e,this.renderRoot,this.renderOptions)}connectedCallback(){var t;super.connectedCallback(),null===(t=this._$Dt)||void 0===t||t.setConnected(!0)}disconnectedCallback(){var t;super.disconnectedCallback(),null===(t=this._$Dt)||void 0===t||t.setConnected(!1)}render(){return I}};et.finalized=!0,et._$litElement$=!0,null===(H=globalThis.litElementHydrateSupport)||void 0===H||H.call(globalThis,{LitElement:et});var ot=globalThis.litElementPolyfillSupport;null==ot||ot({LitElement:et}),(null!==(q=globalThis.litElementVersions)&&void 0!==q?q:globalThis.litElementVersions=[]).push("3.2.0");var rt=n`
  .form-control .form-control__label {
    display: none;
  }

  .form-control .form-control__help-text {
    display: none;
  }

  /* Label */
  .form-control--has-label .form-control__label {
    display: inline-block;
    color: var(--sl-input-label-color);
    margin-bottom: var(--sl-spacing-3x-small);
  }

  .form-control--has-label.form-control--small .form-control__label {
    font-size: var(--sl-input-label-font-size-small);
  }

  .form-control--has-label.form-control--medium .form-control__label {
    font-size: var(--sl-input-label-font-size-medium);
  }

  .form-control--has-label.form-control--large .form-control_label {
    font-size: var(--sl-input-label-font-size-large);
  }

  :host([required]) .form-control--has-label .form-control__label::after {
    content: var(--sl-input-required-content);
    margin-inline-start: var(--sl-input-required-content-offset);
  }

  /* Help text */
  .form-control--has-help-text .form-control__help-text {
    display: block;
    color: var(--sl-input-help-text-color);
  }

  .form-control--has-help-text .form-control__help-text ::slotted(*) {
    margin-top: var(--sl-spacing-3x-small);
  }

  .form-control--has-help-text.form-control--small .form-control__help-text {
    font-size: var(--sl-input-help-text-font-size-small);
  }

  .form-control--has-help-text.form-control--medium .form-control__help-text {
    font-size: var(--sl-input-help-text-font-size-medium);
  }

  .form-control--has-help-text.form-control--large .form-control__help-text {
    font-size: var(--sl-input-help-text-font-size-large);
  }
`,it=n`
  :host {
    box-sizing: border-box;
  }

  :host *,
  :host *::before,
  :host *::after {
    box-sizing: inherit;
  }

  [hidden] {
    display: none !important;
  }
`,at=n`
  ${it}
  ${rt}

  :host {
    display: block;
  }

  .textarea {
    display: flex;
    align-items: center;
    position: relative;
    width: 100%;
    font-family: var(--sl-input-font-family);
    font-weight: var(--sl-input-font-weight);
    line-height: var(--sl-line-height-normal);
    letter-spacing: var(--sl-input-letter-spacing);
    vertical-align: middle;
    transition: var(--sl-transition-fast) color, var(--sl-transition-fast) border, var(--sl-transition-fast) box-shadow,
      var(--sl-transition-fast) background-color;
    cursor: text;
  }

  /* Standard textareas */
  .textarea--standard {
    background-color: var(--sl-input-background-color);
    border: solid var(--sl-input-border-width) var(--sl-input-border-color);
  }

  .textarea--standard:hover:not(.textarea--disabled) {
    background-color: var(--sl-input-background-color-hover);
    border-color: var(--sl-input-border-color-hover);
  }
  .textarea--standard:hover:not(.textarea--disabled) .textarea__control {
    color: var(--sl-input-color-hover);
  }

  .textarea--standard.textarea--focused:not(.textarea--disabled) {
    background-color: var(--sl-input-background-color-focus);
    border-color: var(--sl-input-border-color-focus);
    color: var(--sl-input-color-focus);
    box-shadow: 0 0 0 var(--sl-focus-ring-width) var(--sl-input-focus-ring-color);
  }

  .textarea--standard.textarea--focused:not(.textarea--disabled) .textarea__control {
    color: var(--sl-input-color-focus);
  }

  .textarea--standard.textarea--disabled {
    background-color: var(--sl-input-background-color-disabled);
    border-color: var(--sl-input-border-color-disabled);
    opacity: 0.5;
    cursor: not-allowed;
  }

  .textarea--standard.textarea--disabled .textarea__control {
    color: var(--sl-input-color-disabled);
  }

  .textarea--standard.textarea--disabled .textarea__control::placeholder {
    color: var(--sl-input-placeholder-color-disabled);
  }

  /* Filled textareas */
  .textarea--filled {
    border: none;
    background-color: var(--sl-input-filled-background-color);
    color: var(--sl-input-color);
  }

  .textarea--filled:hover:not(.textarea--disabled) {
    background-color: var(--sl-input-filled-background-color-hover);
  }

  .textarea--filled.textarea--focused:not(.textarea--disabled) {
    background-color: var(--sl-input-filled-background-color-focus);
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .textarea--filled.textarea--disabled {
    background-color: var(--sl-input-filled-background-color-disabled);
    opacity: 0.5;
    cursor: not-allowed;
  }

  .textarea__control {
    flex: 1 1 auto;
    font-family: inherit;
    font-size: inherit;
    font-weight: inherit;
    line-height: 1.4;
    color: var(--sl-input-color);
    border: none;
    background: none;
    box-shadow: none;
    cursor: inherit;
    -webkit-appearance: none;
  }

  .textarea__control::-webkit-search-decoration,
  .textarea__control::-webkit-search-cancel-button,
  .textarea__control::-webkit-search-results-button,
  .textarea__control::-webkit-search-results-decoration {
    -webkit-appearance: none;
  }

  .textarea__control::placeholder {
    color: var(--sl-input-placeholder-color);
    user-select: none;
  }

  .textarea__control:focus {
    outline: none;
  }

  /*
   * Size modifiers
   */

  .textarea--small {
    border-radius: var(--sl-input-border-radius-small);
    font-size: var(--sl-input-font-size-small);
  }

  .textarea--small .textarea__control {
    padding: 0.5em var(--sl-input-spacing-small);
  }

  .textarea--medium {
    border-radius: var(--sl-input-border-radius-medium);
    font-size: var(--sl-input-font-size-medium);
  }

  .textarea--medium .textarea__control {
    padding: 0.5em var(--sl-input-spacing-medium);
  }

  .textarea--large {
    border-radius: var(--sl-input-border-radius-large);
    font-size: var(--sl-input-font-size-large);
  }

  .textarea--large .textarea__control {
    padding: 0.5em var(--sl-input-spacing-large);
  }

  /*
   * Resize types
   */

  .textarea--resize-none .textarea__control {
    resize: none;
  }

  .textarea--resize-vertical .textarea__control {
    resize: vertical;
  }

  .textarea--resize-auto .textarea__control {
    height: auto;
    resize: none;
  }
`,st=1,nt=2,lt=3,ct=4,dt=t=>(...e)=>({_$litDirective$:t,values:e}),ht=class{constructor(t){}get _$AU(){return this._$AM._$AU}_$AT(t,e,o){this._$Ct=t,this._$AM=e,this._$Ci=o}_$AS(t,e){return this.update(t,e)}update(t,e){return this.render(...e)}},{H:ut}=J,pt={},ft=dt(class extends ht{constructor(t){if(super(t),t.type!==lt&&t.type!==st&&t.type!==ct)throw Error("The `live` directive is not allowed on child or event bindings");if(!(t=>void 0===t.strings)(t))throw Error("`live` bindings can only contain a single expression")}render(t){return t}update(t,[e]){if(e===I||e===B)return e;const o=t.element,r=t.name;if(t.type===lt){if(e===o[r])return I}else if(t.type===ct){if(!!e===o.hasAttribute(r))return I}else if(t.type===st&&o.getAttribute(r)===e+"")return I;return((t,e=pt)=>{t._$AH=e})(t),e}}),mt=(t="value")=>(e,o)=>{const r=e.constructor,i=r.prototype.attributeChangedCallback;r.prototype.attributeChangedCallback=function(e,a,s){var n;const l=r.getPropertyOptions(t);if(e===("string"==typeof l.attribute?l.attribute:t)){const e=l.converter||u,r=("function"==typeof e?e:null!=(n=null==e?void 0:e.fromAttribute)?n:u.fromAttribute)(s,l.type);this[t]!==r&&(this[o]=r)}i.call(this,e,a,s)}},bt=Object.create,gt=Object.defineProperty,vt=Object.defineProperties,yt=Object.getOwnPropertyDescriptor,wt=Object.getOwnPropertyDescriptors,_t=Object.getOwnPropertyNames,xt=Object.getOwnPropertySymbols,kt=Object.getPrototypeOf,$t=Object.prototype.hasOwnProperty,Ct=Object.prototype.propertyIsEnumerable,zt=(t,e,o)=>e in t?gt(t,e,{enumerable:!0,configurable:!0,writable:!0,value:o}):t[e]=o,St=(t,e)=>{for(var o in e||(e={}))$t.call(e,o)&&zt(t,o,e[o]);if(xt)for(var o of xt(e))Ct.call(e,o)&&zt(t,o,e[o]);return t},At=(t,e)=>vt(t,wt(e)),Tt=(t,e)=>{var o={};for(var r in t)$t.call(t,r)&&e.indexOf(r)<0&&(o[r]=t[r]);if(null!=t&&xt)for(var r of xt(t))e.indexOf(r)<0&&Ct.call(t,r)&&(o[r]=t[r]);return o},Et=(t,e)=>function(){return e||(0,t[_t(t)[0]])((e={exports:{}}).exports,e),e.exports},Dt=(t,e,o,r)=>{for(var i,a=r>1?void 0:r?yt(e,o):e,s=t.length-1;s>=0;s--)(i=t[s])&&(a=(r?i(e,o,a):i(a))||a);return r&&a&&gt(e,o,a),a},Lt=class extends Event{constructor(t){super("formdata"),this.formData=t}},Mt=class extends FormData{constructor(t){var e=(...t)=>{super(...t)};t?(e(t),this.form=t,t.dispatchEvent(new Lt(this))):e()}append(t,e){if(!this.form)return super.append(t,e);let o=this.form.elements[t];if(o||(o=document.createElement("input"),o.type="hidden",o.name=t,this.form.appendChild(o)),this.has(t)){const r=this.getAll(t),i=r.indexOf(o.value);-1!==i&&r.splice(i,1),r.push(e),this.set(t,r)}else super.append(t,e);o.value=e}};function Ot(){window.FormData&&!function(){const t=document.createElement("form");let e=!1;return document.body.append(t),t.addEventListener("submit",(t=>{new FormData(t.target),t.preventDefault()})),t.addEventListener("formdata",(()=>e=!0)),t.dispatchEvent(new Event("submit",{cancelable:!0})),t.remove(),e}()&&(window.FormData=Mt,window.addEventListener("submit",(t=>{t.defaultPrevented||new FormData(t.target)})))}"complete"===document.readyState?Ot():window.addEventListener("DOMContentLoaded",(()=>Ot()));var Ft=new WeakMap,It=class{constructor(t,e){(this.host=t).addController(this),this.options=St({form:t=>t.closest("form"),name:t=>t.name,value:t=>t.value,defaultValue:t=>t.defaultValue,disabled:t=>t.disabled,reportValidity:t=>"function"!=typeof t.reportValidity||t.reportValidity(),setValue:(t,e)=>{t.value=e}},e),this.handleFormData=this.handleFormData.bind(this),this.handleFormSubmit=this.handleFormSubmit.bind(this),this.handleFormReset=this.handleFormReset.bind(this),this.reportFormValidity=this.reportFormValidity.bind(this)}hostConnected(){this.form=this.options.form(this.host),this.form&&(this.form.addEventListener("formdata",this.handleFormData),this.form.addEventListener("submit",this.handleFormSubmit),this.form.addEventListener("reset",this.handleFormReset),Ft.has(this.form)||(Ft.set(this.form,this.form.reportValidity),this.form.reportValidity=()=>this.reportFormValidity()))}hostDisconnected(){this.form&&(this.form.removeEventListener("formdata",this.handleFormData),this.form.removeEventListener("submit",this.handleFormSubmit),this.form.removeEventListener("reset",this.handleFormReset),Ft.has(this.form)&&(this.form.reportValidity=Ft.get(this.form),Ft.delete(this.form)),this.form=void 0)}handleFormData(t){const e=this.options.disabled(this.host),o=this.options.name(this.host),r=this.options.value(this.host);e||"string"!=typeof o||void 0===r||(Array.isArray(r)?r.forEach((e=>{t.formData.append(o,e.toString())})):t.formData.append(o,r.toString()))}handleFormSubmit(t){const e=this.options.disabled(this.host),o=this.options.reportValidity;!this.form||this.form.noValidate||e||o(this.host)||(t.preventDefault(),t.stopImmediatePropagation())}handleFormReset(){this.options.setValue(this.host,this.options.defaultValue(this.host))}reportFormValidity(){if(this.form&&!this.form.noValidate){const t=this.form.querySelectorAll("*");for(const e of t)if("function"==typeof e.reportValidity&&!e.reportValidity())return!1}return!0}doAction(t,e){if(this.form){const o=document.createElement("button");o.type=t,o.style.position="absolute",o.style.width="0",o.style.height="0",o.style.clipPath="inset(50%)",o.style.overflow="hidden",o.style.whiteSpace="nowrap",e&&["formaction","formmethod","formnovalidate","formtarget"].forEach((t=>{e.hasAttribute(t)&&o.setAttribute(t,e.getAttribute(t))})),this.form.append(o),o.click(),o.remove()}}reset(t){this.doAction("reset",t)}submit(t){this.doAction("submit",t)}},Bt=class{constructor(t,...e){this.slotNames=[],(this.host=t).addController(this),this.slotNames=e,this.handleSlotChange=this.handleSlotChange.bind(this)}hasDefaultSlot(){return[...this.host.childNodes].some((t=>{if(t.nodeType===t.TEXT_NODE&&""!==t.textContent.trim())return!0;if(t.nodeType===t.ELEMENT_NODE){const e=t;if("sl-visually-hidden"===e.tagName.toLowerCase())return!1;if(!e.hasAttribute("slot"))return!0}return!1}))}hasNamedSlot(t){return null!==this.host.querySelector(`:scope > [slot="${t}"]`)}test(t){return"[default]"===t?this.hasDefaultSlot():this.hasNamedSlot(t)}hostConnected(){this.host.shadowRoot.addEventListener("slotchange",this.handleSlotChange)}hostDisconnected(){this.host.shadowRoot.removeEventListener("slotchange",this.handleSlotChange)}handleSlotChange(t){const e=t.target;(this.slotNames.includes("[default]")&&!e.name||e.name&&this.slotNames.includes(e.name))&&this.host.requestUpdate()}};function Pt(t){if(!t)return"";const e=t.assignedNodes({flatten:!0});let o="";return[...e].forEach((t=>{t.nodeType===Node.TEXT_NODE&&(o+=t.textContent)})),o}var Vt=dt(class extends ht{constructor(t){var e;if(super(t),t.type!==st||"class"!==t.name||(null===(e=t.strings)||void 0===e?void 0:e.length)>2)throw Error("`classMap()` can only be used in the `class` attribute and must be the only part in the attribute.")}render(t){return" "+Object.keys(t).filter((e=>t[e])).join(" ")+" "}update(t,[e]){var o,r;if(void 0===this.et){this.et=new Set,void 0!==t.strings&&(this.st=new Set(t.strings.join(" ").split(/\s/).filter((t=>""!==t))));for(const t in e)e[t]&&!(null===(o=this.st)||void 0===o?void 0:o.has(t))&&this.et.add(t);return this.render(e)}const i=t.element.classList;this.et.forEach((t=>{t in e||(i.remove(t),this.et.delete(t))}));for(const t in e){const o=!!e[t];o===this.et.has(t)||(null===(r=this.st)||void 0===r?void 0:r.has(t))||(o?(i.add(t),this.et.add(t)):(i.remove(t),this.et.delete(t)))}return I}}),Rt=t=>null!=t?t:B
/**
 * @license
 * Copyright 2018 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */;
/**
 * @license
 * Copyright 2018 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */function Ut(t,e){const o=St({waitUntilFirstUpdate:!1},e);return(e,r)=>{const{update:i}=e;if(t in e){const a=t;e.update=function(t){if(t.has(a)){const e=t.get(a),i=this[a];e!==i&&(o.waitUntilFirstUpdate&&!this.hasUpdated||this[r](e,i))}i.call(this,t)}}}}function Nt(t,e,o){const r=new CustomEvent(e,St({bubbles:!0,cancelable:!1,composed:!0,detail:{}},o));return t.dispatchEvent(r),r}function Ht(t,e){return new Promise((o=>{t.addEventListener(e,(function r(i){i.target===t&&(t.removeEventListener(e,r),o())}))}))}var qt=t=>e=>"function"==typeof e?((t,e)=>(window.customElements.define(t,e),e))(t,e):((t,e)=>{const{kind:o,elements:r}=e;return{kind:o,elements:r,finisher(e){window.customElements.define(t,e)}}})(t,e),jt=(t,e)=>"method"===e.kind&&e.descriptor&&!("value"in e.descriptor)?At(St({},e),{finisher(o){o.createProperty(e.key,t)}}):{kind:"field",key:Symbol(),placement:"own",descriptor:{},originalKey:e.key,initializer(){"function"==typeof e.initializer&&(this[e.key]=e.initializer.call(this))},finisher(o){o.createProperty(e.key,t)}};function Kt(t){return(e,o)=>void 0!==o?((t,e,o)=>{e.constructor.createProperty(o,t)})(t,e,o):jt(t,e)}function Xt(t){return Kt(At(St({},t),{state:!0}))}var Wt,Yt=({finisher:t,descriptor:e})=>(o,r)=>{var i;if(void 0===r){const r=null!==(i=o.originalKey)&&void 0!==i?i:o.key,a=null!=e?{kind:"method",placement:"prototype",key:r,descriptor:e(o.key)}:At(St({},o),{key:r});return null!=t&&(a.finisher=function(e){t(e,r)}),a}{const i=o.constructor;void 0!==e&&Object.defineProperty(o,r,e(r)),null==t||t(i,r)}};function Gt(t,e){return Yt({descriptor:o=>{const r={get(){var e,o;return null!==(o=null===(e=this.renderRoot)||void 0===e?void 0:e.querySelector(t))&&void 0!==o?o:null},enumerable:!0,configurable:!0};if(e){const e="symbol"==typeof o?Symbol():"__"+o;r.get=function(){var o,r;return void 0===this[e]&&(this[e]=null!==(r=null===(o=this.renderRoot)||void 0===o?void 0:o.querySelector(t))&&void 0!==r?r:null),this[e]}}return r}})}null===(Wt=window.HTMLSlotElement)||void 0===Wt||Wt.prototype.assignedElements;
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
/**
 * @license
 * Copyright 2021 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */var Zt=class extends et{constructor(){super(...arguments),this.formSubmitController=new It(this),this.hasSlotController=new Bt(this,"help-text","label"),this.hasFocus=!1,this.size="medium",this.value="",this.filled=!1,this.label="",this.helpText="",this.rows=4,this.resize="vertical",this.disabled=!1,this.readonly=!1,this.required=!1,this.invalid=!1,this.defaultValue=""}connectedCallback(){super.connectedCallback(),this.resizeObserver=new ResizeObserver((()=>this.setTextareaHeight())),this.updateComplete.then((()=>{this.setTextareaHeight(),this.resizeObserver.observe(this.input)}))}firstUpdated(){this.invalid=!this.input.checkValidity()}disconnectedCallback(){super.disconnectedCallback(),this.resizeObserver.unobserve(this.input)}focus(t){this.input.focus(t)}blur(){this.input.blur()}select(){this.input.select()}scrollPosition(t){return t?("number"==typeof t.top&&(this.input.scrollTop=t.top),void("number"==typeof t.left&&(this.input.scrollLeft=t.left))):{top:this.input.scrollTop,left:this.input.scrollTop}}setSelectionRange(t,e,o="none"){this.input.setSelectionRange(t,e,o)}setRangeText(t,e,o,r="preserve"){this.input.setRangeText(t,e,o,r),this.value!==this.input.value&&(this.value=this.input.value,Nt(this,"sl-input")),this.value!==this.input.value&&(this.value=this.input.value,this.setTextareaHeight(),Nt(this,"sl-input"),Nt(this,"sl-change"))}reportValidity(){return this.input.reportValidity()}setCustomValidity(t){this.input.setCustomValidity(t),this.invalid=!this.input.checkValidity()}handleBlur(){this.hasFocus=!1,Nt(this,"sl-blur")}handleChange(){this.value=this.input.value,this.setTextareaHeight(),Nt(this,"sl-change")}handleDisabledChange(){this.input.disabled=this.disabled,this.invalid=!this.input.checkValidity()}handleFocus(){this.hasFocus=!0,Nt(this,"sl-focus")}handleInput(){this.value=this.input.value,this.setTextareaHeight(),Nt(this,"sl-input")}handleRowsChange(){this.setTextareaHeight()}handleValueChange(){this.invalid=!this.input.checkValidity()}setTextareaHeight(){"auto"===this.resize?(this.input.style.height="auto",this.input.style.height=`${this.input.scrollHeight}px`):this.input.style.height=void 0}render(){const t=this.hasSlotController.test("label"),e=this.hasSlotController.test("help-text"),o=!!this.label||!!t,r=!!this.helpText||!!e;return O`
      <div
        part="form-control"
        class=${Vt({"form-control":!0,"form-control--small":"small"===this.size,"form-control--medium":"medium"===this.size,"form-control--large":"large"===this.size,"form-control--has-label":o,"form-control--has-help-text":r})}
      >
        <label
          part="form-control-label"
          class="form-control__label"
          for="input"
          aria-hidden=${o?"false":"true"}
        >
          <slot name="label">${this.label}</slot>
        </label>

        <div part="form-control-input" class="form-control-input">
          <div
            part="base"
            class=${Vt({textarea:!0,"textarea--small":"small"===this.size,"textarea--medium":"medium"===this.size,"textarea--large":"large"===this.size,"textarea--standard":!this.filled,"textarea--filled":this.filled,"textarea--disabled":this.disabled,"textarea--focused":this.hasFocus,"textarea--empty":!this.value,"textarea--invalid":this.invalid,"textarea--resize-none":"none"===this.resize,"textarea--resize-vertical":"vertical"===this.resize,"textarea--resize-auto":"auto"===this.resize})}
          >
            <textarea
              part="textarea"
              id="input"
              class="textarea__control"
              name=${Rt(this.name)}
              .value=${ft(this.value)}
              ?disabled=${this.disabled}
              ?readonly=${this.readonly}
              ?required=${this.required}
              placeholder=${Rt(this.placeholder)}
              rows=${Rt(this.rows)}
              minlength=${Rt(this.minlength)}
              maxlength=${Rt(this.maxlength)}
              autocapitalize=${Rt(this.autocapitalize)}
              autocorrect=${Rt(this.autocorrect)}
              ?autofocus=${this.autofocus}
              spellcheck=${Rt(this.spellcheck)}
              enterkeyhint=${Rt(this.enterkeyhint)}
              inputmode=${Rt(this.inputmode)}
              aria-describedby="help-text"
              @change=${this.handleChange}
              @input=${this.handleInput}
              @focus=${this.handleFocus}
              @blur=${this.handleBlur}
            ></textarea>
          </div>
        </div>

        <div
          part="form-control-help-text"
          id="help-text"
          class="form-control__help-text"
          aria-hidden=${r?"false":"true"}
        >
          <slot name="help-text">${this.helpText}</slot>
        </div>
      </div>
    `}};Zt.styles=at,Dt([Gt(".textarea__control")],Zt.prototype,"input",2),Dt([Xt()],Zt.prototype,"hasFocus",2),Dt([Kt({reflect:!0})],Zt.prototype,"size",2),Dt([Kt()],Zt.prototype,"name",2),Dt([Kt()],Zt.prototype,"value",2),Dt([Kt({type:Boolean,reflect:!0})],Zt.prototype,"filled",2),Dt([Kt()],Zt.prototype,"label",2),Dt([Kt({attribute:"help-text"})],Zt.prototype,"helpText",2),Dt([Kt()],Zt.prototype,"placeholder",2),Dt([Kt({type:Number})],Zt.prototype,"rows",2),Dt([Kt()],Zt.prototype,"resize",2),Dt([Kt({type:Boolean,reflect:!0})],Zt.prototype,"disabled",2),Dt([Kt({type:Boolean,reflect:!0})],Zt.prototype,"readonly",2),Dt([Kt({type:Number})],Zt.prototype,"minlength",2),Dt([Kt({type:Number})],Zt.prototype,"maxlength",2),Dt([Kt({type:Boolean,reflect:!0})],Zt.prototype,"required",2),Dt([Kt({type:Boolean,reflect:!0})],Zt.prototype,"invalid",2),Dt([Kt()],Zt.prototype,"autocapitalize",2),Dt([Kt()],Zt.prototype,"autocorrect",2),Dt([Kt()],Zt.prototype,"autocomplete",2),Dt([Kt({type:Boolean})],Zt.prototype,"autofocus",2),Dt([Kt()],Zt.prototype,"enterkeyhint",2),Dt([Kt({type:Boolean})],Zt.prototype,"spellcheck",2),Dt([Kt()],Zt.prototype,"inputmode",2),Dt([mt()],Zt.prototype,"defaultValue",2),Dt([Ut("disabled",{waitUntilFirstUpdate:!0})],Zt.prototype,"handleDisabledChange",1),Dt([Ut("rows",{waitUntilFirstUpdate:!0})],Zt.prototype,"handleRowsChange",1),Dt([Ut("value",{waitUntilFirstUpdate:!0})],Zt.prototype,"handleValueChange",1),Zt=Dt([qt("sl-textarea")],Zt);var Qt=n`
  ${it}

  :host {
    --max-width: 20rem;
    --hide-delay: 0ms;
    --show-delay: 150ms;

    display: contents;
  }

  .tooltip {
    --arrow-size: var(--sl-tooltip-arrow-size);
    --arrow-color: var(--sl-tooltip-background-color);
  }

  .tooltip[placement^='top']::part(popup) {
    transform-origin: bottom;
  }

  .tooltip[placement^='bottom']::part(popup) {
    transform-origin: top;
  }

  .tooltip[placement^='left']::part(popup) {
    transform-origin: right;
  }

  .tooltip[placement^='right']::part(popup) {
    transform-origin: left;
  }

  .tooltip__body {
    max-width: var(--max-width);
    border-radius: var(--sl-tooltip-border-radius);
    background-color: var(--sl-tooltip-background-color);
    font-family: var(--sl-tooltip-font-family);
    font-size: var(--sl-tooltip-font-size);
    font-weight: var(--sl-tooltip-font-weight);
    line-height: var(--sl-tooltip-line-height);
    color: var(--sl-tooltip-color);
    padding: var(--sl-tooltip-padding);
    pointer-events: none;
    z-index: var(--sl-z-index-tooltip);
  }
`;function Jt(t,e,o){return new Promise((r=>{if((null==o?void 0:o.duration)===1/0)throw new Error("Promise-based animations must be finite.");const i=t.animate(e,At(St({},o),{duration:ee()?0:o.duration}));i.addEventListener("cancel",r,{once:!0}),i.addEventListener("finish",r,{once:!0})}))}function te(t){return(t=t.toString().toLowerCase()).indexOf("ms")>-1?parseFloat(t):t.indexOf("s")>-1?1e3*parseFloat(t):parseFloat(t)}function ee(){return window.matchMedia("(prefers-reduced-motion: reduce)").matches}function oe(t){return Promise.all(t.getAnimations().map((t=>new Promise((e=>{const o=requestAnimationFrame(e);t.addEventListener("cancel",(()=>o),{once:!0}),t.addEventListener("finish",(()=>o),{once:!0}),t.cancel()})))))}function re(t,e){return t.map((t=>At(St({},t),{height:"auto"===t.height?`${e}px`:t.height})))}var ie=new Map,ae=new WeakMap;function se(t){return null!=t?t:{keyframes:[],options:{duration:0}}}function ne(t,e){return"rtl"===e.toLowerCase()?{keyframes:t.rtlKeyframes||t.keyframes,options:t.options}:t}function le(t,e){ie.set(t,se(e))}function ce(t,e,o){const r=ae.get(t);if(null==r?void 0:r[e])return ne(r[e],o.dir);const i=ie.get(e);return i?ne(i,o.dir):{keyframes:[],options:{duration:0}}}var de,he=new Set,ue=new MutationObserver(be),pe=new Map,fe=document.documentElement.dir||"ltr",me=document.documentElement.lang||navigator.language;function be(){fe=document.documentElement.dir||"ltr",me=document.documentElement.lang||navigator.language,[...he.keys()].map((t=>{"function"==typeof t.requestUpdate&&t.requestUpdate()}))}ue.observe(document.documentElement,{attributes:!0,attributeFilter:["dir","lang"]});var ge=class extends class{constructor(t){this.host=t,this.host.addController(this)}hostConnected(){he.add(this.host)}hostDisconnected(){he.delete(this.host)}dir(){return`${this.host.dir||fe}`.toLowerCase()}lang(){return`${this.host.lang||me}`.toLowerCase()}term(t,...e){const o=this.lang().toLowerCase().slice(0,2),r=this.lang().length>2?this.lang().toLowerCase():"",i=pe.get(r),a=pe.get(o);let s;if(i&&i[t])s=i[t];else if(a&&a[t])s=a[t];else{if(!de||!de[t])return console.error(`No translation found for: ${String(t)}`),t;s=de[t]}return"function"==typeof s?s(...e):s}date(t,e){return t=new Date(t),new Intl.DateTimeFormat(this.lang(),e).format(t)}number(t,e){return t=Number(t),isNaN(t)?"":new Intl.NumberFormat(this.lang(),e).format(t)}relativeTime(t,e,o){return new Intl.RelativeTimeFormat(this.lang(),o).format(t,e)}}{},ve={$code:"en",$name:"English",$dir:"ltr",clearEntry:"Clear entry",close:"Close",copy:"Copy",currentValue:"Current value",hidePassword:"Hide password",progress:"Progress",remove:"Remove",resize:"Resize",scrollToEnd:"Scroll to end",scrollToStart:"Scroll to start",selectAColorFromTheScreen:"Select a color from the screen",showPassword:"Show password",toggleColorFormat:"Toggle color format"};!function(...t){t.map((t=>{const e=t.$code.toLowerCase();pe.has(e)?pe.set(e,Object.assign(Object.assign({},pe.get(e)),t)):pe.set(e,t),de||(de=t)})),be()}(ve);var ye=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.content="",this.placement="top",this.disabled=!1,this.distance=8,this.open=!1,this.skidding=0,this.trigger="hover focus",this.hoist=!1}connectedCallback(){super.connectedCallback(),this.handleBlur=this.handleBlur.bind(this),this.handleClick=this.handleClick.bind(this),this.handleFocus=this.handleFocus.bind(this),this.handleKeyDown=this.handleKeyDown.bind(this),this.handleMouseOver=this.handleMouseOver.bind(this),this.handleMouseOut=this.handleMouseOut.bind(this),this.updateComplete.then((()=>{this.addEventListener("blur",this.handleBlur,!0),this.addEventListener("focus",this.handleFocus,!0),this.addEventListener("click",this.handleClick),this.addEventListener("keydown",this.handleKeyDown),this.addEventListener("mouseover",this.handleMouseOver),this.addEventListener("mouseout",this.handleMouseOut)}))}firstUpdated(){this.body.hidden=!this.open,this.open&&(this.popup.active=!0,this.popup.reposition())}disconnectedCallback(){super.disconnectedCallback(),this.removeEventListener("blur",this.handleBlur,!0),this.removeEventListener("focus",this.handleFocus,!0),this.removeEventListener("click",this.handleClick),this.removeEventListener("keydown",this.handleKeyDown),this.removeEventListener("mouseover",this.handleMouseOver),this.removeEventListener("mouseout",this.handleMouseOut)}async show(){if(!this.open)return this.open=!0,Ht(this,"sl-after-show")}async hide(){if(this.open)return this.open=!1,Ht(this,"sl-after-hide")}getTarget(){const t=[...this.children].find((t=>"style"!==t.tagName.toLowerCase()&&"content"!==t.getAttribute("slot")));if(!t)throw new Error("Invalid tooltip target: no child element was found.");return t}handleBlur(){this.hasTrigger("focus")&&this.hide()}handleClick(){this.hasTrigger("click")&&(this.open?this.hide():this.show())}handleFocus(){this.hasTrigger("focus")&&this.show()}handleKeyDown(t){this.open&&"Escape"===t.key&&(t.stopPropagation(),this.hide())}handleMouseOver(){if(this.hasTrigger("hover")){const t=te(getComputedStyle(this).getPropertyValue("--show-delay"));clearTimeout(this.hoverTimeout),this.hoverTimeout=window.setTimeout((()=>this.show()),t)}}handleMouseOut(){if(this.hasTrigger("hover")){const t=te(getComputedStyle(this).getPropertyValue("--hide-delay"));clearTimeout(this.hoverTimeout),this.hoverTimeout=window.setTimeout((()=>this.hide()),t)}}async handleOpenChange(){if(this.open){if(this.disabled)return;Nt(this,"sl-show"),await oe(this.body),this.body.hidden=!1,this.popup.active=!0;const{keyframes:t,options:e}=ce(this,"tooltip.show",{dir:this.localize.dir()});await Jt(this.popup.popup,t,e),Nt(this,"sl-after-show")}else{Nt(this,"sl-hide"),await oe(this.body);const{keyframes:t,options:e}=ce(this,"tooltip.hide",{dir:this.localize.dir()});await Jt(this.popup.popup,t,e),this.popup.active=!1,this.body.hidden=!0,Nt(this,"sl-after-hide")}}async handleOptionsChange(){this.hasUpdated&&(await this.updateComplete,this.popup.reposition())}handleDisabledChange(){this.disabled&&this.open&&this.hide()}hasTrigger(t){return this.trigger.split(" ").includes(t)}render(){return O`
      <sl-popup
        part="base"
        class=${Vt({tooltip:!0,"tooltip--open":this.open})}
        placement=${this.placement}
        distance=${this.distance}
        skidding=${this.skidding}
        strategy=${this.hoist?"fixed":"absolute"}
        flip
        shift
        arrow
      >
        <slot slot="anchor" aria-describedby="tooltip"></slot>

        <div part="body" id="tooltip" class="tooltip__body" role="tooltip" aria-hidden=${this.open?"false":"true"}>
          <slot name="content" aria-live=${this.open?"polite":"off"}> ${this.content} </slot>
        </div>
      </sl-popup>
    `}};ye.styles=Qt,Dt([Gt("slot:not([name])")],ye.prototype,"defaultSlot",2),Dt([Gt(".tooltip__body")],ye.prototype,"body",2),Dt([Gt("sl-popup")],ye.prototype,"popup",2),Dt([Kt()],ye.prototype,"content",2),Dt([Kt()],ye.prototype,"placement",2),Dt([Kt({type:Boolean,reflect:!0})],ye.prototype,"disabled",2),Dt([Kt({type:Number})],ye.prototype,"distance",2),Dt([Kt({type:Boolean,reflect:!0})],ye.prototype,"open",2),Dt([Kt({type:Number})],ye.prototype,"skidding",2),Dt([Kt()],ye.prototype,"trigger",2),Dt([Kt({type:Boolean})],ye.prototype,"hoist",2),Dt([Ut("open",{waitUntilFirstUpdate:!0})],ye.prototype,"handleOpenChange",1),Dt([Ut("content"),Ut("distance"),Ut("hoist"),Ut("placement"),Ut("skidding")],ye.prototype,"handleOptionsChange",1),Dt([Ut("disabled")],ye.prototype,"handleDisabledChange",1),ye=Dt([qt("sl-tooltip")],ye),le("tooltip.show",{keyframes:[{opacity:0,transform:"scale(0.8)"},{opacity:1,transform:"scale(1)"}],options:{duration:150,easing:"ease"}}),le("tooltip.hide",{keyframes:[{opacity:1,transform:"scale(1)"},{opacity:0,transform:"scale(0.8)"}],options:{duration:150,easing:"ease"}});var we=n`
  ${it}

  :host {
    /*
     * These are actually used by tree item, but we define them here so they can more easily be set and all tree items
     * stay consistent.
     */
    --indent-guide-color: var(--sl-color-neutral-200);
    --indent-guide-offset: 0;
    --indent-guide-style: solid;
    --indent-guide-width: 0;
    --indent-size: var(--sl-spacing-large);

    display: block;
    isolation: isolate;

    /*
     * Tree item indentation uses the "em" unit to increment its width on each level, so setting the font size to zero
     * here removes the indentation for all the nodes on the first level.
     */
    font-size: 0;
  }
`,_e=n`
  ${it}

  :host {
    display: block;
    outline: 0;
    z-index: 0;
  }

  :host(:focus) {
    outline: 0;
  }

  slot:not([name])::slotted(sl-icon) {
    margin-inline-end: var(--sl-spacing-x-small);
  }

  .tree-item {
    position: relative;
    display: flex;
    align-items: stretch;
    flex-direction: column;
    color: var(--sl-color-neutral-700);
    user-select: none;
    white-space: nowrap;
  }

  .tree-item__checkbox {
    pointer-events: none;
  }

  .tree-item__expand-button,
  .tree-item__checkbox,
  .tree-item__label {
    font-family: var(--sl-font-sans);
    font-size: var(--sl-font-size-medium);
    font-weight: var(--sl-font-weight-normal);
    line-height: var(--sl-line-height-normal);
    letter-spacing: var(--sl-letter-spacing-normal);
  }

  .tree-item__checkbox::part(base) {
    display: flex;
    align-items: center;
  }

  .tree-item__indentation {
    display: block;
    width: 1em;
    flex-shrink: 0;
  }

  .tree-item__expand-button {
    display: flex;
    align-items: center;
    justify-content: center;
    box-sizing: content-box;
    color: var(--sl-color-neutral-500);
    padding: var(--sl-spacing-x-small);
    width: 1rem;
    height: 1rem;
    cursor: pointer;
  }

  .tree-item__expand-button--visible {
    cursor: pointer;
  }

  .tree-item__item {
    display: flex;
    align-items: center;
    border-inline-start: solid 3px transparent;
  }

  .tree-item--disabled .tree-item__item {
    opacity: 0.5;
    outline: none;
    cursor: not-allowed;
  }

  :host(:not([aria-disabled='true'])) .tree-item__item:hover {
    background-color: var(--sl-color-neutral-100);
  }

  :host(:focus-visible) .tree-item__item {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
    z-index: 2;
  }

  :host(:not([aria-disabled='true'])) .tree-item--selected .tree-item__item {
    background-color: var(--sl-color-neutral-100);
    border-inline-start-color: var(--sl-color-primary-600);
  }

  :host(:not([aria-disabled='true'])) .tree-item__expand-button {
    color: var(--sl-color-neutral-600);
  }

  .tree-item__label {
    display: flex;
    align-items: center;
    transition: var(--sl-transition-fast) color;
  }

  .tree-item__children {
    font-size: calc(1em + var(--indent-size, var(--sl-spacing-medium)));
  }

  /* Indentation lines */
  .tree-item__children {
    position: relative;
  }

  .tree-item__children::before {
    content: '';
    position: absolute;
    top: var(--indent-guide-offset);
    bottom: var(--indent-guide-offset);
    left: calc(1em - (var(--indent-guide-width) / 2) - 1px);
    border-inline-end: var(--indent-guide-width) var(--indent-guide-style) var(--indent-guide-color);
    z-index: 1;
  }

  .tree-item--rtl .tree-item__children::before {
    left: auto;
    right: 1em;
  }
`;function xe(t,e,o){return t?e():null==o?void 0:o()}function ke(t){return t&&"treeitem"===t.getAttribute("role")}var $e=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.indeterminate=!1,this.isLeaf=!1,this.loading=!1,this.selectable=!1,this.expanded=!1,this.selected=!1,this.disabled=!1,this.lazy=!1}connectedCallback(){super.connectedCallback(),this.setAttribute("role","treeitem"),this.setAttribute("tabindex","-1"),this.isNestedItem()&&(this.slot="children")}firstUpdated(){this.childrenContainer.hidden=!this.expanded,this.childrenContainer.style.height=this.expanded?"auto":"0",this.isLeaf=0===this.getChildrenItems().length,this.handleExpandedChange()}handleLoadingChange(){this.setAttribute("aria-busy",this.loading?"true":"false"),this.loading||this.animateExpand()}handleDisabledChange(){this.setAttribute("aria-disabled",this.disabled?"true":"false")}handleSelectedChange(){this.setAttribute("aria-selected",this.selected?"true":"false")}handleExpandedChange(){this.isLeaf?this.removeAttribute("aria-expanded"):this.setAttribute("aria-expanded",this.expanded?"true":"false")}handleExpandAnimation(){this.expanded?this.lazy?(this.loading=!0,Nt(this,"sl-lazy-load")):this.animateExpand():this.animateCollapse()}async animateExpand(){Nt(this,"sl-expand"),await oe(this.childrenContainer),this.childrenContainer.hidden=!1;const{keyframes:t,options:e}=ce(this,"tree-item.expand",{dir:this.localize.dir()});await Jt(this.childrenContainer,re(t,this.childrenContainer.scrollHeight),e),this.childrenContainer.style.height="auto",Nt(this,"sl-after-expand")}async animateCollapse(){Nt(this,"sl-collapse"),await oe(this.childrenContainer);const{keyframes:t,options:e}=ce(this,"tree-item.collapse",{dir:this.localize.dir()});await Jt(this.childrenContainer,re(t,this.childrenContainer.scrollHeight),e),this.childrenContainer.hidden=!0,Nt(this,"sl-after-collapse")}getChildrenItems({includeDisabled:t=!0}={}){return this.childrenSlot?[...this.childrenSlot.assignedElements({flatten:!0})].filter((e=>ke(e)&&(t||!e.disabled))):[]}isNestedItem(){const t=this.parentElement;return!!t&&ke(t)}handleToggleExpand(t){t.preventDefault(),t.stopImmediatePropagation(),this.disabled||(this.expanded=!this.expanded)}handleChildrenSlotChange(){this.loading=!1,this.isLeaf=0===this.getChildrenItems().length}willUpdate(t){t.has("selected")&&!t.has("indeterminate")&&(this.indeterminate=!1)}render(){const t="rtl"===this.localize.dir(),e=!this.loading&&(!this.isLeaf||this.lazy);return O`
      <div
        part="base"
        class="${Vt({"tree-item":!0,"tree-item--selected":this.selected,"tree-item--disabled":this.disabled,"tree-item--leaf":this.isLeaf,"tree-item--rtl":"rtl"===this.localize.dir()})}"
      >
        <div
          class="tree-item__item"
          part="
            item
            ${this.disabled?"item--disabled":""}
            ${this.expanded?"item--expanded":""}
            ${this.indeterminate?"item--indeterminate":""}
            ${this.selected?"item--selected":""}
          "
        >
          <div class="tree-item__indentation" part="indentation"></div>

          <div
            class=${Vt({"tree-item__expand-button":!0,"tree-item__expand-button--visible":e})}
            aria-hidden="true"
            @click="${this.handleToggleExpand}"
          >
            ${xe(this.loading,(()=>O` <sl-spinner></sl-spinner> `))}
            ${xe(e,(()=>O`
                <slot name="${this.expanded?"expanded-icon":"collapsed-icon"}">
                  <sl-icon
                    library="system"
                    name="${this.expanded?"chevron-down":t?"chevron-left":"chevron-right"}"
                  ></sl-icon>
                </slot>
              `))}
          </div>

          ${xe(this.selectable,(()=>O`
                <sl-checkbox
                  tabindex="-1"
                  class="tree-item__checkbox"
                  ?disabled="${this.disabled}"
                  ?checked="${ft(this.selected)}"
                  ?indeterminate="${this.indeterminate}"
                >
                  <div class="tree-item__label" part="label">
                    <slot></slot>
                  </div>
                </sl-checkbox>
              `),(()=>O`
              <div class="tree-item__label" part="label">
                <slot></slot>
              </div>
            `))}
        </div>

        <div class="tree-item__children" part="children" role="group">
          <slot name="children" @slotchange="${this.handleChildrenSlotChange}"></slot>
        </div>
      </div>
    `}};function Ce(t,e,o){return t<e?e:t>o?o:t}$e.styles=_e,Dt([Xt()],$e.prototype,"indeterminate",2),Dt([Xt()],$e.prototype,"isLeaf",2),Dt([Xt()],$e.prototype,"loading",2),Dt([Xt()],$e.prototype,"selectable",2),Dt([Kt({type:Boolean,reflect:!0})],$e.prototype,"expanded",2),Dt([Kt({type:Boolean,reflect:!0})],$e.prototype,"selected",2),Dt([Kt({type:Boolean,reflect:!0})],$e.prototype,"disabled",2),Dt([Kt({type:Boolean,reflect:!0})],$e.prototype,"lazy",2),Dt([Gt("slot:not([name])")],$e.prototype,"defaultSlot",2),Dt([Gt("slot[name=children]")],$e.prototype,"childrenSlot",2),Dt([Gt(".tree-item__item")],$e.prototype,"itemElement",2),Dt([Gt(".tree-item__children")],$e.prototype,"childrenContainer",2),Dt([Ut("loading",{waitUntilFirstUpdate:!0})],$e.prototype,"handleLoadingChange",1),Dt([Ut("disabled")],$e.prototype,"handleDisabledChange",1),Dt([Ut("selected")],$e.prototype,"handleSelectedChange",1),Dt([Ut("expanded",{waitUntilFirstUpdate:!0})],$e.prototype,"handleExpandedChange",1),Dt([Ut("expanded",{waitUntilFirstUpdate:!0})],$e.prototype,"handleExpandAnimation",1),$e=Dt([qt("sl-tree-item")],$e),le("tree-item.expand",{keyframes:[{height:"0",opacity:"0",overflow:"hidden"},{height:"auto",opacity:"1",overflow:"hidden"}],options:{duration:250,easing:"cubic-bezier(0.4, 0.0, 0.2, 1)"}}),le("tree-item.collapse",{keyframes:[{height:"auto",opacity:"1",overflow:"hidden"},{height:"0",opacity:"0",overflow:"hidden"}],options:{duration:200,easing:"cubic-bezier(0.4, 0.0, 0.2, 1)"}});var ze=class extends et{constructor(){super(...arguments),this.selection="single",this.treeItems=this.getElementsByTagName("sl-tree-item"),this.localize=new ge(this),this.initTreeItem=t=>{t.selectable="multiple"===this.selection,["expanded","collapsed"].filter((t=>!!this.querySelector(`[slot="${t}-icon"]`))).forEach((e=>{const o=t.querySelector(`[slot="${e}-icon"]`);null===o?t.append(this.getExpandButtonIcon(e)):o.hasAttribute("data-default")&&o.replaceWith(this.getExpandButtonIcon(e))}))},this.handleTreeChanged=t=>{for(const e of t){const t=[...e.addedNodes].filter(ke),o=[...e.removedNodes].filter(ke);t.forEach(this.initTreeItem),o.includes(this.lastFocusedItem)&&this.focusItem(this.getFocusableItems()[0])}},this.handleFocusOut=t=>{const e=t.relatedTarget;e&&this.contains(e)||(this.tabIndex=0)},this.handleFocusIn=t=>{const e=t.target;t.target===this&&this.focusItem(this.lastFocusedItem||this.treeItems[0]),ke(e)&&!e.disabled&&(this.lastFocusedItem&&(this.lastFocusedItem.tabIndex=-1),this.lastFocusedItem=e,this.tabIndex=-1,e.tabIndex=0)}}connectedCallback(){super.connectedCallback(),this.setAttribute("role","tree"),this.setAttribute("tabindex","0"),this.mutationObserver=new MutationObserver(this.handleTreeChanged),this.addEventListener("focusin",this.handleFocusIn),this.addEventListener("focusout",this.handleFocusOut)}disconnectedCallback(){super.disconnectedCallback(),this.mutationObserver.disconnect(),this.removeEventListener("focusin",this.handleFocusIn),this.removeEventListener("focusout",this.handleFocusOut)}getExpandButtonIcon(t){const e=("expanded"===t?this.expandedIconSlot:this.collapsedIconSlot).assignedElements({flatten:!0})[0];if(e){const o=e.cloneNode(!0);return[o,...o.querySelectorAll("[id]")].forEach((t=>t.removeAttribute("id"))),o.setAttribute("data-default",""),o.slot=`${t}-icon`,o}return null}firstUpdated(){[...this.treeItems].forEach(this.initTreeItem),this.mutationObserver.observe(this,{childList:!0,subtree:!0})}handleSelectionChange(){this.setAttribute("aria-multiselectable","multiple"===this.selection?"true":"false");for(const t of this.treeItems)t.selectable="multiple"===this.selection}syncTreeItems(t){if("multiple"===this.selection)(function t(e){const o=e.parentElement;if(ke(o)){const e=o.getChildrenItems({includeDisabled:!1}),r=!!e.length&&e.every((t=>t.selected)),i=e.every((t=>!t.selected&&!t.indeterminate));o.selected=r,o.indeterminate=!r&&!i,t(o)}})(e=t),function t(e){for(const o of e.getChildrenItems())o.selected=!o.disabled&&e.selected,t(o)}(e);else for(const e of this.treeItems)e!==t&&(e.selected=!1);var e}selectItem(t){"multiple"===this.selection?(t.selected=!t.selected,t.lazy&&(t.expanded=!0),this.syncTreeItems(t)):"single"===this.selection||t.isLeaf?(t.selected=!0,this.syncTreeItems(t)):"leaf"===this.selection&&(t.expanded=!t.expanded),Nt(this,"sl-selection-change",{detail:{selection:this.selectedItems}})}get selectedItems(){return[...this.treeItems].filter((t=>t.selected))}getFocusableItems(){const t=new Set;return[...this.treeItems].filter((e=>{var o;if(e.disabled)return!1;const r=null==(o=e.parentElement)?void 0:o.closest("[role=treeitem]");return r&&(!r.expanded||r.loading||t.has(r))&&t.add(e),!t.has(e)}))}focusItem(t){null==t||t.focus()}handleKeyDown(t){if(!["ArrowDown","ArrowUp","ArrowRight","ArrowLeft","Home","End","Enter"," "].includes(t.key))return;const e=this.getFocusableItems(),o="ltr"===this.localize.dir(),r="rtl"===this.localize.dir();if(e.length>0){t.preventDefault();const i=e.findIndex((t=>document.activeElement===t)),a=e[i],s=t=>{const o=e[Ce(t,0,e.length-1)];this.focusItem(o)},n=t=>{a.expanded=t};"ArrowDown"===t.key?s(i+1):"ArrowUp"===t.key?s(i-1):o&&"ArrowRight"===t.key||r&&"ArrowLeft"===t.key?!a||a.disabled||a.expanded||a.isLeaf&&!a.lazy?s(i+1):n(!0):o&&"ArrowLeft"===t.key||r&&"ArrowRight"===t.key?!a||a.disabled||a.isLeaf||!a.expanded?s(i-1):n(!1):"Home"===t.key?s(0):"End"===t.key?s(e.length-1):"Enter"!==t.key&&" "!==t.key||a.disabled||this.selectItem(a)}}handleClick(t){const e=t.target.closest("sl-tree-item");e.disabled||this.selectItem(e)}render(){return O`
      <div part="base" class="tree" @click="${this.handleClick}" @keydown="${this.handleKeyDown}">
        <slot></slot>
        <slot name="expanded-icon" hidden aria-hidden="true"> </slot>
        <slot name="collapsed-icon" hidden aria-hidden="true"> </slot>
      </div>
    `}};ze.styles=we,Dt([Gt("slot")],ze.prototype,"defaultSlot",2),Dt([Gt("slot[name=expanded-icon]")],ze.prototype,"expandedIconSlot",2),Dt([Gt("slot[name=collapsed-icon]")],ze.prototype,"collapsedIconSlot",2),Dt([Kt()],ze.prototype,"selection",2),Dt([Ut("selection")],ze.prototype,"handleSelectionChange",1),ze=Dt([qt("sl-tree")],ze);var Se=0;function Ae(){return++Se}var Te=n`
  ${it}

  :host {
    display: inline-block;
  }

  .tab {
    display: inline-flex;
    align-items: center;
    font-family: var(--sl-font-sans);
    font-size: var(--sl-font-size-small);
    font-weight: var(--sl-font-weight-semibold);
    border-radius: var(--sl-border-radius-medium);
    color: var(--sl-color-neutral-600);
    padding: var(--sl-spacing-medium) var(--sl-spacing-large);
    white-space: nowrap;
    user-select: none;
    cursor: pointer;
    transition: var(--transition-speed) box-shadow, var(--transition-speed) color;
  }

  .tab:hover:not(.tab--disabled) {
    color: var(--sl-color-primary-600);
  }

  .tab:focus {
    outline: none;
  }

  .tab:focus-visible:not(.tab--disabled) {
    color: var(--sl-color-primary-600);
  }

  .tab:focus-visible {
    outline: var(--sl-focus-ring);
    outline-offset: calc(-1 * var(--sl-focus-ring-width) - var(--sl-focus-ring-offset));
  }

  .tab.tab--active:not(.tab--disabled) {
    color: var(--sl-color-primary-600);
  }

  .tab.tab--closable {
    padding-inline-end: var(--sl-spacing-small);
  }

  .tab.tab--disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .tab__close-button {
    font-size: var(--sl-font-size-large);
    margin-inline-start: var(--sl-spacing-2x-small);
  }

  .tab__close-button::part(base) {
    padding: var(--sl-spacing-3x-small);
  }
`,Ee=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.attrId=Ae(),this.componentId=`sl-tab-${this.attrId}`,this.panel="",this.active=!1,this.closable=!1,this.disabled=!1}connectedCallback(){super.connectedCallback(),this.setAttribute("role","tab")}focus(t){this.tab.focus(t)}blur(){this.tab.blur()}handleCloseClick(){Nt(this,"sl-close")}handleActiveChange(){this.setAttribute("aria-selected",this.active?"true":"false")}handleDisabledChange(){this.setAttribute("aria-disabled",this.disabled?"true":"false")}render(){return this.id=this.id.length>0?this.id:this.componentId,O`
      <div
        part="base"
        class=${Vt({tab:!0,"tab--active":this.active,"tab--closable":this.closable,"tab--disabled":this.disabled})}
        tabindex="0"
      >
        <slot></slot>
        ${this.closable?O`
              <sl-icon-button
                part="close-button"
                exportparts="base:close-button__base"
                name="x"
                library="system"
                label=${this.localize.term("close")}
                class="tab__close-button"
                @click=${this.handleCloseClick}
                tabindex="-1"
              ></sl-icon-button>
            `:""}
      </div>
    `}};Ee.styles=Te,Dt([Gt(".tab")],Ee.prototype,"tab",2),Dt([Kt({reflect:!0})],Ee.prototype,"panel",2),Dt([Kt({type:Boolean,reflect:!0})],Ee.prototype,"active",2),Dt([Kt({type:Boolean})],Ee.prototype,"closable",2),Dt([Kt({type:Boolean,reflect:!0})],Ee.prototype,"disabled",2),Dt([Kt()],Ee.prototype,"lang",2),Dt([Ut("active")],Ee.prototype,"handleActiveChange",1),Dt([Ut("disabled")],Ee.prototype,"handleDisabledChange",1),Ee=Dt([qt("sl-tab")],Ee);var De=n`
  ${it}

  :host {
    --indicator-color: var(--sl-color-primary-600);
    --track-color: var(--sl-color-neutral-200);
    --track-width: 2px;

    display: block;
  }

  .tab-group {
    display: flex;
    border: solid 1px transparent;
    border-radius: 0;
  }

  .tab-group__tabs {
    display: flex;
    position: relative;
  }

  .tab-group__indicator {
    position: absolute;
    transition: var(--sl-transition-fast) transform ease, var(--sl-transition-fast) width ease;
  }

  .tab-group--has-scroll-controls .tab-group__nav-container {
    position: relative;
    padding: 0 var(--sl-spacing-x-large);
  }

  .tab-group__body {
    overflow: auto;
  }

  .tab-group__scroll-button {
    display: flex;
    align-items: center;
    justify-content: center;
    position: absolute;
    top: 0;
    bottom: 0;
    width: var(--sl-spacing-x-large);
  }

  .tab-group__scroll-button--start {
    left: 0;
  }

  .tab-group__scroll-button--end {
    right: 0;
  }

  .tab-group--rtl .tab-group__scroll-button--start {
    left: auto;
    right: 0;
  }

  .tab-group--rtl .tab-group__scroll-button--end {
    left: 0;
    right: auto;
  }

  /*
   * Top
   */

  .tab-group--top {
    flex-direction: column;
  }

  .tab-group--top .tab-group__nav-container {
    order: 1;
  }

  .tab-group--top .tab-group__nav {
    display: flex;
    overflow-x: auto;

    /* Hide scrollbar in Firefox */
    scrollbar-width: none;
  }

  /* Hide scrollbar in Chrome/Safari */
  .tab-group--top .tab-group__nav::-webkit-scrollbar {
    width: 0;
    height: 0;
  }

  .tab-group--top .tab-group__tabs {
    flex: 1 1 auto;
    position: relative;
    flex-direction: row;
    border-bottom: solid var(--track-width) var(--track-color);
  }

  .tab-group--top .tab-group__indicator {
    bottom: calc(-1 * var(--track-width));
    border-bottom: solid var(--track-width) var(--indicator-color);
  }

  .tab-group--top .tab-group__body {
    order: 2;
  }

  .tab-group--top ::slotted(sl-tab-panel) {
    --padding: var(--sl-spacing-medium) 0;
  }

  /*
   * Bottom
   */

  .tab-group--bottom {
    flex-direction: column;
  }

  .tab-group--bottom .tab-group__nav-container {
    order: 2;
  }

  .tab-group--bottom .tab-group__nav {
    display: flex;
    overflow-x: auto;

    /* Hide scrollbar in Firefox */
    scrollbar-width: none;
  }

  /* Hide scrollbar in Chrome/Safari */
  .tab-group--bottom .tab-group__nav::-webkit-scrollbar {
    width: 0;
    height: 0;
  }

  .tab-group--bottom .tab-group__tabs {
    flex: 1 1 auto;
    position: relative;
    flex-direction: row;
    border-top: solid var(--track-width) var(--track-color);
  }

  .tab-group--bottom .tab-group__indicator {
    top: calc(-1 * var(--track-width));
    border-top: solid var(--track-width) var(--indicator-color);
  }

  .tab-group--bottom .tab-group__body {
    order: 1;
  }

  .tab-group--bottom ::slotted(sl-tab-panel) {
    --padding: var(--sl-spacing-medium) 0;
  }

  /*
   * Start
   */

  .tab-group--start {
    flex-direction: row;
  }

  .tab-group--start .tab-group__nav-container {
    order: 1;
  }

  .tab-group--start .tab-group__tabs {
    flex: 0 0 auto;
    flex-direction: column;
    border-inline-end: solid var(--track-width) var(--track-color);
  }

  .tab-group--start .tab-group__indicator {
    right: calc(-1 * var(--track-width));
    border-right: solid var(--track-width) var(--indicator-color);
  }

  .tab-group--start.tab-group--rtl .tab-group__indicator {
    right: auto;
    left: calc(-1 * var(--track-width));
  }

  .tab-group--start .tab-group__body {
    flex: 1 1 auto;
    order: 2;
  }

  .tab-group--start ::slotted(sl-tab-panel) {
    --padding: 0 var(--sl-spacing-medium);
  }

  /*
   * End
   */

  .tab-group--end {
    flex-direction: row;
  }

  .tab-group--end .tab-group__nav-container {
    order: 2;
  }

  .tab-group--end .tab-group__tabs {
    flex: 0 0 auto;
    flex-direction: column;
    border-left: solid var(--track-width) var(--track-color);
  }

  .tab-group--end .tab-group__indicator {
    left: calc(-1 * var(--track-width));
    border-inline-start: solid var(--track-width) var(--indicator-color);
  }

  .tab-group--end.tab-group--rtl .tab-group__indicator {
    right: calc(-1 * var(--track-width));
    left: auto;
  }

  .tab-group--end .tab-group__body {
    flex: 1 1 auto;
    order: 1;
  }

  .tab-group--end ::slotted(sl-tab-panel) {
    --padding: 0 var(--sl-spacing-medium);
  }
`;var Le=new Set;function Me(t){Le.add(t),document.body.classList.add("sl-scroll-lock")}function Oe(t){Le.delete(t),0===Le.size&&document.body.classList.remove("sl-scroll-lock")}function Fe(t,e,o="vertical",r="smooth"){const i=function(t,e){return{top:Math.round(t.getBoundingClientRect().top-e.getBoundingClientRect().top),left:Math.round(t.getBoundingClientRect().left-e.getBoundingClientRect().left)}}(t,e),a=i.top+e.scrollTop,s=i.left+e.scrollLeft,n=e.scrollLeft,l=e.scrollLeft+e.offsetWidth,c=e.scrollTop,d=e.scrollTop+e.offsetHeight;"horizontal"!==o&&"both"!==o||(s<n?e.scrollTo({left:s,behavior:r}):s+t.clientWidth>l&&e.scrollTo({left:s-e.offsetWidth+t.clientWidth,behavior:r})),"vertical"!==o&&"both"!==o||(a<c?e.scrollTo({top:a,behavior:r}):a+t.clientHeight>d&&e.scrollTo({top:a-e.offsetHeight+t.clientHeight,behavior:r}))}var Ie=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.tabs=[],this.panels=[],this.hasScrollControls=!1,this.placement="top",this.activation="auto",this.noScrollControls=!1}connectedCallback(){super.connectedCallback(),this.resizeObserver=new ResizeObserver((()=>{this.preventIndicatorTransition(),this.repositionIndicator(),this.updateScrollControls()})),this.mutationObserver=new MutationObserver((t=>{t.some((t=>!["aria-labelledby","aria-controls"].includes(t.attributeName)))&&setTimeout((()=>this.setAriaLabels())),t.some((t=>"disabled"===t.attributeName))&&this.syncTabsAndPanels()})),this.updateComplete.then((()=>{this.syncTabsAndPanels(),this.mutationObserver.observe(this,{attributes:!0,childList:!0,subtree:!0}),this.resizeObserver.observe(this.nav);new IntersectionObserver(((t,e)=>{var o;t[0].intersectionRatio>0&&(this.setAriaLabels(),this.setActiveTab(null!=(o=this.getActiveTab())?o:this.tabs[0],{emitEvents:!1}),e.unobserve(t[0].target))})).observe(this.tabGroup)}))}disconnectedCallback(){this.mutationObserver.disconnect(),this.resizeObserver.unobserve(this.nav)}show(t){const e=this.tabs.find((e=>e.panel===t));e&&this.setActiveTab(e,{scrollBehavior:"smooth"})}getAllTabs(t={includeDisabled:!0}){return[...this.shadowRoot.querySelector('slot[name="nav"]').assignedElements()].filter((e=>t.includeDisabled?"sl-tab"===e.tagName.toLowerCase():"sl-tab"===e.tagName.toLowerCase()&&!e.disabled))}getAllPanels(){return[...this.body.querySelector("slot").assignedElements()].filter((t=>"sl-tab-panel"===t.tagName.toLowerCase()))}getActiveTab(){return this.tabs.find((t=>t.active))}handleClick(t){const e=t.target.closest("sl-tab");(null==e?void 0:e.closest("sl-tab-group"))===this&&null!==e&&this.setActiveTab(e,{scrollBehavior:"smooth"})}handleKeyDown(t){const e=t.target.closest("sl-tab");if((null==e?void 0:e.closest("sl-tab-group"))===this&&(["Enter"," "].includes(t.key)&&null!==e&&(this.setActiveTab(e,{scrollBehavior:"smooth"}),t.preventDefault()),["ArrowLeft","ArrowRight","ArrowUp","ArrowDown","Home","End"].includes(t.key))){const e=document.activeElement,o="rtl"===this.localize.dir();if("sl-tab"===(null==e?void 0:e.tagName.toLowerCase())){let r=this.tabs.indexOf(e);"Home"===t.key?r=0:"End"===t.key?r=this.tabs.length-1:["top","bottom"].includes(this.placement)&&t.key===(o?"ArrowRight":"ArrowLeft")||["start","end"].includes(this.placement)&&"ArrowUp"===t.key?r--:(["top","bottom"].includes(this.placement)&&t.key===(o?"ArrowLeft":"ArrowRight")||["start","end"].includes(this.placement)&&"ArrowDown"===t.key)&&r++,r<0&&(r=this.tabs.length-1),r>this.tabs.length-1&&(r=0),this.tabs[r].focus({preventScroll:!0}),"auto"===this.activation&&this.setActiveTab(this.tabs[r],{scrollBehavior:"smooth"}),["top","bottom"].includes(this.placement)&&Fe(this.tabs[r],this.nav,"horizontal"),t.preventDefault()}}}handleScrollToStart(){this.nav.scroll({left:"rtl"===this.localize.dir()?this.nav.scrollLeft+this.nav.clientWidth:this.nav.scrollLeft-this.nav.clientWidth,behavior:"smooth"})}handleScrollToEnd(){this.nav.scroll({left:"rtl"===this.localize.dir()?this.nav.scrollLeft-this.nav.clientWidth:this.nav.scrollLeft+this.nav.clientWidth,behavior:"smooth"})}updateScrollControls(){this.noScrollControls?this.hasScrollControls=!1:this.hasScrollControls=["top","bottom"].includes(this.placement)&&this.nav.scrollWidth>this.nav.clientWidth}setActiveTab(t,e){if(e=St({emitEvents:!0,scrollBehavior:"auto"},e),t!==this.activeTab&&!t.disabled){const o=this.activeTab;this.activeTab=t,this.tabs.map((t=>t.active=t===this.activeTab)),this.panels.map((t=>{var e;return t.active=t.name===(null==(e=this.activeTab)?void 0:e.panel)})),this.syncIndicator(),["top","bottom"].includes(this.placement)&&Fe(this.activeTab,this.nav,"horizontal",e.scrollBehavior),e.emitEvents&&(o&&Nt(this,"sl-tab-hide",{detail:{name:o.panel}}),Nt(this,"sl-tab-show",{detail:{name:this.activeTab.panel}}))}}setAriaLabels(){this.tabs.forEach((t=>{const e=this.panels.find((e=>e.name===t.panel));e&&(t.setAttribute("aria-controls",e.getAttribute("id")),e.setAttribute("aria-labelledby",t.getAttribute("id")))}))}syncIndicator(){this.getActiveTab()?(this.indicator.style.display="block",this.repositionIndicator()):this.indicator.style.display="none"}repositionIndicator(){const t=this.getActiveTab();if(!t)return;const e=t.clientWidth,o=t.clientHeight,r="rtl"===this.localize.dir(),i=this.getAllTabs(),a=i.slice(0,i.indexOf(t)).reduce(((t,e)=>({left:t.left+e.clientWidth,top:t.top+e.clientHeight})),{left:0,top:0});switch(this.placement){case"top":case"bottom":this.indicator.style.width=`${e}px`,this.indicator.style.height="auto",this.indicator.style.transform=r?`translateX(${-1*a.left}px)`:`translateX(${a.left}px)`;break;case"start":case"end":this.indicator.style.width="auto",this.indicator.style.height=`${o}px`,this.indicator.style.transform=`translateY(${a.top}px)`}}preventIndicatorTransition(){const t=this.indicator.style.transition;this.indicator.style.transition="none",requestAnimationFrame((()=>{this.indicator.style.transition=t}))}syncTabsAndPanels(){this.tabs=this.getAllTabs({includeDisabled:!1}),this.panels=this.getAllPanels(),this.syncIndicator()}render(){const t="rtl"===this.localize.dir();return O`
      <div
        part="base"
        class=${Vt({"tab-group":!0,"tab-group--top":"top"===this.placement,"tab-group--bottom":"bottom"===this.placement,"tab-group--start":"start"===this.placement,"tab-group--end":"end"===this.placement,"tab-group--rtl":"rtl"===this.localize.dir(),"tab-group--has-scroll-controls":this.hasScrollControls})}
        @click=${this.handleClick}
        @keydown=${this.handleKeyDown}
      >
        <div class="tab-group__nav-container" part="nav">
          ${this.hasScrollControls?O`
                <sl-icon-button
                  part="scroll-button scroll-button--start"
                  exportparts="base:scroll-button__base"
                  class="tab-group__scroll-button tab-group__scroll-button--start"
                  name=${t?"chevron-right":"chevron-left"}
                  library="system"
                  label=${this.localize.term("scrollToStart")}
                  @click=${this.handleScrollToStart}
                ></sl-icon-button>
              `:""}

          <div class="tab-group__nav">
            <div part="tabs" class="tab-group__tabs" role="tablist">
              <div part="active-tab-indicator" class="tab-group__indicator"></div>
              <slot name="nav" @slotchange=${this.syncTabsAndPanels}></slot>
            </div>
          </div>

          ${this.hasScrollControls?O`
                <sl-icon-button
                  part="scroll-button scroll-button--end"
                  exportparts="base:scroll-button__base"
                  class="tab-group__scroll-button tab-group__scroll-button--end"
                  name=${t?"chevron-left":"chevron-right"}
                  library="system"
                  label=${this.localize.term("scrollToEnd")}
                  @click=${this.handleScrollToEnd}
                ></sl-icon-button>
              `:""}
        </div>

        <div part="body" class="tab-group__body">
          <slot @slotchange=${this.syncTabsAndPanels}></slot>
        </div>
      </div>
    `}};Ie.styles=De,Dt([Gt(".tab-group")],Ie.prototype,"tabGroup",2),Dt([Gt(".tab-group__body")],Ie.prototype,"body",2),Dt([Gt(".tab-group__nav")],Ie.prototype,"nav",2),Dt([Gt(".tab-group__indicator")],Ie.prototype,"indicator",2),Dt([Xt()],Ie.prototype,"hasScrollControls",2),Dt([Kt()],Ie.prototype,"placement",2),Dt([Kt()],Ie.prototype,"activation",2),Dt([Kt({attribute:"no-scroll-controls",type:Boolean})],Ie.prototype,"noScrollControls",2),Dt([Kt()],Ie.prototype,"lang",2),Dt([Ut("noScrollControls",{waitUntilFirstUpdate:!0})],Ie.prototype,"updateScrollControls",1),Dt([Ut("placement",{waitUntilFirstUpdate:!0})],Ie.prototype,"syncIndicator",1),Ie=Dt([qt("sl-tab-group")],Ie);var Be=n`
  ${it}

  :host {
    --padding: 0;

    display: block;
  }

  .tab-panel {
    border: solid 1px transparent;
    padding: var(--padding);
  }

  .tab-panel:not(.tab-panel--active) {
    display: none;
  }
`,Pe=class extends et{constructor(){super(...arguments),this.attrId=Ae(),this.componentId=`sl-tab-panel-${this.attrId}`,this.name="",this.active=!1}connectedCallback(){super.connectedCallback(),this.id=this.id.length>0?this.id:this.componentId,this.setAttribute("role","tabpanel")}handleActiveChange(){this.setAttribute("aria-hidden",this.active?"false":"true")}render(){return O`
      <div
        part="base"
        class=${Vt({"tab-panel":!0,"tab-panel--active":this.active})}
      >
        <slot></slot>
      </div>
    `}};Pe.styles=Be,Dt([Kt({reflect:!0})],Pe.prototype,"name",2),Dt([Kt({type:Boolean,reflect:!0})],Pe.prototype,"active",2),Dt([Ut("active")],Pe.prototype,"handleActiveChange",1),Pe=Dt([qt("sl-tab-panel")],Pe);var Ve=n`
  ${it}

  :host {
    --border-radius: var(--sl-border-radius-pill);
    --color: var(--sl-color-neutral-200);
    --sheen-color: var(--sl-color-neutral-300);

    display: block;
    position: relative;
  }

  .skeleton {
    display: flex;
    width: 100%;
    height: 100%;
    min-height: 1rem;
  }

  .skeleton__indicator {
    flex: 1 1 auto;
    background: var(--color);
    border-radius: var(--border-radius);
  }

  .skeleton--sheen .skeleton__indicator {
    background: linear-gradient(270deg, var(--sheen-color), var(--color), var(--color), var(--sheen-color));
    background-size: 400% 100%;
    background-size: 400% 100%;
    animation: sheen 8s ease-in-out infinite;
  }

  .skeleton--pulse .skeleton__indicator {
    animation: pulse 2s ease-in-out 0.5s infinite;
  }

  @keyframes sheen {
    0% {
      background-position: 200% 0;
    }
    to {
      background-position: -200% 0;
    }
  }

  @keyframes pulse {
    0% {
      opacity: 1;
    }
    50% {
      opacity: 0.4;
    }
    100% {
      opacity: 1;
    }
  }
`,Re=class extends et{constructor(){super(...arguments),this.effect="none"}render(){return O`
      <div
        part="base"
        class=${Vt({skeleton:!0,"skeleton--pulse":"pulse"===this.effect,"skeleton--sheen":"sheen"===this.effect})}
        aria-busy="true"
        aria-live="polite"
      >
        <div part="indicator" class="skeleton__indicator"></div>
      </div>
    `}};Re.styles=Ve,Dt([Kt()],Re.prototype,"effect",2),Re=Dt([qt("sl-skeleton")],Re);var Ue=n`
  ${it}

  :host {
    --divider-width: 4px;
    --divider-hit-area: 12px;
    --min: 0%;
    --max: 100%;

    display: grid;
  }

  .start,
  .end {
    overflow: hidden;
  }

  .divider {
    flex: 0 0 var(--divider-width);
    display: flex;
    position: relative;
    align-items: center;
    justify-content: center;
    background-color: var(--sl-color-neutral-200);
    color: var(--sl-color-neutral-900);
    z-index: 1;
  }

  .divider:focus {
    outline: none;
  }

  :host(:not([disabled])) .divider:focus-visible {
    background-color: var(--sl-color-primary-600);
    color: var(--sl-color-neutral-0);
  }

  :host([disabled]) .divider {
    cursor: not-allowed;
  }

  /* Horizontal */
  :host(:not([vertical], [disabled])) .divider {
    cursor: col-resize;
  }

  :host(:not([vertical])) .divider::after {
    display: flex;
    content: '';
    position: absolute;
    height: 100%;
    left: calc(var(--divider-hit-area) / -2 + var(--divider-width) / 2);
    width: var(--divider-hit-area);
  }

  /* Vertical */
  :host([vertical]) {
    flex-direction: column;
  }

  :host([vertical]:not([disabled])) .divider {
    cursor: row-resize;
  }

  :host([vertical]) .divider::after {
    content: '';
    position: absolute;
    width: 100%;
    top: calc(var(--divider-hit-area) / -2 + var(--divider-width) / 2);
    height: var(--divider-hit-area);
  }
`;function Ne(t,e){function o(o){const r=t.getBoundingClientRect(),i=t.ownerDocument.defaultView,a=r.left+i.pageXOffset,s=r.top+i.pageYOffset,n=o.pageX-a,l=o.pageY-s;(null==e?void 0:e.onMove)&&e.onMove(n,l)}document.addEventListener("pointermove",o,{passive:!0}),document.addEventListener("pointerup",(function t(){document.removeEventListener("pointermove",o),document.removeEventListener("pointerup",t),(null==e?void 0:e.onStop)&&e.onStop()})),(null==e?void 0:e.initialEvent)&&o(e.initialEvent)}var He=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.position=50,this.vertical=!1,this.disabled=!1,this.snapThreshold=12}connectedCallback(){super.connectedCallback(),this.resizeObserver=new ResizeObserver((t=>this.handleResize(t))),this.updateComplete.then((()=>this.resizeObserver.observe(this))),this.detectSize(),this.cachedPositionInPixels=this.percentageToPixels(this.position)}disconnectedCallback(){super.disconnectedCallback(),this.resizeObserver.unobserve(this)}detectSize(){const{width:t,height:e}=this.getBoundingClientRect();this.size=this.vertical?e:t}percentageToPixels(t){return this.size*(t/100)}pixelsToPercentage(t){return t/this.size*100}handleDrag(t){const e="rtl"===this.localize.dir();this.disabled||(t.preventDefault(),Ne(this,{onMove:(t,o)=>{let r=this.vertical?o:t;if("end"===this.primary&&(r=this.size-r),this.snap){this.snap.split(" ").forEach((t=>{let o;o=t.endsWith("%")?this.size*(parseFloat(t)/100):parseFloat(t),e&&!this.vertical&&(o=this.size-o),r>=o-this.snapThreshold&&r<=o+this.snapThreshold&&(r=o)}))}this.position=Ce(this.pixelsToPercentage(r),0,100)},initialEvent:t}))}handleKeyDown(t){if(!this.disabled&&["ArrowLeft","ArrowRight","ArrowUp","ArrowDown","Home","End"].includes(t.key)){let e=this.position;const o=(t.shiftKey?10:1)*("end"===this.primary?-1:1);t.preventDefault(),("ArrowLeft"===t.key&&!this.vertical||"ArrowUp"===t.key&&this.vertical)&&(e-=o),("ArrowRight"===t.key&&!this.vertical||"ArrowDown"===t.key&&this.vertical)&&(e+=o),"Home"===t.key&&(e="end"===this.primary?100:0),"End"===t.key&&(e="end"===this.primary?0:100),this.position=Ce(e,0,100)}}handlePositionChange(){this.cachedPositionInPixels=this.percentageToPixels(this.position),this.positionInPixels=this.percentageToPixels(this.position),Nt(this,"sl-reposition")}handlePositionInPixelsChange(){this.position=this.pixelsToPercentage(this.positionInPixels)}handleVerticalChange(){this.detectSize()}handleResize(t){const{width:e,height:o}=t[0].contentRect;this.size=this.vertical?o:e,this.primary&&(this.position=this.pixelsToPercentage(this.cachedPositionInPixels))}render(){const t=this.vertical?"gridTemplateRows":"gridTemplateColumns",e=this.vertical?"gridTemplateColumns":"gridTemplateRows",o="rtl"===this.localize.dir(),r=`\n      clamp(\n        0%,\n        clamp(\n          var(--min),\n          ${this.position}% - var(--divider-width) / 2,\n          var(--max)\n        ),\n        calc(100% - var(--divider-width))\n      )\n    `;return"end"===this.primary?o&&!this.vertical?this.style[t]=`${r} var(--divider-width) auto`:this.style[t]=`auto var(--divider-width) ${r}`:o&&!this.vertical?this.style[t]=`auto var(--divider-width) ${r}`:this.style[t]=`${r} var(--divider-width) auto`,this.style[e]="",O`
      <div part="panel start" class="start">
        <slot name="start"></slot>
      </div>

      <div
        part="divider"
        class="divider"
        tabindex=${Rt(this.disabled?void 0:"0")}
        role="separator"
        aria-label=${this.localize.term("resize")}
        @keydown=${this.handleKeyDown}
        @mousedown=${this.handleDrag}
        @touchstart=${this.handleDrag}
      >
        <slot name="handle"></slot>
      </div>

      <div part="panel end" class="end">
        <slot name="end"></slot>
      </div>
    `}};He.styles=Ue,Dt([Gt(".divider")],He.prototype,"divider",2),Dt([Kt({type:Number,reflect:!0})],He.prototype,"position",2),Dt([Kt({attribute:"position-in-pixels",type:Number})],He.prototype,"positionInPixels",2),Dt([Kt({type:Boolean,reflect:!0})],He.prototype,"vertical",2),Dt([Kt({type:Boolean,reflect:!0})],He.prototype,"disabled",2),Dt([Kt()],He.prototype,"primary",2),Dt([Kt()],He.prototype,"snap",2),Dt([Kt({type:Number,attribute:"snap-threshold"})],He.prototype,"snapThreshold",2),Dt([Ut("position")],He.prototype,"handlePositionChange",1),Dt([Ut("positionInPixels")],He.prototype,"handlePositionInPixelsChange",1),Dt([Ut("vertical")],He.prototype,"handleVerticalChange",1),He=Dt([qt("sl-split-panel")],He);var qe=n`
  ${it}

  :host {
    --height: var(--sl-toggle-size);
    --thumb-size: calc(var(--sl-toggle-size) + 4px);
    --width: calc(var(--height) * 2);

    display: inline-block;
  }

  .switch {
    display: inline-flex;
    align-items: center;
    font-family: var(--sl-input-font-family);
    font-size: var(--sl-input-font-size-medium);
    font-weight: var(--sl-input-font-weight);
    color: var(--sl-input-color);
    vertical-align: middle;
    cursor: pointer;
  }

  .switch__control {
    flex: 0 0 auto;
    position: relative;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: var(--width);
    height: var(--height);
    background-color: var(--sl-color-neutral-400);
    border: solid var(--sl-input-border-width) var(--sl-color-neutral-400);
    border-radius: var(--height);
    transition: var(--sl-transition-fast) border-color, var(--sl-transition-fast) background-color;
  }

  .switch__control .switch__thumb {
    width: var(--thumb-size);
    height: var(--thumb-size);
    background-color: var(--sl-color-neutral-0);
    border-radius: 50%;
    border: solid var(--sl-input-border-width) var(--sl-color-neutral-400);
    transform: translateX(calc((var(--width) - var(--height)) / -2));
    transition: var(--sl-transition-fast) transform ease, var(--sl-transition-fast) background-color,
      var(--sl-transition-fast) border-color, var(--sl-transition-fast) box-shadow;
  }

  .switch__input {
    position: absolute;
    opacity: 0;
    padding: 0;
    margin: 0;
    pointer-events: none;
  }

  /* Hover */
  .switch:not(.switch--checked):not(.switch--disabled) .switch__control:hover {
    background-color: var(--sl-color-neutral-400);
    border-color: var(--sl-color-neutral-400);
  }

  .switch:not(.switch--checked):not(.switch--disabled) .switch__control:hover .switch__thumb {
    background-color: var(--sl-color-neutral-0);
    border-color: var(--sl-color-neutral-400);
  }

  /* Focus */
  .switch:not(.switch--checked):not(.switch--disabled) .switch__input:focus-visible ~ .switch__control {
    background-color: var(--sl-color-neutral-400);
    border-color: var(--sl-color-neutral-400);
  }

  .switch:not(.switch--checked):not(.switch--disabled) .switch__input:focus-visible ~ .switch__control .switch__thumb {
    background-color: var(--sl-color-neutral-0);
    border-color: var(--sl-color-primary-600);
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  /* Checked */
  .switch--checked .switch__control {
    background-color: var(--sl-color-primary-600);
    border-color: var(--sl-color-primary-600);
  }

  .switch--checked .switch__control .switch__thumb {
    background-color: var(--sl-color-neutral-0);
    border-color: var(--sl-color-primary-600);
    transform: translateX(calc((var(--width) - var(--height)) / 2));
  }

  /* Checked + hover */
  .switch.switch--checked:not(.switch--disabled) .switch__control:hover {
    background-color: var(--sl-color-primary-600);
    border-color: var(--sl-color-primary-600);
  }

  .switch.switch--checked:not(.switch--disabled) .switch__control:hover .switch__thumb {
    background-color: var(--sl-color-neutral-0);
    border-color: var(--sl-color-primary-600);
  }

  /* Checked + focus */
  .switch.switch--checked:not(.switch--disabled) .switch__input:focus-visible ~ .switch__control {
    background-color: var(--sl-color-primary-600);
    border-color: var(--sl-color-primary-600);
  }

  .switch.switch--checked:not(.switch--disabled) .switch__input:focus-visible ~ .switch__control .switch__thumb {
    background-color: var(--sl-color-neutral-0);
    border-color: var(--sl-color-primary-600);
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  /* Disabled */
  .switch--disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .switch__label {
    line-height: var(--height);
    margin-inline-start: 0.5em;
    user-select: none;
  }

  :host([required]) .switch__label::after {
    content: var(--sl-input-required-content);
    margin-inline-start: var(--sl-input-required-content-offset);
  }
`,je=class extends et{constructor(){super(...arguments),this.formSubmitController=new It(this,{value:t=>t.checked?t.value:void 0,defaultValue:t=>t.defaultChecked,setValue:(t,e)=>t.checked=e}),this.hasFocus=!1,this.disabled=!1,this.required=!1,this.checked=!1,this.invalid=!1,this.defaultChecked=!1}firstUpdated(){this.invalid=!this.input.checkValidity()}click(){this.input.click()}focus(t){this.input.focus(t)}blur(){this.input.blur()}reportValidity(){return this.input.reportValidity()}setCustomValidity(t){this.input.setCustomValidity(t),this.invalid=!this.input.checkValidity()}handleBlur(){this.hasFocus=!1,Nt(this,"sl-blur")}handleCheckedChange(){this.input.checked=this.checked,this.invalid=!this.input.checkValidity()}handleClick(){this.checked=!this.checked,Nt(this,"sl-change")}handleDisabledChange(){this.input.disabled=this.disabled,this.invalid=!this.input.checkValidity()}handleFocus(){this.hasFocus=!0,Nt(this,"sl-focus")}handleKeyDown(t){"ArrowLeft"===t.key&&(t.preventDefault(),this.checked=!1,Nt(this,"sl-change")),"ArrowRight"===t.key&&(t.preventDefault(),this.checked=!0,Nt(this,"sl-change"))}render(){return O`
      <label
        part="base"
        class=${Vt({switch:!0,"switch--checked":this.checked,"switch--disabled":this.disabled,"switch--focused":this.hasFocus})}
      >
        <input
          class="switch__input"
          type="checkbox"
          name=${Rt(this.name)}
          value=${Rt(this.value)}
          .checked=${ft(this.checked)}
          .disabled=${this.disabled}
          .required=${this.required}
          role="switch"
          aria-checked=${this.checked?"true":"false"}
          @click=${this.handleClick}
          @blur=${this.handleBlur}
          @focus=${this.handleFocus}
          @keydown=${this.handleKeyDown}
        />

        <span part="control" class="switch__control">
          <span part="thumb" class="switch__thumb"></span>
        </span>

        <span part="label" class="switch__label">
          <slot></slot>
        </span>
      </label>
    `}};je.styles=qe,Dt([Gt('input[type="checkbox"]')],je.prototype,"input",2),Dt([Xt()],je.prototype,"hasFocus",2),Dt([Kt()],je.prototype,"name",2),Dt([Kt()],je.prototype,"value",2),Dt([Kt({type:Boolean,reflect:!0})],je.prototype,"disabled",2),Dt([Kt({type:Boolean,reflect:!0})],je.prototype,"required",2),Dt([Kt({type:Boolean,reflect:!0})],je.prototype,"checked",2),Dt([Kt({type:Boolean,reflect:!0})],je.prototype,"invalid",2),Dt([mt("checked")],je.prototype,"defaultChecked",2),Dt([Ut("checked",{waitUntilFirstUpdate:!0})],je.prototype,"handleCheckedChange",1),Dt([Ut("disabled",{waitUntilFirstUpdate:!0})],je.prototype,"handleDisabledChange",1),je=Dt([qt("sl-switch")],je);var Ke=[{max:276e4,value:6e4,unit:"minute"},{max:72e6,value:36e5,unit:"hour"},{max:5184e5,value:864e5,unit:"day"},{max:24192e5,value:6048e5,unit:"week"},{max:28512e6,value:2592e6,unit:"month"},{max:1/0,value:31536e6,unit:"year"}],Xe=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.isoTime="",this.relativeTime="",this.titleTime="",this.format="long",this.numeric="auto",this.sync=!1}disconnectedCallback(){super.disconnectedCallback(),clearTimeout(this.updateTimeout)}render(){const t=new Date,e=new Date(this.date);if(isNaN(e.getMilliseconds()))return this.relativeTime="",this.isoTime="","";const o=e.getTime()-t.getTime(),{unit:r,value:i}=Ke.find((t=>Math.abs(o)<t.max));if(this.isoTime=e.toISOString(),this.titleTime=this.localize.date(e,{month:"long",year:"numeric",day:"numeric",hour:"numeric",minute:"numeric",timeZoneName:"short"}),this.relativeTime=this.localize.relativeTime(Math.round(o/i),r,{numeric:this.numeric,style:this.format}),clearTimeout(this.updateTimeout),this.sync){let t;t=We("minute"===r?"second":"hour"===r?"minute":"day"===r?"hour":"day"),this.updateTimeout=window.setTimeout((()=>this.requestUpdate()),t)}return O` <time datetime=${this.isoTime} title=${this.titleTime}>${this.relativeTime}</time> `}};function We(t){const e={second:1e3,minute:6e4,hour:36e5,day:864e5}[t];return e-Date.now()%e}Dt([Xt()],Xe.prototype,"isoTime",2),Dt([Xt()],Xe.prototype,"relativeTime",2),Dt([Xt()],Xe.prototype,"titleTime",2),Dt([Kt()],Xe.prototype,"date",2),Dt([Kt()],Xe.prototype,"lang",2),Dt([Kt()],Xe.prototype,"format",2),Dt([Kt()],Xe.prototype,"numeric",2),Dt([Kt({type:Boolean})],Xe.prototype,"sync",2),Xe=Dt([qt("sl-relative-time")],Xe);var Ye=n`
  ${it}

  :host {
    display: contents;
  }
`,Ge=class extends et{constructor(){super(...arguments),this.observedElements=[],this.disabled=!1}connectedCallback(){super.connectedCallback(),this.resizeObserver=new ResizeObserver((t=>{Nt(this,"sl-resize",{detail:{entries:t}})})),this.disabled||this.startObserver()}disconnectedCallback(){super.disconnectedCallback(),this.stopObserver()}handleSlotChange(){this.disabled||this.startObserver()}startObserver(){const t=this.shadowRoot.querySelector("slot");if(null!==t){const e=t.assignedElements({flatten:!0});this.observedElements.forEach((t=>this.resizeObserver.unobserve(t))),this.observedElements=[],e.forEach((t=>{this.resizeObserver.observe(t),this.observedElements.push(t)}))}}stopObserver(){this.resizeObserver.disconnect()}handleDisabledChange(){this.disabled?this.stopObserver():this.startObserver()}render(){return O` <slot @slotchange=${this.handleSlotChange}></slot> `}};Ge.styles=Ye,Dt([Kt({type:Boolean,reflect:!0})],Ge.prototype,"disabled",2),Dt([Ut("disabled",{waitUntilFirstUpdate:!0})],Ge.prototype,"handleDisabledChange",1),Ge=Dt([qt("sl-resize-observer")],Ge);var Ze=n`
  ${it}

  :host {
    display: block;
  }

  .responsive-media {
    position: relative;
  }

  .responsive-media ::slotted(*) {
    position: absolute !important;
    top: 0 !important;
    left: 0 !important;
    width: 100% !important;
    height: 100% !important;
  }

  .responsive-media--cover ::slotted(embed),
  .responsive-media--cover ::slotted(iframe),
  .responsive-media--cover ::slotted(img),
  .responsive-media--cover ::slotted(video) {
    object-fit: cover !important;
  }

  .responsive-media--contain ::slotted(embed),
  .responsive-media--contain ::slotted(iframe),
  .responsive-media--contain ::slotted(img),
  .responsive-media--contain ::slotted(video) {
    object-fit: contain !important;
  }
`,Qe=class extends et{constructor(){super(...arguments),this.aspectRatio="16:9",this.fit="cover"}render(){const t=this.aspectRatio.split(":"),e=parseFloat(t[0]),o=parseFloat(t[1]),r=!isNaN(e)&&!isNaN(o)&&e>0&&o>0?o/e*100+"%":"0";return O`
      <div
        class=${Vt({"responsive-media":!0,"responsive-media--cover":"cover"===this.fit,"responsive-media--contain":"contain"===this.fit})}
        style="padding-bottom: ${r}"
      >
        <slot></slot>
      </div>
    `}};Qe.styles=Ze,Dt([Kt({attribute:"aspect-ratio"})],Qe.prototype,"aspectRatio",2),Dt([Kt()],Qe.prototype,"fit",2),Qe=Dt([qt("sl-responsive-media")],Qe);var Je=n`
  ${it}
  ${rt}

  :host {
    display: block;
  }

  .select {
    display: block;
  }

  .select::part(panel) {
    overflow: hidden;
  }

  .select__control {
    display: inline-flex;
    align-items: center;
    justify-content: start;
    position: relative;
    width: 100%;
    font-family: var(--sl-input-font-family);
    font-weight: var(--sl-input-font-weight);
    letter-spacing: var(--sl-input-letter-spacing);
    vertical-align: middle;
    overflow: hidden;
    transition: var(--sl-transition-fast) color, var(--sl-transition-fast) border, var(--sl-transition-fast) box-shadow;
    cursor: pointer;
  }

  .select__menu {
    max-height: 50vh;
    overflow: auto;
  }

  .select__menu::part(base) {
    border: none;
  }

  .select::part(panel) {
    background: var(--sl-panel-background-color);
    border: solid var(--sl-panel-border-width) var(--sl-panel-border-color);
    border-radius: var(--sl-border-radius-medium);
  }

  /* Standard selects */
  .select--standard .select__control {
    background-color: var(--sl-input-background-color);
    border: solid var(--sl-input-border-width) var(--sl-input-border-color);
    color: var(--sl-input-color);
  }

  .select--standard:not(.select--disabled) .select__control:hover {
    background-color: var(--sl-input-background-color-hover);
    border-color: var(--sl-input-border-color-hover);
    color: var(--sl-input-color-hover);
  }

  .select--standard.select--focused:not(.select--disabled) .select__control {
    background-color: var(--sl-input-background-color-focus);
    border-color: var(--sl-input-border-color-focus);
    color: var(--sl-input-color-focus);
    box-shadow: 0 0 0 var(--sl-focus-ring-width) var(--sl-input-focus-ring-color);
    outline: none;
  }

  .select--standard.select--disabled .select__control {
    background-color: var(--sl-input-background-color-disabled);
    border-color: var(--sl-input-border-color-disabled);
    color: var(--sl-input-color-disabled);
    opacity: 0.5;
    cursor: not-allowed;
    outline: none;
  }

  /* Filled selects */
  .select--filled .select__control {
    border: none;
    background-color: var(--sl-input-filled-background-color);
    color: var(--sl-input-color);
  }

  .select--filled:hover:not(.select--disabled) .select__control {
    background-color: var(--sl-input-filled-background-color-hover);
  }

  .select--filled.select--focused:not(.select--disabled) .select__control {
    background-color: var(--sl-input-filled-background-color-focus);
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .select--filled.select--disabled .select__control {
    background-color: var(--sl-input-filled-background-color-disabled);
    opacity: 0.5;
    cursor: not-allowed;
  }

  .select--disabled .select__tags,
  .select--disabled .select__clear {
    pointer-events: none;
  }

  .select__prefix {
    display: inline-flex;
    align-items: center;
    color: var(--sl-input-placeholder-color);
  }

  .select__label {
    flex: 1 1 auto;
    display: flex;
    align-items: center;
    user-select: none;
    overflow-x: auto;
    overflow-y: hidden;
    white-space: nowrap;

    /* Hide scrollbar in Firefox */
    scrollbar-width: none;
  }

  /* Hide scrollbar in Chrome/Safari */
  .select__label::-webkit-scrollbar {
    width: 0;
    height: 0;
  }

  .select__clear {
    flex: 0 0 auto;
    display: inline-flex;
    align-items: center;
    width: 1.25em;
    font-size: inherit;
    color: var(--sl-input-icon-color);
    border: none;
    background: none;
    padding: 0;
    transition: var(--sl-transition-fast) color;
    cursor: pointer;
  }

  .select__clear:hover {
    color: var(--sl-input-icon-color-hover);
  }

  .select__suffix {
    display: inline-flex;
    align-items: center;
    color: var(--sl-input-placeholder-color);
  }

  .select__icon {
    flex: 0 0 auto;
    display: inline-flex;
    transition: var(--sl-transition-medium) transform ease;
  }

  .select--open .select__icon {
    transform: rotate(-180deg);
  }

  /* Placeholder */
  .select--placeholder-visible .select__label {
    color: var(--sl-input-placeholder-color);
  }

  .select--disabled.select--placeholder-visible .select__label {
    color: var(--sl-input-placeholder-color-disabled);
  }

  /* Tags */
  .select__tags {
    display: inline-flex;
    align-items: center;
    flex-wrap: wrap;
    justify-content: left;
    margin-inline-start: var(--sl-spacing-2x-small);
  }

  /* Hidden input (for form control validation to show) */
  .select__hidden-select {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    clip: rect(0 0 0 0);
    clip-path: inset(50%);
    overflow: hidden;
    white-space: nowrap;
  }

  /*
   * Size modifiers
   */

  /* Small */
  .select--small .select__control {
    border-radius: var(--sl-input-border-radius-small);
    font-size: var(--sl-input-font-size-small);
    min-height: var(--sl-input-height-small);
  }

  .select--small .select__prefix ::slotted(*) {
    margin-inline-start: var(--sl-input-spacing-small);
  }

  .select--small .select__label {
    margin: 0 var(--sl-input-spacing-small);
  }

  .select--small .select__clear {
    margin-inline-end: var(--sl-input-spacing-small);
  }

  .select--small .select__suffix ::slotted(*) {
    margin-inline-end: var(--sl-input-spacing-small);
  }

  .select--small .select__icon {
    margin-inline-end: var(--sl-input-spacing-small);
  }

  .select--small .select__tags {
    padding-bottom: 2px;
  }

  .select--small .select__tags sl-tag {
    padding-top: 2px;
  }

  .select--small .select__tags sl-tag:not(:last-of-type) {
    margin-inline-end: var(--sl-spacing-2x-small);
  }

  .select--small.select--has-tags .select__label {
    margin-inline-start: 0;
  }

  /* Medium */
  .select--medium .select__control {
    border-radius: var(--sl-input-border-radius-medium);
    font-size: var(--sl-input-font-size-medium);
    min-height: var(--sl-input-height-medium);
  }

  .select--medium .select__prefix ::slotted(*) {
    margin-inline-start: var(--sl-input-spacing-medium);
  }

  .select--medium .select__label {
    margin: 0 var(--sl-input-spacing-medium);
  }

  .select--medium .select__clear {
    margin-inline-end: var(--sl-input-spacing-medium);
  }

  .select--medium .select__suffix ::slotted(*) {
    margin-inline-end: var(--sl-input-spacing-medium);
  }

  .select--medium .select__icon {
    margin-inline-end: var(--sl-input-spacing-medium);
  }

  .select--medium .select__tags {
    padding-bottom: 3px;
  }

  .select--medium .select__tags sl-tag {
    padding-top: 3px;
  }

  .select--medium .select__tags sl-tag:not(:last-of-type) {
    margin-inline-end: var(--sl-spacing-2x-small);
  }

  .select--medium.select--has-tags .select__label {
    margin-inline-start: 0;
  }

  /* Large */
  .select--large .select__control {
    border-radius: var(--sl-input-border-radius-large);
    font-size: var(--sl-input-font-size-large);
    min-height: var(--sl-input-height-large);
  }

  .select--large .select__prefix ::slotted(*) {
    margin-inline-start: var(--sl-input-spacing-large);
  }

  .select--large .select__label {
    margin: 0 var(--sl-input-spacing-large);
  }

  .select--large .select__clear {
    margin-inline-end: var(--sl-input-spacing-large);
  }

  .select--large .select__suffix ::slotted(*) {
    margin-inline-end: var(--sl-input-spacing-large);
  }

  .select--large .select__icon {
    margin-inline-end: var(--sl-input-spacing-large);
  }

  .select--large .select__tags {
    padding-bottom: 4px;
  }
  .select--large .select__tags sl-tag {
    padding-top: 4px;
  }

  .select--large .select__tags sl-tag:not(:last-of-type) {
    margin-inline-end: var(--sl-spacing-2x-small);
  }

  .select--large.select--has-tags .select__label {
    margin-inline-start: 0;
  }

  /*
   * Pill modifier
   */
  .select--pill.select--small .select__control {
    border-radius: var(--sl-input-height-small);
  }

  .select--pill.select--medium .select__control {
    border-radius: var(--sl-input-height-medium);
  }

  .select--pill.select--large .select__control {
    border-radius: var(--sl-input-height-large);
  }
`,to=class extends et{constructor(){super(...arguments),this.formSubmitController=new It(this),this.hasSlotController=new Bt(this,"help-text","label"),this.localize=new ge(this),this.menuItems=[],this.hasFocus=!1,this.isOpen=!1,this.displayLabel="",this.displayTags=[],this.multiple=!1,this.maxTagsVisible=3,this.disabled=!1,this.name="",this.placeholder="",this.size="medium",this.hoist=!1,this.value="",this.filled=!1,this.pill=!1,this.label="",this.placement="bottom",this.helpText="",this.required=!1,this.clearable=!1,this.invalid=!1,this.defaultValue=""}connectedCallback(){super.connectedCallback(),this.resizeObserver=new ResizeObserver((()=>this.resizeMenu())),this.updateComplete.then((()=>{this.resizeObserver.observe(this),this.syncItemsFromValue()}))}firstUpdated(){this.invalid=!this.input.checkValidity()}disconnectedCallback(){super.disconnectedCallback(),this.resizeObserver.unobserve(this)}reportValidity(){return this.input.reportValidity()}setCustomValidity(t){this.input.setCustomValidity(t),this.invalid=!this.input.checkValidity()}getValueAsArray(){return this.multiple&&""===this.value?[]:Array.isArray(this.value)?this.value:[this.value]}focus(t){this.control.focus(t)}blur(){this.control.blur()}handleBlur(){this.isOpen||(this.hasFocus=!1,Nt(this,"sl-blur"))}handleClearClick(t){t.stopPropagation(),this.value=this.multiple?[]:"",Nt(this,"sl-clear"),this.syncItemsFromValue()}handleDisabledChange(){this.disabled&&this.isOpen&&this.dropdown.hide(),this.input.disabled=this.disabled,this.invalid=!this.input.checkValidity()}handleFocus(){this.hasFocus||(this.hasFocus=!0,Nt(this,"sl-focus"))}handleKeyDown(t){const e=t.target,o=this.menuItems[0],r=this.menuItems[this.menuItems.length-1];if("sl-tag"!==e.tagName.toLowerCase())if("Tab"!==t.key){if(["ArrowDown","ArrowUp"].includes(t.key)){if(t.preventDefault(),this.isOpen||this.dropdown.show(),"ArrowDown"===t.key)return this.menu.setCurrentItem(o),void o.focus();if("ArrowUp"===t.key)return this.menu.setCurrentItem(r),void r.focus()}t.ctrlKey||t.metaKey||this.isOpen||1!==t.key.length||(t.stopPropagation(),t.preventDefault(),this.dropdown.show(),this.menu.typeToSelect(t))}else this.isOpen&&this.dropdown.hide()}handleLabelClick(){this.focus()}handleMenuSelect(t){const e=t.detail.item;this.multiple?this.value=this.value.includes(e.value)?this.value.filter((t=>t!==e.value)):[...this.value,e.value]:this.value=e.value,this.syncItemsFromValue()}handleMenuShow(){this.resizeMenu(),this.isOpen=!0}handleMenuHide(){this.isOpen=!1,this.control.focus()}handleMenuItemLabelChange(){if(!this.multiple){const t=this.menuItems.find((t=>t.value===this.value));this.displayLabel=t?t.getTextLabel():""}}handleMultipleChange(){var t;const e=this.getValueAsArray();this.value=this.multiple?e:null!=(t=e[0])?t:"",this.syncItemsFromValue()}async handleMenuSlotChange(){this.menuItems=[...this.querySelectorAll("sl-menu-item")];const t=[];this.menuItems.forEach((e=>{t.includes(e.value)&&console.error(`Duplicate value found in <sl-select> menu item: '${e.value}'`,e),t.push(e.value)})),await Promise.all(this.menuItems.map((t=>t.render))),this.syncItemsFromValue()}handleTagInteraction(t){t.composedPath().find((t=>{if(t instanceof HTMLElement){return t.classList.contains("tag__remove")}return!1}))&&t.stopPropagation()}async handleValueChange(){this.syncItemsFromValue(),await this.updateComplete,this.invalid=!this.input.checkValidity(),Nt(this,"sl-change")}resizeMenu(){this.menu.style.width=`${this.control.clientWidth}px`,requestAnimationFrame((()=>this.dropdown.reposition()))}syncItemsFromValue(){const t=this.getValueAsArray();if(this.menuItems.forEach((e=>e.checked=t.includes(e.value))),this.multiple){const e=this.menuItems.filter((e=>t.includes(e.value)));if(this.displayLabel=e.length>0?e[0].getTextLabel():"",this.displayTags=e.map((t=>O`
          <sl-tag
            part="tag"
            exportparts="
              base:tag__base,
              content:tag__content,
              remove-button:tag__remove-button
            "
            variant="neutral"
            size=${this.size}
            ?pill=${this.pill}
            removable
            @click=${this.handleTagInteraction}
            @keydown=${this.handleTagInteraction}
            @sl-remove=${e=>{e.stopPropagation(),this.disabled||(t.checked=!1,this.syncValueFromItems())}}
          >
            ${t.getTextLabel()}
          </sl-tag>
        `)),this.maxTagsVisible>0&&this.displayTags.length>this.maxTagsVisible){const t=this.displayTags.length;this.displayLabel="",this.displayTags=this.displayTags.slice(0,this.maxTagsVisible),this.displayTags.push(O`
          <sl-tag
            part="tag"
            exportparts="
              base:tag__base,
              content:tag__content,
              remove-button:tag__remove-button
            "
            variant="neutral"
            size=${this.size}
          >
            +${t-this.maxTagsVisible}
          </sl-tag>
        `)}}else{const e=this.menuItems.find((e=>e.value===t[0]));this.displayLabel=e?e.getTextLabel():"",this.displayTags=[]}}syncValueFromItems(){const t=this.menuItems.filter((t=>t.checked)).map((t=>t.value));this.multiple?this.value=this.value.filter((e=>t.includes(e))):this.value=t.length>0?t[0]:""}render(){const t=this.hasSlotController.test("label"),e=this.hasSlotController.test("help-text"),o=this.multiple?this.value.length>0:""!==this.value,r=!!this.label||!!t,i=!!this.helpText||!!e,a=this.clearable&&!this.disabled&&o;return O`
      <div
        part="form-control"
        class=${Vt({"form-control":!0,"form-control--small":"small"===this.size,"form-control--medium":"medium"===this.size,"form-control--large":"large"===this.size,"form-control--has-label":r,"form-control--has-help-text":i})}
      >
        <label
          part="form-control-label"
          class="form-control__label"
          for="input"
          aria-hidden=${r?"false":"true"}
          @click=${this.handleLabelClick}
        >
          <slot name="label">${this.label}</slot>
        </label>

        <div part="form-control-input" class="form-control-input">
          <sl-dropdown
            part="base"
            .hoist=${this.hoist}
            .placement=${this.placement}
            .stayOpenOnSelect=${this.multiple}
            .containingElement=${this}
            ?disabled=${this.disabled}
            class=${Vt({select:!0,"select--open":this.isOpen,"select--empty":!this.value,"select--focused":this.hasFocus,"select--clearable":this.clearable,"select--disabled":this.disabled,"select--multiple":this.multiple,"select--standard":!this.filled,"select--filled":this.filled,"select--has-tags":this.multiple&&this.displayTags.length>0,"select--placeholder-visible":""===this.displayLabel,"select--small":"small"===this.size,"select--medium":"medium"===this.size,"select--large":"large"===this.size,"select--pill":this.pill,"select--invalid":this.invalid})}
            @sl-show=${this.handleMenuShow}
            @sl-hide=${this.handleMenuHide}
          >
            <div
              part="control"
              slot="trigger"
              id="input"
              class="select__control"
              role="combobox"
              aria-describedby="help-text"
              aria-haspopup="true"
              aria-disabled=${this.disabled?"true":"false"}
              aria-expanded=${this.isOpen?"true":"false"}
              aria-controls="menu"
              tabindex=${this.disabled?"-1":"0"}
              @blur=${this.handleBlur}
              @focus=${this.handleFocus}
              @keydown=${this.handleKeyDown}
            >
              <span part="prefix" class="select__prefix">
                <slot name="prefix"></slot>
              </span>

              <div part="display-label" class="select__label">
                ${this.displayTags.length>0?O` <span part="tags" class="select__tags"> ${this.displayTags} </span> `:this.displayLabel.length>0?this.displayLabel:this.placeholder}
              </div>

              ${a?O`
                    <button
                      part="clear-button"
                      class="select__clear"
                      @click=${this.handleClearClick}
                      aria-label=${this.localize.term("clearEntry")}
                      tabindex="-1"
                    >
                      <slot name="clear-icon">
                        <sl-icon name="x-circle-fill" library="system"></sl-icon>
                      </slot>
                    </button>
                  `:""}

              <span part="suffix" class="select__suffix">
                <slot name="suffix"></slot>
              </span>

              <span part="icon" class="select__icon" aria-hidden="true">
                <sl-icon name="chevron-down" library="system"></sl-icon>
              </span>

              <!-- The hidden input tricks the browser's built-in validation so it works as expected. We use an input
              instead of a select because, otherwise, iOS will show a list of options during validation. The focus
              handler is used to move focus to the primary control when it's marked invalid.  -->
              <input
                class="select__hidden-select"
                aria-hidden="true"
                ?required=${this.required}
                .value=${o?"1":""}
                tabindex="-1"
                @focus=${()=>this.control.focus()}
              />
            </div>

            <sl-menu part="menu" id="menu" class="select__menu" @sl-select=${this.handleMenuSelect}>
              <slot @slotchange=${this.handleMenuSlotChange} @sl-label-change=${this.handleMenuItemLabelChange}></slot>
            </sl-menu>
          </sl-dropdown>
        </div>

        <div
          part="form-control-help-text"
          id="help-text"
          class="form-control__help-text"
          aria-hidden=${i?"false":"true"}
        >
          <slot name="help-text">${this.helpText}</slot>
        </div>
      </div>
    `}};to.styles=Je,Dt([Gt(".select")],to.prototype,"dropdown",2),Dt([Gt(".select__control")],to.prototype,"control",2),Dt([Gt(".select__hidden-select")],to.prototype,"input",2),Dt([Gt(".select__menu")],to.prototype,"menu",2),Dt([Xt()],to.prototype,"hasFocus",2),Dt([Xt()],to.prototype,"isOpen",2),Dt([Xt()],to.prototype,"displayLabel",2),Dt([Xt()],to.prototype,"displayTags",2),Dt([Kt({type:Boolean,reflect:!0})],to.prototype,"multiple",2),Dt([Kt({attribute:"max-tags-visible",type:Number})],to.prototype,"maxTagsVisible",2),Dt([Kt({type:Boolean,reflect:!0})],to.prototype,"disabled",2),Dt([Kt()],to.prototype,"name",2),Dt([Kt()],to.prototype,"placeholder",2),Dt([Kt()],to.prototype,"size",2),Dt([Kt({type:Boolean})],to.prototype,"hoist",2),Dt([Kt()],to.prototype,"value",2),Dt([Kt({type:Boolean,reflect:!0})],to.prototype,"filled",2),Dt([Kt({type:Boolean,reflect:!0})],to.prototype,"pill",2),Dt([Kt()],to.prototype,"label",2),Dt([Kt()],to.prototype,"placement",2),Dt([Kt({attribute:"help-text"})],to.prototype,"helpText",2),Dt([Kt({type:Boolean,reflect:!0})],to.prototype,"required",2),Dt([Kt({type:Boolean})],to.prototype,"clearable",2),Dt([Kt({type:Boolean,reflect:!0})],to.prototype,"invalid",2),Dt([mt()],to.prototype,"defaultValue",2),Dt([Ut("disabled",{waitUntilFirstUpdate:!0})],to.prototype,"handleDisabledChange",1),Dt([Ut("multiple")],to.prototype,"handleMultipleChange",1),Dt([Ut("value",{waitUntilFirstUpdate:!0})],to.prototype,"handleValueChange",1),to=Dt([qt("sl-select")],to);var eo=n`
  ${it}

  :host {
    display: inline-block;
  }

  .tag {
    display: flex;
    align-items: center;
    border: solid 1px;
    line-height: 1;
    white-space: nowrap;
    user-select: none;
    cursor: default;
  }

  .tag__remove::part(base) {
    color: inherit;
    padding: 0;
  }

  /*
   * Variant modifiers
   */

  .tag--primary {
    background-color: var(--sl-color-primary-50);
    border-color: var(--sl-color-primary-200);
    color: var(--sl-color-primary-800);
  }

  .tag--success {
    background-color: var(--sl-color-success-50);
    border-color: var(--sl-color-success-200);
    color: var(--sl-color-success-800);
  }

  .tag--neutral {
    background-color: var(--sl-color-neutral-50);
    border-color: var(--sl-color-neutral-200);
    color: var(--sl-color-neutral-800);
  }

  .tag--warning {
    background-color: var(--sl-color-warning-50);
    border-color: var(--sl-color-warning-200);
    color: var(--sl-color-warning-800);
  }

  .tag--danger {
    background-color: var(--sl-color-danger-50);
    border-color: var(--sl-color-danger-200);
    color: var(--sl-color-danger-800);
  }

  /*
   * Size modifiers
   */

  .tag--small {
    font-size: var(--sl-button-font-size-small);
    height: calc(var(--sl-input-height-small) * 0.8);
    line-height: calc(var(--sl-input-height-small) - var(--sl-input-border-width) * 2);
    border-radius: var(--sl-input-border-radius-small);
    padding: 0 var(--sl-spacing-x-small);
  }

  .tag--small .tag__remove {
    margin-inline-start: var(--sl-spacing-2x-small);
    margin-right: calc(-1 * var(--sl-spacing-3x-small));
  }

  .tag--medium {
    font-size: var(--sl-button-font-size-medium);
    height: calc(var(--sl-input-height-medium) * 0.8);
    line-height: calc(var(--sl-input-height-medium) - var(--sl-input-border-width) * 2);
    border-radius: var(--sl-input-border-radius-medium);
    padding: 0 var(--sl-spacing-small);
  }

  .tag--large {
    font-size: var(--sl-button-font-size-large);
    height: calc(var(--sl-input-height-large) * 0.8);
    line-height: calc(var(--sl-input-height-large) - var(--sl-input-border-width) * 2);
    border-radius: var(--sl-input-border-radius-large);
    padding: 0 var(--sl-spacing-medium);
  }

  .tag__remove {
    font-size: 1.4em;
    margin-inline-start: var(--sl-spacing-2x-small);
    margin-inline-end: calc(-1 * var(--sl-spacing-x-small));
  }

  /*
   * Pill modifier
   */

  .tag--pill {
    border-radius: var(--sl-border-radius-pill);
  }
`,oo=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.variant="neutral",this.size="medium",this.pill=!1,this.removable=!1}handleRemoveClick(){Nt(this,"sl-remove")}render(){return O`
      <span
        part="base"
        class=${Vt({tag:!0,"tag--primary":"primary"===this.variant,"tag--success":"success"===this.variant,"tag--neutral":"neutral"===this.variant,"tag--warning":"warning"===this.variant,"tag--danger":"danger"===this.variant,"tag--text":"text"===this.variant,"tag--small":"small"===this.size,"tag--medium":"medium"===this.size,"tag--large":"large"===this.size,"tag--pill":this.pill,"tag--removable":this.removable})}
      >
        <span part="content" class="tag__content">
          <slot></slot>
        </span>

        ${this.removable?O`
              <sl-icon-button
                part="remove-button"
                exportparts="base:remove-button__base"
                name="x"
                library="system"
                label=${this.localize.term("remove")}
                class="tag__remove"
                @click=${this.handleRemoveClick}
              ></sl-icon-button>
            `:""}
      </span>
    `}};oo.styles=eo,Dt([Kt({reflect:!0})],oo.prototype,"variant",2),Dt([Kt({reflect:!0})],oo.prototype,"size",2),Dt([Kt({type:Boolean,reflect:!0})],oo.prototype,"pill",2),Dt([Kt({type:Boolean})],oo.prototype,"removable",2),oo=Dt([qt("sl-tag")],oo);var ro=n`
  ${it}

  :host {
    display: inline-block;
    position: relative;
    width: auto;
    cursor: pointer;
  }

  .button {
    display: inline-flex;
    align-items: stretch;
    justify-content: center;
    width: 100%;
    border-style: solid;
    border-width: var(--sl-input-border-width);
    font-family: var(--sl-input-font-family);
    font-weight: var(--sl-font-weight-semibold);
    text-decoration: none;
    user-select: none;
    white-space: nowrap;
    vertical-align: middle;
    padding: 0;
    transition: var(--sl-transition-x-fast) background-color, var(--sl-transition-x-fast) color,
      var(--sl-transition-x-fast) border, var(--sl-transition-x-fast) box-shadow;
    cursor: inherit;
  }

  .button::-moz-focus-inner {
    border: 0;
  }

  .button:focus {
    outline: none;
  }

  .button:focus-visible {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .button--disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  /* When disabled, prevent mouse events from bubbling up */
  .button--disabled * {
    pointer-events: none;
  }

  .button__prefix,
  .button__suffix {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
    pointer-events: none;
  }

  .button__label ::slotted(sl-icon) {
    vertical-align: -2px;
  }

  /*
   * Standard buttons
   */

  /* Default */
  .button--standard.button--default {
    background-color: var(--sl-color-neutral-0);
    border-color: var(--sl-color-neutral-300);
    color: var(--sl-color-neutral-700);
  }

  .button--standard.button--default:hover:not(.button--disabled) {
    background-color: var(--sl-color-primary-50);
    border-color: var(--sl-color-primary-300);
    color: var(--sl-color-primary-700);
  }

  .button--standard.button--default:active:not(.button--disabled) {
    background-color: var(--sl-color-primary-100);
    border-color: var(--sl-color-primary-400);
    color: var(--sl-color-primary-700);
  }

  /* Primary */
  .button--standard.button--primary {
    background-color: var(--sl-color-primary-600);
    border-color: var(--sl-color-primary-600);
    color: var(--sl-color-neutral-0);
  }

  .button--standard.button--primary:hover:not(.button--disabled) {
    background-color: var(--sl-color-primary-500);
    border-color: var(--sl-color-primary-500);
    color: var(--sl-color-neutral-0);
  }

  .button--standard.button--primary:active:not(.button--disabled) {
    background-color: var(--sl-color-primary-600);
    border-color: var(--sl-color-primary-600);
    color: var(--sl-color-neutral-0);
  }

  /* Success */
  .button--standard.button--success {
    background-color: var(--sl-color-success-600);
    border-color: var(--sl-color-success-600);
    color: var(--sl-color-neutral-0);
  }

  .button--standard.button--success:hover:not(.button--disabled) {
    background-color: var(--sl-color-success-500);
    border-color: var(--sl-color-success-500);
    color: var(--sl-color-neutral-0);
  }

  .button--standard.button--success:active:not(.button--disabled) {
    background-color: var(--sl-color-success-600);
    border-color: var(--sl-color-success-600);
    color: var(--sl-color-neutral-0);
  }

  /* Neutral */
  .button--standard.button--neutral {
    background-color: var(--sl-color-neutral-600);
    border-color: var(--sl-color-neutral-600);
    color: var(--sl-color-neutral-0);
  }

  .button--standard.button--neutral:hover:not(.button--disabled) {
    background-color: var(--sl-color-neutral-500);
    border-color: var(--sl-color-neutral-500);
    color: var(--sl-color-neutral-0);
  }

  .button--standard.button--neutral:active:not(.button--disabled) {
    background-color: var(--sl-color-neutral-600);
    border-color: var(--sl-color-neutral-600);
    color: var(--sl-color-neutral-0);
  }

  /* Warning */
  .button--standard.button--warning {
    background-color: var(--sl-color-warning-600);
    border-color: var(--sl-color-warning-600);
    color: var(--sl-color-neutral-0);
  }
  .button--standard.button--warning:hover:not(.button--disabled) {
    background-color: var(--sl-color-warning-500);
    border-color: var(--sl-color-warning-500);
    color: var(--sl-color-neutral-0);
  }

  .button--standard.button--warning:active:not(.button--disabled) {
    background-color: var(--sl-color-warning-600);
    border-color: var(--sl-color-warning-600);
    color: var(--sl-color-neutral-0);
  }

  /* Danger */
  .button--standard.button--danger {
    background-color: var(--sl-color-danger-600);
    border-color: var(--sl-color-danger-600);
    color: var(--sl-color-neutral-0);
  }

  .button--standard.button--danger:hover:not(.button--disabled) {
    background-color: var(--sl-color-danger-500);
    border-color: var(--sl-color-danger-500);
    color: var(--sl-color-neutral-0);
  }

  .button--standard.button--danger:active:not(.button--disabled) {
    background-color: var(--sl-color-danger-600);
    border-color: var(--sl-color-danger-600);
    color: var(--sl-color-neutral-0);
  }

  /*
   * Outline buttons
   */

  .button--outline {
    background: none;
    border: solid 1px;
  }

  /* Default */
  .button--outline.button--default {
    border-color: var(--sl-color-neutral-300);
    color: var(--sl-color-neutral-700);
  }

  .button--outline.button--default:hover:not(.button--disabled),
  .button--outline.button--default.button--checked:not(.button--disabled) {
    border-color: var(--sl-color-primary-600);
    background-color: var(--sl-color-primary-600);
    color: var(--sl-color-neutral-0);
  }

  .button--outline.button--default:active:not(.button--disabled) {
    border-color: var(--sl-color-primary-700);
    background-color: var(--sl-color-primary-700);
    color: var(--sl-color-neutral-0);
  }

  /* Primary */
  .button--outline.button--primary {
    border-color: var(--sl-color-primary-600);
    color: var(--sl-color-primary-600);
  }

  .button--outline.button--primary:hover:not(.button--disabled),
  .button--outline.button--primary.button--checked:not(.button--disabled) {
    background-color: var(--sl-color-primary-600);
    color: var(--sl-color-neutral-0);
  }

  .button--outline.button--primary:active:not(.button--disabled) {
    border-color: var(--sl-color-primary-700);
    background-color: var(--sl-color-primary-700);
    color: var(--sl-color-neutral-0);
  }

  /* Success */
  .button--outline.button--success {
    border-color: var(--sl-color-success-600);
    color: var(--sl-color-success-600);
  }

  .button--outline.button--success:hover:not(.button--disabled),
  .button--outline.button--success.button--checked:not(.button--disabled) {
    background-color: var(--sl-color-success-600);
    color: var(--sl-color-neutral-0);
  }

  .button--outline.button--success:active:not(.button--disabled) {
    border-color: var(--sl-color-success-700);
    background-color: var(--sl-color-success-700);
    color: var(--sl-color-neutral-0);
  }

  /* Neutral */
  .button--outline.button--neutral {
    border-color: var(--sl-color-neutral-600);
    color: var(--sl-color-neutral-600);
  }

  .button--outline.button--neutral:hover:not(.button--disabled),
  .button--outline.button--neutral.button--checked:not(.button--disabled) {
    background-color: var(--sl-color-neutral-600);
    color: var(--sl-color-neutral-0);
  }

  .button--outline.button--neutral:active:not(.button--disabled) {
    border-color: var(--sl-color-neutral-700);
    background-color: var(--sl-color-neutral-700);
    color: var(--sl-color-neutral-0);
  }

  /* Warning */
  .button--outline.button--warning {
    border-color: var(--sl-color-warning-600);
    color: var(--sl-color-warning-600);
  }

  .button--outline.button--warning:hover:not(.button--disabled),
  .button--outline.button--warning.button--checked:not(.button--disabled) {
    background-color: var(--sl-color-warning-600);
    color: var(--sl-color-neutral-0);
  }

  .button--outline.button--warning:active:not(.button--disabled) {
    border-color: var(--sl-color-warning-700);
    background-color: var(--sl-color-warning-700);
    color: var(--sl-color-neutral-0);
  }

  /* Danger */
  .button--outline.button--danger {
    border-color: var(--sl-color-danger-600);
    color: var(--sl-color-danger-600);
  }

  .button--outline.button--danger:hover:not(.button--disabled),
  .button--outline.button--danger.button--checked:not(.button--disabled) {
    background-color: var(--sl-color-danger-600);
    color: var(--sl-color-neutral-0);
  }

  .button--outline.button--danger:active:not(.button--disabled) {
    border-color: var(--sl-color-danger-700);
    background-color: var(--sl-color-danger-700);
    color: var(--sl-color-neutral-0);
  }

  /*
   * Text buttons
   */

  .button--text {
    background-color: transparent;
    border-color: transparent;
    color: var(--sl-color-primary-600);
  }

  .button--text:hover:not(.button--disabled) {
    background-color: transparent;
    border-color: transparent;
    color: var(--sl-color-primary-500);
  }

  .button--text:focus-visible:not(.button--disabled) {
    background-color: transparent;
    border-color: transparent;
    color: var(--sl-color-primary-500);
  }

  .button--text:active:not(.button--disabled) {
    background-color: transparent;
    border-color: transparent;
    color: var(--sl-color-primary-700);
  }

  /*
   * Size modifiers
   */

  .button--small {
    font-size: var(--sl-button-font-size-small);
    height: var(--sl-input-height-small);
    line-height: calc(var(--sl-input-height-small) - var(--sl-input-border-width) * 2);
    border-radius: var(--sl-input-border-radius-small);
  }

  .button--medium {
    font-size: var(--sl-button-font-size-medium);
    height: var(--sl-input-height-medium);
    line-height: calc(var(--sl-input-height-medium) - var(--sl-input-border-width) * 2);
    border-radius: var(--sl-input-border-radius-medium);
  }

  .button--large {
    font-size: var(--sl-button-font-size-large);
    height: var(--sl-input-height-large);
    line-height: calc(var(--sl-input-height-large) - var(--sl-input-border-width) * 2);
    border-radius: var(--sl-input-border-radius-large);
  }

  /*
   * Pill modifier
   */

  .button--pill.button--small {
    border-radius: var(--sl-input-height-small);
  }

  .button--pill.button--medium {
    border-radius: var(--sl-input-height-medium);
  }

  .button--pill.button--large {
    border-radius: var(--sl-input-height-large);
  }

  /*
   * Circle modifier
   */

  .button--circle {
    padding-left: 0;
    padding-right: 0;
  }

  .button--circle.button--small {
    width: var(--sl-input-height-small);
    border-radius: 50%;
  }

  .button--circle.button--medium {
    width: var(--sl-input-height-medium);
    border-radius: 50%;
  }

  .button--circle.button--large {
    width: var(--sl-input-height-large);
    border-radius: 50%;
  }

  .button--circle .button__prefix,
  .button--circle .button__suffix,
  .button--circle .button__caret {
    display: none;
  }

  /*
   * Caret modifier
   */

  .button--caret .button__suffix {
    display: none;
  }

  .button--caret .button__caret {
    display: flex;
    align-items: center;
  }

  .button--caret .button__caret svg {
    width: 1em;
    height: 1em;
  }

  /*
   * Loading modifier
   */

  .button--loading {
    position: relative;
    cursor: wait;
  }

  .button--loading .button__prefix,
  .button--loading .button__label,
  .button--loading .button__suffix,
  .button--loading .button__caret {
    visibility: hidden;
  }

  .button--loading sl-spinner {
    --indicator-color: currentColor;
    position: absolute;
    font-size: 1em;
    height: 1em;
    width: 1em;
    top: calc(50% - 0.5em);
    left: calc(50% - 0.5em);
  }

  /*
   * Badges
   */

  .button ::slotted(sl-badge) {
    position: absolute;
    top: 0;
    right: 0;
    transform: translateY(-50%) translateX(50%);
    pointer-events: none;
  }

  .button--rtl ::slotted(sl-badge) {
    right: auto;
    left: 0;
    transform: translateY(-50%) translateX(-50%);
  }

  /*
   * Button spacing
   */

  .button--has-label.button--small .button__label {
    padding: 0 var(--sl-spacing-small);
  }

  .button--has-label.button--medium .button__label {
    padding: 0 var(--sl-spacing-medium);
  }

  .button--has-label.button--large .button__label {
    padding: 0 var(--sl-spacing-large);
  }

  .button--has-prefix.button--small {
    padding-inline-start: var(--sl-spacing-x-small);
  }

  .button--has-prefix.button--small .button__label {
    padding-inline-start: var(--sl-spacing-x-small);
  }

  .button--has-prefix.button--medium {
    padding-inline-start: var(--sl-spacing-small);
  }

  .button--has-prefix.button--medium .button__label {
    padding-inline-start: var(--sl-spacing-small);
  }

  .button--has-prefix.button--large {
    padding-inline-start: var(--sl-spacing-small);
  }

  .button--has-prefix.button--large .button__label {
    padding-inline-start: var(--sl-spacing-small);
  }

  .button--has-suffix.button--small,
  .button--caret.button--small {
    padding-inline-end: var(--sl-spacing-x-small);
  }

  .button--has-suffix.button--small .button__label,
  .button--caret.button--small .button__label {
    padding-inline-end: var(--sl-spacing-x-small);
  }

  .button--has-suffix.button--medium,
  .button--caret.button--medium {
    padding-inline-end: var(--sl-spacing-small);
  }

  .button--has-suffix.button--medium .button__label,
  .button--caret.button--medium .button__label {
    padding-inline-end: var(--sl-spacing-small);
  }

  .button--has-suffix.button--large,
  .button--caret.button--large {
    padding-inline-end: var(--sl-spacing-small);
  }

  .button--has-suffix.button--large .button__label,
  .button--caret.button--large .button__label {
    padding-inline-end: var(--sl-spacing-small);
  }

  /*
   * Button groups support a variety of button types (e.g. buttons with tooltips, buttons as dropdown triggers, etc.).
   * This means buttons aren't always direct descendants of the button group, thus we can't target them with the
   * ::slotted selector. To work around this, the button group component does some magic to add these special classes to
   * buttons and we style them here instead.
   */

  :host(.sl-button-group__button--first:not(.sl-button-group__button--last)) .button {
    border-start-end-radius: 0;
    border-end-end-radius: 0;
  }

  :host(.sl-button-group__button--inner) .button {
    border-radius: 0;
  }

  :host(.sl-button-group__button--last:not(.sl-button-group__button--first)) .button {
    border-start-start-radius: 0;
    border-end-start-radius: 0;
  }

  /* All except the first */
  :host(.sl-button-group__button:not(.sl-button-group__button--first)) {
    margin-inline-start: calc(-1 * var(--sl-input-border-width));
  }

  /* Add a visual separator between solid buttons */
  :host(.sl-button-group__button:not(.sl-button-group__button--focus, .sl-button-group__button--first, .sl-button-group__button--radio, [variant='default']):not(:hover, :active, :focus))
    .button:after {
    content: '';
    position: absolute;
    top: 0;
    inset-inline-start: 0;
    bottom: 0;
    border-left: solid 1px rgb(128 128 128 / 33%);
    mix-blend-mode: multiply;
  }

  /* Bump hovered, focused, and checked buttons up so their focus ring isn't clipped */
  :host(.sl-button-group__button--hover) {
    z-index: 1;
  }

  :host(.sl-button-group__button--focus),
  :host(.sl-button-group__button[checked]) {
    z-index: 2;
  }
`,io=n`
  ${ro}

  label {
    display: inline-block;
    position: relative;
  }
  /* We use a hidden input so constraint validation errors work, since they don't appear to show when used with buttons.
    We can't actually hide it, though, otherwise the messages will be suppressed by the browser. */
  .hidden-input {
    all: unset;
    position: absolute;
    top: 0;
    left: 0;
    bottom: 0;
    right: 0;
    outline: dotted 1px red;
    opacity: 0;
    z-index: -1;
  }
`,ao=Symbol.for(""),so=t=>{var e,o;if((null===(e=t)||void 0===e?void 0:e.r)===ao)return null===(o=t)||void 0===o?void 0:o._$litStatic$},no=(t,...e)=>({_$litStatic$:e.reduce(((e,o,r)=>e+(t=>{if(void 0!==t._$litStatic$)return t._$litStatic$;throw Error(`Value passed to 'literal' function must be a 'literal' result: ${t}. Use 'unsafeStatic' to pass non-literal values, but\n            take care to ensure page security.`)})(o)+t[r+1]),t[0]),r:ao}),lo=new Map,co=t=>(e,...o)=>{const r=o.length;let i,a;const s=[],n=[];let l,c=0,d=!1;for(;c<r;){for(l=e[c];c<r&&void 0!==(a=o[c],i=so(a));)l+=i+e[++c],d=!0;n.push(a),s.push(l),c++}if(c===r&&s.push(e[r]),d){const t=s.join("$$lit$$");void 0===(e=lo.get(t))&&(s.raw=s,lo.set(t,e=s)),o=n}return t(e,...o)},ho=co(O),uo=(co(F),class extends et{constructor(){super(...arguments),this.hasSlotController=new Bt(this,"[default]","prefix","suffix"),this.hasFocus=!1,this.checked=!1,this.disabled=!1,this.size="medium",this.pill=!1}connectedCallback(){super.connectedCallback(),this.setAttribute("role","presentation")}handleDisabledChange(){this.setAttribute("aria-disabled",this.disabled?"true":"false")}handleBlur(){this.hasFocus=!1,Nt(this,"sl-blur")}handleClick(t){if(this.disabled)return t.preventDefault(),void t.stopPropagation();this.checked=!0}handleFocus(){this.hasFocus=!0,Nt(this,"sl-focus")}render(){return ho`
      <div part="base" role="presentation">
        <button
          part="button"
          role="radio"
          aria-checked="${this.checked}"
          class=${Vt({button:!0,"button--default":!0,"button--small":"small"===this.size,"button--medium":"medium"===this.size,"button--large":"large"===this.size,"button--checked":this.checked,"button--disabled":this.disabled,"button--focused":this.hasFocus,"button--outline":!0,"button--pill":this.pill,"button--has-label":this.hasSlotController.test("[default]"),"button--has-prefix":this.hasSlotController.test("prefix"),"button--has-suffix":this.hasSlotController.test("suffix")})}
          aria-disabled=${this.disabled}
          type="button"
          value=${Rt(this.value)}
          tabindex="${this.checked?"0":"-1"}"
          @blur=${this.handleBlur}
          @focus=${this.handleFocus}
          @click=${this.handleClick}
        >
          <span part="prefix" class="button__prefix">
            <slot name="prefix"></slot>
          </span>
          <span part="label" class="button__label">
            <slot></slot>
          </span>
          <span part="suffix" class="button__suffix">
            <slot name="suffix"></slot>
          </span>
        </button>
      </div>
    `}});uo.styles=io,Dt([Gt(".button")],uo.prototype,"input",2),Dt([Gt(".hidden-input")],uo.prototype,"hiddenInput",2),Dt([Xt()],uo.prototype,"hasFocus",2),Dt([Xt()],uo.prototype,"checked",2),Dt([Kt()],uo.prototype,"value",2),Dt([Kt({type:Boolean,reflect:!0})],uo.prototype,"disabled",2),Dt([Kt({reflect:!0})],uo.prototype,"size",2),Dt([Kt({type:Boolean,reflect:!0})],uo.prototype,"pill",2),Dt([Ut("disabled",{waitUntilFirstUpdate:!0})],uo.prototype,"handleDisabledChange",1),uo=Dt([qt("sl-radio-button")],uo);var po=n`
  ${it}
  ${rt}

  :host {
    --thumb-size: 20px;
    --tooltip-offset: 10px;
    --track-color-active: var(--sl-color-neutral-200);
    --track-color-inactive: var(--sl-color-neutral-200);
    --track-active-offset: 0%;
    --track-height: 6px;

    display: block;
  }

  .range {
    position: relative;
  }

  .range__control {
    --percent: 0%;
    -webkit-appearance: none;
    border-radius: 3px;
    width: 100%;
    height: var(--track-height);
    background: transparent;
    line-height: var(--sl-input-height-medium);
    vertical-align: middle;

    background-image: linear-gradient(
      to right,
      var(--track-color-inactive) 0%,
      var(--track-color-inactive) min(var(--percent), var(--track-active-offset)),
      var(--track-color-active) min(var(--percent), var(--track-active-offset)),
      var(--track-color-active) max(var(--percent), var(--track-active-offset)),
      var(--track-color-inactive) max(var(--percent), var(--track-active-offset)),
      var(--track-color-inactive) 100%
    );
  }

  /* Webkit */
  .range__control::-webkit-slider-runnable-track {
    width: 100%;
    height: var(--track-height);
    border-radius: 3px;
    border: none;
  }

  .range__control::-webkit-slider-thumb {
    border: none;
    width: var(--thumb-size);
    height: var(--thumb-size);
    border-radius: 50%;
    background-color: var(--sl-color-primary-600);
    border: solid var(--sl-input-border-width) var(--sl-color-primary-600);
    -webkit-appearance: none;
    margin-top: calc(var(--thumb-size) / -2 + var(--track-height) / 2);
    transition: var(--sl-transition-fast) border-color, var(--sl-transition-fast) background-color,
      var(--sl-transition-fast) color, var(--sl-transition-fast) box-shadow, var(--sl-transition-fast) transform;
    cursor: pointer;
  }

  .range__control:enabled::-webkit-slider-thumb:hover {
    background-color: var(--sl-color-primary-500);
    border-color: var(--sl-color-primary-500);
  }

  .range__control:enabled:focus-visible::-webkit-slider-thumb {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .range__control:enabled::-webkit-slider-thumb:active {
    background-color: var(--sl-color-primary-500);
    border-color: var(--sl-color-primary-500);
    cursor: grabbing;
  }

  /* Firefox */
  .range__control::-moz-focus-outer {
    border: 0;
  }

  .range__control::-moz-range-progress {
    background-color: var(--track-color-active);
    border-radius: 3px;
    height: var(--track-height);
  }

  .range__control::-moz-range-track {
    width: 100%;
    height: var(--track-height);
    background-color: var(--track-color-inactive);
    border-radius: 3px;
    border: none;
  }

  .range__control::-moz-range-thumb {
    border: none;
    height: var(--thumb-size);
    width: var(--thumb-size);
    border-radius: 50%;
    background-color: var(--sl-color-primary-600);
    border-color: var(--sl-color-primary-600);
    transition: var(--sl-transition-fast) border-color, var(--sl-transition-fast) background-color,
      var(--sl-transition-fast) color, var(--sl-transition-fast) box-shadow, var(--sl-transition-fast) transform;
    cursor: pointer;
  }

  .range__control:enabled::-moz-range-thumb:hover {
    background-color: var(--sl-color-primary-500);
    border-color: var(--sl-color-primary-500);
  }

  .range__control:enabled:focus-visible::-moz-range-thumb {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .range__control:enabled::-moz-range-thumb:active {
    background-color: var(--sl-color-primary-500);
    border-color: var(--sl-color-primary-500);
    cursor: grabbing;
  }

  /* States */
  .range__control:focus-visible {
    outline: none;
  }

  .range__control:disabled {
    opacity: 0.5;
  }

  .range__control:disabled::-webkit-slider-thumb {
    cursor: not-allowed;
  }

  .range__control:disabled::-moz-range-thumb {
    cursor: not-allowed;
  }

  /* Tooltip output */
  .range__tooltip {
    position: absolute;
    z-index: var(--sl-z-index-tooltip);
    left: 1px;
    border-radius: var(--sl-tooltip-border-radius);
    background-color: var(--sl-tooltip-background-color);
    font-family: var(--sl-tooltip-font-family);
    font-size: var(--sl-tooltip-font-size);
    font-weight: var(--sl-tooltip-font-weight);
    line-height: var(--sl-tooltip-line-height);
    color: var(--sl-tooltip-color);
    opacity: 0;
    padding: var(--sl-tooltip-padding);
    transition: var(--sl-transition-fast) opacity;
    pointer-events: none;
  }

  .range__tooltip:after {
    content: '';
    position: absolute;
    width: 0;
    height: 0;
    left: 50%;
    transform: translateX(calc(-1 * var(--sl-tooltip-arrow-size)));
  }

  .range--tooltip-visible .range__tooltip {
    opacity: 1;
  }

  /* Tooltip on top */
  .range--tooltip-top .range__tooltip {
    top: calc(-1 * var(--thumb-size) - var(--tooltip-offset));
  }

  .range--tooltip-top .range__tooltip:after {
    border-top: var(--sl-tooltip-arrow-size) solid var(--sl-tooltip-background-color);
    border-left: var(--sl-tooltip-arrow-size) solid transparent;
    border-right: var(--sl-tooltip-arrow-size) solid transparent;
    top: 100%;
  }

  /* Tooltip on bottom */
  .range--tooltip-bottom .range__tooltip {
    bottom: calc(-1 * var(--thumb-size) - var(--tooltip-offset));
  }

  .range--tooltip-bottom .range__tooltip:after {
    border-bottom: var(--sl-tooltip-arrow-size) solid var(--sl-tooltip-background-color);
    border-left: var(--sl-tooltip-arrow-size) solid transparent;
    border-right: var(--sl-tooltip-arrow-size) solid transparent;
    bottom: 100%;
  }
`,fo=class extends et{constructor(){super(...arguments),this.formSubmitController=new It(this),this.hasSlotController=new Bt(this,"help-text","label"),this.localize=new ge(this),this.hasFocus=!1,this.hasTooltip=!1,this.name="",this.value=0,this.label="",this.helpText="",this.disabled=!1,this.invalid=!1,this.min=0,this.max=100,this.step=1,this.tooltip="top",this.tooltipFormatter=t=>t.toString(),this.defaultValue=0}connectedCallback(){super.connectedCallback(),this.resizeObserver=new ResizeObserver((()=>this.syncRange())),this.value<this.min&&(this.value=this.min),this.value>this.max&&(this.value=this.max),this.updateComplete.then((()=>{this.syncRange(),this.resizeObserver.observe(this.input)}))}disconnectedCallback(){super.disconnectedCallback(),this.resizeObserver.unobserve(this.input)}focus(t){this.input.focus(t)}blur(){this.input.blur()}setCustomValidity(t){this.input.setCustomValidity(t),this.invalid=!this.input.checkValidity()}handleInput(){this.value=parseFloat(this.input.value),Nt(this,"sl-change"),this.syncRange()}handleBlur(){this.hasFocus=!1,this.hasTooltip=!1,Nt(this,"sl-blur")}handleValueChange(){this.invalid=!this.input.checkValidity(),this.input.value=this.value.toString(),this.value=parseFloat(this.input.value),this.syncRange()}handleDisabledChange(){this.input.disabled=this.disabled,this.invalid=!this.input.checkValidity()}handleFocus(){this.hasFocus=!0,this.hasTooltip=!0,Nt(this,"sl-focus")}handleThumbDragStart(){this.hasTooltip=!0}handleThumbDragEnd(){this.hasTooltip=!1}syncRange(){const t=Math.max(0,(this.value-this.min)/(this.max-this.min));this.syncProgress(t),"none"!==this.tooltip&&this.syncTooltip(t)}syncProgress(t){this.input.style.setProperty("--percent",100*t+"%")}syncTooltip(t){if(null!==this.output){const e=this.input.offsetWidth,o=this.output.offsetWidth,r=getComputedStyle(this.input).getPropertyValue("--thumb-size"),i=e*t;if("rtl"===this.localize.dir()){const a=`${e-i}px + ${t} * ${r}`;this.output.style.transform=`translateX(calc((${a} - ${o/2}px - ${r} / 2)))`}else{const e=`${i}px - ${t} * ${r}`;this.output.style.transform=`translateX(calc(${e} - ${o/2}px + ${r} / 2))`}}}render(){const t=this.hasSlotController.test("label"),e=this.hasSlotController.test("help-text"),o=!!this.label||!!t,r=!!this.helpText||!!e;return O`
      <div
        part="form-control"
        class=${Vt({"form-control":!0,"form-control--medium":!0,"form-control--has-label":o,"form-control--has-help-text":r})}
      >
        <label
          part="form-control-label"
          class="form-control__label"
          for="input"
          aria-hidden=${o?"false":"true"}
        >
          <slot name="label">${this.label}</slot>
        </label>

        <div part="form-control-input" class="form-control-input">
          <div
            part="base"
            class=${Vt({range:!0,"range--disabled":this.disabled,"range--focused":this.hasFocus,"range--tooltip-visible":this.hasTooltip,"range--tooltip-top":"top"===this.tooltip,"range--tooltip-bottom":"bottom"===this.tooltip})}
            @mousedown=${this.handleThumbDragStart}
            @mouseup=${this.handleThumbDragEnd}
            @touchstart=${this.handleThumbDragStart}
            @touchend=${this.handleThumbDragEnd}
          >
            <input
              part="input"
              id="input"
              type="range"
              class="range__control"
              name=${Rt(this.name)}
              ?disabled=${this.disabled}
              min=${Rt(this.min)}
              max=${Rt(this.max)}
              step=${Rt(this.step)}
              .value=${ft(this.value.toString())}
              aria-describedby="help-text"
              @input=${this.handleInput}
              @focus=${this.handleFocus}
              @blur=${this.handleBlur}
            />
            ${"none"===this.tooltip||this.disabled?"":O`
                  <output part="tooltip" class="range__tooltip">
                    ${"function"==typeof this.tooltipFormatter?this.tooltipFormatter(this.value):this.value}
                  </output>
                `}
          </div>
        </div>

        <div
          part="form-control-help-text"
          id="help-text"
          class="form-control__help-text"
          aria-hidden=${r?"false":"true"}
        >
          <slot name="help-text">${this.helpText}</slot>
        </div>
      </div>
    `}};fo.styles=po,Dt([Gt(".range__control")],fo.prototype,"input",2),Dt([Gt(".range__tooltip")],fo.prototype,"output",2),Dt([Xt()],fo.prototype,"hasFocus",2),Dt([Xt()],fo.prototype,"hasTooltip",2),Dt([Kt()],fo.prototype,"name",2),Dt([Kt({type:Number})],fo.prototype,"value",2),Dt([Kt()],fo.prototype,"label",2),Dt([Kt({attribute:"help-text"})],fo.prototype,"helpText",2),Dt([Kt({type:Boolean,reflect:!0})],fo.prototype,"disabled",2),Dt([Kt({type:Boolean,reflect:!0})],fo.prototype,"invalid",2),Dt([Kt({type:Number})],fo.prototype,"min",2),Dt([Kt({type:Number})],fo.prototype,"max",2),Dt([Kt({type:Number})],fo.prototype,"step",2),Dt([Kt()],fo.prototype,"tooltip",2),Dt([Kt({attribute:!1})],fo.prototype,"tooltipFormatter",2),Dt([mt()],fo.prototype,"defaultValue",2),Dt([Ut("value",{waitUntilFirstUpdate:!0})],fo.prototype,"handleValueChange",1),Dt([Ut("disabled",{waitUntilFirstUpdate:!0})],fo.prototype,"handleDisabledChange",1),Dt([Ut("hasTooltip",{waitUntilFirstUpdate:!0})],fo.prototype,"syncRange",1),fo=Dt([qt("sl-range")],fo);var mo=n`
  ${it}

  :host {
    display: block;
  }

  .radio-group {
    border: solid var(--sl-panel-border-width) var(--sl-panel-border-color);
    border-radius: var(--sl-border-radius-medium);
    padding: var(--sl-spacing-large);
    padding-top: var(--sl-spacing-x-small);
  }

  .radio-group .radio-group__label {
    font-family: var(--sl-input-font-family);
    font-size: var(--sl-input-font-size-medium);
    font-weight: var(--sl-input-font-weight);
    color: var(--sl-input-color);
    padding: 0 var(--sl-spacing-2x-small);
  }

  ::slotted(sl-radio:not(:last-of-type)) {
    margin-bottom: var(--sl-spacing-2x-small);
  }

  .radio-group:not(.radio-group--has-fieldset) {
    border: none;
    padding: 0;
    margin: 0;
    min-width: 0;
  }

  .radio-group:not(.radio-group--has-fieldset) .radio-group__label {
    position: absolute;
    width: 0;
    height: 0;
    clip: rect(0 0 0 0);
    clip-path: inset(50%);
    overflow: hidden;
    white-space: nowrap;
  }

  .radio-group--required .radio-group__label::after {
    content: var(--sl-input-required-content);
    margin-inline-start: var(--sl-input-required-content-offset);
  }

  .visually-hidden {
    position: absolute;
    width: 1px;
    height: 1px;
    padding: 0;
    margin: -1px;
    overflow: hidden;
    clip: rect(0, 0, 0, 0);
    white-space: nowrap;
    border: 0;
  }
`,bo=class extends et{constructor(){super(...arguments),this.formSubmitController=new It(this,{defaultValue:t=>t.defaultValue}),this.hasButtonGroup=!1,this.errorMessage="",this.customErrorMessage="",this.defaultValue="",this.label="",this.value="",this.name="option",this.invalid=!1,this.fieldset=!1,this.required=!1}handleValueChange(){this.hasUpdated&&(Nt(this,"sl-change"),this.updateCheckedRadio())}connectedCallback(){super.connectedCallback(),this.defaultValue=this.value}setCustomValidity(t=""){this.customErrorMessage=t,this.errorMessage=t,t?(this.invalid=!0,this.input.setCustomValidity(t)):this.invalid=!1}get validity(){const t=!(this.value&&this.required||!this.required),e=""!==this.customErrorMessage;return{badInput:!1,customError:e,patternMismatch:!1,rangeOverflow:!1,rangeUnderflow:!1,stepMismatch:!1,tooLong:!1,tooShort:!1,typeMismatch:!1,valid:!t&&!e,valueMissing:!t}}reportValidity(){const t=this.validity;return this.errorMessage=this.customErrorMessage||t.valid?"":this.input.validationMessage,this.invalid=!t.valid,t.valid||this.showNativeErrorMessage(),!this.invalid}getAllRadios(){return[...this.querySelectorAll("sl-radio, sl-radio-button")]}handleRadioClick(t){const e=t.target;if(e.disabled)return;this.value=e.value;this.getAllRadios().forEach((t=>t.checked=t===e))}handleKeyDown(t){var e;if(!["ArrowUp","ArrowDown","ArrowLeft","ArrowRight"," "].includes(t.key))return;const o=this.getAllRadios().filter((t=>!t.disabled)),r=null!=(e=o.find((t=>t.checked)))?e:o[0],i=" "===t.key?0:["ArrowUp","ArrowLeft"].includes(t.key)?-1:1;let a=o.indexOf(r)+i;a<0&&(a=o.length-1),a>o.length-1&&(a=0),this.getAllRadios().forEach((t=>{t.checked=!1,this.hasButtonGroup||(t.tabIndex=-1)})),this.value=o[a].value,o[a].checked=!0,this.hasButtonGroup?o[a].shadowRoot.querySelector("button").focus():(o[a].tabIndex=0,o[a].focus()),t.preventDefault()}handleSlotChange(){var t;const e=this.getAllRadios();if(e.forEach((t=>t.checked=t.value===this.value)),this.hasButtonGroup=e.some((t=>"sl-radio-button"===t.tagName.toLowerCase())),!e.some((t=>t.checked)))if(this.hasButtonGroup){e[0].shadowRoot.querySelector("button").tabIndex=0}else e[0].tabIndex=0;if(this.hasButtonGroup){const e=null==(t=this.shadowRoot)?void 0:t.querySelector("sl-button-group");e&&(e.disableRole=!0)}}showNativeErrorMessage(){this.input.hidden=!1,this.input.reportValidity(),setTimeout((()=>this.input.hidden=!0),1e4)}updateCheckedRadio(){this.getAllRadios().forEach((t=>t.checked=t.value===this.value))}render(){const t=O`
      <slot
        @click=${this.handleRadioClick}
        @keydown=${this.handleKeyDown}
        @slotchange=${this.handleSlotChange}
        role="presentation"
      ></slot>
    `;return O`
      <fieldset
        part="base"
        role="radiogroup"
        aria-errormessage="radio-error-message"
        aria-invalid="${this.invalid}"
        class=${Vt({"radio-group":!0,"radio-group--has-fieldset":this.fieldset,"radio-group--required":this.required})}
      >
        <legend part="label" class="radio-group__label">
          <slot name="label">${this.label}</slot>
        </legend>
        <div class="visually-hidden">
          <div id="radio-error-message" aria-live="assertive">${this.errorMessage}</div>
          <label class="radio-group__validation visually-hidden">
            <input type="text" class="radio-group__validation-input" ?required=${this.required} tabindex="-1" hidden />
          </label>
        </div>
        ${this.hasButtonGroup?O`<sl-button-group part="button-group">${t}</sl-button-group>`:t}
      </fieldset>
    `}};bo.styles=mo,Dt([Gt("slot:not([name])")],bo.prototype,"defaultSlot",2),Dt([Gt(".radio-group__validation-input")],bo.prototype,"input",2),Dt([Xt()],bo.prototype,"hasButtonGroup",2),Dt([Xt()],bo.prototype,"errorMessage",2),Dt([Xt()],bo.prototype,"customErrorMessage",2),Dt([Xt()],bo.prototype,"defaultValue",2),Dt([Kt()],bo.prototype,"label",2),Dt([Kt({reflect:!0})],bo.prototype,"value",2),Dt([Kt()],bo.prototype,"name",2),Dt([Kt({type:Boolean,reflect:!0})],bo.prototype,"invalid",2),Dt([Kt({type:Boolean,attribute:"fieldset",reflect:!0})],bo.prototype,"fieldset",2),Dt([Kt({type:Boolean,reflect:!0})],bo.prototype,"required",2),Dt([Ut("value")],bo.prototype,"handleValueChange",1),bo=Dt([qt("sl-radio-group")],bo);var go=n`
  ${it}

  :host {
    --symbol-color: var(--sl-color-neutral-300);
    --symbol-color-active: var(--sl-color-amber-500);
    --symbol-size: 1.2rem;
    --symbol-spacing: var(--sl-spacing-3x-small);

    display: inline-flex;
  }

  .rating {
    position: relative;
    display: inline-flex;
    border-radius: var(--sl-border-radius-medium);
    vertical-align: middle;
  }

  .rating:focus {
    outline: none;
  }

  .rating:focus-visible {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .rating__symbols {
    display: inline-flex;
    position: relative;
    font-size: var(--symbol-size);
    line-height: 0;
    color: var(--symbol-color);
    white-space: nowrap;
    cursor: pointer;
  }

  .rating__symbols > * {
    padding: var(--symbol-spacing);
  }

  .rating__symbols--indicator {
    position: absolute;
    top: 0;
    left: 0;
    color: var(--symbol-color-active);
    pointer-events: none;
  }

  .rating__symbol {
    transition: var(--sl-transition-fast) transform;
  }

  .rating__symbol--hover {
    transform: scale(1.2);
  }

  .rating--disabled .rating__symbols,
  .rating--readonly .rating__symbols {
    cursor: default;
  }

  .rating--disabled .rating__symbol--hover,
  .rating--readonly .rating__symbol--hover {
    transform: none;
  }

  .rating--disabled {
    opacity: 0.5;
  }

  .rating--disabled .rating__symbols {
    cursor: not-allowed;
  }
`,vo=dt(class extends ht{constructor(t){var e;if(super(t),t.type!==st||"style"!==t.name||(null===(e=t.strings)||void 0===e?void 0:e.length)>2)throw Error("The `styleMap` directive must be used in the `style` attribute and must be the only part in the attribute.")}render(t){return Object.keys(t).reduce(((e,o)=>{const r=t[o];return null==r?e:e+`${o=o.replace(/(?:^(webkit|moz|ms|o)|)(?=[A-Z])/g,"-$&").toLowerCase()}:${r};`}),"")}update(t,[e]){const{style:o}=t.element;if(void 0===this.ct){this.ct=new Set;for(const t in e)this.ct.add(t);return this.render(e)}this.ct.forEach((t=>{null==e[t]&&(this.ct.delete(t),t.includes("-")?o.removeProperty(t):o[t]="")}));for(const t in e){const r=e[t];null!=r&&(this.ct.add(t),t.includes("-")?o.setProperty(t,r):o[t]=r)}return I}}),yo="";function wo(t){yo=t}var _o={name:"default",resolver:t=>`${function(){if(!yo){const t=[...document.getElementsByTagName("script")],e=t.find((t=>t.hasAttribute("data-shoelace")));if(e)wo(e.getAttribute("data-shoelace"));else{const e=t.find((t=>/shoelace(\.min)?\.js($|\?)/.test(t.src)));let o="";e&&(o=e.getAttribute("src")),wo(o.split("/").slice(0,-1).join("/"))}}return yo.replace(/\/$/,"")}()}/assets/icons/${t}.svg`},xo={"check-lg":'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-check-lg" viewBox="0 0 16 16">\n      <path d="M12.736 3.97a.733.733 0 0 1 1.047 0c.286.289.29.756.01 1.05L7.88 12.01a.733.733 0 0 1-1.065.02L3.217 8.384a.757.757 0 0 1 0-1.06.733.733 0 0 1 1.047 0l3.052 3.093 5.4-6.425a.247.247 0 0 1 .02-.022Z"></path>\n    </svg>\n  ',"chevron-down":'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-down" viewBox="0 0 16 16">\n      <path fill-rule="evenodd" d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708z"/>\n    </svg>\n  ',"chevron-left":'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-left" viewBox="0 0 16 16">\n      <path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/>\n    </svg>\n  ',"chevron-right":'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-right" viewBox="0 0 16 16">\n      <path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/>\n    </svg>\n  ',eye:'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-eye" viewBox="0 0 16 16">\n      <path d="M16 8s-3-5.5-8-5.5S0 8 0 8s3 5.5 8 5.5S16 8 16 8zM1.173 8a13.133 13.133 0 0 1 1.66-2.043C4.12 4.668 5.88 3.5 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13.133 13.133 0 0 1 14.828 8c-.058.087-.122.183-.195.288-.335.48-.83 1.12-1.465 1.755C11.879 11.332 10.119 12.5 8 12.5c-2.12 0-3.879-1.168-5.168-2.457A13.134 13.134 0 0 1 1.172 8z"/>\n      <path d="M8 5.5a2.5 2.5 0 1 0 0 5 2.5 2.5 0 0 0 0-5zM4.5 8a3.5 3.5 0 1 1 7 0 3.5 3.5 0 0 1-7 0z"/>\n    </svg>\n  ',"eye-slash":'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-eye-slash" viewBox="0 0 16 16">\n      <path d="M13.359 11.238C15.06 9.72 16 8 16 8s-3-5.5-8-5.5a7.028 7.028 0 0 0-2.79.588l.77.771A5.944 5.944 0 0 1 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13.134 13.134 0 0 1 14.828 8c-.058.087-.122.183-.195.288-.335.48-.83 1.12-1.465 1.755-.165.165-.337.328-.517.486l.708.709z"/>\n      <path d="M11.297 9.176a3.5 3.5 0 0 0-4.474-4.474l.823.823a2.5 2.5 0 0 1 2.829 2.829l.822.822zm-2.943 1.299.822.822a3.5 3.5 0 0 1-4.474-4.474l.823.823a2.5 2.5 0 0 0 2.829 2.829z"/>\n      <path d="M3.35 5.47c-.18.16-.353.322-.518.487A13.134 13.134 0 0 0 1.172 8l.195.288c.335.48.83 1.12 1.465 1.755C4.121 11.332 5.881 12.5 8 12.5c.716 0 1.39-.133 2.02-.36l.77.772A7.029 7.029 0 0 1 8 13.5C3 13.5 0 8 0 8s.939-1.721 2.641-3.238l.708.709zm10.296 8.884-12-12 .708-.708 12 12-.708.708z"/>\n    </svg>\n  ',eyedropper:'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-eyedropper" viewBox="0 0 16 16">\n      <path d="M13.354.646a1.207 1.207 0 0 0-1.708 0L8.5 3.793l-.646-.647a.5.5 0 1 0-.708.708L8.293 5l-7.147 7.146A.5.5 0 0 0 1 12.5v1.793l-.854.853a.5.5 0 1 0 .708.707L1.707 15H3.5a.5.5 0 0 0 .354-.146L11 7.707l1.146 1.147a.5.5 0 0 0 .708-.708l-.647-.646 3.147-3.146a1.207 1.207 0 0 0 0-1.708l-2-2zM2 12.707l7-7L10.293 7l-7 7H2v-1.293z"></path>\n    </svg>\n  ',"person-fill":'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-person-fill" viewBox="0 0 16 16">\n      <path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/>\n    </svg>\n  ',"play-fill":'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-play-fill" viewBox="0 0 16 16">\n      <path d="m11.596 8.697-6.363 3.692c-.54.313-1.233-.066-1.233-.697V4.308c0-.63.692-1.01 1.233-.696l6.363 3.692a.802.802 0 0 1 0 1.393z"></path>\n    </svg>\n  ',"pause-fill":'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-pause-fill" viewBox="0 0 16 16">\n      <path d="M5.5 3.5A1.5 1.5 0 0 1 7 5v6a1.5 1.5 0 0 1-3 0V5a1.5 1.5 0 0 1 1.5-1.5zm5 0A1.5 1.5 0 0 1 12 5v6a1.5 1.5 0 0 1-3 0V5a1.5 1.5 0 0 1 1.5-1.5z"></path>\n    </svg>\n  ',"star-fill":'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-star-fill" viewBox="0 0 16 16">\n      <path d="M3.612 15.443c-.386.198-.824-.149-.746-.592l.83-4.73L.173 6.765c-.329-.314-.158-.888.283-.95l4.898-.696L7.538.792c.197-.39.73-.39.927 0l2.184 4.327 4.898.696c.441.062.612.636.282.95l-3.522 3.356.83 4.73c.078.443-.36.79-.746.592L8 13.187l-4.389 2.256z"/>\n    </svg>\n  ',x:'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x" viewBox="0 0 16 16">\n      <path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708z"/>\n    </svg>\n  ',"x-circle-fill":'\n    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-circle-fill" viewBox="0 0 16 16">\n      <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM5.354 4.646a.5.5 0 1 0-.708.708L7.293 8l-2.647 2.646a.5.5 0 0 0 .708.708L8 8.707l2.646 2.647a.5.5 0 0 0 .708-.708L8.707 8l2.647-2.646a.5.5 0 0 0-.708-.708L8 7.293 5.354 4.646z"></path>\n    </svg>\n  '},ko=[_o,{name:"system",resolver:t=>t in xo?`data:image/svg+xml,${encodeURIComponent(xo[t])}`:""}],$o=[];function Co(t){return ko.find((e=>e.name===t))}var zo=new Map;function So(t,e="cors"){if(zo.has(t))return zo.get(t);const o=fetch(t,{mode:e}).then((async t=>({ok:t.ok,status:t.status,html:await t.text()})));return zo.set(t,o),o}var Ao=new Map;var To=n`
  ${it}

  :host {
    display: inline-block;
    width: 1em;
    height: 1em;
    contain: strict;
    box-sizing: content-box !important;
  }

  .icon,
  svg {
    display: block;
    height: 100%;
    width: 100%;
  }
`,Eo=class extends ht{constructor(t){if(super(t),this.it=B,t.type!==nt)throw Error(this.constructor.directiveName+"() can only be used in child bindings")}render(t){if(t===B||null==t)return this.ft=void 0,this.it=t;if(t===I)return t;if("string"!=typeof t)throw Error(this.constructor.directiveName+"() called with a non-string value");if(t===this.it)return this.ft;this.it=t;const e=[t];return e.raw=e,this.ft={_$litType$:this.constructor.resultType,strings:e,values:[]}}};Eo.directiveName="unsafeHTML",Eo.resultType=1;var Do=dt(Eo),Lo=class extends Eo{};Lo.directiveName="unsafeSVG",Lo.resultType=2;var Mo,Oo=dt(Lo),Fo=class extends et{constructor(){super(...arguments),this.svg="",this.label="",this.library="default"}connectedCallback(){var t;super.connectedCallback(),t=this,$o.push(t)}firstUpdated(){this.setIcon()}disconnectedCallback(){var t;super.disconnectedCallback(),t=this,$o=$o.filter((e=>e!==t))}getUrl(){const t=Co(this.library);return this.name&&t?t.resolver(this.name):this.src}redraw(){this.setIcon()}async setIcon(){var t;const e=Co(this.library),o=this.getUrl();if(Mo||(Mo=new DOMParser),o)try{const r=await async function(t){if(Ao.has(t))return Ao.get(t);const e=await So(t),o={ok:e.ok,status:e.status,svg:null};if(e.ok){const t=document.createElement("div");t.innerHTML=e.html;const r=t.firstElementChild;o.svg="svg"===(null==r?void 0:r.tagName.toLowerCase())?r.outerHTML:""}return Ao.set(t,o),o}(o);if(o!==this.getUrl())return;if(r.ok){const o=Mo.parseFromString(r.svg,"text/html").body.querySelector("svg");null!==o?(null==(t=null==e?void 0:e.mutator)||t.call(e,o),this.svg=o.outerHTML,Nt(this,"sl-load")):(this.svg="",Nt(this,"sl-error"))}else this.svg="",Nt(this,"sl-error")}catch(t){Nt(this,"sl-error")}else this.svg.length>0&&(this.svg="")}handleChange(){this.setIcon()}render(){const t="string"==typeof this.label&&this.label.length>0;return O` <div
      part="base"
      class="icon"
      role=${Rt(t?"img":void 0)}
      aria-label=${Rt(t?this.label:void 0)}
      aria-hidden=${Rt(t?void 0:"true")}
    >
      ${Oo(this.svg)}
    </div>`}};Fo.styles=To,Dt([Xt()],Fo.prototype,"svg",2),Dt([Kt({reflect:!0})],Fo.prototype,"name",2),Dt([Kt()],Fo.prototype,"src",2),Dt([Kt()],Fo.prototype,"label",2),Dt([Kt({reflect:!0})],Fo.prototype,"library",2),Dt([Ut("name"),Ut("src"),Ut("library")],Fo.prototype,"setIcon",1),Fo=Dt([qt("sl-icon")],Fo);var Io=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.hoverValue=0,this.isHovering=!1,this.value=0,this.max=5,this.precision=1,this.readonly=!1,this.disabled=!1,this.getSymbol=()=>'<sl-icon name="star-fill" library="system"></sl-icon>'}focus(t){this.rating.focus(t)}blur(){this.rating.blur()}getValueFromMousePosition(t){return this.getValueFromXCoordinate(t.clientX)}getValueFromTouchPosition(t){return this.getValueFromXCoordinate(t.touches[0].clientX)}getValueFromXCoordinate(t){const e="rtl"===this.localize.dir(),{left:o,right:r,width:i}=this.rating.getBoundingClientRect();return Ce(e?this.roundToPrecision((r-t)/i*this.max,this.precision):this.roundToPrecision((t-o)/i*this.max,this.precision),0,this.max)}handleClick(t){this.setValue(this.getValueFromMousePosition(t))}setValue(t){this.disabled||this.readonly||(this.value=t===this.value?0:t,this.isHovering=!1)}handleKeyDown(t){const e="ltr"===this.localize.dir(),o="rtl"===this.localize.dir();if(!this.disabled&&!this.readonly){if(e&&"ArrowLeft"===t.key||o&&"ArrowRight"===t.key){const e=t.shiftKey?1:this.precision;this.value=Math.max(0,this.value-e),t.preventDefault()}if(e&&"ArrowRight"===t.key||o&&"ArrowLeft"===t.key){const e=t.shiftKey?1:this.precision;this.value=Math.min(this.max,this.value+e),t.preventDefault()}"Home"===t.key&&(this.value=0,t.preventDefault()),"End"===t.key&&(this.value=this.max,t.preventDefault())}}handleMouseEnter(){this.isHovering=!0}handleMouseMove(t){this.hoverValue=this.getValueFromMousePosition(t)}handleMouseLeave(){this.isHovering=!1}handleTouchStart(t){this.hoverValue=this.getValueFromTouchPosition(t),t.preventDefault()}handleTouchMove(t){this.isHovering=!0,this.hoverValue=this.getValueFromTouchPosition(t)}handleTouchEnd(t){this.isHovering=!1,this.setValue(this.hoverValue),t.preventDefault()}handleValueChange(){Nt(this,"sl-change")}roundToPrecision(t,e=.5){const o=1/e;return Math.ceil(t*o)/o}render(){const t="rtl"===this.localize.dir(),e=Array.from(Array(this.max).keys());let o=0;return o=this.disabled||this.readonly?this.value:this.isHovering?this.hoverValue:this.value,O`
      <div
        part="base"
        class=${Vt({rating:!0,"rating--readonly":this.readonly,"rating--disabled":this.disabled,"rating--rtl":t})}
        aria-disabled=${this.disabled?"true":"false"}
        aria-readonly=${this.readonly?"true":"false"}
        aria-valuenow=${this.value}
        aria-valuemin=${0}
        aria-valuemax=${this.max}
        tabindex=${this.disabled?"-1":"0"}
        @click=${this.handleClick}
        @keydown=${this.handleKeyDown}
        @mouseenter=${this.handleMouseEnter}
        @touchstart=${this.handleTouchStart}
        @mouseleave=${this.handleMouseLeave}
        @touchend=${this.handleTouchEnd}
        @mousemove=${this.handleMouseMove}
        @touchmove=${this.handleTouchMove}
      >
        <span class="rating__symbols rating__symbols--inactive">
          ${e.map((t=>O`
              <span
                class=${Vt({rating__symbol:!0,"rating__symbol--hover":this.isHovering&&Math.ceil(o)===t+1})}
                role="presentation"
                @mouseenter=${this.handleMouseEnter}
              >
                ${Do(this.getSymbol(t+1))}
              </span>
            `))}
        </span>

        <span class="rating__symbols rating__symbols--indicator">
          ${e.map((e=>O`
              <span
                class=${Vt({rating__symbol:!0,"rating__symbol--hover":this.isHovering&&Math.ceil(o)===e+1})}
                style=${vo({clipPath:o>e+1?"none":t?`inset(0 0 0 ${100-(o-e)/1*100}%)`:`inset(0 ${100-(o-e)/1*100}% 0 0)`})}
                role="presentation"
              >
                ${Do(this.getSymbol(e+1))}
              </span>
            `))}
        </span>
      </div>
    `}};Io.styles=go,Dt([Gt(".rating")],Io.prototype,"rating",2),Dt([Xt()],Io.prototype,"hoverValue",2),Dt([Xt()],Io.prototype,"isHovering",2),Dt([Kt({type:Number})],Io.prototype,"value",2),Dt([Kt({type:Number})],Io.prototype,"max",2),Dt([Kt({type:Number})],Io.prototype,"precision",2),Dt([Kt({type:Boolean,reflect:!0})],Io.prototype,"readonly",2),Dt([Kt({type:Boolean,reflect:!0})],Io.prototype,"disabled",2),Dt([Kt()],Io.prototype,"getSymbol",2),Dt([Ut("value",{waitUntilFirstUpdate:!0})],Io.prototype,"handleValueChange",1),Io=Dt([qt("sl-rating")],Io);var Bo=n`
  ${it}

  :host {
    --height: 1rem;
    --track-color: var(--sl-color-neutral-200);
    --indicator-color: var(--sl-color-primary-600);
    --label-color: var(--sl-color-neutral-0);

    display: block;
  }

  .progress-bar {
    position: relative;
    background-color: var(--track-color);
    height: var(--height);
    border-radius: var(--sl-border-radius-pill);
    box-shadow: inset var(--sl-shadow-small);
    overflow: hidden;
  }

  .progress-bar__indicator {
    height: 100%;
    font-family: var(--sl-font-sans);
    font-size: 12px;
    font-weight: var(--sl-font-weight-normal);
    background-color: var(--indicator-color);
    color: var(--label-color);
    text-align: center;
    line-height: var(--height);
    white-space: nowrap;
    overflow: hidden;
    transition: 400ms width, 400ms background-color;
    user-select: none;
  }

  /* Indeterminate */
  .progress-bar--indeterminate .progress-bar__indicator {
    position: absolute;
    animation: indeterminate 2.5s infinite cubic-bezier(0.37, 0, 0.63, 1);
  }

  @keyframes indeterminate {
    0% {
      inset-inline-start: -50%;
      width: 50%;
    }
    75%,
    100% {
      inset-inline-start: 100%;
      width: 50%;
    }
  }
`,Po=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.value=0,this.indeterminate=!1,this.label=""}render(){return O`
      <div
        part="base"
        class=${Vt({"progress-bar":!0,"progress-bar--indeterminate":this.indeterminate})}
        role="progressbar"
        title=${Rt(this.title)}
        aria-label=${this.label.length>0?this.label:this.localize.term("progress")}
        aria-valuemin="0"
        aria-valuemax="100"
        aria-valuenow=${this.indeterminate?0:this.value}
      >
        <div part="indicator" class="progress-bar__indicator" style=${vo({width:`${this.value}%`})}>
          ${this.indeterminate?"":O`
                <span part="label" class="progress-bar__label">
                  <slot></slot>
                </span>
              `}
        </div>
      </div>
    `}};Po.styles=Bo,Dt([Kt({type:Number,reflect:!0})],Po.prototype,"value",2),Dt([Kt({type:Boolean,reflect:!0})],Po.prototype,"indeterminate",2),Dt([Kt()],Po.prototype,"label",2),Dt([Kt()],Po.prototype,"lang",2),Po=Dt([qt("sl-progress-bar")],Po);var Vo=n`
  ${it}

  :host {
    --size: 128px;
    --track-width: 4px;
    --track-color: var(--sl-color-neutral-200);
    --indicator-width: var(--track-width);
    --indicator-color: var(--sl-color-primary-600);

    display: inline-flex;
  }

  .progress-ring {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    position: relative;
  }

  .progress-ring__image {
    width: var(--size);
    height: var(--size);
    transform: rotate(-90deg);
    transform-origin: 50% 50%;
  }

  .progress-ring__track,
  .progress-ring__indicator {
    --radius: calc(var(--size) / 2 - max(var(--track-width), var(--indicator-width)) * 0.5);
    --circumference: calc(var(--radius) * 2 * 3.141592654);

    fill: none;
    r: var(--radius);
    cx: calc(var(--size) / 2);
    cy: calc(var(--size) / 2);
  }

  .progress-ring__track {
    stroke: var(--track-color);
    stroke-width: var(--track-width);
  }

  .progress-ring__indicator {
    stroke: var(--indicator-color);
    stroke-width: var(--indicator-width);
    stroke-linecap: round;
    transition: 0.35s stroke-dashoffset;
    stroke-dasharray: var(--circumference) var(--circumference);
    stroke-dashoffset: calc(var(--circumference) - var(--percentage) * var(--circumference));
  }

  .progress-ring__label {
    display: flex;
    align-items: center;
    justify-content: center;
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    text-align: center;
    user-select: none;
  }
`,Ro=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.value=0,this.label=""}updated(t){if(super.updated(t),t.has("percentage")){const t=parseFloat(getComputedStyle(this.indicator).getPropertyValue("r")),e=2*Math.PI*t,o=e-this.value/100*e;this.indicatorOffset=`${o}px`}}render(){return O`
      <div
        part="base"
        class="progress-ring"
        role="progressbar"
        aria-label=${this.label.length>0?this.label:this.localize.term("progress")}
        aria-valuemin="0"
        aria-valuemax="100"
        aria-valuenow="${this.value}"
        style="--percentage: ${this.value/100}"
      >
        <svg class="progress-ring__image">
          <circle class="progress-ring__track"></circle>
          <circle class="progress-ring__indicator" style="stroke-dashoffset: ${this.indicatorOffset}"></circle>
        </svg>

        <span part="label" class="progress-ring__label">
          <slot></slot>
        </span>
      </div>
    `}};Ro.styles=Vo,Dt([Gt(".progress-ring__indicator")],Ro.prototype,"indicator",2),Dt([Xt()],Ro.prototype,"indicatorOffset",2),Dt([Kt({type:Number,reflect:!0})],Ro.prototype,"value",2),Dt([Kt()],Ro.prototype,"label",2),Dt([Kt()],Ro.prototype,"lang",2),Ro=Dt([qt("sl-progress-ring")],Ro);var Uo=n`
  ${it}

  :host {
    display: inline-block;
  }

  .qr-code {
    position: relative;
  }

  canvas {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
  }
`,No=null,Ho=class{};Ho.render=function(t,e){No(t,e)},self.QrCreator=Ho,function(t){function e(e,o,r,i){var a={},s=t(r,o);s.u(e),s.J(),i=i||0;var n=s.h(),l=s.h()+2*i;return a.text=e,a.level=o,a.version=r,a.O=l,a.a=function(t,e){return e-=i,!(0>(t-=i)||t>=n||0>e||e>=n)&&s.a(t,e)},a}function o(t,e,o,r,i,a,s,n,l,c){function d(e,o,r,i,s,n,l){e?(t.lineTo(o+n,r+l),t.arcTo(o,r,i,s,a)):t.lineTo(o,r)}s?t.moveTo(e+a,o):t.moveTo(e,o),d(n,r,o,r,i,-a,0),d(l,r,i,e,i,0,-a),d(c,e,i,e,o,a,0),d(s,e,o,r,o,0,a)}function r(t,e,o,r,i,a,s,n,l,c){function d(e,o,r,i){t.moveTo(e+r,o),t.lineTo(e,o),t.lineTo(e,o+i),t.arcTo(e,o,e+r,o,a)}s&&d(e,o,a,a),n&&d(r,o,-a,a),l&&d(r,i,-a,-a),c&&d(e,i,a,-a)}function i(t,i){t:{var a=i.text,s=i.v,n=i.N,l=i.K,c=i.P;for(n=Math.max(1,n||1),l=Math.min(40,l||40);n<=l;n+=1)try{var d=e(a,s,n,c);break t}catch(t){}d=void 0}if(!d)return null;for(a=t.getContext("2d"),i.background&&(a.fillStyle=i.background,a.fillRect(i.left,i.top,i.size,i.size)),s=d.O,l=i.size/s,a.beginPath(),c=0;c<s;c+=1)for(n=0;n<s;n+=1){var h=a,u=i.left+n*l,p=i.top+c*l,f=c,m=n,b=d.a,g=u+l,v=p+l,y=f-1,w=f+1,_=m-1,x=m+1,k=Math.floor(Math.min(.5,Math.max(0,i.R))*l),$=b(f,m),C=b(y,_),z=b(y,m);y=b(y,x);var S=b(f,x);x=b(w,x),m=b(w,m),w=b(w,_),f=b(f,_),u=Math.round(u),p=Math.round(p),g=Math.round(g),v=Math.round(v),$?o(h,u,p,g,v,k,!z&&!f,!z&&!S,!m&&!S,!m&&!f):r(h,u,p,g,v,k,z&&f&&C,z&&S&&y,m&&S&&x,m&&f&&w)}return function(t,e){var o=e.fill;if("string"==typeof o)t.fillStyle=o;else{var r=o.type,i=o.colorStops;if(o=o.position.map((t=>Math.round(t*e.size))),"linear-gradient"===r)var a=t.createLinearGradient.apply(t,o);else{if("radial-gradient"!==r)throw Error("Unsupported fill");a=t.createRadialGradient.apply(t,o)}i.forEach((([t,e])=>{a.addColorStop(t,e)})),t.fillStyle=a}}(a,i),a.fill(),t}var a={minVersion:1,maxVersion:40,ecLevel:"L",left:0,top:0,size:200,fill:"#000",background:null,text:"no text",radius:.5,quiet:0};No=function(t,e){var o={};Object.assign(o,a,t),o.N=o.minVersion,o.K=o.maxVersion,o.v=o.ecLevel,o.left=o.left,o.top=o.top,o.size=o.size,o.fill=o.fill,o.background=o.background,o.text=o.text,o.R=o.radius,o.P=o.quiet,e instanceof HTMLCanvasElement?(e.width===o.size&&e.height===o.size||(e.width=o.size,e.height=o.size),e.getContext("2d").clearRect(0,0,e.width,e.height),i(e,o)):((t=document.createElement("canvas")).width=o.size,t.height=o.size,o=i(t,o),e.appendChild(o))}}(function(){function t(i,s){function n(t,e){for(var o=-1;7>=o;o+=1)if(!(-1>=t+o||h<=t+o))for(var r=-1;7>=r;r+=1)-1>=e+r||h<=e+r||(d[t+o][e+r]=0<=o&&6>=o&&(0==r||6==r)||0<=r&&6>=r&&(0==o||6==o)||2<=o&&4>=o&&2<=r&&4>=r)}function l(t,o){for(var s=h=4*i+17,l=Array(s),f=0;f<s;f+=1){l[f]=Array(s);for(var m=0;m<s;m+=1)l[f][m]=null}for(d=l,n(0,0),n(h-7,0),n(0,h-7),s=r.G(i),l=0;l<s.length;l+=1)for(f=0;f<s.length;f+=1){m=s[l];var b=s[f];if(null==d[m][b])for(var g=-2;2>=g;g+=1)for(var v=-2;2>=v;v+=1)d[m+g][b+v]=-2==g||2==g||-2==v||2==v||0==g&&0==v}for(s=8;s<h-8;s+=1)null==d[s][6]&&(d[s][6]=0==s%2);for(s=8;s<h-8;s+=1)null==d[6][s]&&(d[6][s]=0==s%2);for(s=r.w(c<<3|o),l=0;15>l;l+=1)f=!t&&1==(s>>l&1),d[6>l?l:8>l?l+1:h-15+l][8]=f,d[8][8>l?h-l-1:9>l?15-l:14-l]=f;if(d[h-8][8]=!t,7<=i){for(s=r.A(i),l=0;18>l;l+=1)f=!t&&1==(s>>l&1),d[Math.floor(l/3)][l%3+h-8-3]=f;for(l=0;18>l;l+=1)f=!t&&1==(s>>l&1),d[l%3+h-8-3][Math.floor(l/3)]=f}if(null==u){for(t=a.I(i,c),s=function(){var t=[],e=0,o={B:function(){return t},c:function(e){return 1==(t[Math.floor(e/8)]>>>7-e%8&1)},put:function(t,e){for(var r=0;r<e;r+=1)o.m(1==(t>>>e-r-1&1))},f:function(){return e},m:function(o){var r=Math.floor(e/8);t.length<=r&&t.push(0),o&&(t[r]|=128>>>e%8),e+=1}};return o}(),l=0;l<p.length;l+=1)f=p[l],s.put(4,4),s.put(f.b(),r.f(4,i)),f.write(s);for(l=f=0;l<t.length;l+=1)f+=t[l].j;if(s.f()>8*f)throw Error("code length overflow. ("+s.f()+">"+8*f+")");for(s.f()+4<=8*f&&s.put(0,4);0!=s.f()%8;)s.m(!1);for(;!(s.f()>=8*f)&&(s.put(236,8),!(s.f()>=8*f));)s.put(17,8);var y=0;for(f=l=0,m=Array(t.length),b=Array(t.length),g=0;g<t.length;g+=1){var w=t[g].j,_=t[g].o-w;for(l=Math.max(l,w),f=Math.max(f,_),m[g]=Array(w),v=0;v<m[g].length;v+=1)m[g][v]=255&s.B()[v+y];for(y+=w,v=r.C(_),w=e(m[g],v.b()-1).l(v),b[g]=Array(v.b()-1),v=0;v<b[g].length;v+=1)_=v+w.b()-b[g].length,b[g][v]=0<=_?w.c(_):0}for(v=s=0;v<t.length;v+=1)s+=t[v].o;for(s=Array(s),v=y=0;v<l;v+=1)for(g=0;g<t.length;g+=1)v<m[g].length&&(s[y]=m[g][v],y+=1);for(v=0;v<f;v+=1)for(g=0;g<t.length;g+=1)v<b[g].length&&(s[y]=b[g][v],y+=1);u=s}for(t=u,s=-1,l=h-1,f=7,m=0,o=r.F(o),b=h-1;0<b;b-=2)for(6==b&&--b;;){for(g=0;2>g;g+=1)null==d[l][b-g]&&(v=!1,m<t.length&&(v=1==(t[m]>>>f&1)),o(l,b-g)&&(v=!v),d[l][b-g]=v,-1==--f&&(m+=1,f=7));if(0>(l+=s)||h<=l){l-=s,s=-s;break}}}var c=o[s],d=null,h=0,u=null,p=[],f={u:function(e){e=function(e){var o=t.s(e);return{S:function(){return 4},b:function(){return o.length},write:function(t){for(var e=0;e<o.length;e+=1)t.put(o[e],8)}}}(e),p.push(e),u=null},a:function(t,e){if(0>t||h<=t||0>e||h<=e)throw Error(t+","+e);return d[t][e]},h:function(){return h},J:function(){for(var t=0,e=0,o=0;8>o;o+=1){l(!0,o);var i=r.D(f);(0==o||t>i)&&(t=i,e=o)}l(!1,e)}};return f}function e(t,o){if(void 0===t.length)throw Error(t.length+"/"+o);var r=function(){for(var e=0;e<t.length&&0==t[e];)e+=1;for(var r=Array(t.length-e+o),i=0;i<t.length-e;i+=1)r[i]=t[i+e];return r}(),a={c:function(t){return r[t]},b:function(){return r.length},multiply:function(t){for(var o=Array(a.b()+t.b()-1),r=0;r<a.b();r+=1)for(var s=0;s<t.b();s+=1)o[r+s]^=i.i(i.g(a.c(r))+i.g(t.c(s)));return e(o,0)},l:function(t){if(0>a.b()-t.b())return a;for(var o=i.g(a.c(0))-i.g(t.c(0)),r=Array(a.b()),s=0;s<a.b();s+=1)r[s]=a.c(s);for(s=0;s<t.b();s+=1)r[s]^=i.i(i.g(t.c(s))+o);return e(r,0).l(t)}};return a}t.s=function(t){for(var e=[],o=0;o<t.length;o++){var r=t.charCodeAt(o);128>r?e.push(r):2048>r?e.push(192|r>>6,128|63&r):55296>r||57344<=r?e.push(224|r>>12,128|r>>6&63,128|63&r):(o++,r=65536+((1023&r)<<10|1023&t.charCodeAt(o)),e.push(240|r>>18,128|r>>12&63,128|r>>6&63,128|63&r))}return e};var o={L:1,M:0,Q:3,H:2},r=function(){function t(t){for(var e=0;0!=t;)e+=1,t>>>=1;return e}var o=[[],[6,18],[6,22],[6,26],[6,30],[6,34],[6,22,38],[6,24,42],[6,26,46],[6,28,50],[6,30,54],[6,32,58],[6,34,62],[6,26,46,66],[6,26,48,70],[6,26,50,74],[6,30,54,78],[6,30,56,82],[6,30,58,86],[6,34,62,90],[6,28,50,72,94],[6,26,50,74,98],[6,30,54,78,102],[6,28,54,80,106],[6,32,58,84,110],[6,30,58,86,114],[6,34,62,90,118],[6,26,50,74,98,122],[6,30,54,78,102,126],[6,26,52,78,104,130],[6,30,56,82,108,134],[6,34,60,86,112,138],[6,30,58,86,114,142],[6,34,62,90,118,146],[6,30,54,78,102,126,150],[6,24,50,76,102,128,154],[6,28,54,80,106,132,158],[6,32,58,84,110,136,162],[6,26,54,82,110,138,166],[6,30,58,86,114,142,170]],r={w:function(e){for(var o=e<<10;0<=t(o)-t(1335);)o^=1335<<t(o)-t(1335);return 21522^(e<<10|o)},A:function(e){for(var o=e<<12;0<=t(o)-t(7973);)o^=7973<<t(o)-t(7973);return e<<12|o},G:function(t){return o[t-1]},F:function(t){switch(t){case 0:return function(t,e){return 0==(t+e)%2};case 1:return function(t){return 0==t%2};case 2:return function(t,e){return 0==e%3};case 3:return function(t,e){return 0==(t+e)%3};case 4:return function(t,e){return 0==(Math.floor(t/2)+Math.floor(e/3))%2};case 5:return function(t,e){return 0==t*e%2+t*e%3};case 6:return function(t,e){return 0==(t*e%2+t*e%3)%2};case 7:return function(t,e){return 0==(t*e%3+(t+e)%2)%2};default:throw Error("bad maskPattern:"+t)}},C:function(t){for(var o=e([1],0),r=0;r<t;r+=1)o=o.multiply(e([1,i.i(r)],0));return o},f:function(t,e){if(4!=t||1>e||40<e)throw Error("mode: "+t+"; type: "+e);return 10>e?8:16},D:function(t){for(var e=t.h(),o=0,r=0;r<e;r+=1)for(var i=0;i<e;i+=1){for(var a=0,s=t.a(r,i),n=-1;1>=n;n+=1)if(!(0>r+n||e<=r+n))for(var l=-1;1>=l;l+=1)0>i+l||e<=i+l||(0!=n||0!=l)&&s==t.a(r+n,i+l)&&(a+=1);5<a&&(o+=3+a-5)}for(r=0;r<e-1;r+=1)for(i=0;i<e-1;i+=1)a=0,t.a(r,i)&&(a+=1),t.a(r+1,i)&&(a+=1),t.a(r,i+1)&&(a+=1),t.a(r+1,i+1)&&(a+=1),(0==a||4==a)&&(o+=3);for(r=0;r<e;r+=1)for(i=0;i<e-6;i+=1)t.a(r,i)&&!t.a(r,i+1)&&t.a(r,i+2)&&t.a(r,i+3)&&t.a(r,i+4)&&!t.a(r,i+5)&&t.a(r,i+6)&&(o+=40);for(i=0;i<e;i+=1)for(r=0;r<e-6;r+=1)t.a(r,i)&&!t.a(r+1,i)&&t.a(r+2,i)&&t.a(r+3,i)&&t.a(r+4,i)&&!t.a(r+5,i)&&t.a(r+6,i)&&(o+=40);for(i=a=0;i<e;i+=1)for(r=0;r<e;r+=1)t.a(r,i)&&(a+=1);return o+Math.abs(100*a/e/e-50)/5*10}};return r}(),i=function(){for(var t=Array(256),e=Array(256),o=0;8>o;o+=1)t[o]=1<<o;for(o=8;256>o;o+=1)t[o]=t[o-4]^t[o-5]^t[o-6]^t[o-8];for(o=0;255>o;o+=1)e[t[o]]=o;return{g:function(t){if(1>t)throw Error("glog("+t+")");return e[t]},i:function(e){for(;0>e;)e+=255;for(;256<=e;)e-=255;return t[e]}}}(),a=function(){var t=[[1,26,19],[1,26,16],[1,26,13],[1,26,9],[1,44,34],[1,44,28],[1,44,22],[1,44,16],[1,70,55],[1,70,44],[2,35,17],[2,35,13],[1,100,80],[2,50,32],[2,50,24],[4,25,9],[1,134,108],[2,67,43],[2,33,15,2,34,16],[2,33,11,2,34,12],[2,86,68],[4,43,27],[4,43,19],[4,43,15],[2,98,78],[4,49,31],[2,32,14,4,33,15],[4,39,13,1,40,14],[2,121,97],[2,60,38,2,61,39],[4,40,18,2,41,19],[4,40,14,2,41,15],[2,146,116],[3,58,36,2,59,37],[4,36,16,4,37,17],[4,36,12,4,37,13],[2,86,68,2,87,69],[4,69,43,1,70,44],[6,43,19,2,44,20],[6,43,15,2,44,16],[4,101,81],[1,80,50,4,81,51],[4,50,22,4,51,23],[3,36,12,8,37,13],[2,116,92,2,117,93],[6,58,36,2,59,37],[4,46,20,6,47,21],[7,42,14,4,43,15],[4,133,107],[8,59,37,1,60,38],[8,44,20,4,45,21],[12,33,11,4,34,12],[3,145,115,1,146,116],[4,64,40,5,65,41],[11,36,16,5,37,17],[11,36,12,5,37,13],[5,109,87,1,110,88],[5,65,41,5,66,42],[5,54,24,7,55,25],[11,36,12,7,37,13],[5,122,98,1,123,99],[7,73,45,3,74,46],[15,43,19,2,44,20],[3,45,15,13,46,16],[1,135,107,5,136,108],[10,74,46,1,75,47],[1,50,22,15,51,23],[2,42,14,17,43,15],[5,150,120,1,151,121],[9,69,43,4,70,44],[17,50,22,1,51,23],[2,42,14,19,43,15],[3,141,113,4,142,114],[3,70,44,11,71,45],[17,47,21,4,48,22],[9,39,13,16,40,14],[3,135,107,5,136,108],[3,67,41,13,68,42],[15,54,24,5,55,25],[15,43,15,10,44,16],[4,144,116,4,145,117],[17,68,42],[17,50,22,6,51,23],[19,46,16,6,47,17],[2,139,111,7,140,112],[17,74,46],[7,54,24,16,55,25],[34,37,13],[4,151,121,5,152,122],[4,75,47,14,76,48],[11,54,24,14,55,25],[16,45,15,14,46,16],[6,147,117,4,148,118],[6,73,45,14,74,46],[11,54,24,16,55,25],[30,46,16,2,47,17],[8,132,106,4,133,107],[8,75,47,13,76,48],[7,54,24,22,55,25],[22,45,15,13,46,16],[10,142,114,2,143,115],[19,74,46,4,75,47],[28,50,22,6,51,23],[33,46,16,4,47,17],[8,152,122,4,153,123],[22,73,45,3,74,46],[8,53,23,26,54,24],[12,45,15,28,46,16],[3,147,117,10,148,118],[3,73,45,23,74,46],[4,54,24,31,55,25],[11,45,15,31,46,16],[7,146,116,7,147,117],[21,73,45,7,74,46],[1,53,23,37,54,24],[19,45,15,26,46,16],[5,145,115,10,146,116],[19,75,47,10,76,48],[15,54,24,25,55,25],[23,45,15,25,46,16],[13,145,115,3,146,116],[2,74,46,29,75,47],[42,54,24,1,55,25],[23,45,15,28,46,16],[17,145,115],[10,74,46,23,75,47],[10,54,24,35,55,25],[19,45,15,35,46,16],[17,145,115,1,146,116],[14,74,46,21,75,47],[29,54,24,19,55,25],[11,45,15,46,46,16],[13,145,115,6,146,116],[14,74,46,23,75,47],[44,54,24,7,55,25],[59,46,16,1,47,17],[12,151,121,7,152,122],[12,75,47,26,76,48],[39,54,24,14,55,25],[22,45,15,41,46,16],[6,151,121,14,152,122],[6,75,47,34,76,48],[46,54,24,10,55,25],[2,45,15,64,46,16],[17,152,122,4,153,123],[29,74,46,14,75,47],[49,54,24,10,55,25],[24,45,15,46,46,16],[4,152,122,18,153,123],[13,74,46,32,75,47],[48,54,24,14,55,25],[42,45,15,32,46,16],[20,147,117,4,148,118],[40,75,47,7,76,48],[43,54,24,22,55,25],[10,45,15,67,46,16],[19,148,118,6,149,119],[18,75,47,31,76,48],[34,54,24,34,55,25],[20,45,15,61,46,16]],e={I:function(e,r){var i=function(e,r){switch(r){case o.L:return t[4*(e-1)];case o.M:return t[4*(e-1)+1];case o.Q:return t[4*(e-1)+2];case o.H:return t[4*(e-1)+3]}}(e,r);if(void 0===i)throw Error("bad rs block @ typeNumber:"+e+"/errorCorrectLevel:"+r);e=i.length/3,r=[];for(var a=0;a<e;a+=1)for(var s=i[3*a],n=i[3*a+1],l=i[3*a+2],c=0;c<s;c+=1){var d=l,h={};h.o=n,h.j=d,r.push(h)}return r}};return e}();return t}());var qo=QrCreator,jo=class extends et{constructor(){super(...arguments),this.value="",this.label="",this.size=128,this.fill="#000",this.background="#fff",this.radius=0,this.errorCorrection="H"}firstUpdated(){this.generate()}generate(){this.hasUpdated&&qo.render({text:this.value,radius:this.radius,ecLevel:this.errorCorrection,fill:this.fill,background:"transparent"===this.background?null:this.background,size:2*this.size},this.canvas)}render(){return O`
      <div
        class="qr-code"
        part="base"
        style=${vo({width:`${this.size}px`,height:`${this.size}px`})}
      >
        <canvas role="img" aria-label=${this.label.length>0?this.label:this.value}></canvas>
      </div>
    `}};jo.styles=Uo,Dt([Gt("canvas")],jo.prototype,"canvas",2),Dt([Kt()],jo.prototype,"value",2),Dt([Kt()],jo.prototype,"label",2),Dt([Kt({type:Number})],jo.prototype,"size",2),Dt([Kt()],jo.prototype,"fill",2),Dt([Kt()],jo.prototype,"background",2),Dt([Kt({type:Number})],jo.prototype,"radius",2),Dt([Kt({attribute:"error-correction"})],jo.prototype,"errorCorrection",2),Dt([Ut("background"),Ut("errorCorrection"),Ut("fill"),Ut("radius"),Ut("size"),Ut("value")],jo.prototype,"generate",1),jo=Dt([qt("sl-qr-code")],jo);var Ko=n`
  ${it}

  :host {
    display: block;
  }

  :host(:focus-visible) {
    outline: 0px;
  }

  .radio {
    display: inline-flex;
    align-items: top;
    font-family: var(--sl-input-font-family);
    font-size: var(--sl-input-font-size-medium);
    font-weight: var(--sl-input-font-weight);
    color: var(--sl-input-color);
    vertical-align: middle;
    cursor: pointer;
  }

  .radio__icon {
    display: inline-flex;
    width: var(--sl-toggle-size);
    height: var(--sl-toggle-size);
  }

  .radio__icon svg {
    width: 100%;
    height: 100%;
  }

  .radio__control {
    flex: 0 0 auto;
    position: relative;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: var(--sl-toggle-size);
    height: var(--sl-toggle-size);
    border: solid var(--sl-input-border-width) var(--sl-input-border-color);
    border-radius: 50%;
    background-color: var(--sl-input-background-color);
    color: transparent;
    transition: var(--sl-transition-fast) border-color, var(--sl-transition-fast) background-color,
      var(--sl-transition-fast) color, var(--sl-transition-fast) box-shadow;
  }

  .radio__input {
    position: absolute;
    opacity: 0;
    padding: 0;
    margin: 0;
    pointer-events: none;
  }

  /* Hover */
  .radio:not(.radio--checked):not(.radio--disabled) .radio__control:hover {
    border-color: var(--sl-input-border-color-hover);
    background-color: var(--sl-input-background-color-hover);
  }

  /* Checked */
  .radio--checked .radio__control {
    color: var(--sl-color-neutral-0);
    border-color: var(--sl-color-primary-600);
    background-color: var(--sl-color-primary-600);
  }

  /* Checked + hover */
  .radio.radio--checked:not(.radio--disabled) .radio__control:hover {
    border-color: var(--sl-color-primary-500);
    background-color: var(--sl-color-primary-500);
  }

  /* Checked + focus */
  :host(:focus-visible) .radio__control {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  /* Disabled */
  .radio--disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  /* When the control isn't checked, hide the circle for Windows High Contrast mode a11y */
  .radio:not(.radio--checked) svg circle {
    opacity: 0;
  }

  .radio__label {
    color: var(--sl-input-label-color);
    line-height: var(--sl-toggle-size);
    margin-inline-start: 0.5em;
    user-select: none;
  }
`,Xo=class extends et{constructor(){super(...arguments),this.checked=!1,this.hasFocus=!1,this.disabled=!1}connectedCallback(){super.connectedCallback(),this.setInitialAttributes(),this.addEventListeners()}handleCheckedChange(){this.setAttribute("aria-checked",this.checked?"true":"false"),this.setAttribute("tabindex",this.checked?"0":"-1")}handleDisabledChange(){this.setAttribute("aria-disabled",this.disabled?"true":"false")}handleBlur(){this.hasFocus=!1,Nt(this,"sl-blur")}handleClick(){this.disabled||(this.checked=!0)}handleFocus(){this.hasFocus=!0,Nt(this,"sl-focus")}addEventListeners(){this.addEventListener("blur",(()=>this.handleBlur())),this.addEventListener("click",(()=>this.handleClick())),this.addEventListener("focus",(()=>this.handleFocus()))}setInitialAttributes(){this.setAttribute("role","radio"),this.setAttribute("tabindex","-1"),this.setAttribute("aria-disabled",this.disabled?"true":"false")}render(){return O`
      <span
        part="base"
        class=${Vt({radio:!0,"radio--checked":this.checked,"radio--disabled":this.disabled,"radio--focused":this.hasFocus})}
      >
        <span part="control" class="radio__control">
          <svg part="checked-icon" class="radio__icon" viewBox="0 0 16 16">
            <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
              <g fill="currentColor">
                <circle cx="8" cy="8" r="3.42857143"></circle>
              </g>
            </g>
          </svg>
        </span>

        <span part="label" class="radio__label">
          <slot></slot>
        </span>
      </span>
    `}};Xo.styles=Ko,Dt([Xt()],Xo.prototype,"checked",2),Dt([Xt()],Xo.prototype,"hasFocus",2),Dt([Kt()],Xo.prototype,"value",2),Dt([Kt({type:Boolean,reflect:!0})],Xo.prototype,"disabled",2),Dt([Ut("checked")],Xo.prototype,"handleCheckedChange",1),Dt([Ut("disabled",{waitUntilFirstUpdate:!0})],Xo.prototype,"handleDisabledChange",1),Xo=Dt([qt("sl-radio")],Xo);var Wo=n`
  ${it}

  :host {
    display: block;
  }

  .menu-item {
    position: relative;
    display: flex;
    align-items: stretch;
    font-family: var(--sl-font-sans);
    font-size: var(--sl-font-size-medium);
    font-weight: var(--sl-font-weight-normal);
    line-height: var(--sl-line-height-normal);
    letter-spacing: var(--sl-letter-spacing-normal);
    color: var(--sl-color-neutral-700);
    padding: var(--sl-spacing-2x-small) var(--sl-spacing-2x-small);
    transition: var(--sl-transition-fast) fill;
    user-select: none;
    white-space: nowrap;
    cursor: pointer;
  }

  .menu-item.menu-item--disabled {
    outline: none;
    opacity: 0.5;
    cursor: not-allowed;
  }

  .menu-item .menu-item__label {
    flex: 1 1 auto;
  }

  .menu-item .menu-item__prefix {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
  }

  .menu-item .menu-item__prefix ::slotted(*) {
    margin-inline-end: var(--sl-spacing-x-small);
  }

  .menu-item .menu-item__suffix {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
  }

  .menu-item .menu-item__suffix ::slotted(*) {
    margin-inline-start: var(--sl-spacing-x-small);
  }

  :host(:focus) {
    outline: none;
  }

  :host(:hover:not([aria-disabled='true'])) .menu-item,
  :host(:focus-visible:not(.sl-focus-invisible):not([aria-disabled='true'])) .menu-item {
    outline: none;
    background-color: var(--sl-color-primary-600);
    color: var(--sl-color-neutral-0);
  }

  .menu-item .menu-item__check,
  .menu-item .menu-item__chevron {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 1.5em;
    visibility: hidden;
  }

  .menu-item--checked .menu-item__check,
  .menu-item--has-submenu .menu-item__chevron {
    visibility: visible;
  }
`,Yo=class extends et{constructor(){super(...arguments),this.checked=!1,this.value="",this.disabled=!1}firstUpdated(){this.setAttribute("role","menuitem")}getTextLabel(){return Pt(this.defaultSlot)}handleCheckedChange(){this.setAttribute("aria-checked",this.checked?"true":"false")}handleDisabledChange(){this.setAttribute("aria-disabled",this.disabled?"true":"false")}handleDefaultSlotChange(){const t=this.getTextLabel();void 0!==this.cachedTextLabel?t!==this.cachedTextLabel&&(this.cachedTextLabel=t,Nt(this,"sl-label-change")):this.cachedTextLabel=t}render(){return O`
      <div
        part="base"
        class=${Vt({"menu-item":!0,"menu-item--checked":this.checked,"menu-item--disabled":this.disabled,"menu-item--has-submenu":!1})}
      >
        <span part="checked-icon" class="menu-item__check">
          <sl-icon name="check-lg" library="system" aria-hidden="true"></sl-icon>
        </span>

        <span part="prefix" class="menu-item__prefix">
          <slot name="prefix"></slot>
        </span>

        <span part="label" class="menu-item__label">
          <slot @slotchange=${this.handleDefaultSlotChange}></slot>
        </span>

        <span part="suffix" class="menu-item__suffix">
          <slot name="suffix"></slot>
        </span>

        <span class="menu-item__chevron">
          <sl-icon name="chevron-right" library="system" aria-hidden="true"></sl-icon>
        </span>
      </div>
    `}};Yo.styles=Wo,Dt([Gt("slot:not([name])")],Yo.prototype,"defaultSlot",2),Dt([Gt(".menu-item")],Yo.prototype,"menuItem",2),Dt([Kt({type:Boolean,reflect:!0})],Yo.prototype,"checked",2),Dt([Kt()],Yo.prototype,"value",2),Dt([Kt({type:Boolean,reflect:!0})],Yo.prototype,"disabled",2),Dt([Ut("checked")],Yo.prototype,"handleCheckedChange",1),Dt([Ut("disabled")],Yo.prototype,"handleDisabledChange",1),Yo=Dt([qt("sl-menu-item")],Yo);var Go=n`
  ${it}

  :host {
    display: block;
  }

  .menu-label {
    font-family: var(--sl-font-sans);
    font-size: var(--sl-font-size-small);
    font-weight: var(--sl-font-weight-semibold);
    line-height: var(--sl-line-height-normal);
    letter-spacing: var(--sl-letter-spacing-normal);
    color: var(--sl-color-neutral-500);
    padding: var(--sl-spacing-2x-small) var(--sl-spacing-x-large);
    user-select: none;
  }
`,Zo=class extends et{render(){return O`
      <div part="base" class="menu-label">
        <slot></slot>
      </div>
    `}};Zo.styles=Go,Zo=Dt([qt("sl-menu-label")],Zo);var Qo=n`
  ${it}

  :host {
    display: contents;
  }
`,Jo=class extends et{constructor(){super(...arguments),this.attrOldValue=!1,this.charData=!1,this.charDataOldValue=!1,this.childList=!1,this.disabled=!1}connectedCallback(){super.connectedCallback(),this.handleMutation=this.handleMutation.bind(this),this.mutationObserver=new MutationObserver(this.handleMutation),this.disabled||this.startObserver()}disconnectedCallback(){this.stopObserver()}handleDisabledChange(){this.disabled?this.stopObserver():this.startObserver()}handleChange(){this.stopObserver(),this.startObserver()}handleMutation(t){Nt(this,"sl-mutation",{detail:{mutationList:t}})}startObserver(){const t="string"==typeof this.attr&&this.attr.length>0,e=t&&"*"!==this.attr?this.attr.split(" "):void 0;try{this.mutationObserver.observe(this,{subtree:!0,childList:this.childList,attributes:t,attributeFilter:e,attributeOldValue:this.attrOldValue,characterData:this.charData,characterDataOldValue:this.charDataOldValue})}catch(t){}}stopObserver(){this.mutationObserver.disconnect()}render(){return O` <slot></slot> `}};Jo.styles=Qo,Dt([Kt({reflect:!0})],Jo.prototype,"attr",2),Dt([Kt({attribute:"attr-old-value",type:Boolean,reflect:!0})],Jo.prototype,"attrOldValue",2),Dt([Kt({attribute:"char-data",type:Boolean,reflect:!0})],Jo.prototype,"charData",2),Dt([Kt({attribute:"char-data-old-value",type:Boolean,reflect:!0})],Jo.prototype,"charDataOldValue",2),Dt([Kt({attribute:"child-list",type:Boolean,reflect:!0})],Jo.prototype,"childList",2),Dt([Kt({type:Boolean,reflect:!0})],Jo.prototype,"disabled",2),Dt([Ut("disabled")],Jo.prototype,"handleDisabledChange",1),Dt([Ut("attr",{waitUntilFirstUpdate:!0}),Ut("attr-old-value",{waitUntilFirstUpdate:!0}),Ut("char-data",{waitUntilFirstUpdate:!0}),Ut("char-data-old-value",{waitUntilFirstUpdate:!0}),Ut("childList",{waitUntilFirstUpdate:!0})],Jo.prototype,"handleChange",1),Jo=Dt([qt("sl-mutation-observer")],Jo);var tr=n`
  ${it}

  :host {
    --divider-width: 2px;
    --handle-size: 2.5rem;

    display: inline-block;
    position: relative;
  }

  .image-comparer {
    max-width: 100%;
    max-height: 100%;
    overflow: hidden;
  }

  .image-comparer__before,
  .image-comparer__after {
    pointer-events: none;
  }

  .image-comparer__before ::slotted(img),
  .image-comparer__after ::slotted(img),
  .image-comparer__before ::slotted(svg),
  .image-comparer__after ::slotted(svg) {
    display: block;
    max-width: 100% !important;
    height: auto;
  }

  .image-comparer__after {
    position: absolute;
    top: 0;
    left: 0;
    height: 100%;
    width: 100%;
  }

  .image-comparer__divider {
    display: flex;
    align-items: center;
    justify-content: center;
    position: absolute;
    top: 0;
    width: var(--divider-width);
    height: 100%;
    background-color: var(--sl-color-neutral-0);
    transform: translateX(calc(var(--divider-width) / -2));
    cursor: ew-resize;
  }

  .image-comparer__handle {
    display: flex;
    align-items: center;
    justify-content: center;
    position: absolute;
    top: calc(50% - (var(--handle-size) / 2));
    width: var(--handle-size);
    height: var(--handle-size);
    background-color: var(--sl-color-neutral-0);
    border-radius: var(--sl-border-radius-circle);
    font-size: calc(var(--handle-size) * 0.5);
    color: var(--sl-color-neutral-600);
    cursor: inherit;
    z-index: 10;
  }

  .image-comparer__handle:focus-visible {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }
`,er=class extends et{constructor(){super(...arguments),this.position=50}handleDrag(t){const{width:e}=this.base.getBoundingClientRect();t.preventDefault(),Ne(this.base,{onMove:t=>{this.position=parseFloat(Ce(t/e*100,0,100).toFixed(2))},initialEvent:t})}handleKeyDown(t){if(["ArrowLeft","ArrowRight","Home","End"].includes(t.key)){const e=t.shiftKey?10:1;let o=this.position;t.preventDefault(),"ArrowLeft"===t.key&&(o-=e),"ArrowRight"===t.key&&(o+=e),"Home"===t.key&&(o=0),"End"===t.key&&(o=100),o=Ce(o,0,100),this.position=o}}handlePositionChange(){Nt(this,"sl-change")}render(){return O`
      <div part="base" id="image-comparer" class="image-comparer" @keydown=${this.handleKeyDown}>
        <div class="image-comparer__image">
          <div part="before" class="image-comparer__before">
            <slot name="before"></slot>
          </div>

          <div
            part="after"
            class="image-comparer__after"
            style=${vo({clipPath:`inset(0 ${100-this.position}% 0 0)`})}
          >
            <slot name="after"></slot>
          </div>
        </div>

        <div
          part="divider"
          class="image-comparer__divider"
          style=${vo({left:`${this.position}%`})}
          @mousedown=${this.handleDrag}
          @touchstart=${this.handleDrag}
        >
          <div
            part="handle"
            class="image-comparer__handle"
            role="scrollbar"
            aria-valuenow=${this.position}
            aria-valuemin="0"
            aria-valuemax="100"
            aria-controls="image-comparer"
            tabindex="0"
          >
            <slot name="handle-icon">
              <svg width="24" height="24" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <g fill="currentColor" fill-rule="nonzero">
                  <path
                    d="m21.14 12.55-5.482 4.796c-.646.566-1.658.106-1.658-.753V7a1 1 0 0 1 1.659-.753l5.48 4.796a1 1 0 0 1 0 1.506h.001ZM2.341 12.55l5.482 4.796c.646.566 1.658.106 1.658-.753V7a1 1 0 0 0-1.659-.753l-5.48 4.796a1 1 0 0 0 0 1.506h-.001Z"
                  />
                </g>
              </svg>
            </slot>
          </div>
        </div>
      </div>
    `}};er.styles=tr,Dt([Gt(".image-comparer")],er.prototype,"base",2),Dt([Gt(".image-comparer__handle")],er.prototype,"handle",2),Dt([Kt({type:Number,reflect:!0})],er.prototype,"position",2),Dt([Ut("position",{waitUntilFirstUpdate:!0})],er.prototype,"handlePositionChange",1),er=Dt([qt("sl-image-comparer")],er);var or=n`
  ${it}

  :host {
    display: block;
  }
`,rr=class extends et{constructor(){super(...arguments),this.mode="cors",this.allowScripts=!1}executeScript(t){const e=document.createElement("script");[...t.attributes].forEach((t=>e.setAttribute(t.name,t.value))),e.textContent=t.textContent,t.parentNode.replaceChild(e,t)}async handleSrcChange(){try{const t=this.src,e=await So(t,this.mode);if(t!==this.src)return;if(!e.ok)return void Nt(this,"sl-error",{detail:{status:e.status}});this.innerHTML=e.html,this.allowScripts&&[...this.querySelectorAll("script")].forEach((t=>this.executeScript(t))),Nt(this,"sl-load")}catch(t){Nt(this,"sl-error",{detail:{status:-1}})}}render(){return O`<slot></slot>`}};rr.styles=or,Dt([Kt()],rr.prototype,"src",2),Dt([Kt()],rr.prototype,"mode",2),Dt([Kt({attribute:"allow-scripts",type:Boolean})],rr.prototype,"allowScripts",2),Dt([Ut("src")],rr.prototype,"handleSrcChange",1),rr=Dt([qt("sl-include")],rr);var ir=n`
  ${it}

  :host {
    display: block;
  }

  .menu {
    background: var(--sl-panel-background-color);
    border: solid var(--sl-panel-border-width) var(--sl-panel-border-color);
    border-radius: var(--sl-border-radius-medium);
    background: var(--sl-panel-background-color);
    padding: var(--sl-spacing-x-small) 0;
  }

  ::slotted(sl-divider) {
    --spacing: var(--sl-spacing-x-small);
  }
`,ar=class extends et{constructor(){super(...arguments),this.typeToSelectString=""}connectedCallback(){super.connectedCallback(),this.setAttribute("role","menu")}getAllItems(t={includeDisabled:!0}){return[...this.defaultSlot.assignedElements({flatten:!0})].filter((e=>"menuitem"===e.getAttribute("role")&&!(!t.includeDisabled&&e.disabled)))}getCurrentItem(){return this.getAllItems({includeDisabled:!1}).find((t=>"0"===t.getAttribute("tabindex")))}setCurrentItem(t){const e=this.getAllItems({includeDisabled:!1}),o=t.disabled?e[0]:t;e.forEach((t=>{t.setAttribute("tabindex",t===o?"0":"-1")}))}typeToSelect(t){var e;const o=this.getAllItems({includeDisabled:!1});clearTimeout(this.typeToSelectTimeout),this.typeToSelectTimeout=window.setTimeout((()=>this.typeToSelectString=""),1e3),"Backspace"===t.key?t.metaKey||t.ctrlKey?this.typeToSelectString="":this.typeToSelectString=this.typeToSelectString.slice(0,-1):this.typeToSelectString+=t.key.toLowerCase();for(const t of o){if(Pt(null==(e=t.shadowRoot)?void 0:e.querySelector("slot:not([name])")).toLowerCase().trim().startsWith(this.typeToSelectString)){this.setCurrentItem(t),t.focus();break}}}handleClick(t){const e=t.target.closest("sl-menu-item");!1===(null==e?void 0:e.disabled)&&Nt(this,"sl-select",{detail:{item:e}})}handleKeyDown(t){if("Enter"===t.key){const e=this.getCurrentItem();t.preventDefault(),null==e||e.click()}if(" "===t.key&&t.preventDefault(),["ArrowDown","ArrowUp","Home","End"].includes(t.key)){const e=this.getAllItems({includeDisabled:!1}),o=this.getCurrentItem();let r=o?e.indexOf(o):0;if(e.length>0)return t.preventDefault(),"ArrowDown"===t.key?r++:"ArrowUp"===t.key?r--:"Home"===t.key?r=0:"End"===t.key&&(r=e.length-1),r<0&&(r=e.length-1),r>e.length-1&&(r=0),this.setCurrentItem(e[r]),void e[r].focus()}this.typeToSelect(t)}handleMouseDown(t){const e=t.target;"menuitem"===e.getAttribute("role")&&this.setCurrentItem(e)}handleSlotChange(){const t=this.getAllItems({includeDisabled:!1});t.length>0&&this.setCurrentItem(t[0])}render(){return O`
      <div
        part="base"
        class="menu"
        @click=${this.handleClick}
        @keydown=${this.handleKeyDown}
        @mousedown=${this.handleMouseDown}
      >
        <slot @slotchange=${this.handleSlotChange}></slot>
      </div>
    `}};ar.styles=ir,Dt([Gt(".menu")],ar.prototype,"menu",2),Dt([Gt("slot")],ar.prototype,"defaultSlot",2),ar=Dt([qt("sl-menu")],ar);var sr=n`
  ${it}

  :host {
    --size: 25rem;
    --header-spacing: var(--sl-spacing-large);
    --body-spacing: var(--sl-spacing-large);
    --footer-spacing: var(--sl-spacing-large);

    display: contents;
  }

  .drawer {
    top: 0;
    inset-inline-start: 0;
    width: 100%;
    height: 100%;
    pointer-events: none;
    overflow: hidden;
  }

  .drawer--contained {
    position: absolute;
    z-index: initial;
  }

  .drawer--fixed {
    position: fixed;
    z-index: var(--sl-z-index-drawer);
  }

  .drawer__panel {
    position: absolute;
    display: flex;
    flex-direction: column;
    z-index: 2;
    max-width: 100%;
    max-height: 100%;
    background-color: var(--sl-panel-background-color);
    box-shadow: var(--sl-shadow-x-large);
    transition: var(--sl-transition-medium) transform;
    overflow: auto;
    pointer-events: all;
  }

  .drawer__panel:focus {
    outline: none;
  }

  .drawer--top .drawer__panel {
    top: 0;
    inset-inline-end: auto;
    bottom: auto;
    inset-inline-start: 0;
    width: 100%;
    height: var(--size);
  }

  .drawer--end .drawer__panel {
    top: 0;
    inset-inline-end: 0;
    bottom: auto;
    inset-inline-start: auto;
    width: var(--size);
    height: 100%;
  }

  .drawer--bottom .drawer__panel {
    top: auto;
    inset-inline-end: auto;
    bottom: 0;
    inset-inline-start: 0;
    width: 100%;
    height: var(--size);
  }

  .drawer--start .drawer__panel {
    top: 0;
    inset-inline-end: auto;
    bottom: auto;
    inset-inline-start: 0;
    width: var(--size);
    height: 100%;
  }

  .drawer__header {
    display: flex;
  }

  .drawer__title {
    flex: 1 1 auto;
    font: inherit;
    font-size: var(--sl-font-size-large);
    line-height: var(--sl-line-height-dense);
    padding: var(--header-spacing);
    margin: 0;
  }

  .drawer__close {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
    font-size: var(--sl-font-size-x-large);
    padding: 0 var(--header-spacing);
  }

  .drawer__body {
    flex: 1 1 auto;
    padding: var(--body-spacing);
    overflow: auto;
    -webkit-overflow-scrolling: touch;
  }

  .drawer__footer {
    text-align: right;
    padding: var(--footer-spacing);
  }

  .drawer__footer ::slotted(sl-button:not(:last-of-type)) {
    margin-inline-end: var(--sl-spacing-x-small);
  }

  .drawer:not(.drawer--has-footer) .drawer__footer {
    display: none;
  }

  .drawer__overlay {
    display: block;
    position: fixed;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    background-color: var(--sl-overlay-background-color);
    pointer-events: all;
  }

  .drawer--contained .drawer__overlay {
    position: absolute;
  }
`;function lr(t){const e=t.tagName.toLowerCase();return"-1"!==t.getAttribute("tabindex")&&(!t.hasAttribute("disabled")&&((!t.hasAttribute("aria-disabled")||"false"===t.getAttribute("aria-disabled"))&&(!("input"===e&&"radio"===t.getAttribute("type")&&!t.hasAttribute("checked"))&&(null!==t.offsetParent&&("hidden"!==window.getComputedStyle(t).visibility&&(!("audio"!==e&&"video"!==e||!t.hasAttribute("controls"))||(!!t.hasAttribute("tabindex")||(!(!t.hasAttribute("contenteditable")||"false"===t.getAttribute("contenteditable"))||["button","input","select","textarea","a","audio","video","summary"].includes(e)))))))))}function cr(t){var e,o;const r=[];!function t(e){e instanceof HTMLElement&&(r.push(e),null!==e.shadowRoot&&"open"===e.shadowRoot.mode&&t(e.shadowRoot)),[...e.children].forEach((e=>t(e)))}(t);return{start:null!=(e=r.find((t=>lr(t))))?e:null,end:null!=(o=r.reverse().find((t=>lr(t))))?o:null}}var dr=[],hr=class{constructor(t){this.tabDirection="forward",this.element=t,this.handleFocusIn=this.handleFocusIn.bind(this),this.handleKeyDown=this.handleKeyDown.bind(this),this.handleKeyUp=this.handleKeyUp.bind(this)}activate(){dr.push(this.element),document.addEventListener("focusin",this.handleFocusIn),document.addEventListener("keydown",this.handleKeyDown),document.addEventListener("keyup",this.handleKeyUp)}deactivate(){dr=dr.filter((t=>t!==this.element)),document.removeEventListener("focusin",this.handleFocusIn),document.removeEventListener("keydown",this.handleKeyDown),document.removeEventListener("keyup",this.handleKeyUp)}isActive(){return dr[dr.length-1]===this.element}checkFocus(){if(this.isActive()&&!this.element.matches(":focus-within")){const{start:t,end:e}=cr(this.element),o="forward"===this.tabDirection?t:e;"function"==typeof(null==o?void 0:o.focus)&&o.focus({preventScroll:!0})}}handleFocusIn(){this.checkFocus()}handleKeyDown(t){"Tab"===t.key&&t.shiftKey&&(this.tabDirection="backward"),requestAnimationFrame((()=>this.checkFocus()))}handleKeyUp(){this.tabDirection="forward"}};function ur(t){return t.charAt(0).toUpperCase()+t.slice(1)}var pr=class extends et{constructor(){super(...arguments),this.hasSlotController=new Bt(this,"footer"),this.localize=new ge(this),this.open=!1,this.label="",this.placement="end",this.contained=!1,this.noHeader=!1}connectedCallback(){super.connectedCallback(),this.modal=new hr(this)}firstUpdated(){this.drawer.hidden=!this.open,this.open&&!this.contained&&(this.modal.activate(),Me(this))}disconnectedCallback(){super.disconnectedCallback(),Oe(this)}async show(){if(!this.open)return this.open=!0,Ht(this,"sl-after-show")}async hide(){if(this.open)return this.open=!1,Ht(this,"sl-after-hide")}requestClose(t){if(Nt(this,"sl-request-close",{cancelable:!0,detail:{source:t}}).defaultPrevented){const t=ce(this,"drawer.denyClose",{dir:this.localize.dir()});Jt(this.panel,t.keyframes,t.options)}else this.hide()}handleKeyDown(t){"Escape"===t.key&&(t.stopPropagation(),this.requestClose("keyboard"))}async handleOpenChange(){if(this.open){Nt(this,"sl-show"),this.originalTrigger=document.activeElement,this.contained||(this.modal.activate(),Me(this));const t=this.querySelector("[autofocus]");t&&t.removeAttribute("autofocus"),await Promise.all([oe(this.drawer),oe(this.overlay)]),this.drawer.hidden=!1,requestAnimationFrame((()=>{Nt(this,"sl-initial-focus",{cancelable:!0}).defaultPrevented||(t?t.focus({preventScroll:!0}):this.panel.focus({preventScroll:!0})),t&&t.setAttribute("autofocus","")}));const e=ce(this,`drawer.show${ur(this.placement)}`,{dir:this.localize.dir()}),o=ce(this,"drawer.overlay.show",{dir:this.localize.dir()});await Promise.all([Jt(this.panel,e.keyframes,e.options),Jt(this.overlay,o.keyframes,o.options)]),Nt(this,"sl-after-show")}else{Nt(this,"sl-hide"),this.modal.deactivate(),Oe(this),await Promise.all([oe(this.drawer),oe(this.overlay)]);const t=ce(this,`drawer.hide${ur(this.placement)}`,{dir:this.localize.dir()}),e=ce(this,"drawer.overlay.hide",{dir:this.localize.dir()});await Promise.all([Jt(this.panel,t.keyframes,t.options),Jt(this.overlay,e.keyframes,e.options)]),this.drawer.hidden=!0;const o=this.originalTrigger;"function"==typeof(null==o?void 0:o.focus)&&setTimeout((()=>o.focus())),Nt(this,"sl-after-hide")}}render(){return O`
      <div
        part="base"
        class=${Vt({drawer:!0,"drawer--open":this.open,"drawer--top":"top"===this.placement,"drawer--end":"end"===this.placement,"drawer--bottom":"bottom"===this.placement,"drawer--start":"start"===this.placement,"drawer--contained":this.contained,"drawer--fixed":!this.contained,"drawer--rtl":"rtl"===this.localize.dir(),"drawer--has-footer":this.hasSlotController.test("footer")})}
        @keydown=${this.handleKeyDown}
      >
        <div part="overlay" class="drawer__overlay" @click=${()=>this.requestClose("overlay")} tabindex="-1"></div>

        <div
          part="panel"
          class="drawer__panel"
          role="dialog"
          aria-modal="true"
          aria-hidden=${this.open?"false":"true"}
          aria-label=${Rt(this.noHeader?this.label:void 0)}
          aria-labelledby=${Rt(this.noHeader?void 0:"title")}
          tabindex="0"
        >
          ${this.noHeader?"":O`
                <header part="header" class="drawer__header">
                  <h2 part="title" class="drawer__title" id="title">
                    <!-- If there's no label, use an invisible character to prevent the header from collapsing -->
                    <slot name="label"> ${this.label.length>0?this.label:String.fromCharCode(65279)} </slot>
                  </h2>
                  <sl-icon-button
                    part="close-button"
                    exportparts="base:close-button__base"
                    class="drawer__close"
                    name="x"
                    label=${this.localize.term("close")}
                    library="system"
                    @click=${()=>this.requestClose("close-button")}
                  ></sl-icon-button>
                </header>
              `}

          <div part="body" class="drawer__body">
            <slot></slot>
          </div>

          <footer part="footer" class="drawer__footer">
            <slot name="footer"></slot>
          </footer>
        </div>
      </div>
    `}};pr.styles=sr,Dt([Gt(".drawer")],pr.prototype,"drawer",2),Dt([Gt(".drawer__panel")],pr.prototype,"panel",2),Dt([Gt(".drawer__overlay")],pr.prototype,"overlay",2),Dt([Kt({type:Boolean,reflect:!0})],pr.prototype,"open",2),Dt([Kt({reflect:!0})],pr.prototype,"label",2),Dt([Kt({reflect:!0})],pr.prototype,"placement",2),Dt([Kt({type:Boolean,reflect:!0})],pr.prototype,"contained",2),Dt([Kt({attribute:"no-header",type:Boolean,reflect:!0})],pr.prototype,"noHeader",2),Dt([Ut("open",{waitUntilFirstUpdate:!0})],pr.prototype,"handleOpenChange",1),pr=Dt([qt("sl-drawer")],pr),le("drawer.showTop",{keyframes:[{opacity:0,transform:"translateY(-100%)"},{opacity:1,transform:"translateY(0)"}],options:{duration:250,easing:"ease"}}),le("drawer.hideTop",{keyframes:[{opacity:1,transform:"translateY(0)"},{opacity:0,transform:"translateY(-100%)"}],options:{duration:250,easing:"ease"}}),le("drawer.showEnd",{keyframes:[{opacity:0,transform:"translateX(100%)"},{opacity:1,transform:"translateX(0)"}],rtlKeyframes:[{opacity:0,transform:"translateX(-100%)"},{opacity:1,transform:"translateX(0)"}],options:{duration:250,easing:"ease"}}),le("drawer.hideEnd",{keyframes:[{opacity:1,transform:"translateX(0)"},{opacity:0,transform:"translateX(100%)"}],rtlKeyframes:[{opacity:1,transform:"translateX(0)"},{opacity:0,transform:"translateX(-100%)"}],options:{duration:250,easing:"ease"}}),le("drawer.showBottom",{keyframes:[{opacity:0,transform:"translateY(100%)"},{opacity:1,transform:"translateY(0)"}],options:{duration:250,easing:"ease"}}),le("drawer.hideBottom",{keyframes:[{opacity:1,transform:"translateY(0)"},{opacity:0,transform:"translateY(100%)"}],options:{duration:250,easing:"ease"}}),le("drawer.showStart",{keyframes:[{opacity:0,transform:"translateX(-100%)"},{opacity:1,transform:"translateX(0)"}],rtlKeyframes:[{opacity:0,transform:"translateX(100%)"},{opacity:1,transform:"translateX(0)"}],options:{duration:250,easing:"ease"}}),le("drawer.hideStart",{keyframes:[{opacity:1,transform:"translateX(0)"},{opacity:0,transform:"translateX(-100%)"}],rtlKeyframes:[{opacity:1,transform:"translateX(0)"},{opacity:0,transform:"translateX(100%)"}],options:{duration:250,easing:"ease"}}),le("drawer.denyClose",{keyframes:[{transform:"scale(1)"},{transform:"scale(1.01)"},{transform:"scale(1)"}],options:{duration:250}}),le("drawer.overlay.show",{keyframes:[{opacity:0},{opacity:1}],options:{duration:250}}),le("drawer.overlay.hide",{keyframes:[{opacity:1},{opacity:0}],options:{duration:250}});var fr=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.value=0,this.unit="byte",this.display="short"}render(){if(isNaN(this.value))return"";const t="bit"===this.unit?["","kilo","mega","giga","tera"]:["","kilo","mega","giga","tera","peta"],e=Math.max(0,Math.min(Math.floor(Math.log10(this.value)/3),t.length-1)),o=t[e]+this.unit,r=parseFloat((this.value/Math.pow(1e3,e)).toPrecision(3));return this.localize.number(r,{style:"unit",unit:o,unitDisplay:this.display})}};Dt([Kt({type:Number})],fr.prototype,"value",2),Dt([Kt()],fr.prototype,"unit",2),Dt([Kt()],fr.prototype,"display",2),Dt([Kt()],fr.prototype,"lang",2),fr=Dt([qt("sl-format-bytes")],fr);var mr=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.date=new Date,this.hourFormat="auto"}render(){const t=new Date(this.date),e="auto"===this.hourFormat?void 0:"12"===this.hourFormat;if(!isNaN(t.getMilliseconds()))return O`
      <time datetime=${t.toISOString()}>
        ${this.localize.date(t,{weekday:this.weekday,era:this.era,year:this.year,month:this.month,day:this.day,hour:this.hour,minute:this.minute,second:this.second,timeZoneName:this.timeZoneName,timeZone:this.timeZone,hour12:e})}
      </time>
    `}};Dt([Kt()],mr.prototype,"date",2),Dt([Kt()],mr.prototype,"lang",2),Dt([Kt()],mr.prototype,"weekday",2),Dt([Kt()],mr.prototype,"era",2),Dt([Kt()],mr.prototype,"year",2),Dt([Kt()],mr.prototype,"month",2),Dt([Kt()],mr.prototype,"day",2),Dt([Kt()],mr.prototype,"hour",2),Dt([Kt()],mr.prototype,"minute",2),Dt([Kt()],mr.prototype,"second",2),Dt([Kt({attribute:"time-zone-name"})],mr.prototype,"timeZoneName",2),Dt([Kt({attribute:"time-zone"})],mr.prototype,"timeZone",2),Dt([Kt({attribute:"hour-format"})],mr.prototype,"hourFormat",2),mr=Dt([qt("sl-format-date")],mr);var br=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.value=0,this.type="decimal",this.noGrouping=!1,this.currency="USD",this.currencyDisplay="symbol"}render(){return isNaN(this.value)?"":this.localize.number(this.value,{style:this.type,currency:this.currency,currencyDisplay:this.currencyDisplay,useGrouping:!this.noGrouping,minimumIntegerDigits:this.minimumIntegerDigits,minimumFractionDigits:this.minimumFractionDigits,maximumFractionDigits:this.maximumFractionDigits,minimumSignificantDigits:this.minimumSignificantDigits,maximumSignificantDigits:this.maximumSignificantDigits})}};Dt([Kt({type:Number})],br.prototype,"value",2),Dt([Kt()],br.prototype,"lang",2),Dt([Kt()],br.prototype,"type",2),Dt([Kt({attribute:"no-grouping",type:Boolean})],br.prototype,"noGrouping",2),Dt([Kt()],br.prototype,"currency",2),Dt([Kt({attribute:"currency-display"})],br.prototype,"currencyDisplay",2),Dt([Kt({attribute:"minimum-integer-digits",type:Number})],br.prototype,"minimumIntegerDigits",2),Dt([Kt({attribute:"minimum-fraction-digits",type:Number})],br.prototype,"minimumFractionDigits",2),Dt([Kt({attribute:"maximum-fraction-digits",type:Number})],br.prototype,"maximumFractionDigits",2),Dt([Kt({attribute:"minimum-significant-digits",type:Number})],br.prototype,"minimumSignificantDigits",2),Dt([Kt({attribute:"maximum-significant-digits",type:Number})],br.prototype,"maximumSignificantDigits",2),br=Dt([qt("sl-format-number")],br);var gr,vr,yr,wr=n`
  ${it}

  :host {
    --grid-width: 280px;
    --grid-height: 200px;
    --grid-handle-size: 16px;
    --slider-height: 15px;
    --slider-handle-size: 17px;
    --swatch-size: 25px;

    display: inline-block;
  }

  .color-picker {
    width: var(--grid-width);
    font-family: var(--sl-font-sans);
    font-size: var(--sl-font-size-medium);
    font-weight: var(--sl-font-weight-normal);
    color: var(--color);
    background-color: var(--sl-panel-background-color);
    border-radius: var(--sl-border-radius-medium);
    user-select: none;
  }

  .color-picker--inline {
    border: solid var(--sl-panel-border-width) var(--sl-panel-border-color);
  }

  .color-picker--inline:focus-visible {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .color-picker__grid {
    position: relative;
    height: var(--grid-height);
    background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0) 0%, rgba(0, 0, 0, 1) 100%),
      linear-gradient(to right, #fff 0%, rgba(255, 255, 255, 0) 100%);
    border-top-left-radius: var(--sl-border-radius-medium);
    border-top-right-radius: var(--sl-border-radius-medium);
    cursor: crosshair;
  }

  .color-picker__grid-handle {
    position: absolute;
    width: var(--grid-handle-size);
    height: var(--grid-handle-size);
    border-radius: 50%;
    box-shadow: 0 0 0 1px rgba(0, 0, 0, 0.25);
    border: solid 2px white;
    margin-top: calc(var(--grid-handle-size) / -2);
    margin-left: calc(var(--grid-handle-size) / -2);
    transition: var(--sl-transition-fast) transform;
  }

  .color-picker__grid-handle--dragging {
    cursor: none;
    transform: scale(1.5);
  }

  .color-picker__grid-handle:focus-visible {
    outline: var(--sl-focus-ring);
  }

  .color-picker__controls {
    padding: var(--sl-spacing-small);
    display: flex;
    align-items: center;
  }

  .color-picker__sliders {
    flex: 1 1 auto;
  }

  .color-picker__slider {
    position: relative;
    height: var(--slider-height);
    border-radius: var(--sl-border-radius-pill);
    box-shadow: inset 0 0 0 1px rgba(0, 0, 0, 0.2);
  }

  .color-picker__slider:not(:last-of-type) {
    margin-bottom: var(--sl-spacing-small);
  }

  .color-picker__slider-handle {
    position: absolute;
    top: calc(50% - var(--slider-handle-size) / 2);
    width: var(--slider-handle-size);
    height: var(--slider-handle-size);
    background-color: white;
    border-radius: 50%;
    box-shadow: 0 0 0 1px rgba(0, 0, 0, 0.25);
    margin-left: calc(var(--slider-handle-size) / -2);
  }

  .color-picker__slider-handle:focus-visible {
    outline: var(--sl-focus-ring);
  }

  .color-picker__hue {
    background-image: linear-gradient(
      to right,
      rgb(255, 0, 0) 0%,
      rgb(255, 255, 0) 17%,
      rgb(0, 255, 0) 33%,
      rgb(0, 255, 255) 50%,
      rgb(0, 0, 255) 67%,
      rgb(255, 0, 255) 83%,
      rgb(255, 0, 0) 100%
    );
  }

  .color-picker__alpha .color-picker__alpha-gradient {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    border-radius: inherit;
  }

  .color-picker__preview {
    flex: 0 0 auto;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    position: relative;
    width: 2.25rem;
    height: 2.25rem;
    border: none;
    border-radius: var(--sl-border-radius-circle);
    background: none;
    margin-left: var(--sl-spacing-small);
    cursor: copy;
  }

  .color-picker__preview:before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    border-radius: inherit;
    box-shadow: inset 0 0 0 1px rgba(0, 0, 0, 0.2);

    /* We use a custom property in lieu of currentColor because of https://bugs.webkit.org/show_bug.cgi?id=216780 */
    background-color: var(--preview-color);
  }

  .color-picker__preview:focus-visible {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .color-picker__preview-color {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    border: solid 1px rgba(0, 0, 0, 0.125);
  }

  .color-picker__preview-color--copied {
    animation: pulse 0.75s;
  }

  @keyframes pulse {
    0% {
      box-shadow: 0 0 0 0 var(--sl-color-primary-500);
    }
    70% {
      box-shadow: 0 0 0 0.5rem transparent;
    }
    100% {
      box-shadow: 0 0 0 0 transparent;
    }
  }

  .color-picker__user-input {
    display: flex;
    padding: 0 var(--sl-spacing-small) var(--sl-spacing-small) var(--sl-spacing-small);
  }

  .color-picker__user-input sl-input {
    min-width: 0; /* fix input width in Safari */
    flex: 1 1 auto;
  }

  .color-picker__user-input sl-button-group {
    margin-left: var(--sl-spacing-small);
  }

  .color-picker__user-input sl-button {
    min-width: 3.25rem;
    max-width: 3.25rem;
    font-size: 1rem;
  }

  .color-picker__swatches {
    display: grid;
    grid-template-columns: repeat(8, 1fr);
    grid-gap: 0.5rem;
    justify-items: center;
    border-top: solid 1px var(--sl-color-neutral-200);
    padding: var(--sl-spacing-small);
  }

  .color-picker__swatch {
    position: relative;
    width: var(--swatch-size);
    height: var(--swatch-size);
    border-radius: var(--sl-border-radius-small);
  }

  .color-picker__swatch .color-picker__swatch-color {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    border: solid 1px rgba(0, 0, 0, 0.125);
    border-radius: inherit;
    cursor: pointer;
  }

  .color-picker__swatch:focus-visible {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .color-picker__transparent-bg {
    background-image: linear-gradient(45deg, var(--sl-color-neutral-300) 25%, transparent 25%),
      linear-gradient(45deg, transparent 75%, var(--sl-color-neutral-300) 75%),
      linear-gradient(45deg, transparent 75%, var(--sl-color-neutral-300) 75%),
      linear-gradient(45deg, var(--sl-color-neutral-300) 25%, transparent 25%);
    background-size: 10px 10px;
    background-position: 0 0, 0 0, -5px -5px, 5px 5px;
  }

  .color-picker--disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .color-picker--disabled .color-picker__grid,
  .color-picker--disabled .color-picker__grid-handle,
  .color-picker--disabled .color-picker__slider,
  .color-picker--disabled .color-picker__slider-handle,
  .color-picker--disabled .color-picker__preview,
  .color-picker--disabled .color-picker__swatch,
  .color-picker--disabled .color-picker__swatch-color {
    pointer-events: none;
  }

  /*
   * Color dropdown
   */

  .color-dropdown::part(panel) {
    max-height: none;
    background-color: var(--sl-panel-background-color);
    border: solid var(--sl-panel-border-width) var(--sl-panel-border-color);
    border-radius: var(--sl-border-radius-medium);
    overflow: visible;
  }

  .color-dropdown__trigger {
    display: inline-block;
    position: relative;
    background-color: transparent;
    border: none;
    cursor: pointer;
  }

  .color-dropdown__trigger.color-dropdown__trigger--small {
    width: var(--sl-input-height-small);
    height: var(--sl-input-height-small);
    border-radius: var(--sl-border-radius-circle);
  }

  .color-dropdown__trigger.color-dropdown__trigger--medium {
    width: var(--sl-input-height-medium);
    height: var(--sl-input-height-medium);
    border-radius: var(--sl-border-radius-circle);
  }

  .color-dropdown__trigger.color-dropdown__trigger--large {
    width: var(--sl-input-height-large);
    height: var(--sl-input-height-large);
    border-radius: var(--sl-border-radius-circle);
  }

  .color-dropdown__trigger:before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    border-radius: inherit;
    background-color: currentColor;
    box-shadow: inset 0 0 0 2px var(--sl-input-border-color), inset 0 0 0 4px var(--sl-color-neutral-0);
  }

  .color-dropdown__trigger--empty:before {
    background-color: transparent;
  }

  .color-dropdown__trigger:focus-visible {
    outline: none;
  }

  .color-dropdown__trigger:focus-visible:not(.color-dropdown__trigger--disabled) {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .color-dropdown__trigger.color-dropdown__trigger--disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }
`,_r=Et({"node_modules/color-name/index.js"(t,e){e.exports={aliceblue:[240,248,255],antiquewhite:[250,235,215],aqua:[0,255,255],aquamarine:[127,255,212],azure:[240,255,255],beige:[245,245,220],bisque:[255,228,196],black:[0,0,0],blanchedalmond:[255,235,205],blue:[0,0,255],blueviolet:[138,43,226],brown:[165,42,42],burlywood:[222,184,135],cadetblue:[95,158,160],chartreuse:[127,255,0],chocolate:[210,105,30],coral:[255,127,80],cornflowerblue:[100,149,237],cornsilk:[255,248,220],crimson:[220,20,60],cyan:[0,255,255],darkblue:[0,0,139],darkcyan:[0,139,139],darkgoldenrod:[184,134,11],darkgray:[169,169,169],darkgreen:[0,100,0],darkgrey:[169,169,169],darkkhaki:[189,183,107],darkmagenta:[139,0,139],darkolivegreen:[85,107,47],darkorange:[255,140,0],darkorchid:[153,50,204],darkred:[139,0,0],darksalmon:[233,150,122],darkseagreen:[143,188,143],darkslateblue:[72,61,139],darkslategray:[47,79,79],darkslategrey:[47,79,79],darkturquoise:[0,206,209],darkviolet:[148,0,211],deeppink:[255,20,147],deepskyblue:[0,191,255],dimgray:[105,105,105],dimgrey:[105,105,105],dodgerblue:[30,144,255],firebrick:[178,34,34],floralwhite:[255,250,240],forestgreen:[34,139,34],fuchsia:[255,0,255],gainsboro:[220,220,220],ghostwhite:[248,248,255],gold:[255,215,0],goldenrod:[218,165,32],gray:[128,128,128],green:[0,128,0],greenyellow:[173,255,47],grey:[128,128,128],honeydew:[240,255,240],hotpink:[255,105,180],indianred:[205,92,92],indigo:[75,0,130],ivory:[255,255,240],khaki:[240,230,140],lavender:[230,230,250],lavenderblush:[255,240,245],lawngreen:[124,252,0],lemonchiffon:[255,250,205],lightblue:[173,216,230],lightcoral:[240,128,128],lightcyan:[224,255,255],lightgoldenrodyellow:[250,250,210],lightgray:[211,211,211],lightgreen:[144,238,144],lightgrey:[211,211,211],lightpink:[255,182,193],lightsalmon:[255,160,122],lightseagreen:[32,178,170],lightskyblue:[135,206,250],lightslategray:[119,136,153],lightslategrey:[119,136,153],lightsteelblue:[176,196,222],lightyellow:[255,255,224],lime:[0,255,0],limegreen:[50,205,50],linen:[250,240,230],magenta:[255,0,255],maroon:[128,0,0],mediumaquamarine:[102,205,170],mediumblue:[0,0,205],mediumorchid:[186,85,211],mediumpurple:[147,112,219],mediumseagreen:[60,179,113],mediumslateblue:[123,104,238],mediumspringgreen:[0,250,154],mediumturquoise:[72,209,204],mediumvioletred:[199,21,133],midnightblue:[25,25,112],mintcream:[245,255,250],mistyrose:[255,228,225],moccasin:[255,228,181],navajowhite:[255,222,173],navy:[0,0,128],oldlace:[253,245,230],olive:[128,128,0],olivedrab:[107,142,35],orange:[255,165,0],orangered:[255,69,0],orchid:[218,112,214],palegoldenrod:[238,232,170],palegreen:[152,251,152],paleturquoise:[175,238,238],palevioletred:[219,112,147],papayawhip:[255,239,213],peachpuff:[255,218,185],peru:[205,133,63],pink:[255,192,203],plum:[221,160,221],powderblue:[176,224,230],purple:[128,0,128],rebeccapurple:[102,51,153],red:[255,0,0],rosybrown:[188,143,143],royalblue:[65,105,225],saddlebrown:[139,69,19],salmon:[250,128,114],sandybrown:[244,164,96],seagreen:[46,139,87],seashell:[255,245,238],sienna:[160,82,45],silver:[192,192,192],skyblue:[135,206,235],slateblue:[106,90,205],slategray:[112,128,144],slategrey:[112,128,144],snow:[255,250,250],springgreen:[0,255,127],steelblue:[70,130,180],tan:[210,180,140],teal:[0,128,128],thistle:[216,191,216],tomato:[255,99,71],turquoise:[64,224,208],violet:[238,130,238],wheat:[245,222,179],white:[255,255,255],whitesmoke:[245,245,245],yellow:[255,255,0],yellowgreen:[154,205,50]}}}),xr=Et({"node_modules/simple-swizzle/node_modules/is-arrayish/index.js"(t,e){e.exports=function(t){return!(!t||"string"==typeof t)&&(t instanceof Array||Array.isArray(t)||t.length>=0&&(t.splice instanceof Function||Object.getOwnPropertyDescriptor(t,t.length-1)&&"String"!==t.constructor.name))}}}),kr=Et({"node_modules/simple-swizzle/index.js"(t,e){var o=xr(),r=Array.prototype.concat,i=Array.prototype.slice,a=e.exports=function(t){for(var e=[],a=0,s=t.length;a<s;a++){var n=t[a];o(n)?e=r.call(e,i.call(n)):e.push(n)}return e};a.wrap=function(t){return function(){return t(a(arguments))}}}}),$r=Et({"node_modules/color-string/index.js"(t,e){var o,r=_r(),i=kr(),a=Object.hasOwnProperty,s={};for(o in r)a.call(r,o)&&(s[r[o]]=o);var n=e.exports={to:{},get:{}};function l(t,e,o){return Math.min(Math.max(e,t),o)}function c(t){var e=Math.round(t).toString(16).toUpperCase();return e.length<2?"0"+e:e}n.get=function(t){var e,o;switch(t.substring(0,3).toLowerCase()){case"hsl":e=n.get.hsl(t),o="hsl";break;case"hwb":e=n.get.hwb(t),o="hwb";break;default:e=n.get.rgb(t),o="rgb"}return e?{model:o,value:e}:null},n.get.rgb=function(t){if(!t)return null;var e,o,i,s=[0,0,0,1];if(e=t.match(/^#([a-f0-9]{6})([a-f0-9]{2})?$/i)){for(i=e[2],e=e[1],o=0;o<3;o++){var n=2*o;s[o]=parseInt(e.slice(n,n+2),16)}i&&(s[3]=parseInt(i,16)/255)}else if(e=t.match(/^#([a-f0-9]{3,4})$/i)){for(i=(e=e[1])[3],o=0;o<3;o++)s[o]=parseInt(e[o]+e[o],16);i&&(s[3]=parseInt(i+i,16)/255)}else if(e=t.match(/^rgba?\(\s*([+-]?\d+)(?=[\s,])\s*(?:,\s*)?([+-]?\d+)(?=[\s,])\s*(?:,\s*)?([+-]?\d+)\s*(?:[,|\/]\s*([+-]?[\d\.]+)(%?)\s*)?\)$/)){for(o=0;o<3;o++)s[o]=parseInt(e[o+1],0);e[4]&&(e[5]?s[3]=.01*parseFloat(e[4]):s[3]=parseFloat(e[4]))}else{if(!(e=t.match(/^rgba?\(\s*([+-]?[\d\.]+)\%\s*,?\s*([+-]?[\d\.]+)\%\s*,?\s*([+-]?[\d\.]+)\%\s*(?:[,|\/]\s*([+-]?[\d\.]+)(%?)\s*)?\)$/)))return(e=t.match(/^(\w+)$/))?"transparent"===e[1]?[0,0,0,0]:a.call(r,e[1])?((s=r[e[1]])[3]=1,s):null:null;for(o=0;o<3;o++)s[o]=Math.round(2.55*parseFloat(e[o+1]));e[4]&&(e[5]?s[3]=.01*parseFloat(e[4]):s[3]=parseFloat(e[4]))}for(o=0;o<3;o++)s[o]=l(s[o],0,255);return s[3]=l(s[3],0,1),s},n.get.hsl=function(t){if(!t)return null;var e=t.match(/^hsla?\(\s*([+-]?(?:\d{0,3}\.)?\d+)(?:deg)?\s*,?\s*([+-]?[\d\.]+)%\s*,?\s*([+-]?[\d\.]+)%\s*(?:[,|\/]\s*([+-]?(?=\.\d|\d)(?:0|[1-9]\d*)?(?:\.\d*)?(?:[eE][+-]?\d+)?)\s*)?\)$/);if(e){var o=parseFloat(e[4]);return[(parseFloat(e[1])%360+360)%360,l(parseFloat(e[2]),0,100),l(parseFloat(e[3]),0,100),l(isNaN(o)?1:o,0,1)]}return null},n.get.hwb=function(t){if(!t)return null;var e=t.match(/^hwb\(\s*([+-]?\d{0,3}(?:\.\d+)?)(?:deg)?\s*,\s*([+-]?[\d\.]+)%\s*,\s*([+-]?[\d\.]+)%\s*(?:,\s*([+-]?(?=\.\d|\d)(?:0|[1-9]\d*)?(?:\.\d*)?(?:[eE][+-]?\d+)?)\s*)?\)$/);if(e){var o=parseFloat(e[4]);return[(parseFloat(e[1])%360+360)%360,l(parseFloat(e[2]),0,100),l(parseFloat(e[3]),0,100),l(isNaN(o)?1:o,0,1)]}return null},n.to.hex=function(){var t=i(arguments);return"#"+c(t[0])+c(t[1])+c(t[2])+(t[3]<1?c(Math.round(255*t[3])):"")},n.to.rgb=function(){var t=i(arguments);return t.length<4||1===t[3]?"rgb("+Math.round(t[0])+", "+Math.round(t[1])+", "+Math.round(t[2])+")":"rgba("+Math.round(t[0])+", "+Math.round(t[1])+", "+Math.round(t[2])+", "+t[3]+")"},n.to.rgb.percent=function(){var t=i(arguments),e=Math.round(t[0]/255*100),o=Math.round(t[1]/255*100),r=Math.round(t[2]/255*100);return t.length<4||1===t[3]?"rgb("+e+"%, "+o+"%, "+r+"%)":"rgba("+e+"%, "+o+"%, "+r+"%, "+t[3]+")"},n.to.hsl=function(){var t=i(arguments);return t.length<4||1===t[3]?"hsl("+t[0]+", "+t[1]+"%, "+t[2]+"%)":"hsla("+t[0]+", "+t[1]+"%, "+t[2]+"%, "+t[3]+")"},n.to.hwb=function(){var t=i(arguments),e="";return t.length>=4&&1!==t[3]&&(e=", "+t[3]),"hwb("+t[0]+", "+t[1]+"%, "+t[2]+"%"+e+")"},n.to.keyword=function(t){return s[t.slice(0,3)]}}}),Cr=Et({"node_modules/color-convert/conversions.js"(t,e){var o=_r(),r={};for(const t of Object.keys(o))r[o[t]]=t;var i={rgb:{channels:3,labels:"rgb"},hsl:{channels:3,labels:"hsl"},hsv:{channels:3,labels:"hsv"},hwb:{channels:3,labels:"hwb"},cmyk:{channels:4,labels:"cmyk"},xyz:{channels:3,labels:"xyz"},lab:{channels:3,labels:"lab"},lch:{channels:3,labels:"lch"},hex:{channels:1,labels:["hex"]},keyword:{channels:1,labels:["keyword"]},ansi16:{channels:1,labels:["ansi16"]},ansi256:{channels:1,labels:["ansi256"]},hcg:{channels:3,labels:["h","c","g"]},apple:{channels:3,labels:["r16","g16","b16"]},gray:{channels:1,labels:["gray"]}};e.exports=i;for(const t of Object.keys(i)){if(!("channels"in i[t]))throw new Error("missing channels property: "+t);if(!("labels"in i[t]))throw new Error("missing channel labels property: "+t);if(i[t].labels.length!==i[t].channels)throw new Error("channel and label counts mismatch: "+t);const{channels:e,labels:o}=i[t];delete i[t].channels,delete i[t].labels,Object.defineProperty(i[t],"channels",{value:e}),Object.defineProperty(i[t],"labels",{value:o})}function a(t,e){return(t[0]-e[0])**2+(t[1]-e[1])**2+(t[2]-e[2])**2}i.rgb.hsl=function(t){const e=t[0]/255,o=t[1]/255,r=t[2]/255,i=Math.min(e,o,r),a=Math.max(e,o,r),s=a-i;let n,l;a===i?n=0:e===a?n=(o-r)/s:o===a?n=2+(r-e)/s:r===a&&(n=4+(e-o)/s),n=Math.min(60*n,360),n<0&&(n+=360);const c=(i+a)/2;return l=a===i?0:c<=.5?s/(a+i):s/(2-a-i),[n,100*l,100*c]},i.rgb.hsv=function(t){let e,o,r,i,a;const s=t[0]/255,n=t[1]/255,l=t[2]/255,c=Math.max(s,n,l),d=c-Math.min(s,n,l),h=function(t){return(c-t)/6/d+.5};return 0===d?(i=0,a=0):(a=d/c,e=h(s),o=h(n),r=h(l),s===c?i=r-o:n===c?i=1/3+e-r:l===c&&(i=2/3+o-e),i<0?i+=1:i>1&&(i-=1)),[360*i,100*a,100*c]},i.rgb.hwb=function(t){const e=t[0],o=t[1];let r=t[2];const a=i.rgb.hsl(t)[0],s=1/255*Math.min(e,Math.min(o,r));return r=1-1/255*Math.max(e,Math.max(o,r)),[a,100*s,100*r]},i.rgb.cmyk=function(t){const e=t[0]/255,o=t[1]/255,r=t[2]/255,i=Math.min(1-e,1-o,1-r);return[100*((1-e-i)/(1-i)||0),100*((1-o-i)/(1-i)||0),100*((1-r-i)/(1-i)||0),100*i]},i.rgb.keyword=function(t){const e=r[t];if(e)return e;let i,s=1/0;for(const e of Object.keys(o)){const r=a(t,o[e]);r<s&&(s=r,i=e)}return i},i.keyword.rgb=function(t){return o[t]},i.rgb.xyz=function(t){let e=t[0]/255,o=t[1]/255,r=t[2]/255;e=e>.04045?((e+.055)/1.055)**2.4:e/12.92,o=o>.04045?((o+.055)/1.055)**2.4:o/12.92,r=r>.04045?((r+.055)/1.055)**2.4:r/12.92;return[100*(.4124*e+.3576*o+.1805*r),100*(.2126*e+.7152*o+.0722*r),100*(.0193*e+.1192*o+.9505*r)]},i.rgb.lab=function(t){const e=i.rgb.xyz(t);let o=e[0],r=e[1],a=e[2];o/=95.047,r/=100,a/=108.883,o=o>.008856?o**(1/3):7.787*o+16/116,r=r>.008856?r**(1/3):7.787*r+16/116,a=a>.008856?a**(1/3):7.787*a+16/116;return[116*r-16,500*(o-r),200*(r-a)]},i.hsl.rgb=function(t){const e=t[0]/360,o=t[1]/100,r=t[2]/100;let i,a,s;if(0===o)return s=255*r,[s,s,s];i=r<.5?r*(1+o):r+o-r*o;const n=2*r-i,l=[0,0,0];for(let t=0;t<3;t++)a=e+1/3*-(t-1),a<0&&a++,a>1&&a--,s=6*a<1?n+6*(i-n)*a:2*a<1?i:3*a<2?n+(i-n)*(2/3-a)*6:n,l[t]=255*s;return l},i.hsl.hsv=function(t){const e=t[0];let o=t[1]/100,r=t[2]/100,i=o;const a=Math.max(r,.01);r*=2,o*=r<=1?r:2-r,i*=a<=1?a:2-a;return[e,100*(0===r?2*i/(a+i):2*o/(r+o)),100*((r+o)/2)]},i.hsv.rgb=function(t){const e=t[0]/60,o=t[1]/100;let r=t[2]/100;const i=Math.floor(e)%6,a=e-Math.floor(e),s=255*r*(1-o),n=255*r*(1-o*a),l=255*r*(1-o*(1-a));switch(r*=255,i){case 0:return[r,l,s];case 1:return[n,r,s];case 2:return[s,r,l];case 3:return[s,n,r];case 4:return[l,s,r];case 5:return[r,s,n]}},i.hsv.hsl=function(t){const e=t[0],o=t[1]/100,r=t[2]/100,i=Math.max(r,.01);let a,s;s=(2-o)*r;const n=(2-o)*i;return a=o*i,a/=n<=1?n:2-n,a=a||0,s/=2,[e,100*a,100*s]},i.hwb.rgb=function(t){const e=t[0]/360;let o=t[1]/100,r=t[2]/100;const i=o+r;let a;i>1&&(o/=i,r/=i);const s=Math.floor(6*e),n=1-r;a=6*e-s,0!=(1&s)&&(a=1-a);const l=o+a*(n-o);let c,d,h;switch(s){default:case 6:case 0:c=n,d=l,h=o;break;case 1:c=l,d=n,h=o;break;case 2:c=o,d=n,h=l;break;case 3:c=o,d=l,h=n;break;case 4:c=l,d=o,h=n;break;case 5:c=n,d=o,h=l}return[255*c,255*d,255*h]},i.cmyk.rgb=function(t){const e=t[0]/100,o=t[1]/100,r=t[2]/100,i=t[3]/100;return[255*(1-Math.min(1,e*(1-i)+i)),255*(1-Math.min(1,o*(1-i)+i)),255*(1-Math.min(1,r*(1-i)+i))]},i.xyz.rgb=function(t){const e=t[0]/100,o=t[1]/100,r=t[2]/100;let i,a,s;return i=3.2406*e+-1.5372*o+-.4986*r,a=-.9689*e+1.8758*o+.0415*r,s=.0557*e+-.204*o+1.057*r,i=i>.0031308?1.055*i**(1/2.4)-.055:12.92*i,a=a>.0031308?1.055*a**(1/2.4)-.055:12.92*a,s=s>.0031308?1.055*s**(1/2.4)-.055:12.92*s,i=Math.min(Math.max(0,i),1),a=Math.min(Math.max(0,a),1),s=Math.min(Math.max(0,s),1),[255*i,255*a,255*s]},i.xyz.lab=function(t){let e=t[0],o=t[1],r=t[2];e/=95.047,o/=100,r/=108.883,e=e>.008856?e**(1/3):7.787*e+16/116,o=o>.008856?o**(1/3):7.787*o+16/116,r=r>.008856?r**(1/3):7.787*r+16/116;return[116*o-16,500*(e-o),200*(o-r)]},i.lab.xyz=function(t){let e,o,r;o=(t[0]+16)/116,e=t[1]/500+o,r=o-t[2]/200;const i=o**3,a=e**3,s=r**3;return o=i>.008856?i:(o-16/116)/7.787,e=a>.008856?a:(e-16/116)/7.787,r=s>.008856?s:(r-16/116)/7.787,e*=95.047,o*=100,r*=108.883,[e,o,r]},i.lab.lch=function(t){const e=t[0],o=t[1],r=t[2];let i;i=360*Math.atan2(r,o)/2/Math.PI,i<0&&(i+=360);return[e,Math.sqrt(o*o+r*r),i]},i.lch.lab=function(t){const e=t[0],o=t[1],r=t[2]/360*2*Math.PI;return[e,o*Math.cos(r),o*Math.sin(r)]},i.rgb.ansi16=function(t,e=null){const[o,r,a]=t;let s=null===e?i.rgb.hsv(t)[2]:e;if(s=Math.round(s/50),0===s)return 30;let n=30+(Math.round(a/255)<<2|Math.round(r/255)<<1|Math.round(o/255));return 2===s&&(n+=60),n},i.hsv.ansi16=function(t){return i.rgb.ansi16(i.hsv.rgb(t),t[2])},i.rgb.ansi256=function(t){const e=t[0],o=t[1],r=t[2];if(e===o&&o===r)return e<8?16:e>248?231:Math.round((e-8)/247*24)+232;return 16+36*Math.round(e/255*5)+6*Math.round(o/255*5)+Math.round(r/255*5)},i.ansi16.rgb=function(t){let e=t%10;if(0===e||7===e)return t>50&&(e+=3.5),e=e/10.5*255,[e,e,e];const o=.5*(1+~~(t>50));return[(1&e)*o*255,(e>>1&1)*o*255,(e>>2&1)*o*255]},i.ansi256.rgb=function(t){if(t>=232){const e=10*(t-232)+8;return[e,e,e]}let e;t-=16;return[Math.floor(t/36)/5*255,Math.floor((e=t%36)/6)/5*255,e%6/5*255]},i.rgb.hex=function(t){const e=(((255&Math.round(t[0]))<<16)+((255&Math.round(t[1]))<<8)+(255&Math.round(t[2]))).toString(16).toUpperCase();return"000000".substring(e.length)+e},i.hex.rgb=function(t){const e=t.toString(16).match(/[a-f0-9]{6}|[a-f0-9]{3}/i);if(!e)return[0,0,0];let o=e[0];3===e[0].length&&(o=o.split("").map((t=>t+t)).join(""));const r=parseInt(o,16);return[r>>16&255,r>>8&255,255&r]},i.rgb.hcg=function(t){const e=t[0]/255,o=t[1]/255,r=t[2]/255,i=Math.max(Math.max(e,o),r),a=Math.min(Math.min(e,o),r),s=i-a;let n,l;return n=s<1?a/(1-s):0,l=s<=0?0:i===e?(o-r)/s%6:i===o?2+(r-e)/s:4+(e-o)/s,l/=6,l%=1,[360*l,100*s,100*n]},i.hsl.hcg=function(t){const e=t[1]/100,o=t[2]/100,r=o<.5?2*e*o:2*e*(1-o);let i=0;return r<1&&(i=(o-.5*r)/(1-r)),[t[0],100*r,100*i]},i.hsv.hcg=function(t){const e=t[1]/100,o=t[2]/100,r=e*o;let i=0;return r<1&&(i=(o-r)/(1-r)),[t[0],100*r,100*i]},i.hcg.rgb=function(t){const e=t[0]/360,o=t[1]/100,r=t[2]/100;if(0===o)return[255*r,255*r,255*r];const i=[0,0,0],a=e%1*6,s=a%1,n=1-s;let l=0;switch(Math.floor(a)){case 0:i[0]=1,i[1]=s,i[2]=0;break;case 1:i[0]=n,i[1]=1,i[2]=0;break;case 2:i[0]=0,i[1]=1,i[2]=s;break;case 3:i[0]=0,i[1]=n,i[2]=1;break;case 4:i[0]=s,i[1]=0,i[2]=1;break;default:i[0]=1,i[1]=0,i[2]=n}return l=(1-o)*r,[255*(o*i[0]+l),255*(o*i[1]+l),255*(o*i[2]+l)]},i.hcg.hsv=function(t){const e=t[1]/100,o=e+t[2]/100*(1-e);let r=0;return o>0&&(r=e/o),[t[0],100*r,100*o]},i.hcg.hsl=function(t){const e=t[1]/100,o=t[2]/100*(1-e)+.5*e;let r=0;return o>0&&o<.5?r=e/(2*o):o>=.5&&o<1&&(r=e/(2*(1-o))),[t[0],100*r,100*o]},i.hcg.hwb=function(t){const e=t[1]/100,o=e+t[2]/100*(1-e);return[t[0],100*(o-e),100*(1-o)]},i.hwb.hcg=function(t){const e=t[1]/100,o=1-t[2]/100,r=o-e;let i=0;return r<1&&(i=(o-r)/(1-r)),[t[0],100*r,100*i]},i.apple.rgb=function(t){return[t[0]/65535*255,t[1]/65535*255,t[2]/65535*255]},i.rgb.apple=function(t){return[t[0]/255*65535,t[1]/255*65535,t[2]/255*65535]},i.gray.rgb=function(t){return[t[0]/100*255,t[0]/100*255,t[0]/100*255]},i.gray.hsl=function(t){return[0,0,t[0]]},i.gray.hsv=i.gray.hsl,i.gray.hwb=function(t){return[0,100,t[0]]},i.gray.cmyk=function(t){return[0,0,0,t[0]]},i.gray.lab=function(t){return[t[0],0,0]},i.gray.hex=function(t){const e=255&Math.round(t[0]/100*255),o=((e<<16)+(e<<8)+e).toString(16).toUpperCase();return"000000".substring(o.length)+o},i.rgb.gray=function(t){return[(t[0]+t[1]+t[2])/3/255*100]}}}),zr=Et({"node_modules/color-convert/route.js"(t,e){var o=Cr();function r(t){const e=function(){const t={},e=Object.keys(o);for(let o=e.length,r=0;r<o;r++)t[e[r]]={distance:-1,parent:null};return t}(),r=[t];for(e[t].distance=0;r.length;){const t=r.pop(),i=Object.keys(o[t]);for(let o=i.length,a=0;a<o;a++){const o=i[a],s=e[o];-1===s.distance&&(s.distance=e[t].distance+1,s.parent=t,r.unshift(o))}}return e}function i(t,e){return function(o){return e(t(o))}}function a(t,e){const r=[e[t].parent,t];let a=o[e[t].parent][t],s=e[t].parent;for(;e[s].parent;)r.unshift(e[s].parent),a=i(o[e[s].parent][s],a),s=e[s].parent;return a.conversion=r,a}e.exports=function(t){const e=r(t),o={},i=Object.keys(e);for(let t=i.length,r=0;r<t;r++){const t=i[r];null!==e[t].parent&&(o[t]=a(t,e))}return o}}}),Sr=Et({"node_modules/color-convert/index.js"(t,e){var o=Cr(),r=zr(),i={};Object.keys(o).forEach((t=>{i[t]={},Object.defineProperty(i[t],"channels",{value:o[t].channels}),Object.defineProperty(i[t],"labels",{value:o[t].labels});const e=r(t);Object.keys(e).forEach((o=>{const r=e[o];i[t][o]=function(t){const e=function(...e){const o=e[0];if(null==o)return o;o.length>1&&(e=o);const r=t(e);if("object"==typeof r)for(let t=r.length,e=0;e<t;e++)r[e]=Math.round(r[e]);return r};return"conversion"in t&&(e.conversion=t.conversion),e}(r),i[t][o].raw=function(t){const e=function(...e){const o=e[0];return null==o?o:(o.length>1&&(e=o),t(e))};return"conversion"in t&&(e.conversion=t.conversion),e}(r)}))})),e.exports=i}}),Ar=Et({"node_modules/color/index.js"(t,e){var o=$r(),r=Sr(),i=[].slice,a=["keyword","gray","hex"],s={};for(const t of Object.keys(r))s[i.call(r[t].labels).sort().join("")]=t;var n={};function l(t,e){if(!(this instanceof l))return new l(t,e);if(e&&e in a&&(e=null),e&&!(e in r))throw new Error("Unknown model: "+e);let c,d;if(null==t)this.model="rgb",this.color=[0,0,0],this.valpha=1;else if(t instanceof l)this.model=t.model,this.color=t.color.slice(),this.valpha=t.valpha;else if("string"==typeof t){const e=o.get(t);if(null===e)throw new Error("Unable to parse color from string: "+t);this.model=e.model,d=r[this.model].channels,this.color=e.value.slice(0,d),this.valpha="number"==typeof e.value[d]?e.value[d]:1}else if(t.length>0){this.model=e||"rgb",d=r[this.model].channels;const o=i.call(t,0,d);this.color=u(o,d),this.valpha="number"==typeof t[d]?t[d]:1}else if("number"==typeof t)this.model="rgb",this.color=[t>>16&255,t>>8&255,255&t],this.valpha=1;else{this.valpha=1;const e=Object.keys(t);"alpha"in t&&(e.splice(e.indexOf("alpha"),1),this.valpha="number"==typeof t.alpha?t.alpha:0);const o=e.sort().join("");if(!(o in s))throw new Error("Unable to parse color from object: "+JSON.stringify(t));this.model=s[o];const i=r[this.model].labels,a=[];for(c=0;c<i.length;c++)a.push(t[i[c]]);this.color=u(a)}if(n[this.model])for(d=r[this.model].channels,c=0;c<d;c++){const t=n[this.model][c];t&&(this.color[c]=t(this.color[c]))}this.valpha=Math.max(0,Math.min(1,this.valpha)),Object.freeze&&Object.freeze(this)}l.prototype={toString(){return this.string()},toJSON(){return this[this.model]()},string(t){let e=this.model in o.to?this:this.rgb();e=e.round("number"==typeof t?t:1);const r=1===e.valpha?e.color:e.color.concat(this.valpha);return o.to[e.model](r)},percentString(t){const e=this.rgb().round("number"==typeof t?t:1),r=1===e.valpha?e.color:e.color.concat(this.valpha);return o.to.rgb.percent(r)},array(){return 1===this.valpha?this.color.slice():this.color.concat(this.valpha)},object(){const t={},e=r[this.model].channels,o=r[this.model].labels;for(let r=0;r<e;r++)t[o[r]]=this.color[r];return 1!==this.valpha&&(t.alpha=this.valpha),t},unitArray(){const t=this.rgb().color;return t[0]/=255,t[1]/=255,t[2]/=255,1!==this.valpha&&t.push(this.valpha),t},unitObject(){const t=this.rgb().object();return t.r/=255,t.g/=255,t.b/=255,1!==this.valpha&&(t.alpha=this.valpha),t},round(t){return t=Math.max(t||0,0),new l(this.color.map(function(t){return function(e){return function(t,e){return Number(t.toFixed(e))}(e,t)}}(t)).concat(this.valpha),this.model)},alpha(t){return arguments.length>0?new l(this.color.concat(Math.max(0,Math.min(1,t))),this.model):this.valpha},red:c("rgb",0,d(255)),green:c("rgb",1,d(255)),blue:c("rgb",2,d(255)),hue:c(["hsl","hsv","hsl","hwb","hcg"],0,(t=>(t%360+360)%360)),saturationl:c("hsl",1,d(100)),lightness:c("hsl",2,d(100)),saturationv:c("hsv",1,d(100)),value:c("hsv",2,d(100)),chroma:c("hcg",1,d(100)),gray:c("hcg",2,d(100)),white:c("hwb",1,d(100)),wblack:c("hwb",2,d(100)),cyan:c("cmyk",0,d(100)),magenta:c("cmyk",1,d(100)),yellow:c("cmyk",2,d(100)),black:c("cmyk",3,d(100)),x:c("xyz",0,d(100)),y:c("xyz",1,d(100)),z:c("xyz",2,d(100)),l:c("lab",0,d(100)),a:c("lab",1),b:c("lab",2),keyword(t){return arguments.length>0?new l(t):r[this.model].keyword(this.color)},hex(t){return arguments.length>0?new l(t):o.to.hex(this.rgb().round().color)},hexa(t){if(arguments.length>0)return new l(t);const e=this.rgb().round().color;let r=Math.round(255*this.valpha).toString(16).toUpperCase();return 1===r.length&&(r="0"+r),o.to.hex(e)+r},rgbNumber(){const t=this.rgb().color;return(255&t[0])<<16|(255&t[1])<<8|255&t[2]},luminosity(){const t=this.rgb().color,e=[];for(const[o,r]of t.entries()){const t=r/255;e[o]=t<=.03928?t/12.92:((t+.055)/1.055)**2.4}return.2126*e[0]+.7152*e[1]+.0722*e[2]},contrast(t){const e=this.luminosity(),o=t.luminosity();return e>o?(e+.05)/(o+.05):(o+.05)/(e+.05)},level(t){const e=this.contrast(t);return e>=7.1?"AAA":e>=4.5?"AA":""},isDark(){const t=this.rgb().color;return(299*t[0]+587*t[1]+114*t[2])/1e3<128},isLight(){return!this.isDark()},negate(){const t=this.rgb();for(let e=0;e<3;e++)t.color[e]=255-t.color[e];return t},lighten(t){const e=this.hsl();return e.color[2]+=e.color[2]*t,e},darken(t){const e=this.hsl();return e.color[2]-=e.color[2]*t,e},saturate(t){const e=this.hsl();return e.color[1]+=e.color[1]*t,e},desaturate(t){const e=this.hsl();return e.color[1]-=e.color[1]*t,e},whiten(t){const e=this.hwb();return e.color[1]+=e.color[1]*t,e},blacken(t){const e=this.hwb();return e.color[2]+=e.color[2]*t,e},grayscale(){const t=this.rgb().color,e=.3*t[0]+.59*t[1]+.11*t[2];return l.rgb(e,e,e)},fade(t){return this.alpha(this.valpha-this.valpha*t)},opaquer(t){return this.alpha(this.valpha+this.valpha*t)},rotate(t){const e=this.hsl();let o=e.color[0];return o=(o+t)%360,o=o<0?360+o:o,e.color[0]=o,e},mix(t,e){if(!t||!t.rgb)throw new Error('Argument to "mix" was not a Color instance, but rather an instance of '+typeof t);const o=t.rgb(),r=this.rgb(),i=void 0===e?.5:e,a=2*i-1,s=o.alpha()-r.alpha(),n=((a*s==-1?a:(a+s)/(1+a*s))+1)/2,c=1-n;return l.rgb(n*o.red()+c*r.red(),n*o.green()+c*r.green(),n*o.blue()+c*r.blue(),o.alpha()*i+r.alpha()*(1-i))}};for(const t of Object.keys(r)){if(a.includes(t))continue;const e=r[t].channels;l.prototype[t]=function(){if(this.model===t)return new l(this);if(arguments.length>0)return new l(arguments,t);const o="number"==typeof arguments[e]?e:this.valpha;return new l(h(r[this.model][t].raw(this.color)).concat(o),t)},l[t]=function(o){return"number"==typeof o&&(o=u(i.call(arguments),e)),new l(o,t)}}function c(t,e,o){t=Array.isArray(t)?t:[t];for(const r of t)(n[r]||(n[r]=[]))[e]=o;return t=t[0],function(r){let i;return arguments.length>0?(o&&(r=o(r)),i=this[t](),i.color[e]=r,i):(i=this[t]().color[e],o&&(i=o(i)),i)}}function d(t){return function(e){return Math.max(0,Math.min(t,e))}}function h(t){return Array.isArray(t)?t:[t]}function u(t,e){for(let o=0;o<e;o++)"number"!=typeof t[o]&&(t[o]=0);return t}e.exports=l}}),Tr=(gr=Ar(),vr=1,yr=null!=gr?bt(kt(gr)):{},((t,e,o,r)=>{if(e&&"object"==typeof e||"function"==typeof e)for(let i of _t(e))$t.call(t,i)||i===o||gt(t,i,{get:()=>e[i],enumerable:!(r=yt(e,i))||r.enumerable});return t})(!vr&&gr&&gr.__esModule?yr:gt(yr,"default",{value:gr,enumerable:!0}),gr)),Er="EyeDropper"in window,Dr=class extends et{constructor(){super(...arguments),this.formSubmitController=new It(this),this.isSafeValue=!1,this.localize=new ge(this),this.isDraggingGridHandle=!1,this.isEmpty=!1,this.inputValue="",this.hue=0,this.saturation=100,this.lightness=100,this.brightness=100,this.alpha=100,this.value="",this.defaultValue="",this.label="",this.format="hex",this.inline=!1,this.size="medium",this.noFormatToggle=!1,this.name="",this.disabled=!1,this.invalid=!1,this.hoist=!1,this.opacity=!1,this.uppercase=!1,this.swatches=["#d0021b","#f5a623","#f8e71c","#8b572a","#7ed321","#417505","#bd10e0","#9013fe","#4a90e2","#50e3c2","#b8e986","#000","#444","#888","#ccc","#fff"]}connectedCallback(){super.connectedCallback(),this.value?(this.setColor(this.value),this.inputValue=this.value,this.lastValueEmitted=this.value,this.syncValues()):(this.isEmpty=!0,this.inputValue="",this.lastValueEmitted="")}getFormattedValue(t="hex"){const e=this.parseColor(`hsla(${this.hue}, ${this.saturation}%, ${this.lightness}%, ${this.alpha/100})`);if(null===e)return"";switch(t){case"hex":return e.hex;case"hexa":return e.hexa;case"rgb":return e.rgb.string;case"rgba":return e.rgba.string;case"hsl":return e.hsl.string;case"hsla":return e.hsla.string;default:return""}}getBrightness(t){return Ce(200*t/(this.saturation-200)*-1,0,100)}getLightness(t){return Ce((200-this.saturation)*t/100*5/10,0,100)}reportValidity(){return!this.inline&&this.input.invalid?new Promise((t=>{this.dropdown.addEventListener("sl-after-show",(()=>{this.input.reportValidity(),t()}),{once:!0}),this.dropdown.show()})):this.input.reportValidity()}setCustomValidity(t){this.input.setCustomValidity(t),this.invalid=this.input.invalid}handleCopy(){this.input.select(),document.execCommand("copy"),this.previewButton.focus(),this.previewButton.classList.add("color-picker__preview-color--copied"),this.previewButton.addEventListener("animationend",(()=>{this.previewButton.classList.remove("color-picker__preview-color--copied")}))}handleFormatToggle(){const t=["hex","rgb","hsl"],e=(t.indexOf(this.format)+1)%t.length;this.format=t[e]}handleAlphaDrag(t){const e=this.shadowRoot.querySelector(".color-picker__slider.color-picker__alpha"),o=e.querySelector(".color-picker__slider-handle"),{width:r}=e.getBoundingClientRect();o.focus(),t.preventDefault(),Ne(e,{onMove:t=>{this.alpha=Ce(t/r*100,0,100),this.syncValues()},initialEvent:t})}handleHueDrag(t){const e=this.shadowRoot.querySelector(".color-picker__slider.color-picker__hue"),o=e.querySelector(".color-picker__slider-handle"),{width:r}=e.getBoundingClientRect();o.focus(),t.preventDefault(),Ne(e,{onMove:t=>{this.hue=Ce(t/r*360,0,360),this.syncValues()},initialEvent:t})}handleGridDrag(t){const e=this.shadowRoot.querySelector(".color-picker__grid"),o=e.querySelector(".color-picker__grid-handle"),{width:r,height:i}=e.getBoundingClientRect();o.focus(),t.preventDefault(),this.isDraggingGridHandle=!0,Ne(e,{onMove:(t,e)=>{this.saturation=Ce(t/r*100,0,100),this.brightness=Ce(100-e/i*100,0,100),this.lightness=this.getLightness(this.brightness),this.syncValues()},onStop:()=>this.isDraggingGridHandle=!1,initialEvent:t})}handleAlphaKeyDown(t){const e=t.shiftKey?10:1;"ArrowLeft"===t.key&&(t.preventDefault(),this.alpha=Ce(this.alpha-e,0,100),this.syncValues()),"ArrowRight"===t.key&&(t.preventDefault(),this.alpha=Ce(this.alpha+e,0,100),this.syncValues()),"Home"===t.key&&(t.preventDefault(),this.alpha=0,this.syncValues()),"End"===t.key&&(t.preventDefault(),this.alpha=100,this.syncValues())}handleHueKeyDown(t){const e=t.shiftKey?10:1;"ArrowLeft"===t.key&&(t.preventDefault(),this.hue=Ce(this.hue-e,0,360),this.syncValues()),"ArrowRight"===t.key&&(t.preventDefault(),this.hue=Ce(this.hue+e,0,360),this.syncValues()),"Home"===t.key&&(t.preventDefault(),this.hue=0,this.syncValues()),"End"===t.key&&(t.preventDefault(),this.hue=360,this.syncValues())}handleGridKeyDown(t){const e=t.shiftKey?10:1;"ArrowLeft"===t.key&&(t.preventDefault(),this.saturation=Ce(this.saturation-e,0,100),this.lightness=this.getLightness(this.brightness),this.syncValues()),"ArrowRight"===t.key&&(t.preventDefault(),this.saturation=Ce(this.saturation+e,0,100),this.lightness=this.getLightness(this.brightness),this.syncValues()),"ArrowUp"===t.key&&(t.preventDefault(),this.brightness=Ce(this.brightness+e,0,100),this.lightness=this.getLightness(this.brightness),this.syncValues()),"ArrowDown"===t.key&&(t.preventDefault(),this.brightness=Ce(this.brightness-e,0,100),this.lightness=this.getLightness(this.brightness),this.syncValues())}handleInputChange(t){const e=t.target;this.input.value?(this.setColor(e.value),e.value=this.value):this.value="",t.stopPropagation()}handleInputKeyDown(t){"Enter"===t.key&&(this.input.value?(this.setColor(this.input.value),this.input.value=this.value,setTimeout((()=>this.input.select()))):this.hue=0)}normalizeColorString(t){if(/rgba?/i.test(t)){const e=t.replace(/[^\d.%]/g," ").split(" ").map((t=>t.trim())).filter((t=>t.length));return e.length<4&&(e[3]="1"),e[3].indexOf("%")>-1&&(e[3]=(parseFloat(e[3].replace(/%/g,""))/100).toString()),`rgba(${e[0]}, ${e[1]}, ${e[2]}, ${e[3]})`}if(/hsla?/i.test(t)){const e=t.replace(/[^\d.%]/g," ").split(" ").map((t=>t.trim())).filter((t=>t.length));return e.length<4&&(e[3]="1"),e[3].indexOf("%")>-1&&(e[3]=(parseFloat(e[3].replace(/%/g,""))/100).toString()),`hsla(${e[0]}, ${e[1]}, ${e[2]}, ${e[3]})`}return/^[0-9a-f]+$/i.test(t)?`#${t}`:t}parseColor(t){let e;t=this.normalizeColorString(t);try{e=(0,Tr.default)(t)}catch(t){return null}const o=e.hsl(),r={h:o.hue(),s:o.saturationl(),l:o.lightness(),a:o.alpha()},i=e.rgb(),a={r:i.red(),g:i.green(),b:i.blue(),a:i.alpha()},s=Lr(a.r),n=Lr(a.g),l=Lr(a.b),c=Lr(255*a.a);return{hsl:{h:r.h,s:r.s,l:r.l,string:this.setLetterCase(`hsl(${Math.round(r.h)}, ${Math.round(r.s)}%, ${Math.round(r.l)}%)`)},hsla:{h:r.h,s:r.s,l:r.l,a:r.a,string:this.setLetterCase(`hsla(${Math.round(r.h)}, ${Math.round(r.s)}%, ${Math.round(r.l)}%, ${r.a.toFixed(2).toString()})`)},rgb:{r:a.r,g:a.g,b:a.b,string:this.setLetterCase(`rgb(${Math.round(a.r)}, ${Math.round(a.g)}, ${Math.round(a.b)})`)},rgba:{r:a.r,g:a.g,b:a.b,a:a.a,string:this.setLetterCase(`rgba(${Math.round(a.r)}, ${Math.round(a.g)}, ${Math.round(a.b)}, ${a.a.toFixed(2).toString()})`)},hex:this.setLetterCase(`#${s}${n}${l}`),hexa:this.setLetterCase(`#${s}${n}${l}${c}`)}}setColor(t){const e=this.parseColor(t);return null!==e&&(this.hue=e.hsla.h,this.saturation=e.hsla.s,this.lightness=e.hsla.l,this.brightness=this.getBrightness(e.hsla.l),this.alpha=this.opacity?100*e.hsla.a:100,this.syncValues(),!0)}setLetterCase(t){return"string"!=typeof t?"":this.uppercase?t.toUpperCase():t.toLowerCase()}async syncValues(){const t=this.parseColor(`hsla(${this.hue}, ${this.saturation}%, ${this.lightness}%, ${this.alpha/100})`);null!==t&&("hsl"===this.format?this.inputValue=this.opacity?t.hsla.string:t.hsl.string:"rgb"===this.format?this.inputValue=this.opacity?t.rgba.string:t.rgb.string:this.inputValue=this.opacity?t.hexa:t.hex,this.isSafeValue=!0,this.value=this.inputValue,await this.updateComplete,this.isSafeValue=!1)}handleAfterHide(){this.previewButton.classList.remove("color-picker__preview-color--copied")}handleEyeDropper(){if(!Er)return;(new EyeDropper).open().then((t=>this.setColor(t.sRGBHex))).catch((()=>{}))}handleFormatChange(){this.syncValues()}handleOpacityChange(){this.alpha=100}handleValueChange(t,e){if(this.isEmpty=!e,e||(this.hue=0,this.saturation=100,this.brightness=100,this.lightness=this.getLightness(this.brightness),this.alpha=100),!this.isSafeValue&&void 0!==t){const o=this.parseColor(e);null!==o?(this.inputValue=this.value,this.hue=o.hsla.h,this.saturation=o.hsla.s,this.lightness=o.hsla.l,this.brightness=this.getBrightness(o.hsla.l),this.alpha=100*o.hsla.a):this.inputValue=t}this.value!==this.lastValueEmitted&&(Nt(this,"sl-change"),this.lastValueEmitted=this.value)}render(){const t=this.saturation,e=100-this.brightness,o=O`
      <div
        part="base"
        class=${Vt({"color-picker":!0,"color-picker--inline":this.inline,"color-picker--disabled":this.disabled})}
        aria-disabled=${this.disabled?"true":"false"}
        aria-labelledby="label"
        tabindex=${this.inline?"0":"-1"}
      >
        ${this.inline?O`
              <sl-visually-hidden id="label">
                <slot name="label">${this.label}</slot>
              </sl-visually-hidden>
            `:null}

        <div
          part="grid"
          class="color-picker__grid"
          style=${vo({backgroundColor:`hsl(${this.hue}deg, 100%, 50%)`})}
          @mousedown=${this.handleGridDrag}
          @touchstart=${this.handleGridDrag}
        >
          <span
            part="grid-handle"
            class=${Vt({"color-picker__grid-handle":!0,"color-picker__grid-handle--dragging":this.isDraggingGridHandle})}
            style=${vo({top:`${e}%`,left:`${t}%`,backgroundColor:`hsla(${this.hue}deg, ${this.saturation}%, ${this.lightness}%)`})}
            role="application"
            aria-label="HSL"
            tabindex=${Rt(this.disabled?void 0:"0")}
            @keydown=${this.handleGridKeyDown}
          ></span>
        </div>

        <div class="color-picker__controls">
          <div class="color-picker__sliders">
            <div
              part="slider hue-slider"
              class="color-picker__hue color-picker__slider"
              @mousedown=${this.handleHueDrag}
              @touchstart=${this.handleHueDrag}
            >
              <span
                part="slider-handle"
                class="color-picker__slider-handle"
                style=${vo({left:(0===this.hue?0:100/(360/this.hue))+"%"})}
                role="slider"
                aria-label="hue"
                aria-orientation="horizontal"
                aria-valuemin="0"
                aria-valuemax="360"
                aria-valuenow=${`${Math.round(this.hue)}`}
                tabindex=${Rt(this.disabled?void 0:"0")}
                @keydown=${this.handleHueKeyDown}
              ></span>
            </div>

            ${this.opacity?O`
                  <div
                    part="slider opacity-slider"
                    class="color-picker__alpha color-picker__slider color-picker__transparent-bg"
                    @mousedown="${this.handleAlphaDrag}"
                    @touchstart="${this.handleAlphaDrag}"
                  >
                    <div
                      class="color-picker__alpha-gradient"
                      style=${vo({backgroundImage:`linear-gradient(\n                          to right,\n                          hsl(${this.hue}deg, ${this.saturation}%, ${this.lightness}%, 0%) 0%,\n                          hsl(${this.hue}deg, ${this.saturation}%, ${this.lightness}%) 100%\n                        )`})}
                    ></div>
                    <span
                      part="slider-handle"
                      class="color-picker__slider-handle"
                      style=${vo({left:`${this.alpha}%`})}
                      role="slider"
                      aria-label="alpha"
                      aria-orientation="horizontal"
                      aria-valuemin="0"
                      aria-valuemax="100"
                      aria-valuenow=${Math.round(this.alpha)}
                      tabindex=${Rt(this.disabled?void 0:"0")}
                      @keydown=${this.handleAlphaKeyDown}
                    ></span>
                  </div>
                `:""}
          </div>

          <button
            type="button"
            part="preview"
            class="color-picker__preview color-picker__transparent-bg"
            aria-label=${this.localize.term("copy")}
            style=${vo({"--preview-color":`hsla(${this.hue}deg, ${this.saturation}%, ${this.lightness}%, ${this.alpha/100})`})}
            @click=${this.handleCopy}
          ></button>
        </div>

        <div class="color-picker__user-input" aria-live="polite">
          <sl-input
            part="input"
            type="text"
            name=${this.name}
            autocomplete="off"
            autocorrect="off"
            autocapitalize="off"
            spellcheck="false"
            .value=${ft(this.isEmpty?"":this.inputValue)}
            ?disabled=${this.disabled}
            aria-label=${this.localize.term("currentValue")}
            @keydown=${this.handleInputKeyDown}
            @sl-change=${this.handleInputChange}
          ></sl-input>

          <sl-button-group>
            ${this.noFormatToggle?"":O`
                  <sl-button
                    part="format-button"
                    aria-label=${this.localize.term("toggleColorFormat")}
                    exportparts="
                      base:format-button__base,
                      prefix:format-button__prefix,
                      label:format-button__label,
                      suffix:format-button__suffix,
                      caret:format-button__caret
                    "
                    @click=${this.handleFormatToggle}
                  >
                    ${this.setLetterCase(this.format)}
                  </sl-button>
                `}
            ${Er?O`
                  <sl-button
                    part="eye-dropper-button"
                    exportparts="
                      base:eye-dropper-button__base,
                      prefix:eye-dropper-button__prefix,
                      label:eye-dropper-button__label,
                      suffix:eye-dropper-button__suffix,
                      caret:eye-dropper-button__caret
                    "
                    @click=${this.handleEyeDropper}
                  >
                    <sl-icon
                      library="system"
                      name="eyedropper"
                      label=${this.localize.term("selectAColorFromTheScreen")}
                    ></sl-icon>
                  </sl-button>
                `:""}
          </sl-button-group>
        </div>

        ${this.swatches.length>0?O`
              <div part="swatches" class="color-picker__swatches">
                ${this.swatches.map((t=>O`
                    <div
                      part="swatch"
                      class="color-picker__swatch color-picker__transparent-bg"
                      tabindex=${Rt(this.disabled?void 0:"0")}
                      role="button"
                      aria-label=${t}
                      @click=${()=>!this.disabled&&this.setColor(t)}
                      @keydown=${e=>!this.disabled&&"Enter"===e.key&&this.setColor(t)}
                    >
                      <div class="color-picker__swatch-color" style=${vo({backgroundColor:t})}></div>
                    </div>
                  `))}
              </div>
            `:""}
      </div>
    `;return this.inline?o:O`
      <sl-dropdown
        class="color-dropdown"
        aria-disabled=${this.disabled?"true":"false"}
        .containing-element=${this}
        ?disabled=${this.disabled}
        ?hoist=${this.hoist}
        @sl-after-hide=${this.handleAfterHide}
      >
        <button
          part="trigger"
          slot="trigger"
          class=${Vt({"color-dropdown__trigger":!0,"color-dropdown__trigger--disabled":this.disabled,"color-dropdown__trigger--small":"small"===this.size,"color-dropdown__trigger--medium":"medium"===this.size,"color-dropdown__trigger--large":"large"===this.size,"color-dropdown__trigger--empty":this.isEmpty,"color-picker__transparent-bg":!0})}
          style=${vo({color:`hsla(${this.hue}deg, ${this.saturation}%, ${this.lightness}%, ${this.alpha/100})`})}
          type="button"
        >
          <sl-visually-hidden>
            <slot name="label">${this.label}</slot>
          </sl-visually-hidden>
        </button>
        ${o}
      </sl-dropdown>
    `}};function Lr(t){const e=Math.round(t).toString(16);return 1===e.length?`0${e}`:e}Dr.styles=wr,Dt([Gt('[part="input"]')],Dr.prototype,"input",2),Dt([Gt('[part="preview"]')],Dr.prototype,"previewButton",2),Dt([Gt(".color-dropdown")],Dr.prototype,"dropdown",2),Dt([Xt()],Dr.prototype,"isDraggingGridHandle",2),Dt([Xt()],Dr.prototype,"isEmpty",2),Dt([Xt()],Dr.prototype,"inputValue",2),Dt([Xt()],Dr.prototype,"hue",2),Dt([Xt()],Dr.prototype,"saturation",2),Dt([Xt()],Dr.prototype,"lightness",2),Dt([Xt()],Dr.prototype,"brightness",2),Dt([Xt()],Dr.prototype,"alpha",2),Dt([Kt()],Dr.prototype,"value",2),Dt([mt()],Dr.prototype,"defaultValue",2),Dt([Kt()],Dr.prototype,"label",2),Dt([Kt()],Dr.prototype,"format",2),Dt([Kt({type:Boolean,reflect:!0})],Dr.prototype,"inline",2),Dt([Kt()],Dr.prototype,"size",2),Dt([Kt({attribute:"no-format-toggle",type:Boolean})],Dr.prototype,"noFormatToggle",2),Dt([Kt()],Dr.prototype,"name",2),Dt([Kt({type:Boolean,reflect:!0})],Dr.prototype,"disabled",2),Dt([Kt({type:Boolean,reflect:!0})],Dr.prototype,"invalid",2),Dt([Kt({type:Boolean})],Dr.prototype,"hoist",2),Dt([Kt({type:Boolean})],Dr.prototype,"opacity",2),Dt([Kt({type:Boolean})],Dr.prototype,"uppercase",2),Dt([Kt({attribute:!1})],Dr.prototype,"swatches",2),Dt([Kt()],Dr.prototype,"lang",2),Dt([Ut("format",{waitUntilFirstUpdate:!0})],Dr.prototype,"handleFormatChange",1),Dt([Ut("opacity")],Dr.prototype,"handleOpacityChange",1),Dt([Ut("value")],Dr.prototype,"handleValueChange",1),Dr=Dt([qt("sl-color-picker")],Dr);var Mr=n`
  ${it}

  :host(:not(:focus-within)) {
    position: absolute !important;
    width: 1px !important;
    height: 1px !important;
    clip: rect(0 0 0 0) !important;
    clip-path: inset(50%) !important;
    border: none !important;
    overflow: hidden !important;
    white-space: nowrap !important;
    padding: 0 !important;
  }
`,Or=class extends et{render(){return O` <slot></slot> `}};Or.styles=Mr,Or=Dt([qt("sl-visually-hidden")],Or);var Fr=n`
  ${it}
  ${rt}

  :host {
    display: block;
  }

  .input {
    flex: 1 1 auto;
    display: inline-flex;
    align-items: stretch;
    justify-content: start;
    position: relative;
    width: 100%;
    font-family: var(--sl-input-font-family);
    font-weight: var(--sl-input-font-weight);
    letter-spacing: var(--sl-input-letter-spacing);
    vertical-align: middle;
    overflow: hidden;
    cursor: text;
    transition: var(--sl-transition-fast) color, var(--sl-transition-fast) border, var(--sl-transition-fast) box-shadow,
      var(--sl-transition-fast) background-color;
  }

  /* Standard inputs */
  .input--standard {
    background-color: var(--sl-input-background-color);
    border: solid var(--sl-input-border-width) var(--sl-input-border-color);
  }

  .input--standard:hover:not(.input--disabled) {
    background-color: var(--sl-input-background-color-hover);
    border-color: var(--sl-input-border-color-hover);
  }

  .input--standard.input--focused:not(.input--disabled) {
    background-color: var(--sl-input-background-color-focus);
    border-color: var(--sl-input-border-color-focus);
    box-shadow: 0 0 0 var(--sl-focus-ring-width) var(--sl-input-focus-ring-color);
  }

  .input--standard.input--focused:not(.input--disabled) .input__control {
    color: var(--sl-input-color-focus);
  }

  .input--standard.input--disabled {
    background-color: var(--sl-input-background-color-disabled);
    border-color: var(--sl-input-border-color-disabled);
    opacity: 0.5;
    cursor: not-allowed;
  }

  .input--standard.input--disabled .input__control {
    color: var(--sl-input-color-disabled);
  }

  .input--standard.input--disabled .input__control::placeholder {
    color: var(--sl-input-placeholder-color-disabled);
  }

  /* Filled inputs */
  .input--filled {
    border: none;
    background-color: var(--sl-input-filled-background-color);
    color: var(--sl-input-color);
  }

  .input--filled:hover:not(.input--disabled) {
    background-color: var(--sl-input-filled-background-color-hover);
  }

  .input--filled.input--focused:not(.input--disabled) {
    background-color: var(--sl-input-filled-background-color-focus);
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .input--filled.input--disabled {
    background-color: var(--sl-input-filled-background-color-disabled);
    opacity: 0.5;
    cursor: not-allowed;
  }

  .input__control {
    flex: 1 1 auto;
    font-family: inherit;
    font-size: inherit;
    font-weight: inherit;
    min-width: 0;
    height: 100%;
    color: var(--sl-input-color);
    border: none;
    background: none;
    box-shadow: none;
    padding: 0;
    margin: 0;
    cursor: inherit;
    -webkit-appearance: none;
  }

  .input__control::-webkit-search-decoration,
  .input__control::-webkit-search-cancel-button,
  .input__control::-webkit-search-results-button,
  .input__control::-webkit-search-results-decoration {
    -webkit-appearance: none;
  }

  .input__control:-webkit-autofill,
  .input__control:-webkit-autofill:hover,
  .input__control:-webkit-autofill:focus,
  .input__control:-webkit-autofill:active {
    box-shadow: 0 0 0 var(--sl-input-height-large) var(--sl-input-background-color-hover) inset !important;
    -webkit-text-fill-color: var(--sl-color-primary-500);
    caret-color: var(--sl-input-color);
  }

  .input--filled .input__control:-webkit-autofill,
  .input--filled .input__control:-webkit-autofill:hover,
  .input--filled .input__control:-webkit-autofill:focus,
  .input--filled .input__control:-webkit-autofill:active {
    box-shadow: 0 0 0 var(--sl-input-height-large) var(--sl-input-filled-background-color) inset !important;
  }

  .input__control::placeholder {
    color: var(--sl-input-placeholder-color);
    user-select: none;
  }

  .input:hover:not(.input--disabled) .input__control {
    color: var(--sl-input-color-hover);
  }

  .input__control:focus {
    outline: none;
  }

  .input__prefix,
  .input__suffix {
    display: inline-flex;
    flex: 0 0 auto;
    align-items: center;
    cursor: default;
  }

  .input__prefix ::slotted(sl-icon),
  .input__suffix ::slotted(sl-icon) {
    color: var(--sl-input-icon-color);
  }

  /*
   * Size modifiers
   */

  .input--small {
    border-radius: var(--sl-input-border-radius-small);
    font-size: var(--sl-input-font-size-small);
    height: var(--sl-input-height-small);
  }

  .input--small .input__control {
    height: calc(var(--sl-input-height-small) - var(--sl-input-border-width) * 2);
    padding: 0 var(--sl-input-spacing-small);
  }

  .input--small .input__clear,
  .input--small .input__password-toggle {
    width: calc(1em + var(--sl-input-spacing-small) * 2);
  }

  .input--small .input__prefix ::slotted(*) {
    padding-inline-start: var(--sl-input-spacing-small);
  }

  .input--small .input__suffix ::slotted(*) {
    padding-inline-end: var(--sl-input-spacing-small);
  }

  .input--medium {
    border-radius: var(--sl-input-border-radius-medium);
    font-size: var(--sl-input-font-size-medium);
    height: var(--sl-input-height-medium);
  }

  .input--medium .input__control {
    height: calc(var(--sl-input-height-medium) - var(--sl-input-border-width) * 2);
    padding: 0 var(--sl-input-spacing-medium);
  }

  .input--medium .input__clear,
  .input--medium .input__password-toggle {
    width: calc(1em + var(--sl-input-spacing-medium) * 2);
  }

  .input--medium .input__prefix ::slotted(*) {
    padding-inline-start: var(--sl-input-spacing-medium);
  }

  .input--medium .input__suffix ::slotted(*) {
    padding-inline-end: var(--sl-input-spacing-medium);
  }

  .input--large {
    border-radius: var(--sl-input-border-radius-large);
    font-size: var(--sl-input-font-size-large);
    height: var(--sl-input-height-large);
  }

  .input--large .input__control {
    height: calc(var(--sl-input-height-large) - var(--sl-input-border-width) * 2);
    padding: 0 var(--sl-input-spacing-large);
  }

  .input--large .input__clear,
  .input--large .input__password-toggle {
    width: calc(1em + var(--sl-input-spacing-large) * 2);
  }

  .input--large .input__prefix ::slotted(*) {
    padding-inline-start: var(--sl-input-spacing-large);
  }

  .input--large .input__suffix ::slotted(*) {
    padding-inline-end: var(--sl-input-spacing-large);
  }

  /*
   * Pill modifier
   */

  .input--pill.input--small {
    border-radius: var(--sl-input-height-small);
  }

  .input--pill.input--medium {
    border-radius: var(--sl-input-height-medium);
  }

  .input--pill.input--large {
    border-radius: var(--sl-input-height-large);
  }

  /*
   * Clearable + Password Toggle
   */

  .input__clear,
  .input__password-toggle {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    font-size: inherit;
    color: var(--sl-input-icon-color);
    border: none;
    background: none;
    padding: 0;
    transition: var(--sl-transition-fast) color;
    cursor: pointer;
  }

  .input__clear:hover,
  .input__password-toggle:hover {
    color: var(--sl-input-icon-color-hover);
  }

  .input__clear:focus,
  .input__password-toggle:focus {
    outline: none;
  }

  .input--empty .input__clear {
    visibility: hidden;
  }

  /* Don't show the browser's password toggle in Edge */
  ::-ms-reveal {
    display: none;
  }

  /* Hide Firefox's clear button on date and time inputs */
  .input--is-firefox input[type='date'],
  .input--is-firefox input[type='time'] {
    clip-path: inset(0 2em 0 0);
  }

  /* Hide the built-in number spinner */
  .input--no-spin-buttons input[type='number']::-webkit-outer-spin-button,
  .input--no-spin-buttons input[type='number']::-webkit-inner-spin-button {
    -webkit-appearance: none;
    display: none;
  }

  .input--no-spin-buttons input[type='number'] {
    -moz-appearance: textfield;
  }
`,Ir=class extends et{constructor(){super(...arguments),this.formSubmitController=new It(this),this.hasSlotController=new Bt(this,"help-text","label"),this.localize=new ge(this),this.hasFocus=!1,this.isPasswordVisible=!1,this.type="text",this.size="medium",this.value="",this.defaultValue="",this.filled=!1,this.pill=!1,this.label="",this.helpText="",this.clearable=!1,this.togglePassword=!1,this.noSpinButtons=!1,this.disabled=!1,this.readonly=!1,this.required=!1,this.invalid=!1}get valueAsDate(){var t,e;return null!=(e=null==(t=this.input)?void 0:t.valueAsDate)?e:null}set valueAsDate(t){const e=document.createElement("input");e.type="date",e.valueAsDate=t,this.value=e.value}get valueAsNumber(){var t,e;return null!=(e=null==(t=this.input)?void 0:t.valueAsNumber)?e:parseFloat(this.value)}set valueAsNumber(t){const e=document.createElement("input");e.type="number",e.valueAsNumber=t,this.value=e.value}firstUpdated(){this.invalid=!this.input.checkValidity()}focus(t){this.input.focus(t)}blur(){this.input.blur()}select(){this.input.select()}setSelectionRange(t,e,o="none"){this.input.setSelectionRange(t,e,o)}setRangeText(t,e,o,r="preserve"){this.input.setRangeText(t,e,o,r),this.value!==this.input.value&&(this.value=this.input.value,Nt(this,"sl-input"),Nt(this,"sl-change"))}reportValidity(){return this.input.reportValidity()}setCustomValidity(t){this.input.setCustomValidity(t),this.invalid=!this.input.checkValidity()}handleBlur(){this.hasFocus=!1,Nt(this,"sl-blur")}handleChange(){this.value=this.input.value,Nt(this,"sl-change")}handleClearClick(t){this.value="",Nt(this,"sl-clear"),Nt(this,"sl-input"),Nt(this,"sl-change"),this.input.focus(),t.stopPropagation()}handleDisabledChange(){this.input.disabled=this.disabled,this.invalid=!this.input.checkValidity()}handleStepChange(){this.input.step=String(this.step),this.invalid=!this.input.checkValidity()}handleFocus(){this.hasFocus=!0,Nt(this,"sl-focus")}handleInput(){this.value=this.input.value,Nt(this,"sl-input")}handleInvalid(){this.invalid=!0}handleKeyDown(t){const e=t.metaKey||t.ctrlKey||t.shiftKey||t.altKey;"Enter"!==t.key||e||setTimeout((()=>{t.defaultPrevented||this.formSubmitController.submit()}))}handlePasswordToggle(){this.isPasswordVisible=!this.isPasswordVisible}handleValueChange(){this.invalid=!this.input.checkValidity()}render(){const t=this.hasSlotController.test("label"),e=this.hasSlotController.test("help-text"),o=!!this.label||!!t,r=!!this.helpText||!!e,i=this.clearable&&!this.disabled&&!this.readonly&&("number"==typeof this.value||this.value.length>0);return O`
      <div
        part="form-control"
        class=${Vt({"form-control":!0,"form-control--small":"small"===this.size,"form-control--medium":"medium"===this.size,"form-control--large":"large"===this.size,"form-control--has-label":o,"form-control--has-help-text":r})}
      >
        <label
          part="form-control-label"
          class="form-control__label"
          for="input"
          aria-hidden=${o?"false":"true"}
        >
          <slot name="label">${this.label}</slot>
        </label>

        <div part="form-control-input" class="form-control-input">
          <div
            part="base"
            class=${Vt({input:!0,"input--small":"small"===this.size,"input--medium":"medium"===this.size,"input--large":"large"===this.size,"input--pill":this.pill,"input--standard":!this.filled,"input--filled":this.filled,"input--disabled":this.disabled,"input--focused":this.hasFocus,"input--empty":!this.value,"input--invalid":this.invalid,"input--no-spin-buttons":this.noSpinButtons,"input--is-firefox":navigator.userAgent.includes("Firefox")})}
          >
            <span part="prefix" class="input__prefix">
              <slot name="prefix"></slot>
            </span>

            <input
              part="input"
              id="input"
              class="input__control"
              type=${"password"===this.type&&this.isPasswordVisible?"text":this.type}
              name=${Rt(this.name)}
              ?disabled=${this.disabled}
              ?readonly=${this.readonly}
              ?required=${this.required}
              placeholder=${Rt(this.placeholder)}
              minlength=${Rt(this.minlength)}
              maxlength=${Rt(this.maxlength)}
              min=${Rt(this.min)}
              max=${Rt(this.max)}
              step=${Rt(this.step)}
              .value=${ft(this.value)}
              autocapitalize=${Rt("password"===this.type?"off":this.autocapitalize)}
              autocomplete=${Rt("password"===this.type?"off":this.autocomplete)}
              autocorrect=${Rt("password"===this.type?"off":this.autocorrect)}
              ?autofocus=${this.autofocus}
              spellcheck=${Rt(this.spellcheck)}
              pattern=${Rt(this.pattern)}
              enterkeyhint=${Rt(this.enterkeyhint)}
              inputmode=${Rt(this.inputmode)}
              aria-describedby="help-text"
              aria-invalid=${this.invalid?"true":"false"}
              @change=${this.handleChange}
              @input=${this.handleInput}
              @invalid=${this.handleInvalid}
              @keydown=${this.handleKeyDown}
              @focus=${this.handleFocus}
              @blur=${this.handleBlur}
            />

            ${i?O`
                  <button
                    part="clear-button"
                    class="input__clear"
                    type="button"
                    aria-label=${this.localize.term("clearEntry")}
                    @click=${this.handleClearClick}
                    tabindex="-1"
                  >
                    <slot name="clear-icon">
                      <sl-icon name="x-circle-fill" library="system"></sl-icon>
                    </slot>
                  </button>
                `:""}
            ${this.togglePassword&&!this.disabled?O`
                  <button
                    part="password-toggle-button"
                    class="input__password-toggle"
                    type="button"
                    aria-label=${this.localize.term(this.isPasswordVisible?"hidePassword":"showPassword")}
                    @click=${this.handlePasswordToggle}
                    tabindex="-1"
                  >
                    ${this.isPasswordVisible?O`
                          <slot name="show-password-icon">
                            <sl-icon name="eye-slash" library="system"></sl-icon>
                          </slot>
                        `:O`
                          <slot name="hide-password-icon">
                            <sl-icon name="eye" library="system"></sl-icon>
                          </slot>
                        `}
                  </button>
                `:""}

            <span part="suffix" class="input__suffix">
              <slot name="suffix"></slot>
            </span>
          </div>
        </div>

        <div
          part="form-control-help-text"
          id="help-text"
          class="form-control__help-text"
          aria-hidden=${r?"false":"true"}
        >
          <slot name="help-text">${this.helpText}</slot>
        </div>
      </div>
    `}};Ir.styles=Fr,Dt([Gt(".input__control")],Ir.prototype,"input",2),Dt([Xt()],Ir.prototype,"hasFocus",2),Dt([Xt()],Ir.prototype,"isPasswordVisible",2),Dt([Kt({reflect:!0})],Ir.prototype,"type",2),Dt([Kt({reflect:!0})],Ir.prototype,"size",2),Dt([Kt()],Ir.prototype,"name",2),Dt([Kt()],Ir.prototype,"value",2),Dt([mt()],Ir.prototype,"defaultValue",2),Dt([Kt({type:Boolean,reflect:!0})],Ir.prototype,"filled",2),Dt([Kt({type:Boolean,reflect:!0})],Ir.prototype,"pill",2),Dt([Kt()],Ir.prototype,"label",2),Dt([Kt({attribute:"help-text"})],Ir.prototype,"helpText",2),Dt([Kt({type:Boolean})],Ir.prototype,"clearable",2),Dt([Kt({attribute:"toggle-password",type:Boolean})],Ir.prototype,"togglePassword",2),Dt([Kt({attribute:"no-spin-buttons",type:Boolean})],Ir.prototype,"noSpinButtons",2),Dt([Kt()],Ir.prototype,"placeholder",2),Dt([Kt({type:Boolean,reflect:!0})],Ir.prototype,"disabled",2),Dt([Kt({type:Boolean,reflect:!0})],Ir.prototype,"readonly",2),Dt([Kt({type:Number})],Ir.prototype,"minlength",2),Dt([Kt({type:Number})],Ir.prototype,"maxlength",2),Dt([Kt()],Ir.prototype,"min",2),Dt([Kt()],Ir.prototype,"max",2),Dt([Kt()],Ir.prototype,"step",2),Dt([Kt()],Ir.prototype,"pattern",2),Dt([Kt({type:Boolean,reflect:!0})],Ir.prototype,"required",2),Dt([Kt({type:Boolean,reflect:!0})],Ir.prototype,"invalid",2),Dt([Kt()],Ir.prototype,"autocapitalize",2),Dt([Kt()],Ir.prototype,"autocorrect",2),Dt([Kt()],Ir.prototype,"autocomplete",2),Dt([Kt({type:Boolean})],Ir.prototype,"autofocus",2),Dt([Kt()],Ir.prototype,"enterkeyhint",2),Dt([Kt({type:Boolean})],Ir.prototype,"spellcheck",2),Dt([Kt()],Ir.prototype,"inputmode",2),Dt([Ut("disabled",{waitUntilFirstUpdate:!0})],Ir.prototype,"handleDisabledChange",1),Dt([Ut("step",{waitUntilFirstUpdate:!0})],Ir.prototype,"handleStepChange",1),Dt([Ut("value",{waitUntilFirstUpdate:!0})],Ir.prototype,"handleValueChange",1),Ir=Dt([qt("sl-input")],Ir);var Br=n`
  ${it}

  :host {
    display: inline-block;
  }

  .dropdown::part(popup) {
    z-index: var(--sl-z-index-dropdown);
  }

  .dropdown[data-current-placement^='top']::part(popup) {
    transform-origin: bottom;
  }

  .dropdown[data-current-placement^='bottom']::part(popup) {
    transform-origin: top;
  }

  .dropdown[data-current-placement^='left']::part(popup) {
    transform-origin: right;
  }

  .dropdown[data-current-placement^='right']::part(popup) {
    transform-origin: left;
  }

  .dropdown__trigger {
    display: block;
  }

  .dropdown__panel {
    font-family: var(--sl-font-sans);
    font-size: var(--sl-font-size-medium);
    font-weight: var(--sl-font-weight-normal);
    color: var(--color);
    box-shadow: var(--sl-shadow-large);
    overflow: auto;
    overscroll-behavior: none;
    pointer-events: none;
  }

  .dropdown--open .dropdown__panel {
    pointer-events: all;
  }
`,Pr=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.open=!1,this.placement="bottom-start",this.disabled=!1,this.stayOpenOnSelect=!1,this.distance=0,this.skidding=0,this.hoist=!1}connectedCallback(){super.connectedCallback(),this.handleMenuItemActivate=this.handleMenuItemActivate.bind(this),this.handlePanelSelect=this.handlePanelSelect.bind(this),this.handleDocumentKeyDown=this.handleDocumentKeyDown.bind(this),this.handleDocumentMouseDown=this.handleDocumentMouseDown.bind(this),this.containingElement||(this.containingElement=this)}firstUpdated(){this.panel.hidden=!this.open,this.open&&(this.addOpenListeners(),this.popup.active=!0)}disconnectedCallback(){super.disconnectedCallback(),this.removeOpenListeners(),this.hide()}focusOnTrigger(){const t=this.trigger.querySelector("slot").assignedElements({flatten:!0})[0];"function"==typeof(null==t?void 0:t.focus)&&t.focus()}getMenu(){return this.panel.querySelector("slot").assignedElements({flatten:!0}).find((t=>"sl-menu"===t.tagName.toLowerCase()))}handleDocumentKeyDown(t){var e;if("Escape"===t.key)return this.hide(),void this.focusOnTrigger();if("Tab"===t.key){if(this.open&&"sl-menu-item"===(null==(e=document.activeElement)?void 0:e.tagName.toLowerCase()))return t.preventDefault(),this.hide(),void this.focusOnTrigger();setTimeout((()=>{var t,e,o;const r=(null==(t=this.containingElement)?void 0:t.getRootNode())instanceof ShadowRoot?null==(o=null==(e=document.activeElement)?void 0:e.shadowRoot)?void 0:o.activeElement:document.activeElement;this.containingElement&&(null==r?void 0:r.closest(this.containingElement.tagName.toLowerCase()))===this.containingElement||this.hide()}))}}handleDocumentMouseDown(t){const e=t.composedPath();this.containingElement&&!e.includes(this.containingElement)&&this.hide()}handleMenuItemActivate(t){Fe(t.target,this.panel)}handlePanelSelect(t){const e=t.target;this.stayOpenOnSelect||"sl-menu"!==e.tagName.toLowerCase()||(this.hide(),this.focusOnTrigger())}handleTriggerClick(){this.open?this.hide():this.show()}handleTriggerKeyDown(t){if("Escape"===t.key)return this.focusOnTrigger(),void this.hide();if([" ","Enter"].includes(t.key))return t.preventDefault(),void this.handleTriggerClick();const e=this.getMenu();if(e){const o=e.defaultSlot.assignedElements({flatten:!0}),r=o[0],i=o[o.length-1];["ArrowDown","ArrowUp","Home","End"].includes(t.key)&&(t.preventDefault(),this.open||this.show(),o.length>0&&requestAnimationFrame((()=>{"ArrowDown"!==t.key&&"Home"!==t.key||(e.setCurrentItem(r),r.focus()),"ArrowUp"!==t.key&&"End"!==t.key||(e.setCurrentItem(i),i.focus())})));const a=["Tab","Shift","Meta","Ctrl","Alt"];this.open&&!a.includes(t.key)&&e.typeToSelect(t)}}handleTriggerKeyUp(t){" "===t.key&&t.preventDefault()}handleTriggerSlotChange(){this.updateAccessibleTrigger()}updateAccessibleTrigger(){const t=this.trigger.querySelector("slot").assignedElements({flatten:!0}).find((t=>cr(t).start));let e;if(t){switch(t.tagName.toLowerCase()){case"sl-button":case"sl-icon-button":e=t.button;break;default:e=t}e.setAttribute("aria-haspopup","true"),e.setAttribute("aria-expanded",this.open?"true":"false")}}async show(){if(!this.open)return this.open=!0,Ht(this,"sl-after-show")}async hide(){if(this.open)return this.open=!1,Ht(this,"sl-after-hide")}reposition(){this.popup.reposition()}addOpenListeners(){this.panel.addEventListener("sl-activate",this.handleMenuItemActivate),this.panel.addEventListener("sl-select",this.handlePanelSelect),document.addEventListener("keydown",this.handleDocumentKeyDown),document.addEventListener("mousedown",this.handleDocumentMouseDown)}removeOpenListeners(){this.panel.removeEventListener("sl-activate",this.handleMenuItemActivate),this.panel.removeEventListener("sl-select",this.handlePanelSelect),document.removeEventListener("keydown",this.handleDocumentKeyDown),document.removeEventListener("mousedown",this.handleDocumentMouseDown)}async handleOpenChange(){if(this.disabled)this.open=!1;else if(this.updateAccessibleTrigger(),this.open){Nt(this,"sl-show"),this.addOpenListeners(),await oe(this),this.panel.hidden=!1,this.popup.active=!0;const{keyframes:t,options:e}=ce(this,"dropdown.show",{dir:this.localize.dir()});await Jt(this.popup.popup,t,e),Nt(this,"sl-after-show")}else{Nt(this,"sl-hide"),this.removeOpenListeners(),await oe(this);const{keyframes:t,options:e}=ce(this,"dropdown.hide",{dir:this.localize.dir()});await Jt(this.popup.popup,t,e),this.panel.hidden=!0,this.popup.active=!1,Nt(this,"sl-after-hide")}}render(){return O`
      <sl-popup
        part="base"
        id="dropdown"
        placement=${this.placement}
        distance=${this.distance}
        skidding=${this.skidding}
        strategy=${this.hoist?"fixed":"absolute"}
        flip
        shift
        class=${Vt({dropdown:!0,"dropdown--open":this.open})}
      >
        <span
          slot="anchor"
          part="trigger"
          class="dropdown__trigger"
          @click=${this.handleTriggerClick}
          @keydown=${this.handleTriggerKeyDown}
          @keyup=${this.handleTriggerKeyUp}
        >
          <slot name="trigger" @slotchange=${this.handleTriggerSlotChange}></slot>
        </span>

        <div
          part="panel"
          class="dropdown__panel"
          aria-hidden=${this.open?"false":"true"}
          aria-labelledby="dropdown"
        >
          <slot></slot>
        </div>
      </sl-popup>
    `}};Pr.styles=Br,Dt([Gt(".dropdown")],Pr.prototype,"popup",2),Dt([Gt(".dropdown__trigger")],Pr.prototype,"trigger",2),Dt([Gt(".dropdown__panel")],Pr.prototype,"panel",2),Dt([Kt({type:Boolean,reflect:!0})],Pr.prototype,"open",2),Dt([Kt({reflect:!0})],Pr.prototype,"placement",2),Dt([Kt({type:Boolean,reflect:!0})],Pr.prototype,"disabled",2),Dt([Kt({attribute:"stay-open-on-select",type:Boolean,reflect:!0})],Pr.prototype,"stayOpenOnSelect",2),Dt([Kt({attribute:!1})],Pr.prototype,"containingElement",2),Dt([Kt({type:Number})],Pr.prototype,"distance",2),Dt([Kt({type:Number})],Pr.prototype,"skidding",2),Dt([Kt({type:Boolean})],Pr.prototype,"hoist",2),Dt([Ut("open",{waitUntilFirstUpdate:!0})],Pr.prototype,"handleOpenChange",1),Pr=Dt([qt("sl-dropdown")],Pr),le("dropdown.show",{keyframes:[{opacity:0,transform:"scale(0.9)"},{opacity:1,transform:"scale(1)"}],options:{duration:100,easing:"ease"}}),le("dropdown.hide",{keyframes:[{opacity:1,transform:"scale(1)"},{opacity:0,transform:"scale(0.9)"}],options:{duration:100,easing:"ease"}});var Vr=n`
  ${it}

  :host {
    --arrow-size: 4px;
    --arrow-color: var(--sl-color-neutral-1000);

    display: contents;
  }

  .popup {
    position: absolute;
    isolation: isolate;
  }

  .popup--fixed {
    position: fixed;
  }

  .popup:not(.popup--active) {
    display: none;
  }

  .popup__arrow {
    position: absolute;
    width: calc(var(--arrow-size) * 2);
    height: calc(var(--arrow-size) * 2);
    transform: rotate(45deg);
    background: var(--arrow-color);
    z-index: -1;
  }
`;function Rr(t){return t.split("-")[0]}function Ur(t){return t.split("-")[1]}function Nr(t){return["top","bottom"].includes(Rr(t))?"x":"y"}function Hr(t){return"y"===t?"height":"width"}function qr(t,e,o){let{reference:r,floating:i}=t;const a=r.x+r.width/2-i.width/2,s=r.y+r.height/2-i.height/2,n=Nr(e),l=Hr(n),c=r[l]/2-i[l]/2,d="x"===n;let h;switch(Rr(e)){case"top":h={x:a,y:r.y-i.height};break;case"bottom":h={x:a,y:r.y+r.height};break;case"right":h={x:r.x+r.width,y:s};break;case"left":h={x:r.x-i.width,y:s};break;default:h={x:r.x,y:r.y}}switch(Ur(e)){case"start":h[n]-=c*(o&&d?-1:1);break;case"end":h[n]+=c*(o&&d?-1:1)}return h}function jr(t){return"number"!=typeof t?function(t){return St({top:0,right:0,bottom:0,left:0},t)}(t):{top:t,right:t,bottom:t,left:t}}function Kr(t){return At(St({},t),{top:t.y,left:t.x,right:t.x+t.width,bottom:t.y+t.height})}async function Xr(t,e){var o;void 0===e&&(e={});const{x:r,y:i,platform:a,rects:s,elements:n,strategy:l}=t,{boundary:c="clippingAncestors",rootBoundary:d="viewport",elementContext:h="floating",altBoundary:u=!1,padding:p=0}=e,f=jr(p),m=n[u?"floating"===h?"reference":"floating":h],b=Kr(await a.getClippingRect({element:null==(o=await(null==a.isElement?void 0:a.isElement(m)))||o?m:m.contextElement||await(null==a.getDocumentElement?void 0:a.getDocumentElement(n.floating)),boundary:c,rootBoundary:d,strategy:l})),g=Kr(a.convertOffsetParentRelativeRectToViewportRelativeRect?await a.convertOffsetParentRelativeRectToViewportRelativeRect({rect:"floating"===h?At(St({},s.floating),{x:r,y:i}):s.reference,offsetParent:await(null==a.getOffsetParent?void 0:a.getOffsetParent(n.floating)),strategy:l}):s[h]);return{top:b.top-g.top+f.top,bottom:g.bottom-b.bottom+f.bottom,left:b.left-g.left+f.left,right:g.right-b.right+f.right}}var Wr=Math.min,Yr=Math.max;function Gr(t,e,o){return Yr(t,Wr(e,o))}var Zr={left:"right",right:"left",bottom:"top",top:"bottom"};function Qr(t){return t.replace(/left|right|bottom|top/g,(t=>Zr[t]))}var Jr={start:"end",end:"start"};function ti(t){return t.replace(/start|end/g,(t=>Jr[t]))}["top","right","bottom","left"].reduce(((t,e)=>t.concat(e,e+"-start",e+"-end")),[]);var ei=function(t){return void 0===t&&(t={}),{name:"flip",options:t,async fn(e){var o;const{placement:r,middlewareData:i,rects:a,initialPlacement:s,platform:n,elements:l}=e,c=t,{mainAxis:d=!0,crossAxis:h=!0,fallbackPlacements:u,fallbackStrategy:p="bestFit",flipAlignment:f=!0}=c,m=Tt(c,["mainAxis","crossAxis","fallbackPlacements","fallbackStrategy","flipAlignment"]),b=Rr(r),g=u||(b!==s&&f?function(t){const e=Qr(t);return[ti(t),e,ti(e)]}(s):[Qr(s)]),v=[s,...g],y=await Xr(e,m),w=[];let _=(null==(o=i.flip)?void 0:o.overflows)||[];if(d&&w.push(y[b]),h){const{main:t,cross:e}=function(t,e,o){void 0===o&&(o=!1);const r=Ur(t),i=Nr(t),a=Hr(i);let s="x"===i?r===(o?"end":"start")?"right":"left":"start"===r?"bottom":"top";return e.reference[a]>e.floating[a]&&(s=Qr(s)),{main:s,cross:Qr(s)}}(r,a,await(null==n.isRTL?void 0:n.isRTL(l.floating)));w.push(y[t],y[e])}if(_=[..._,{placement:r,overflows:w}],!w.every((t=>t<=0))){var x,k;const t=(null!=(x=null==(k=i.flip)?void 0:k.index)?x:0)+1,e=v[t];if(e)return{data:{index:t,overflows:_},reset:{placement:e}};let o="bottom";switch(p){case"bestFit":{var $;const t=null==($=_.map((t=>[t,t.overflows.filter((t=>t>0)).reduce(((t,e)=>t+e),0)])).sort(((t,e)=>t[1]-e[1]))[0])?void 0:$[0].placement;t&&(o=t);break}case"initialPlacement":o=s}if(r!==o)return{reset:{placement:o}}}return{}}}},oi=function(t){return void 0===t&&(t=0),{name:"offset",options:t,async fn(e){const{x:o,y:r}=e,i=await async function(t,e){const{placement:o,platform:r,elements:i}=t,a=await(null==r.isRTL?void 0:r.isRTL(i.floating)),s=Rr(o),n=Ur(o),l="x"===Nr(o),c=["left","top"].includes(s)?-1:1,d=a&&l?-1:1,h="function"==typeof e?e(t):e;let{mainAxis:u,crossAxis:p,alignmentAxis:f}="number"==typeof h?{mainAxis:h,crossAxis:0,alignmentAxis:null}:St({mainAxis:0,crossAxis:0,alignmentAxis:null},h);return n&&"number"==typeof f&&(p="end"===n?-1*f:f),l?{x:p*d,y:u*c}:{x:u*c,y:p*d}}(e,t);return{x:o+i.x,y:r+i.y,data:i}}}};function ri(t){return t&&t.document&&t.location&&t.alert&&t.setInterval}function ii(t){if(null==t)return window;if(!ri(t)){const e=t.ownerDocument;return e&&e.defaultView||window}return t}function ai(t){return ii(t).getComputedStyle(t)}function si(t){return ri(t)?"":t?(t.nodeName||"").toLowerCase():""}function ni(){const t=navigator.userAgentData;return null!=t&&t.brands?t.brands.map((t=>t.brand+"/"+t.version)).join(" "):navigator.userAgent}function li(t){return t instanceof ii(t).HTMLElement}function ci(t){return t instanceof ii(t).Element}function di(t){return"undefined"!=typeof ShadowRoot&&(t instanceof ii(t).ShadowRoot||t instanceof ShadowRoot)}function hi(t){const{overflow:e,overflowX:o,overflowY:r}=ai(t);return/auto|scroll|overlay|hidden/.test(e+r+o)}function ui(t){return["table","td","th"].includes(si(t))}function pi(t){const e=/firefox/i.test(ni()),o=ai(t);return"none"!==o.transform||"none"!==o.perspective||"paint"===o.contain||["transform","perspective"].includes(o.willChange)||e&&"filter"===o.willChange||e&&!!o.filter&&"none"!==o.filter}function fi(){return!/^((?!chrome|android).)*safari/i.test(ni())}var mi=Math.min,bi=Math.max,gi=Math.round;function vi(t,e,o){var r,i,a,s;void 0===e&&(e=!1),void 0===o&&(o=!1);const n=t.getBoundingClientRect();let l=1,c=1;e&&li(t)&&(l=t.offsetWidth>0&&gi(n.width)/t.offsetWidth||1,c=t.offsetHeight>0&&gi(n.height)/t.offsetHeight||1);const d=ci(t)?ii(t):window,h=!fi()&&o,u=(n.left+(h&&null!=(r=null==(i=d.visualViewport)?void 0:i.offsetLeft)?r:0))/l,p=(n.top+(h&&null!=(a=null==(s=d.visualViewport)?void 0:s.offsetTop)?a:0))/c,f=n.width/l,m=n.height/c;return{width:f,height:m,top:p,right:u+f,bottom:p+m,left:u,x:u,y:p}}function yi(t){return(e=t,(e instanceof ii(e).Node?t.ownerDocument:t.document)||window.document).documentElement;var e}function wi(t){return ci(t)?{scrollLeft:t.scrollLeft,scrollTop:t.scrollTop}:{scrollLeft:t.pageXOffset,scrollTop:t.pageYOffset}}function _i(t){return vi(yi(t)).left+wi(t).scrollLeft}function xi(t,e,o){const r=li(e),i=yi(e),a=vi(t,r&&function(t){const e=vi(t);return gi(e.width)!==t.offsetWidth||gi(e.height)!==t.offsetHeight}(e),"fixed"===o);let s={scrollLeft:0,scrollTop:0};const n={x:0,y:0};if(r||!r&&"fixed"!==o)if(("body"!==si(e)||hi(i))&&(s=wi(e)),li(e)){const t=vi(e,!0);n.x=t.x+e.clientLeft,n.y=t.y+e.clientTop}else i&&(n.x=_i(i));return{x:a.left+s.scrollLeft-n.x,y:a.top+s.scrollTop-n.y,width:a.width,height:a.height}}function ki(t){return"html"===si(t)?t:t.assignedSlot||t.parentNode||(di(t)?t.host:null)||yi(t)}function $i(t){return li(t)&&"fixed"!==getComputedStyle(t).position?t.offsetParent:null}function Ci(t){const e=ii(t);let o=$i(t);for(;o&&ui(o)&&"static"===getComputedStyle(o).position;)o=$i(o);return o&&("html"===si(o)||"body"===si(o)&&"static"===getComputedStyle(o).position&&!pi(o))?e:o||function(t){let e=ki(t);for(di(e)&&(e=e.host);li(e)&&!["html","body"].includes(si(e));){if(pi(e))return e;e=e.parentNode}return null}(t)||e}function zi(t){if(li(t))return{width:t.offsetWidth,height:t.offsetHeight};const e=vi(t);return{width:e.width,height:e.height}}function Si(t){const e=ki(t);return["html","body","#document"].includes(si(e))?t.ownerDocument.body:li(e)&&hi(e)?e:Si(e)}function Ai(t,e){var o;void 0===e&&(e=[]);const r=Si(t),i=r===(null==(o=t.ownerDocument)?void 0:o.body),a=ii(r),s=i?[a].concat(a.visualViewport||[],hi(r)?r:[]):r,n=e.concat(s);return i?n:n.concat(Ai(s))}function Ti(t,e,o){return"viewport"===e?Kr(function(t,e){const o=ii(t),r=yi(t),i=o.visualViewport;let a=r.clientWidth,s=r.clientHeight,n=0,l=0;if(i){a=i.width,s=i.height;const t=fi();(t||!t&&"fixed"===e)&&(n=i.offsetLeft,l=i.offsetTop)}return{width:a,height:s,x:n,y:l}}(t,o)):ci(e)?function(t,e){const o=vi(t,!1,"fixed"===e),r=o.top+t.clientTop,i=o.left+t.clientLeft;return{top:r,left:i,x:i,y:r,right:i+t.clientWidth,bottom:r+t.clientHeight,width:t.clientWidth,height:t.clientHeight}}(e,o):Kr(function(t){var e;const o=yi(t),r=wi(t),i=null==(e=t.ownerDocument)?void 0:e.body,a=bi(o.scrollWidth,o.clientWidth,i?i.scrollWidth:0,i?i.clientWidth:0),s=bi(o.scrollHeight,o.clientHeight,i?i.scrollHeight:0,i?i.clientHeight:0);let n=-r.scrollLeft+_i(t);const l=-r.scrollTop;return"rtl"===ai(i||o).direction&&(n+=bi(o.clientWidth,i?i.clientWidth:0)-a),{width:a,height:s,x:n,y:l}}(yi(t)))}function Ei(t){const e=Ai(t),o=["absolute","fixed"].includes(ai(t).position)&&li(t)?Ci(t):t;return ci(o)?e.filter((t=>ci(t)&&function(t,e){const o=null==e.getRootNode?void 0:e.getRootNode();if(t.contains(e))return!0;if(o&&di(o)){let o=e;do{if(o&&t===o)return!0;o=o.parentNode||o.host}while(o)}return!1}(t,o)&&"body"!==si(t))):[]}var Di={getClippingRect:function(t){let{element:e,boundary:o,rootBoundary:r,strategy:i}=t;const a=[..."clippingAncestors"===o?Ei(e):[].concat(o),r],s=a[0],n=a.reduce(((t,o)=>{const r=Ti(e,o,i);return t.top=bi(r.top,t.top),t.right=mi(r.right,t.right),t.bottom=mi(r.bottom,t.bottom),t.left=bi(r.left,t.left),t}),Ti(e,s,i));return{width:n.right-n.left,height:n.bottom-n.top,x:n.left,y:n.top}},convertOffsetParentRelativeRectToViewportRelativeRect:function(t){let{rect:e,offsetParent:o,strategy:r}=t;const i=li(o),a=yi(o);if(o===a)return e;let s={scrollLeft:0,scrollTop:0};const n={x:0,y:0};if((i||!i&&"fixed"!==r)&&(("body"!==si(o)||hi(a))&&(s=wi(o)),li(o))){const t=vi(o,!0);n.x=t.x+o.clientLeft,n.y=t.y+o.clientTop}return At(St({},e),{x:e.x-s.scrollLeft+n.x,y:e.y-s.scrollTop+n.y})},isElement:ci,getDimensions:zi,getOffsetParent:Ci,getDocumentElement:yi,getElementRects:t=>{let{reference:e,floating:o,strategy:r}=t;return{reference:xi(e,Ci(o),r),floating:At(St({},zi(o)),{x:0,y:0})}},getClientRects:t=>Array.from(t.getClientRects()),isRTL:t=>"rtl"===ai(t).direction};var Li=(t,e,o)=>(async(t,e,o)=>{const{placement:r="bottom",strategy:i="absolute",middleware:a=[],platform:s}=o,n=await(null==s.isRTL?void 0:s.isRTL(e));let l=await s.getElementRects({reference:t,floating:e,strategy:i}),{x:c,y:d}=qr(l,r,n),h=r,u={},p=0;for(let o=0;o<a.length;o++){const{name:f,fn:m}=a[o],{x:b,y:g,data:v,reset:y}=await m({x:c,y:d,initialPlacement:r,placement:h,strategy:i,middlewareData:u,rects:l,platform:s,elements:{reference:t,floating:e}});c=null!=b?b:c,d=null!=g?g:d,u=At(St({},u),{[f]:St(St({},u[f]),v)}),y&&p<=50&&(p++,"object"==typeof y&&(y.placement&&(h=y.placement),y.rects&&(l=!0===y.rects?await s.getElementRects({reference:t,floating:e,strategy:i}):y.rects),({x:c,y:d}=qr(l,h,n))),o=-1)}return{x:c,y:d,placement:h,strategy:i,middlewareData:u}})(t,e,St({platform:Di},o)),Mi=class extends et{constructor(){super(...arguments),this.active=!1,this.placement="top",this.strategy="absolute",this.distance=0,this.skidding=0,this.arrow=!1,this.arrowPadding=10,this.flip=!1,this.flipFallbackPlacement="",this.flipFallbackStrategy="initialPlacement",this.flipPadding=0,this.shift=!1,this.shiftPadding=0,this.autoSize=!1,this.autoSizePadding=0}async connectedCallback(){super.connectedCallback(),await this.updateComplete,this.start()}disconnectedCallback(){this.stop()}async handleAnchorSlotChange(){if(await this.stop(),this.anchor=this.querySelector('[slot="anchor"]'),this.anchor instanceof HTMLSlotElement&&(this.anchor=this.anchor.assignedElements({flatten:!0})[0]),!this.anchor)throw new Error('Invalid anchor element: no child with slot="anchor" was found.');this.start()}start(){this.anchor&&(this.cleanup=function(t,e,o,r){void 0===r&&(r={});const{ancestorScroll:i=!0,ancestorResize:a=!0,elementResize:s=!0,animationFrame:n=!1}=r,l=i&&!n,c=a&&!n,d=l||c?[...ci(t)?Ai(t):[],...Ai(e)]:[];d.forEach((t=>{l&&t.addEventListener("scroll",o,{passive:!0}),c&&t.addEventListener("resize",o)}));let h,u=null;if(s){let r=!0;u=new ResizeObserver((()=>{r||o(),r=!1})),ci(t)&&!n&&u.observe(t),u.observe(e)}let p=n?vi(t):null;return n&&function e(){const r=vi(t);!p||r.x===p.x&&r.y===p.y&&r.width===p.width&&r.height===p.height||o(),p=r,h=requestAnimationFrame(e)}(),o(),()=>{var t;d.forEach((t=>{l&&t.removeEventListener("scroll",o),c&&t.removeEventListener("resize",o)})),null==(t=u)||t.disconnect(),u=null,n&&cancelAnimationFrame(h)}}(this.anchor,this.popup,(()=>{this.reposition()})))}async stop(){return new Promise((t=>{this.cleanup?(this.cleanup(),this.cleanup=void 0,this.removeAttribute("data-current-placement"),requestAnimationFrame((()=>t()))):t()}))}async updated(t){super.updated(t),t.has("active")?this.active?this.start():this.stop():this.active&&(await this.updateComplete,this.reposition())}reposition(){if(!this.active||!this.anchor)return;const t=[oi({mainAxis:this.distance,crossAxis:this.skidding})];this.autoSize?t.push(function(t){return void 0===t&&(t={}),{name:"size",options:t,async fn(e){const{placement:o,rects:r,platform:i,elements:a}=e,s=t,{apply:n=(()=>{})}=s,l=Tt(s,["apply"]),c=await Xr(e,l),d=Rr(o),h=Ur(o);let u,p;"top"===d||"bottom"===d?(u=d,p=h===(await(null==i.isRTL?void 0:i.isRTL(a.floating))?"start":"end")?"left":"right"):(p=d,u="end"===h?"top":"bottom");const f=Yr(c.left,0),m=Yr(c.right,0),b=Yr(c.top,0),g=Yr(c.bottom,0),v={availableHeight:r.floating.height-(["left","right"].includes(o)?2*(0!==b||0!==g?b+g:Yr(c.top,c.bottom)):c[u]),availableWidth:r.floating.width-(["top","bottom"].includes(o)?2*(0!==f||0!==m?f+m:Yr(c.left,c.right)):c[p])};await n(St(St({},e),v));const y=await i.getDimensions(a.floating);return r.floating.width!==y.width||r.floating.height!==y.height?{reset:{rects:!0}}:{}}}}({boundary:this.autoSizeBoundary,padding:this.autoSizePadding,apply:({availableWidth:t,availableHeight:e})=>{Object.assign(this.popup.style,{maxWidth:`${t}px`,maxHeight:`${e}px`})}})):Object.assign(this.popup.style,{maxWidth:"",maxHeight:""}),this.flip&&t.push(ei({boundary:this.flipBoundary,fallbackPlacement:this.flipFallbackPlacement,fallbackStrategy:this.flipFallbackStrategy,padding:this.flipPadding})),this.shift&&t.push(function(t){return void 0===t&&(t={}),{name:"shift",options:t,async fn(e){const{x:o,y:r,placement:i}=e,a=t,{mainAxis:s=!0,crossAxis:n=!1,limiter:l={fn:t=>{let{x:e,y:o}=t;return{x:e,y:o}}}}=a,c=Tt(a,["mainAxis","crossAxis","limiter"]),d={x:o,y:r},h=await Xr(e,c),u=Nr(Rr(i)),p=function(t){return"x"===t?"y":"x"}(u);let f=d[u],m=d[p];if(s){const t="y"===u?"bottom":"right";f=Gr(f+h["y"===u?"top":"left"],f,f-h[t])}if(n){const t="y"===p?"bottom":"right";m=Gr(m+h["y"===p?"top":"left"],m,m-h[t])}const b=l.fn(At(St({},e),{[u]:f,[p]:m}));return At(St({},b),{data:{x:b.x-o,y:b.y-r}})}}}({boundary:this.shiftBoundary,padding:this.shiftPadding})),this.arrow&&t.push((t=>({name:"arrow",options:t,async fn(e){const{element:o,padding:r=0}=null!=t?t:{},{x:i,y:a,placement:s,rects:n,platform:l}=e;if(null==o)return{};const c=jr(r),d={x:i,y:a},h=Nr(s),u=Ur(s),p=Hr(h),f=await l.getDimensions(o),m="y"===h?"top":"left",b="y"===h?"bottom":"right",g=n.reference[p]+n.reference[h]-d[h]-n.floating[p],v=d[h]-n.reference[h],y=await(null==l.getOffsetParent?void 0:l.getOffsetParent(o));let w=y?"y"===h?y.clientHeight||0:y.clientWidth||0:0;0===w&&(w=n.floating[p]);const _=g/2-v/2,x=c[m],k=w-f[p]-c[b],$=w/2-f[p]/2+_,C=Gr(x,$,k),z=("start"===u?c[m]:c[b])>0&&$!==C&&n.reference[p]<=n.floating[p];return{[h]:d[h]-(z?$<x?x-$:k-$:0),data:{[h]:C,centerOffset:$-C}}}}))({element:this.arrowEl,padding:this.arrowPadding})),Li(this.anchor,this.popup,{placement:this.placement,middleware:t,strategy:this.strategy}).then((({x:t,y:e,middlewareData:o,placement:r})=>{var i,a;const s={top:"bottom",right:"left",bottom:"top",left:"right"}[r.split("-")[0]];if(this.setAttribute("data-current-placement",r),Object.assign(this.popup.style,{left:`${t}px`,top:`${e}px`}),this.arrow){const t=null==(i=o.arrow)?void 0:i.x,e=null==(a=o.arrow)?void 0:a.y;Object.assign(this.arrowEl.style,{left:"number"==typeof t?`${t}px`:"",top:"number"==typeof e?`${e}px`:"",right:"",bottom:"",[s]:"calc(var(--arrow-size) * -1)"})}})),Nt(this,"sl-reposition")}render(){return O`
      <slot name="anchor" @slotchange=${this.handleAnchorSlotChange}></slot>

      <div
        part="popup"
        class=${Vt({popup:!0,"popup--active":this.active,"popup--fixed":"fixed"===this.strategy,"popup--has-arrow":this.arrow})}
      >
        <slot></slot>
        ${this.arrow?O`<div part="arrow" class="popup__arrow" role="presentation"></div>`:""}
      </div>
    `}};Mi.styles=Vr,Dt([Gt(".popup")],Mi.prototype,"popup",2),Dt([Gt(".popup__arrow")],Mi.prototype,"arrowEl",2),Dt([Kt({type:Boolean,reflect:!0})],Mi.prototype,"active",2),Dt([Kt({reflect:!0})],Mi.prototype,"placement",2),Dt([Kt({reflect:!0})],Mi.prototype,"strategy",2),Dt([Kt({type:Number})],Mi.prototype,"distance",2),Dt([Kt({type:Number})],Mi.prototype,"skidding",2),Dt([Kt({type:Boolean})],Mi.prototype,"arrow",2),Dt([Kt({type:Number})],Mi.prototype,"arrowPadding",2),Dt([Kt({type:Boolean})],Mi.prototype,"flip",2),Dt([Kt({attribute:"flip-fallback-placement",converter:{fromAttribute:t=>t.split(" ").map((t=>t.trim())).filter((t=>""!==t)),toAttribute:t=>t.join(" ")}})],Mi.prototype,"flipFallbackPlacement",2),Dt([Kt({attribute:"flip-fallback-strategy"})],Mi.prototype,"flipFallbackStrategy",2),Dt([Kt({type:Object})],Mi.prototype,"flipBoundary",2),Dt([Kt({attribute:"flip-padding",type:Number})],Mi.prototype,"flipPadding",2),Dt([Kt({type:Boolean})],Mi.prototype,"shift",2),Dt([Kt({type:Object})],Mi.prototype,"shiftBoundary",2),Dt([Kt({attribute:"shift-padding",type:Number})],Mi.prototype,"shiftPadding",2),Dt([Kt({attribute:"auto-size",type:Boolean})],Mi.prototype,"autoSize",2),Dt([Kt({type:Object})],Mi.prototype,"autoSizeBoundary",2),Dt([Kt({attribute:"auto-size-padding",type:Number})],Mi.prototype,"autoSizePadding",2),Mi=Dt([qt("sl-popup")],Mi);var Oi=n`
  ${it}

  :host {
    display: block;
  }

  .details {
    border: solid 1px var(--sl-color-neutral-200);
    border-radius: var(--sl-border-radius-medium);
    background-color: var(--sl-color-neutral-0);
    overflow-anchor: none;
  }

  .details--disabled {
    opacity: 0.5;
  }

  .details__header {
    display: flex;
    align-items: center;
    border-radius: inherit;
    padding: var(--sl-spacing-medium);
    user-select: none;
    cursor: pointer;
  }

  .details__header:focus {
    outline: none;
  }

  .details__header:focus-visible {
    outline: var(--sl-focus-ring);
    outline-offset: calc(1px + var(--sl-focus-ring-offset));
  }

  .details--disabled .details__header {
    cursor: not-allowed;
  }

  .details--disabled .details__header:focus-visible {
    outline: none;
    box-shadow: none;
  }

  .details__summary {
    flex: 1 1 auto;
    display: flex;
    align-items: center;
  }

  .details__summary-icon {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
    transition: var(--sl-transition-medium) transform ease;
  }

  .details--open .details__summary-icon {
    transform: rotate(90deg);
  }

  .details__body {
    overflow: hidden;
  }

  .details__content {
    padding: var(--sl-spacing-medium);
  }
`,Fi=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.open=!1,this.disabled=!1}firstUpdated(){this.body.hidden=!this.open,this.body.style.height=this.open?"auto":"0"}async show(){if(!this.open&&!this.disabled)return this.open=!0,Ht(this,"sl-after-show")}async hide(){if(this.open&&!this.disabled)return this.open=!1,Ht(this,"sl-after-hide")}handleSummaryClick(){this.disabled||(this.open?this.hide():this.show(),this.header.focus())}handleSummaryKeyDown(t){"Enter"!==t.key&&" "!==t.key||(t.preventDefault(),this.open?this.hide():this.show()),"ArrowUp"!==t.key&&"ArrowLeft"!==t.key||(t.preventDefault(),this.hide()),"ArrowDown"!==t.key&&"ArrowRight"!==t.key||(t.preventDefault(),this.show())}async handleOpenChange(){if(this.open){Nt(this,"sl-show"),await oe(this.body),this.body.hidden=!1;const{keyframes:t,options:e}=ce(this,"details.show",{dir:this.localize.dir()});await Jt(this.body,re(t,this.body.scrollHeight),e),this.body.style.height="auto",Nt(this,"sl-after-show")}else{Nt(this,"sl-hide"),await oe(this.body);const{keyframes:t,options:e}=ce(this,"details.hide",{dir:this.localize.dir()});await Jt(this.body,re(t,this.body.scrollHeight),e),this.body.hidden=!0,this.body.style.height="auto",Nt(this,"sl-after-hide")}}render(){return O`
      <div
        part="base"
        class=${Vt({details:!0,"details--open":this.open,"details--disabled":this.disabled})}
      >
        <header
          part="header"
          id="header"
          class="details__header"
          role="button"
          aria-expanded=${this.open?"true":"false"}
          aria-controls="content"
          aria-disabled=${this.disabled?"true":"false"}
          tabindex=${this.disabled?"-1":"0"}
          @click=${this.handleSummaryClick}
          @keydown=${this.handleSummaryKeyDown}
        >
          <div part="summary" class="details__summary">
            <slot name="summary">${this.summary}</slot>
          </div>

          <span part="summary-icon" class="details__summary-icon">
            <sl-icon name="chevron-right" library="system"></sl-icon>
          </span>
        </header>

        <div class="details__body">
          <div part="content" id="content" class="details__content" role="region" aria-labelledby="header">
            <slot></slot>
          </div>
        </div>
      </div>
    `}};Fi.styles=Oi,Dt([Gt(".details")],Fi.prototype,"details",2),Dt([Gt(".details__header")],Fi.prototype,"header",2),Dt([Gt(".details__body")],Fi.prototype,"body",2),Dt([Kt({type:Boolean,reflect:!0})],Fi.prototype,"open",2),Dt([Kt()],Fi.prototype,"summary",2),Dt([Kt({type:Boolean,reflect:!0})],Fi.prototype,"disabled",2),Dt([Ut("open",{waitUntilFirstUpdate:!0})],Fi.prototype,"handleOpenChange",1),Fi=Dt([qt("sl-details")],Fi),le("details.show",{keyframes:[{height:"0",opacity:"0"},{height:"auto",opacity:"1"}],options:{duration:250,easing:"linear"}}),le("details.hide",{keyframes:[{height:"auto",opacity:"1"},{height:"0",opacity:"0"}],options:{duration:250,easing:"linear"}});var Ii=n`
  ${it}

  :host {
    --width: 31rem;
    --header-spacing: var(--sl-spacing-large);
    --body-spacing: var(--sl-spacing-large);
    --footer-spacing: var(--sl-spacing-large);

    display: contents;
  }

  .dialog {
    display: flex;
    align-items: center;
    justify-content: center;
    position: fixed;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    z-index: var(--sl-z-index-dialog);
  }

  .dialog__panel {
    display: flex;
    flex-direction: column;
    z-index: 2;
    width: var(--width);
    max-width: calc(100% - var(--sl-spacing-2x-large));
    max-height: calc(100% - var(--sl-spacing-2x-large));
    background-color: var(--sl-panel-background-color);
    border-radius: var(--sl-border-radius-medium);
    box-shadow: var(--sl-shadow-x-large);
  }

  .dialog__panel:focus {
    outline: none;
  }

  /* Ensure there's enough vertical padding for phones that don't update vh when chrome appears (e.g. iPhone) */
  @media screen and (max-width: 420px) {
    .dialog__panel {
      max-height: 80vh;
    }
  }

  .dialog--open .dialog__panel {
    display: flex;
    opacity: 1;
    transform: none;
  }

  .dialog__header {
    flex: 0 0 auto;
    display: flex;
  }

  .dialog__title {
    flex: 1 1 auto;
    font: inherit;
    font-size: var(--sl-font-size-large);
    line-height: var(--sl-line-height-dense);
    padding: var(--header-spacing);
    margin: 0;
  }

  .dialog__close {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
    font-size: var(--sl-font-size-x-large);
    padding: 0 var(--header-spacing);
  }

  .dialog__body {
    flex: 1 1 auto;
    padding: var(--body-spacing);
    overflow: auto;
    -webkit-overflow-scrolling: touch;
  }

  .dialog__footer {
    flex: 0 0 auto;
    text-align: right;
    padding: var(--footer-spacing);
  }

  .dialog__footer ::slotted(sl-button:not(:first-of-type)) {
    margin-inline-start: var(--sl-spacing-x-small);
  }

  .dialog:not(.dialog--has-footer) .dialog__footer {
    display: none;
  }

  .dialog__overlay {
    position: fixed;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    background-color: var(--sl-overlay-background-color);
  }
`,Bi=class extends et{constructor(){super(...arguments),this.hasSlotController=new Bt(this,"footer"),this.localize=new ge(this),this.open=!1,this.label="",this.noHeader=!1}connectedCallback(){super.connectedCallback(),this.modal=new hr(this)}firstUpdated(){this.dialog.hidden=!this.open,this.open&&(this.modal.activate(),Me(this))}disconnectedCallback(){super.disconnectedCallback(),Oe(this)}async show(){if(!this.open)return this.open=!0,Ht(this,"sl-after-show")}async hide(){if(this.open)return this.open=!1,Ht(this,"sl-after-hide")}requestClose(t){if(Nt(this,"sl-request-close",{cancelable:!0,detail:{source:t}}).defaultPrevented){const t=ce(this,"dialog.denyClose",{dir:this.localize.dir()});Jt(this.panel,t.keyframes,t.options)}else this.hide()}handleKeyDown(t){"Escape"===t.key&&(t.stopPropagation(),this.requestClose("keyboard"))}async handleOpenChange(){if(this.open){Nt(this,"sl-show"),this.originalTrigger=document.activeElement,this.modal.activate(),Me(this);const t=this.querySelector("[autofocus]");t&&t.removeAttribute("autofocus"),await Promise.all([oe(this.dialog),oe(this.overlay)]),this.dialog.hidden=!1,requestAnimationFrame((()=>{Nt(this,"sl-initial-focus",{cancelable:!0}).defaultPrevented||(t?t.focus({preventScroll:!0}):this.panel.focus({preventScroll:!0})),t&&t.setAttribute("autofocus","")}));const e=ce(this,"dialog.show",{dir:this.localize.dir()}),o=ce(this,"dialog.overlay.show",{dir:this.localize.dir()});await Promise.all([Jt(this.panel,e.keyframes,e.options),Jt(this.overlay,o.keyframes,o.options)]),Nt(this,"sl-after-show")}else{Nt(this,"sl-hide"),this.modal.deactivate(),await Promise.all([oe(this.dialog),oe(this.overlay)]);const t=ce(this,"dialog.hide",{dir:this.localize.dir()}),e=ce(this,"dialog.overlay.hide",{dir:this.localize.dir()});await Promise.all([Jt(this.panel,t.keyframes,t.options),Jt(this.overlay,e.keyframes,e.options)]),this.dialog.hidden=!0,Oe(this);const o=this.originalTrigger;"function"==typeof(null==o?void 0:o.focus)&&setTimeout((()=>o.focus())),Nt(this,"sl-after-hide")}}render(){return O`
      <div
        part="base"
        class=${Vt({dialog:!0,"dialog--open":this.open,"dialog--has-footer":this.hasSlotController.test("footer")})}
        @keydown=${this.handleKeyDown}
      >
        <div part="overlay" class="dialog__overlay" @click=${()=>this.requestClose("overlay")} tabindex="-1"></div>

        <div
          part="panel"
          class="dialog__panel"
          role="dialog"
          aria-modal="true"
          aria-hidden=${this.open?"false":"true"}
          aria-label=${Rt(this.noHeader?this.label:void 0)}
          aria-labelledby=${Rt(this.noHeader?void 0:"title")}
          tabindex="0"
        >
          ${this.noHeader?"":O`
                <header part="header" class="dialog__header">
                  <h2 part="title" class="dialog__title" id="title">
                    <slot name="label"> ${this.label.length>0?this.label:String.fromCharCode(65279)} </slot>
                  </h2>
                  <sl-icon-button
                    part="close-button"
                    exportparts="base:close-button__base"
                    class="dialog__close"
                    name="x"
                    label=${this.localize.term("close")}
                    library="system"
                    @click="${()=>this.requestClose("close-button")}"
                  ></sl-icon-button>
                </header>
              `}

          <div part="body" class="dialog__body">
            <slot></slot>
          </div>

          <footer part="footer" class="dialog__footer">
            <slot name="footer"></slot>
          </footer>
        </div>
      </div>
    `}};Bi.styles=Ii,Dt([Gt(".dialog")],Bi.prototype,"dialog",2),Dt([Gt(".dialog__panel")],Bi.prototype,"panel",2),Dt([Gt(".dialog__overlay")],Bi.prototype,"overlay",2),Dt([Kt({type:Boolean,reflect:!0})],Bi.prototype,"open",2),Dt([Kt({reflect:!0})],Bi.prototype,"label",2),Dt([Kt({attribute:"no-header",type:Boolean,reflect:!0})],Bi.prototype,"noHeader",2),Dt([Ut("open",{waitUntilFirstUpdate:!0})],Bi.prototype,"handleOpenChange",1),Bi=Dt([qt("sl-dialog")],Bi),le("dialog.show",{keyframes:[{opacity:0,transform:"scale(0.8)"},{opacity:1,transform:"scale(1)"}],options:{duration:250,easing:"ease"}}),le("dialog.hide",{keyframes:[{opacity:1,transform:"scale(1)"},{opacity:0,transform:"scale(0.8)"}],options:{duration:250,easing:"ease"}}),le("dialog.denyClose",{keyframes:[{transform:"scale(1)"},{transform:"scale(1.02)"},{transform:"scale(1)"}],options:{duration:250}}),le("dialog.overlay.show",{keyframes:[{opacity:0},{opacity:1}],options:{duration:250}}),le("dialog.overlay.hide",{keyframes:[{opacity:1},{opacity:0}],options:{duration:250}});var Pi=n`
  ${it}

  :host {
    --color: var(--sl-panel-border-color);
    --width: var(--sl-panel-border-width);
    --spacing: var(--sl-spacing-medium);
  }

  :host(:not([vertical])) {
    display: block;
    border-top: solid var(--width) var(--color);
    margin: var(--spacing) 0;
  }

  :host([vertical]) {
    display: inline-block;
    height: 100%;
    border-left: solid var(--width) var(--color);
    margin: 0 var(--spacing);
  }
`,Vi=class extends et{constructor(){super(...arguments),this.vertical=!1}connectedCallback(){super.connectedCallback(),this.setAttribute("role","separator")}handleVerticalChange(){this.setAttribute("aria-orientation",this.vertical?"vertical":"horizontal")}};Vi.styles=Pi,Dt([Kt({type:Boolean,reflect:!0})],Vi.prototype,"vertical",2),Dt([Ut("vertical")],Vi.prototype,"handleVerticalChange",1),Vi=Dt([qt("sl-divider")],Vi);var Ri=class extends et{constructor(){super(...arguments),this.formSubmitController=new It(this,{form:t=>{if(t.hasAttribute("form")){const e=t.getRootNode(),o=t.getAttribute("form");return e.getElementById(o)}return t.closest("form")}}),this.hasSlotController=new Bt(this,"[default]","prefix","suffix"),this.localize=new ge(this),this.hasFocus=!1,this.variant="default",this.size="medium",this.caret=!1,this.disabled=!1,this.loading=!1,this.outline=!1,this.pill=!1,this.circle=!1,this.type="button"}click(){this.button.click()}focus(t){this.button.focus(t)}blur(){this.button.blur()}handleBlur(){this.hasFocus=!1,Nt(this,"sl-blur")}handleFocus(){this.hasFocus=!0,Nt(this,"sl-focus")}handleClick(t){if(this.disabled||this.loading)return t.preventDefault(),void t.stopPropagation();"submit"===this.type&&this.formSubmitController.submit(this),"reset"===this.type&&this.formSubmitController.reset(this)}render(){const t=!!this.href,e=t?no`a`:no`button`;return ho`
      <${e}
        part="base"
        class=${Vt({button:!0,"button--default":"default"===this.variant,"button--primary":"primary"===this.variant,"button--success":"success"===this.variant,"button--neutral":"neutral"===this.variant,"button--warning":"warning"===this.variant,"button--danger":"danger"===this.variant,"button--text":"text"===this.variant,"button--small":"small"===this.size,"button--medium":"medium"===this.size,"button--large":"large"===this.size,"button--caret":this.caret,"button--circle":this.circle,"button--disabled":this.disabled,"button--focused":this.hasFocus,"button--loading":this.loading,"button--standard":!this.outline,"button--outline":this.outline,"button--pill":this.pill,"button--rtl":"rtl"===this.localize.dir(),"button--has-label":this.hasSlotController.test("[default]"),"button--has-prefix":this.hasSlotController.test("prefix"),"button--has-suffix":this.hasSlotController.test("suffix")})}
        ?disabled=${Rt(t?void 0:this.disabled)}
        type=${Rt(t?void 0:this.type)}
        name=${Rt(t?void 0:this.name)}
        value=${Rt(t?void 0:this.value)}
        href=${Rt(t?this.href:void 0)}
        target=${Rt(t?this.target:void 0)}
        download=${Rt(t?this.download:void 0)}
        rel=${Rt(t&&this.target?"noreferrer noopener":void 0)}
        role=${Rt(t?void 0:"button")}
        aria-disabled=${this.disabled?"true":"false"}
        tabindex=${this.disabled?"-1":"0"}
        @blur=${this.handleBlur}
        @focus=${this.handleFocus}
        @click=${this.handleClick}
      >
        <span part="prefix" class="button__prefix">
          <slot name="prefix"></slot>
        </span>
        <span part="label" class="button__label">
          <slot></slot>
        </span>
        <span part="suffix" class="button__suffix">
          <slot name="suffix"></slot>
        </span>
        ${this.caret?ho`
                <span part="caret" class="button__caret">
                  <svg
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  >
                    <polyline points="6 9 12 15 18 9"></polyline>
                  </svg>
                </span>
              `:""}
        ${this.loading?ho`<sl-spinner></sl-spinner>`:""}
      </${e}>
    `}};Ri.styles=ro,Dt([Gt(".button")],Ri.prototype,"button",2),Dt([Xt()],Ri.prototype,"hasFocus",2),Dt([Kt({reflect:!0})],Ri.prototype,"variant",2),Dt([Kt({reflect:!0})],Ri.prototype,"size",2),Dt([Kt({type:Boolean,reflect:!0})],Ri.prototype,"caret",2),Dt([Kt({type:Boolean,reflect:!0})],Ri.prototype,"disabled",2),Dt([Kt({type:Boolean,reflect:!0})],Ri.prototype,"loading",2),Dt([Kt({type:Boolean,reflect:!0})],Ri.prototype,"outline",2),Dt([Kt({type:Boolean,reflect:!0})],Ri.prototype,"pill",2),Dt([Kt({type:Boolean,reflect:!0})],Ri.prototype,"circle",2),Dt([Kt()],Ri.prototype,"type",2),Dt([Kt()],Ri.prototype,"name",2),Dt([Kt()],Ri.prototype,"value",2),Dt([Kt()],Ri.prototype,"href",2),Dt([Kt()],Ri.prototype,"target",2),Dt([Kt()],Ri.prototype,"download",2),Dt([Kt()],Ri.prototype,"form",2),Dt([Kt({attribute:"formaction"})],Ri.prototype,"formAction",2),Dt([Kt({attribute:"formmethod"})],Ri.prototype,"formMethod",2),Dt([Kt({attribute:"formnovalidate",type:Boolean})],Ri.prototype,"formNoValidate",2),Dt([Kt({attribute:"formtarget"})],Ri.prototype,"formTarget",2),Ri=Dt([qt("sl-button")],Ri);var Ui=n`
  ${it}

  :host {
    --track-width: 2px;
    --track-color: rgb(128 128 128 / 25%);
    --indicator-color: var(--sl-color-primary-600);
    --speed: 2s;

    display: inline-flex;
    width: 1em;
    height: 1em;
  }

  .spinner {
    flex: 1 1 auto;
    height: 100%;
    width: 100%;
  }

  .spinner__track,
  .spinner__indicator {
    fill: none;
    stroke-width: var(--track-width);
    r: calc(0.5em - var(--track-width) / 2);
    cx: 0.5em;
    cy: 0.5em;
    transform-origin: 50% 50%;
  }

  .spinner__track {
    stroke: var(--track-color);
    transform-origin: 0% 0%;
    mix-blend-mode: multiply;
  }

  .spinner__indicator {
    stroke: var(--indicator-color);
    stroke-linecap: round;
    stroke-dasharray: 150% 75%;
    animation: spin var(--speed) linear infinite;
  }

  @keyframes spin {
    0% {
      transform: rotate(0deg);
      stroke-dasharray: 0.01em, 2.75em;
    }

    50% {
      transform: rotate(450deg);
      stroke-dasharray: 1.375em, 1.375em;
    }

    100% {
      transform: rotate(1080deg);
      stroke-dasharray: 0.01em, 2.75em;
    }
  }
`,Ni=class extends et{render(){return O`
      <svg part="base" class="spinner" role="status">
        <circle class="spinner__track"></circle>
        <circle class="spinner__indicator"></circle>
      </svg>
    `}};Ni.styles=Ui,Ni=Dt([qt("sl-spinner")],Ni);var Hi=n`
  ${it}

  :host {
    display: inline-block;
  }

  .button-group {
    display: flex;
    flex-wrap: nowrap;
  }
`,qi=class extends et{constructor(){super(...arguments),this.disableRole=!1,this.label=""}handleFocus(t){const e=ji(t.target);null==e||e.classList.add("sl-button-group__button--focus")}handleBlur(t){const e=ji(t.target);null==e||e.classList.remove("sl-button-group__button--focus")}handleMouseOver(t){const e=ji(t.target);null==e||e.classList.add("sl-button-group__button--hover")}handleMouseOut(t){const e=ji(t.target);null==e||e.classList.remove("sl-button-group__button--hover")}handleSlotChange(){const t=[...this.defaultSlot.assignedElements({flatten:!0})];t.forEach((e=>{const o=t.indexOf(e),r=ji(e);null!==r&&(r.classList.add("sl-button-group__button"),r.classList.toggle("sl-button-group__button--first",0===o),r.classList.toggle("sl-button-group__button--inner",o>0&&o<t.length-1),r.classList.toggle("sl-button-group__button--last",o===t.length-1),r.classList.toggle("sl-button-group__button--radio","sl-radio-button"===r.tagName.toLowerCase()))}))}render(){return O`
      <div
        part="base"
        class="button-group"
        role="${this.disableRole?"presentation":"group"}"
        aria-label=${this.label}
        @focusout=${this.handleBlur}
        @focusin=${this.handleFocus}
        @mouseover=${this.handleMouseOver}
        @mouseout=${this.handleMouseOut}
      >
        <slot @slotchange=${this.handleSlotChange} role="none"></slot>
      </div>
    `}};function ji(t){const e=["sl-button","sl-radio-button"];return e.includes(t.tagName.toLowerCase())?t:t.querySelector(e.join(","))}qi.styles=Hi,Dt([Gt("slot")],qi.prototype,"defaultSlot",2),Dt([Xt()],qi.prototype,"disableRole",2),Dt([Kt()],qi.prototype,"label",2),qi=Dt([qt("sl-button-group")],qi);var Ki=n`
  ${it}

  :host {
    --border-color: var(--sl-color-neutral-200);
    --border-radius: var(--sl-border-radius-medium);
    --border-width: 1px;
    --padding: var(--sl-spacing-large);

    display: inline-block;
  }

  .card {
    display: flex;
    flex-direction: column;
    background-color: var(--sl-panel-background-color);
    box-shadow: var(--sl-shadow-x-small);
    border: solid var(--border-width) var(--border-color);
    border-radius: var(--border-radius);
  }

  .card__image {
    border-top-left-radius: var(--border-radius);
    border-top-right-radius: var(--border-radius);
    margin: calc(-1 * var(--border-width));
    overflow: hidden;
  }

  .card__image ::slotted(img) {
    display: block;
    width: 100%;
  }

  .card:not(.card--has-image) .card__image {
    display: none;
  }

  .card__header {
    border-bottom: solid var(--border-width) var(--border-color);
    padding: calc(var(--padding) / 2) var(--padding);
  }

  .card:not(.card--has-header) .card__header {
    display: none;
  }

  .card__body {
    padding: var(--padding);
  }

  .card--has-footer .card__footer {
    border-top: solid var(--border-width) var(--border-color);
    padding: var(--padding);
  }

  .card:not(.card--has-footer) .card__footer {
    display: none;
  }
`,Xi=class extends et{constructor(){super(...arguments),this.hasSlotController=new Bt(this,"footer","header","image")}render(){return O`
      <div
        part="base"
        class=${Vt({card:!0,"card--has-footer":this.hasSlotController.test("footer"),"card--has-image":this.hasSlotController.test("image"),"card--has-header":this.hasSlotController.test("header")})}
      >
        <div part="image" class="card__image">
          <slot name="image"></slot>
        </div>

        <div part="header" class="card__header">
          <slot name="header"></slot>
        </div>

        <div part="body" class="card__body">
          <slot></slot>
        </div>

        <div part="footer" class="card__footer">
          <slot name="footer"></slot>
        </div>
      </div>
    `}};Xi.styles=Ki,Xi=Dt([qt("sl-card")],Xi);var Wi=n`
  ${it}

  :host {
    display: inline-block;
  }

  .checkbox {
    display: inline-flex;
    align-items: top;
    font-family: var(--sl-input-font-family);
    font-size: var(--sl-input-font-size-medium);
    font-weight: var(--sl-input-font-weight);
    color: var(--sl-input-color);
    vertical-align: middle;
    cursor: pointer;
  }

  .checkbox__control {
    flex: 0 0 auto;
    position: relative;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: var(--sl-toggle-size);
    height: var(--sl-toggle-size);
    border: solid var(--sl-input-border-width) var(--sl-input-border-color);
    border-radius: 2px;
    background-color: var(--sl-input-background-color);
    color: var(--sl-color-neutral-0);
    transition: var(--sl-transition-fast) border-color, var(--sl-transition-fast) background-color,
      var(--sl-transition-fast) color, var(--sl-transition-fast) box-shadow;
  }

  .checkbox__input {
    position: absolute;
    opacity: 0;
    padding: 0;
    margin: 0;
    pointer-events: none;
  }

  .checkbox__control .checkbox__icon {
    display: inline-flex;
    width: var(--sl-toggle-size);
    height: var(--sl-toggle-size);
  }

  .checkbox__control .checkbox__icon svg {
    width: 100%;
    height: 100%;
  }

  /* Hover */
  .checkbox:not(.checkbox--checked):not(.checkbox--disabled) .checkbox__control:hover {
    border-color: var(--sl-input-border-color-hover);
    background-color: var(--sl-input-background-color-hover);
  }

  /* Focus */
  .checkbox:not(.checkbox--checked):not(.checkbox--disabled) .checkbox__input:focus-visible ~ .checkbox__control {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  /* Checked/indeterminate */
  .checkbox--checked .checkbox__control,
  .checkbox--indeterminate .checkbox__control {
    border-color: var(--sl-color-primary-600);
    background-color: var(--sl-color-primary-600);
  }

  /* Checked/indeterminate + hover */
  .checkbox.checkbox--checked:not(.checkbox--disabled) .checkbox__control:hover,
  .checkbox.checkbox--indeterminate:not(.checkbox--disabled) .checkbox__control:hover {
    border-color: var(--sl-color-primary-500);
    background-color: var(--sl-color-primary-500);
  }

  /* Checked/indeterminate + focus */
  .checkbox.checkbox--checked:not(.checkbox--disabled) .checkbox__input:focus-visible ~ .checkbox__control,
  .checkbox.checkbox--indeterminate:not(.checkbox--disabled) .checkbox__input:focus-visible ~ .checkbox__control {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  /* Disabled */
  .checkbox--disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .checkbox__label {
    color: var(--sl-input-label-color);
    line-height: var(--sl-toggle-size);
    margin-inline-start: 0.5em;
    user-select: none;
  }

  :host([required]) .checkbox__label::after {
    content: var(--sl-input-required-content);
    margin-inline-start: var(--sl-input-required-content-offset);
  }
`,Yi=class extends et{constructor(){super(...arguments),this.formSubmitController=new It(this,{value:t=>t.checked?t.value||"on":void 0,defaultValue:t=>t.defaultChecked,setValue:(t,e)=>t.checked=e}),this.hasFocus=!1,this.disabled=!1,this.required=!1,this.checked=!1,this.indeterminate=!1,this.invalid=!1,this.defaultChecked=!1}firstUpdated(){this.invalid=!this.input.checkValidity()}click(){this.input.click()}focus(t){this.input.focus(t)}blur(){this.input.blur()}reportValidity(){return this.input.reportValidity()}setCustomValidity(t){this.input.setCustomValidity(t),this.invalid=!this.input.checkValidity()}handleClick(){this.checked=!this.checked,this.indeterminate=!1,Nt(this,"sl-change")}handleBlur(){this.hasFocus=!1,Nt(this,"sl-blur")}handleDisabledChange(){this.input.disabled=this.disabled,this.invalid=!this.input.checkValidity()}handleFocus(){this.hasFocus=!0,Nt(this,"sl-focus")}handleStateChange(){this.invalid=!this.input.checkValidity()}render(){return O`
      <label
        part="base"
        class=${Vt({checkbox:!0,"checkbox--checked":this.checked,"checkbox--disabled":this.disabled,"checkbox--focused":this.hasFocus,"checkbox--indeterminate":this.indeterminate})}
      >
        <input
          class="checkbox__input"
          type="checkbox"
          name=${Rt(this.name)}
          value=${Rt(this.value)}
          .indeterminate=${ft(this.indeterminate)}
          .checked=${ft(this.checked)}
          .disabled=${this.disabled}
          .required=${this.required}
          aria-checked=${this.checked?"true":"false"}
          @click=${this.handleClick}
          @blur=${this.handleBlur}
          @focus=${this.handleFocus}
        />

        <span part="control" class="checkbox__control">
          ${this.checked?O`
                <svg part="checked-icon" class="checkbox__icon" viewBox="0 0 16 16">
                  <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd" stroke-linecap="round">
                    <g stroke="currentColor" stroke-width="2">
                      <g transform="translate(3.428571, 3.428571)">
                        <path d="M0,5.71428571 L3.42857143,9.14285714"></path>
                        <path d="M9.14285714,0 L3.42857143,9.14285714"></path>
                      </g>
                    </g>
                  </g>
                </svg>
              `:""}
          ${!this.checked&&this.indeterminate?O`
                <svg part="indeterminate-icon" class="checkbox__icon" viewBox="0 0 16 16">
                  <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd" stroke-linecap="round">
                    <g stroke="currentColor" stroke-width="2">
                      <g transform="translate(2.285714, 6.857143)">
                        <path d="M10.2857143,1.14285714 L1.14285714,1.14285714"></path>
                      </g>
                    </g>
                  </g>
                </svg>
              `:""}
        </span>

        <span part="label" class="checkbox__label">
          <slot></slot>
        </span>
      </label>
    `}};Yi.styles=Wi,Dt([Gt('input[type="checkbox"]')],Yi.prototype,"input",2),Dt([Xt()],Yi.prototype,"hasFocus",2),Dt([Kt()],Yi.prototype,"name",2),Dt([Kt()],Yi.prototype,"value",2),Dt([Kt({type:Boolean,reflect:!0})],Yi.prototype,"disabled",2),Dt([Kt({type:Boolean,reflect:!0})],Yi.prototype,"required",2),Dt([Kt({type:Boolean,reflect:!0})],Yi.prototype,"checked",2),Dt([Kt({type:Boolean,reflect:!0})],Yi.prototype,"indeterminate",2),Dt([Kt({type:Boolean,reflect:!0})],Yi.prototype,"invalid",2),Dt([mt("checked")],Yi.prototype,"defaultChecked",2),Dt([Ut("disabled",{waitUntilFirstUpdate:!0})],Yi.prototype,"handleDisabledChange",1),Dt([Ut("checked",{waitUntilFirstUpdate:!0}),Ut("indeterminate",{waitUntilFirstUpdate:!0})],Yi.prototype,"handleStateChange",1),Yi=Dt([qt("sl-checkbox")],Yi);var Gi=n`
  ${it}

  :host {
    display: inline-flex;
  }

  .badge {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    font-size: var(--sl-font-size-x-small);
    font-weight: var(--sl-font-weight-semibold);
    letter-spacing: var(--sl-letter-spacing-normal);
    line-height: 1;
    border-radius: var(--sl-border-radius-small);
    border: solid 1px var(--sl-color-neutral-0);
    white-space: nowrap;
    padding: 3px 6px;
    user-select: none;
    cursor: inherit;
  }

  /* Variant modifiers */
  .badge--primary {
    background-color: var(--sl-color-primary-600);
    color: var(--sl-color-neutral-0);
  }

  .badge--success {
    background-color: var(--sl-color-success-600);
    color: var(--sl-color-neutral-0);
  }

  .badge--neutral {
    background-color: var(--sl-color-neutral-600);
    color: var(--sl-color-neutral-0);
  }

  .badge--warning {
    background-color: var(--sl-color-warning-600);
    color: var(--sl-color-neutral-0);
  }

  .badge--danger {
    background-color: var(--sl-color-danger-600);
    color: var(--sl-color-neutral-0);
  }

  /* Pill modifier */
  .badge--pill {
    border-radius: var(--sl-border-radius-pill);
  }

  /* Pulse modifier */
  .badge--pulse {
    animation: pulse 1.5s infinite;
  }

  .badge--pulse.badge--primary {
    --pulse-color: var(--sl-color-primary-600);
  }

  .badge--pulse.badge--success {
    --pulse-color: var(--sl-color-success-600);
  }

  .badge--pulse.badge--neutral {
    --pulse-color: var(--sl-color-neutral-600);
  }

  .badge--pulse.badge--warning {
    --pulse-color: var(--sl-color-warning-600);
  }

  .badge--pulse.badge--danger {
    --pulse-color: var(--sl-color-danger-600);
  }

  @keyframes pulse {
    0% {
      box-shadow: 0 0 0 0 var(--pulse-color);
    }
    70% {
      box-shadow: 0 0 0 0.5rem transparent;
    }
    100% {
      box-shadow: 0 0 0 0 transparent;
    }
  }
`,Zi=class extends et{constructor(){super(...arguments),this.variant="primary",this.pill=!1,this.pulse=!1}render(){return O`
      <span
        part="base"
        class=${Vt({badge:!0,"badge--primary":"primary"===this.variant,"badge--success":"success"===this.variant,"badge--neutral":"neutral"===this.variant,"badge--warning":"warning"===this.variant,"badge--danger":"danger"===this.variant,"badge--pill":this.pill,"badge--pulse":this.pulse})}
        role="status"
      >
        <slot></slot>
      </span>
    `}};Zi.styles=Gi,Dt([Kt({reflect:!0})],Zi.prototype,"variant",2),Dt([Kt({type:Boolean,reflect:!0})],Zi.prototype,"pill",2),Dt([Kt({type:Boolean,reflect:!0})],Zi.prototype,"pulse",2),Zi=Dt([qt("sl-badge")],Zi);var Qi=n`
  ${it}

  .breadcrumb {
    display: flex;
    align-items: center;
    flex-wrap: wrap;
  }
`,Ji=class extends et{constructor(){super(...arguments),this.localize=new ge(this),this.separatorDir=this.localize.dir(),this.label="Breadcrumb"}getSeparator(){const t=this.separatorSlot.assignedElements({flatten:!0})[0].cloneNode(!0);return[t,...t.querySelectorAll("[id]")].forEach((t=>t.removeAttribute("id"))),t.setAttribute("data-default",""),t.slot="separator",t}handleSlotChange(){const t=[...this.defaultSlot.assignedElements({flatten:!0})].filter((t=>"sl-breadcrumb-item"===t.tagName.toLowerCase()));t.forEach(((e,o)=>{const r=e.querySelector('[slot="separator"]');null===r?e.append(this.getSeparator()):r.hasAttribute("data-default")&&r.replaceWith(this.getSeparator()),o===t.length-1?e.setAttribute("aria-current","page"):e.removeAttribute("aria-current")}))}render(){return this.separatorDir!==this.localize.dir()&&(this.separatorDir=this.localize.dir(),this.updateComplete.then((()=>this.handleSlotChange()))),O`
      <nav part="base" class="breadcrumb" aria-label=${this.label}>
        <slot @slotchange=${this.handleSlotChange}></slot>
      </nav>

      <slot name="separator" hidden aria-hidden="true">
        <sl-icon name=${"rtl"===this.localize.dir()?"chevron-left":"chevron-right"} library="system"></sl-icon>
      </slot>
    `}};Ji.styles=Qi,Dt([Gt("slot")],Ji.prototype,"defaultSlot",2),Dt([Gt('slot[name="separator"]')],Ji.prototype,"separatorSlot",2),Dt([Kt()],Ji.prototype,"label",2),Ji=Dt([qt("sl-breadcrumb")],Ji);var ta=n`
  ${it}

  :host {
    display: inline-flex;
  }

  .breadcrumb-item {
    display: inline-flex;
    align-items: center;
    font-family: var(--sl-font-sans);
    font-size: var(--sl-font-size-small);
    font-weight: var(--sl-font-weight-semibold);
    color: var(--sl-color-neutral-600);
    line-height: var(--sl-line-height-normal);
    white-space: nowrap;
  }

  .breadcrumb-item__label {
    display: inline-block;
    font-family: inherit;
    font-size: inherit;
    font-weight: inherit;
    line-height: inherit;
    text-decoration: none;
    color: inherit;
    background: none;
    border: none;
    border-radius: var(--sl-border-radius-medium);
    padding: 0;
    margin: 0;
    cursor: pointer;
    transition: var(--sl-transition-fast) --color;
  }

  :host(:not(:last-of-type)) .breadcrumb-item__label {
    color: var(--sl-color-primary-600);
  }

  :host(:not(:last-of-type)) .breadcrumb-item__label:hover {
    color: var(--sl-color-primary-500);
  }

  :host(:not(:last-of-type)) .breadcrumb-item__label:active {
    color: var(--sl-color-primary-600);
  }

  .breadcrumb-item__label:focus {
    outline: none;
  }

  .breadcrumb-item__label:focus-visible {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .breadcrumb-item__prefix,
  .breadcrumb-item__suffix {
    display: none;
    flex: 0 0 auto;
    display: flex;
    align-items: center;
  }

  .breadcrumb-item--has-prefix .breadcrumb-item__prefix {
    display: inline-flex;
    margin-inline-end: var(--sl-spacing-x-small);
  }

  .breadcrumb-item--has-suffix .breadcrumb-item__suffix {
    display: inline-flex;
    margin-inline-start: var(--sl-spacing-x-small);
  }

  :host(:last-of-type) .breadcrumb-item__separator {
    display: none;
  }

  .breadcrumb-item__separator {
    display: inline-flex;
    align-items: center;
    margin: 0 var(--sl-spacing-x-small);
    user-select: none;
  }
`,ea=class extends et{constructor(){super(...arguments),this.hasSlotController=new Bt(this,"prefix","suffix"),this.rel="noreferrer noopener"}render(){const t=!!this.href;return O`
      <div
        part="base"
        class=${Vt({"breadcrumb-item":!0,"breadcrumb-item--has-prefix":this.hasSlotController.test("prefix"),"breadcrumb-item--has-suffix":this.hasSlotController.test("suffix")})}
      >
        <span part="prefix" class="breadcrumb-item__prefix">
          <slot name="prefix"></slot>
        </span>

        ${t?O`
              <a
                part="label"
                class="breadcrumb-item__label breadcrumb-item__label--link"
                href="${this.href}"
                target="${Rt(this.target?this.target:void 0)}"
                rel=${Rt(this.target?this.rel:void 0)}
              >
                <slot></slot>
              </a>
            `:O`
              <button part="label" type="button" class="breadcrumb-item__label breadcrumb-item__label--button">
                <slot></slot>
              </button>
            `}

        <span part="suffix" class="breadcrumb-item__suffix">
          <slot name="suffix"></slot>
        </span>

        <span part="separator" class="breadcrumb-item__separator" aria-hidden="true">
          <slot name="separator"></slot>
        </span>
      </div>
    `}};ea.styles=ta,Dt([Kt()],ea.prototype,"href",2),Dt([Kt()],ea.prototype,"target",2),Dt([Kt()],ea.prototype,"rel",2),ea=Dt([qt("sl-breadcrumb-item")],ea);var oa=n`
  ${it}

  :host {
    display: inline-block;

    --size: 3rem;
  }

  .avatar {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    position: relative;
    width: var(--size);
    height: var(--size);
    background-color: var(--sl-color-neutral-400);
    font-family: var(--sl-font-sans);
    font-size: calc(var(--size) * 0.5);
    font-weight: var(--sl-font-weight-normal);
    color: var(--sl-color-neutral-0);
    user-select: none;
    vertical-align: middle;
  }

  .avatar--circle,
  .avatar--circle .avatar__image {
    border-radius: var(--sl-border-radius-circle);
  }

  .avatar--rounded,
  .avatar--rounded .avatar__image {
    border-radius: var(--sl-border-radius-medium);
  }

  .avatar--square {
    border-radius: 0;
  }

  .avatar__icon {
    display: flex;
    align-items: center;
    justify-content: center;
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
  }

  .avatar__initials {
    line-height: 1;
    text-transform: uppercase;
  }

  .avatar__image {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    object-fit: cover;
    overflow: hidden;
  }
`,ra=class extends et{constructor(){super(...arguments),this.hasError=!1,this.image="",this.label="",this.initials="",this.shape="circle"}handleImageChange(){this.hasError=!1}render(){return O`
      <div
        part="base"
        class=${Vt({avatar:!0,"avatar--circle":"circle"===this.shape,"avatar--rounded":"rounded"===this.shape,"avatar--square":"square"===this.shape})}
        role="img"
        aria-label=${this.label}
      >
        ${this.initials?O` <div part="initials" class="avatar__initials">${this.initials}</div> `:O`
              <div part="icon" class="avatar__icon" aria-hidden="true">
                <slot name="icon">
                  <sl-icon name="person-fill" library="system"></sl-icon>
                </slot>
              </div>
            `}
        ${this.image&&!this.hasError?O`
              <img
                part="image"
                class="avatar__image"
                src="${this.image}"
                alt=""
                @error="${()=>this.hasError=!0}"
              />
            `:""}
      </div>
    `}};ra.styles=oa,Dt([Xt()],ra.prototype,"hasError",2),Dt([Kt()],ra.prototype,"image",2),Dt([Kt()],ra.prototype,"label",2),Dt([Kt()],ra.prototype,"initials",2),Dt([Kt({reflect:!0})],ra.prototype,"shape",2),Dt([Ut("image")],ra.prototype,"handleImageChange",1),ra=Dt([qt("sl-avatar")],ra);var ia=n`
  ${it}

  :host {
    display: contents;

    /* For better DX, we'll reset the margin here so the base part can inherit it */
    margin: 0;
  }

  .alert {
    position: relative;
    display: flex;
    align-items: stretch;
    background-color: var(--sl-panel-background-color);
    border: solid var(--sl-panel-border-width) var(--sl-panel-border-color);
    border-top-width: calc(var(--sl-panel-border-width) * 3);
    border-radius: var(--sl-border-radius-medium);
    box-shadow: var(--box-shadow);
    font-family: var(--sl-font-sans);
    font-size: var(--sl-font-size-small);
    font-weight: var(--sl-font-weight-normal);
    line-height: 1.6;
    color: var(--sl-color-neutral-700);
    margin: inherit;
  }

  .alert:not(.alert--has-icon) .alert__icon,
  .alert:not(.alert--closable) .alert__close-button {
    display: none;
  }

  .alert__icon {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
    font-size: var(--sl-font-size-large);
    padding-inline-start: var(--sl-spacing-large);
  }

  .alert--primary {
    border-top-color: var(--sl-color-primary-600);
  }

  .alert--primary .alert__icon {
    color: var(--sl-color-primary-600);
  }

  .alert--success {
    border-top-color: var(--sl-color-success-600);
  }

  .alert--success .alert__icon {
    color: var(--sl-color-success-600);
  }

  .alert--neutral {
    border-top-color: var(--sl-color-neutral-600);
  }

  .alert--neutral .alert__icon {
    color: var(--sl-color-neutral-600);
  }

  .alert--warning {
    border-top-color: var(--sl-color-warning-600);
  }

  .alert--warning .alert__icon {
    color: var(--sl-color-warning-600);
  }

  .alert--danger {
    border-top-color: var(--sl-color-danger-600);
  }

  .alert--danger .alert__icon {
    color: var(--sl-color-danger-600);
  }

  .alert__message {
    flex: 1 1 auto;
    padding: var(--sl-spacing-large);
    overflow: hidden;
  }

  .alert__close-button {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
    font-size: var(--sl-font-size-large);
    padding-inline-end: var(--sl-spacing-medium);
  }
`,aa=Object.assign(document.createElement("div"),{className:"sl-toast-stack"}),sa=class extends et{constructor(){super(...arguments),this.hasSlotController=new Bt(this,"icon","suffix"),this.localize=new ge(this),this.open=!1,this.closable=!1,this.variant="primary",this.duration=1/0}firstUpdated(){this.base.hidden=!this.open}async show(){if(!this.open)return this.open=!0,Ht(this,"sl-after-show")}async hide(){if(this.open)return this.open=!1,Ht(this,"sl-after-hide")}async toast(){return new Promise((t=>{null===aa.parentElement&&document.body.append(aa),aa.appendChild(this),requestAnimationFrame((()=>{this.clientWidth,this.show()})),this.addEventListener("sl-after-hide",(()=>{aa.removeChild(this),t(),null===aa.querySelector("sl-alert")&&aa.remove()}),{once:!0})}))}restartAutoHide(){clearTimeout(this.autoHideTimeout),this.open&&this.duration<1/0&&(this.autoHideTimeout=window.setTimeout((()=>this.hide()),this.duration))}handleCloseClick(){this.hide()}handleMouseMove(){this.restartAutoHide()}async handleOpenChange(){if(this.open){Nt(this,"sl-show"),this.duration<1/0&&this.restartAutoHide(),await oe(this.base),this.base.hidden=!1;const{keyframes:t,options:e}=ce(this,"alert.show",{dir:this.localize.dir()});await Jt(this.base,t,e),Nt(this,"sl-after-show")}else{Nt(this,"sl-hide"),clearTimeout(this.autoHideTimeout),await oe(this.base);const{keyframes:t,options:e}=ce(this,"alert.hide",{dir:this.localize.dir()});await Jt(this.base,t,e),this.base.hidden=!0,Nt(this,"sl-after-hide")}}handleDurationChange(){this.restartAutoHide()}render(){return O`
      <div
        part="base"
        class=${Vt({alert:!0,"alert--open":this.open,"alert--closable":this.closable,"alert--has-icon":this.hasSlotController.test("icon"),"alert--primary":"primary"===this.variant,"alert--success":"success"===this.variant,"alert--neutral":"neutral"===this.variant,"alert--warning":"warning"===this.variant,"alert--danger":"danger"===this.variant})}
        role="alert"
        aria-live="assertive"
        aria-atomic="true"
        aria-hidden=${this.open?"false":"true"}
        @mousemove=${this.handleMouseMove}
      >
        <span part="icon" class="alert__icon">
          <slot name="icon"></slot>
        </span>

        <span part="message" class="alert__message">
          <slot></slot>
        </span>

        ${this.closable?O`
              <sl-icon-button
                part="close-button"
                exportparts="base:close-button__base"
                class="alert__close-button"
                name="x"
                library="system"
                @click=${this.handleCloseClick}
              ></sl-icon-button>
            `:""}
      </div>
    `}};sa.styles=ia,Dt([Gt('[part="base"]')],sa.prototype,"base",2),Dt([Kt({type:Boolean,reflect:!0})],sa.prototype,"open",2),Dt([Kt({type:Boolean,reflect:!0})],sa.prototype,"closable",2),Dt([Kt({reflect:!0})],sa.prototype,"variant",2),Dt([Kt({type:Number})],sa.prototype,"duration",2),Dt([Ut("open",{waitUntilFirstUpdate:!0})],sa.prototype,"handleOpenChange",1),Dt([Ut("duration")],sa.prototype,"handleDurationChange",1),sa=Dt([qt("sl-alert")],sa),le("alert.show",{keyframes:[{opacity:0,transform:"scale(0.8)"},{opacity:1,transform:"scale(1)"}],options:{duration:250,easing:"ease"}}),le("alert.hide",{keyframes:[{opacity:1,transform:"scale(1)"},{opacity:0,transform:"scale(0.8)"}],options:{duration:250,easing:"ease"}});var na=n`
  ${it}

  :host {
    display: inline-block;
  }

  .icon-button {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
    background: none;
    border: none;
    border-radius: var(--sl-border-radius-medium);
    font-size: inherit;
    color: var(--sl-color-neutral-600);
    padding: var(--sl-spacing-x-small);
    cursor: pointer;
    transition: var(--sl-transition-medium) color;
    -webkit-appearance: none;
  }

  .icon-button:hover:not(.icon-button--disabled),
  .icon-button:focus:not(.icon-button--disabled) {
    color: var(--sl-color-primary-600);
  }

  .icon-button:active:not(.icon-button--disabled) {
    color: var(--sl-color-primary-700);
  }

  .icon-button:focus {
    outline: none;
  }

  .icon-button--disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .icon-button:focus-visible {
    outline: var(--sl-focus-ring);
    outline-offset: var(--sl-focus-ring-offset);
  }

  .icon-button__icon {
    pointer-events: none;
  }
`,la=class extends et{constructor(){super(...arguments),this.hasFocus=!1,this.label="",this.disabled=!1}click(){this.button.click()}focus(t){this.button.focus(t)}blur(){this.button.blur()}handleBlur(){this.hasFocus=!1,Nt(this,"sl-blur")}handleFocus(){this.hasFocus=!0,Nt(this,"sl-focus")}handleClick(t){this.disabled&&(t.preventDefault(),t.stopPropagation())}render(){const t=!!this.href,e=t?no`a`:no`button`;return ho`
      <${e}
        part="base"
        class=${Vt({"icon-button":!0,"icon-button--disabled":!t&&this.disabled,"icon-button--focused":this.hasFocus})}
        ?disabled=${Rt(t?void 0:this.disabled)}
        type=${Rt(t?void 0:"button")}
        href=${Rt(t?this.href:void 0)}
        target=${Rt(t?this.target:void 0)}
        download=${Rt(t?this.download:void 0)}
        rel=${Rt(t&&this.target?"noreferrer noopener":void 0)}
        role=${Rt(t?void 0:"button")}
        aria-disabled=${this.disabled?"true":"false"}
        aria-label="${this.label}"
        tabindex=${this.disabled?"-1":"0"}
        @blur=${this.handleBlur}
        @focus=${this.handleFocus}
        @click=${this.handleClick}
      >
        <sl-icon
          class="icon-button__icon"
          name=${Rt(this.name)}
          library=${Rt(this.library)}
          src=${Rt(this.src)}
          aria-hidden="true"
        ></sl-icon>
      </${e}>
    `}};la.styles=na,Dt([Xt()],la.prototype,"hasFocus",2),Dt([Gt(".icon-button")],la.prototype,"button",2),Dt([Kt()],la.prototype,"name",2),Dt([Kt()],la.prototype,"library",2),Dt([Kt()],la.prototype,"src",2),Dt([Kt()],la.prototype,"href",2),Dt([Kt()],la.prototype,"target",2),Dt([Kt()],la.prototype,"download",2),Dt([Kt()],la.prototype,"label",2),Dt([Kt({type:Boolean,reflect:!0})],la.prototype,"disabled",2),la=Dt([qt("sl-icon-button")],la);var ca=n`
  ${it}

  :host {
    display: contents;
  }
`,da={};((t,e)=>{for(var o in e)gt(t,o,{get:e[o],enumerable:!0})})(da,{backInDown:()=>$a,backInLeft:()=>Ca,backInRight:()=>za,backInUp:()=>Sa,backOutDown:()=>Aa,backOutLeft:()=>Ta,backOutRight:()=>Ea,backOutUp:()=>Da,bounce:()=>ha,bounceIn:()=>La,bounceInDown:()=>Ma,bounceInLeft:()=>Oa,bounceInRight:()=>Fa,bounceInUp:()=>Ia,bounceOut:()=>Ba,bounceOutDown:()=>Pa,bounceOutLeft:()=>Va,bounceOutRight:()=>Ra,bounceOutUp:()=>Ua,easings:()=>en,fadeIn:()=>Na,fadeInBottomLeft:()=>Ha,fadeInBottomRight:()=>qa,fadeInDown:()=>ja,fadeInDownBig:()=>Ka,fadeInLeft:()=>Xa,fadeInLeftBig:()=>Wa,fadeInRight:()=>Ya,fadeInRightBig:()=>Ga,fadeInTopLeft:()=>Za,fadeInTopRight:()=>Qa,fadeInUp:()=>Ja,fadeInUpBig:()=>ts,fadeOut:()=>es,fadeOutBottomLeft:()=>os,fadeOutBottomRight:()=>rs,fadeOutDown:()=>is,fadeOutDownBig:()=>as,fadeOutLeft:()=>ss,fadeOutLeftBig:()=>ns,fadeOutRight:()=>ls,fadeOutRightBig:()=>cs,fadeOutTopLeft:()=>ds,fadeOutTopRight:()=>hs,fadeOutUp:()=>us,fadeOutUpBig:()=>ps,flash:()=>ua,flip:()=>fs,flipInX:()=>ms,flipInY:()=>bs,flipOutX:()=>gs,flipOutY:()=>vs,headShake:()=>pa,heartBeat:()=>fa,hinge:()=>Us,jackInTheBox:()=>Ns,jello:()=>ma,lightSpeedInLeft:()=>ys,lightSpeedInRight:()=>ws,lightSpeedOutLeft:()=>_s,lightSpeedOutRight:()=>xs,pulse:()=>ba,rollIn:()=>Hs,rollOut:()=>qs,rotateIn:()=>ks,rotateInDownLeft:()=>$s,rotateInDownRight:()=>Cs,rotateInUpLeft:()=>zs,rotateInUpRight:()=>Ss,rotateOut:()=>As,rotateOutDownLeft:()=>Ts,rotateOutDownRight:()=>Es,rotateOutUpLeft:()=>Ds,rotateOutUpRight:()=>Ls,rubberBand:()=>ga,shake:()=>va,shakeX:()=>ya,shakeY:()=>wa,slideInDown:()=>Ms,slideInLeft:()=>Os,slideInRight:()=>Fs,slideInUp:()=>Is,slideOutDown:()=>Bs,slideOutLeft:()=>Ps,slideOutRight:()=>Vs,slideOutUp:()=>Rs,swing:()=>_a,tada:()=>xa,wobble:()=>ka,zoomIn:()=>js,zoomInDown:()=>Ks,zoomInLeft:()=>Xs,zoomInRight:()=>Ws,zoomInUp:()=>Ys,zoomOut:()=>Gs,zoomOutDown:()=>Zs,zoomOutLeft:()=>Qs,zoomOutRight:()=>Js,zoomOutUp:()=>tn});var ha=[{offset:0,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)",transform:"translate3d(0, 0, 0)"},{offset:.2,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)",transform:"translate3d(0, 0, 0)"},{offset:.4,easing:"cubic-bezier(0.755, 0.05, 0.855, 0.06)",transform:"translate3d(0, -30px, 0) scaleY(1.1)"},{offset:.43,easing:"cubic-bezier(0.755, 0.05, 0.855, 0.06)",transform:"translate3d(0, -30px, 0) scaleY(1.1)"},{offset:.53,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)",transform:"translate3d(0, 0, 0)"},{offset:.7,easing:"cubic-bezier(0.755, 0.05, 0.855, 0.06)",transform:"translate3d(0, -15px, 0) scaleY(1.05)"},{offset:.8,"transition-timing-function":"cubic-bezier(0.215, 0.61, 0.355, 1)",transform:"translate3d(0, 0, 0) scaleY(0.95)"},{offset:.9,transform:"translate3d(0, -4px, 0) scaleY(1.02)"},{offset:1,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)",transform:"translate3d(0, 0, 0)"}],ua=[{offset:0,opacity:"1"},{offset:.25,opacity:"0"},{offset:.5,opacity:"1"},{offset:.75,opacity:"0"},{offset:1,opacity:"1"}],pa=[{offset:0,transform:"translateX(0)"},{offset:.065,transform:"translateX(-6px) rotateY(-9deg)"},{offset:.185,transform:"translateX(5px) rotateY(7deg)"},{offset:.315,transform:"translateX(-3px) rotateY(-5deg)"},{offset:.435,transform:"translateX(2px) rotateY(3deg)"},{offset:.5,transform:"translateX(0)"}],fa=[{offset:0,transform:"scale(1)"},{offset:.14,transform:"scale(1.3)"},{offset:.28,transform:"scale(1)"},{offset:.42,transform:"scale(1.3)"},{offset:.7,transform:"scale(1)"}],ma=[{offset:0,transform:"translate3d(0, 0, 0)"},{offset:.111,transform:"translate3d(0, 0, 0)"},{offset:.222,transform:"skewX(-12.5deg) skewY(-12.5deg)"},{offset:.33299999999999996,transform:"skewX(6.25deg) skewY(6.25deg)"},{offset:.444,transform:"skewX(-3.125deg) skewY(-3.125deg)"},{offset:.555,transform:"skewX(1.5625deg) skewY(1.5625deg)"},{offset:.6659999999999999,transform:"skewX(-0.78125deg) skewY(-0.78125deg)"},{offset:.777,transform:"skewX(0.390625deg) skewY(0.390625deg)"},{offset:.888,transform:"skewX(-0.1953125deg) skewY(-0.1953125deg)"},{offset:1,transform:"translate3d(0, 0, 0)"}],ba=[{offset:0,transform:"scale3d(1, 1, 1)"},{offset:.5,transform:"scale3d(1.05, 1.05, 1.05)"},{offset:1,transform:"scale3d(1, 1, 1)"}],ga=[{offset:0,transform:"scale3d(1, 1, 1)"},{offset:.3,transform:"scale3d(1.25, 0.75, 1)"},{offset:.4,transform:"scale3d(0.75, 1.25, 1)"},{offset:.5,transform:"scale3d(1.15, 0.85, 1)"},{offset:.65,transform:"scale3d(0.95, 1.05, 1)"},{offset:.75,transform:"scale3d(1.05, 0.95, 1)"},{offset:1,transform:"scale3d(1, 1, 1)"}],va=[{offset:0,transform:"translate3d(0, 0, 0)"},{offset:.1,transform:"translate3d(-10px, 0, 0)"},{offset:.2,transform:"translate3d(10px, 0, 0)"},{offset:.3,transform:"translate3d(-10px, 0, 0)"},{offset:.4,transform:"translate3d(10px, 0, 0)"},{offset:.5,transform:"translate3d(-10px, 0, 0)"},{offset:.6,transform:"translate3d(10px, 0, 0)"},{offset:.7,transform:"translate3d(-10px, 0, 0)"},{offset:.8,transform:"translate3d(10px, 0, 0)"},{offset:.9,transform:"translate3d(-10px, 0, 0)"},{offset:1,transform:"translate3d(0, 0, 0)"}],ya=[{offset:0,transform:"translate3d(0, 0, 0)"},{offset:.1,transform:"translate3d(-10px, 0, 0)"},{offset:.2,transform:"translate3d(10px, 0, 0)"},{offset:.3,transform:"translate3d(-10px, 0, 0)"},{offset:.4,transform:"translate3d(10px, 0, 0)"},{offset:.5,transform:"translate3d(-10px, 0, 0)"},{offset:.6,transform:"translate3d(10px, 0, 0)"},{offset:.7,transform:"translate3d(-10px, 0, 0)"},{offset:.8,transform:"translate3d(10px, 0, 0)"},{offset:.9,transform:"translate3d(-10px, 0, 0)"},{offset:1,transform:"translate3d(0, 0, 0)"}],wa=[{offset:0,transform:"translate3d(0, 0, 0)"},{offset:.1,transform:"translate3d(0, -10px, 0)"},{offset:.2,transform:"translate3d(0, 10px, 0)"},{offset:.3,transform:"translate3d(0, -10px, 0)"},{offset:.4,transform:"translate3d(0, 10px, 0)"},{offset:.5,transform:"translate3d(0, -10px, 0)"},{offset:.6,transform:"translate3d(0, 10px, 0)"},{offset:.7,transform:"translate3d(0, -10px, 0)"},{offset:.8,transform:"translate3d(0, 10px, 0)"},{offset:.9,transform:"translate3d(0, -10px, 0)"},{offset:1,transform:"translate3d(0, 0, 0)"}],_a=[{offset:.2,transform:"rotate3d(0, 0, 1, 15deg)"},{offset:.4,transform:"rotate3d(0, 0, 1, -10deg)"},{offset:.6,transform:"rotate3d(0, 0, 1, 5deg)"},{offset:.8,transform:"rotate3d(0, 0, 1, -5deg)"},{offset:1,transform:"rotate3d(0, 0, 1, 0deg)"}],xa=[{offset:0,transform:"scale3d(1, 1, 1)"},{offset:.1,transform:"scale3d(0.9, 0.9, 0.9) rotate3d(0, 0, 1, -3deg)"},{offset:.2,transform:"scale3d(0.9, 0.9, 0.9) rotate3d(0, 0, 1, -3deg)"},{offset:.3,transform:"scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, 3deg)"},{offset:.4,transform:"scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, -3deg)"},{offset:.5,transform:"scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, 3deg)"},{offset:.6,transform:"scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, -3deg)"},{offset:.7,transform:"scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, 3deg)"},{offset:.8,transform:"scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, -3deg)"},{offset:.9,transform:"scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, 3deg)"},{offset:1,transform:"scale3d(1, 1, 1)"}],ka=[{offset:0,transform:"translate3d(0, 0, 0)"},{offset:.15,transform:"translate3d(-25%, 0, 0) rotate3d(0, 0, 1, -5deg)"},{offset:.3,transform:"translate3d(20%, 0, 0) rotate3d(0, 0, 1, 3deg)"},{offset:.45,transform:"translate3d(-15%, 0, 0) rotate3d(0, 0, 1, -3deg)"},{offset:.6,transform:"translate3d(10%, 0, 0) rotate3d(0, 0, 1, 2deg)"},{offset:.75,transform:"translate3d(-5%, 0, 0) rotate3d(0, 0, 1, -1deg)"},{offset:1,transform:"translate3d(0, 0, 0)"}],$a=[{offset:0,transform:"translateY(-1200px) scale(0.7)",opacity:"0.7"},{offset:.8,transform:"translateY(0px) scale(0.7)",opacity:"0.7"},{offset:1,transform:"scale(1)",opacity:"1"}],Ca=[{offset:0,transform:"translateX(-2000px) scale(0.7)",opacity:"0.7"},{offset:.8,transform:"translateX(0px) scale(0.7)",opacity:"0.7"},{offset:1,transform:"scale(1)",opacity:"1"}],za=[{offset:0,transform:"translateX(2000px) scale(0.7)",opacity:"0.7"},{offset:.8,transform:"translateX(0px) scale(0.7)",opacity:"0.7"},{offset:1,transform:"scale(1)",opacity:"1"}],Sa=[{offset:0,transform:"translateY(1200px) scale(0.7)",opacity:"0.7"},{offset:.8,transform:"translateY(0px) scale(0.7)",opacity:"0.7"},{offset:1,transform:"scale(1)",opacity:"1"}],Aa=[{offset:0,transform:"scale(1)",opacity:"1"},{offset:.2,transform:"translateY(0px) scale(0.7)",opacity:"0.7"},{offset:1,transform:"translateY(700px) scale(0.7)",opacity:"0.7"}],Ta=[{offset:0,transform:"scale(1)",opacity:"1"},{offset:.2,transform:"translateX(0px) scale(0.7)",opacity:"0.7"},{offset:1,transform:"translateX(-2000px) scale(0.7)",opacity:"0.7"}],Ea=[{offset:0,transform:"scale(1)",opacity:"1"},{offset:.2,transform:"translateX(0px) scale(0.7)",opacity:"0.7"},{offset:1,transform:"translateX(2000px) scale(0.7)",opacity:"0.7"}],Da=[{offset:0,transform:"scale(1)",opacity:"1"},{offset:.2,transform:"translateY(0px) scale(0.7)",opacity:"0.7"},{offset:1,transform:"translateY(-700px) scale(0.7)",opacity:"0.7"}],La=[{offset:0,opacity:"0",transform:"scale3d(0.3, 0.3, 0.3)"},{offset:0,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.2,transform:"scale3d(1.1, 1.1, 1.1)"},{offset:.2,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.4,transform:"scale3d(0.9, 0.9, 0.9)"},{offset:.4,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.6,opacity:"1",transform:"scale3d(1.03, 1.03, 1.03)"},{offset:.6,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.8,transform:"scale3d(0.97, 0.97, 0.97)"},{offset:.8,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:1,opacity:"1",transform:"scale3d(1, 1, 1)"},{offset:1,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"}],Ma=[{offset:0,opacity:"0",transform:"translate3d(0, -3000px, 0) scaleY(3)"},{offset:0,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.6,opacity:"1",transform:"translate3d(0, 25px, 0) scaleY(0.9)"},{offset:.6,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.75,transform:"translate3d(0, -10px, 0) scaleY(0.95)"},{offset:.75,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.9,transform:"translate3d(0, 5px, 0) scaleY(0.985)"},{offset:.9,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:1,transform:"translate3d(0, 0, 0)"},{offset:1,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"}],Oa=[{offset:0,opacity:"0",transform:"translate3d(-3000px, 0, 0) scaleX(3)"},{offset:0,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.6,opacity:"1",transform:"translate3d(25px, 0, 0) scaleX(1)"},{offset:.6,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.75,transform:"translate3d(-10px, 0, 0) scaleX(0.98)"},{offset:.75,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.9,transform:"translate3d(5px, 0, 0) scaleX(0.995)"},{offset:.9,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:1,transform:"translate3d(0, 0, 0)"},{offset:1,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"}],Fa=[{offset:0,opacity:"0",transform:"translate3d(3000px, 0, 0) scaleX(3)"},{offset:0,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.6,opacity:"1",transform:"translate3d(-25px, 0, 0) scaleX(1)"},{offset:.6,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.75,transform:"translate3d(10px, 0, 0) scaleX(0.98)"},{offset:.75,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.9,transform:"translate3d(-5px, 0, 0) scaleX(0.995)"},{offset:.9,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:1,transform:"translate3d(0, 0, 0)"},{offset:1,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"}],Ia=[{offset:0,opacity:"0",transform:"translate3d(0, 3000px, 0) scaleY(5)"},{offset:0,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.6,opacity:"1",transform:"translate3d(0, -20px, 0) scaleY(0.9)"},{offset:.6,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.75,transform:"translate3d(0, 10px, 0) scaleY(0.95)"},{offset:.75,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:.9,transform:"translate3d(0, -5px, 0) scaleY(0.985)"},{offset:.9,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"},{offset:1,transform:"translate3d(0, 0, 0)"},{offset:1,easing:"cubic-bezier(0.215, 0.61, 0.355, 1)"}],Ba=[{offset:.2,transform:"scale3d(0.9, 0.9, 0.9)"},{offset:.5,opacity:"1",transform:"scale3d(1.1, 1.1, 1.1)"},{offset:.55,opacity:"1",transform:"scale3d(1.1, 1.1, 1.1)"},{offset:1,opacity:"0",transform:"scale3d(0.3, 0.3, 0.3)"}],Pa=[{offset:.2,transform:"translate3d(0, 10px, 0) scaleY(0.985)"},{offset:.4,opacity:"1",transform:"translate3d(0, -20px, 0) scaleY(0.9)"},{offset:.45,opacity:"1",transform:"translate3d(0, -20px, 0) scaleY(0.9)"},{offset:1,opacity:"0",transform:"translate3d(0, 2000px, 0) scaleY(3)"}],Va=[{offset:.2,opacity:"1",transform:"translate3d(20px, 0, 0) scaleX(0.9)"},{offset:1,opacity:"0",transform:"translate3d(-2000px, 0, 0) scaleX(2)"}],Ra=[{offset:.2,opacity:"1",transform:"translate3d(-20px, 0, 0) scaleX(0.9)"},{offset:1,opacity:"0",transform:"translate3d(2000px, 0, 0) scaleX(2)"}],Ua=[{offset:.2,transform:"translate3d(0, -10px, 0) scaleY(0.985)"},{offset:.4,opacity:"1",transform:"translate3d(0, 20px, 0) scaleY(0.9)"},{offset:.45,opacity:"1",transform:"translate3d(0, 20px, 0) scaleY(0.9)"},{offset:1,opacity:"0",transform:"translate3d(0, -2000px, 0) scaleY(3)"}],Na=[{offset:0,opacity:"0"},{offset:1,opacity:"1"}],Ha=[{offset:0,opacity:"0",transform:"translate3d(-100%, 100%, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],qa=[{offset:0,opacity:"0",transform:"translate3d(100%, 100%, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],ja=[{offset:0,opacity:"0",transform:"translate3d(0, -100%, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],Ka=[{offset:0,opacity:"0",transform:"translate3d(0, -2000px, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],Xa=[{offset:0,opacity:"0",transform:"translate3d(-100%, 0, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],Wa=[{offset:0,opacity:"0",transform:"translate3d(-2000px, 0, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],Ya=[{offset:0,opacity:"0",transform:"translate3d(100%, 0, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],Ga=[{offset:0,opacity:"0",transform:"translate3d(2000px, 0, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],Za=[{offset:0,opacity:"0",transform:"translate3d(-100%, -100%, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],Qa=[{offset:0,opacity:"0",transform:"translate3d(100%, -100%, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],Ja=[{offset:0,opacity:"0",transform:"translate3d(0, 100%, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],ts=[{offset:0,opacity:"0",transform:"translate3d(0, 2000px, 0)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],es=[{offset:0,opacity:"1"},{offset:1,opacity:"0"}],os=[{offset:0,opacity:"1",transform:"translate3d(0, 0, 0)"},{offset:1,opacity:"0",transform:"translate3d(-100%, 100%, 0)"}],rs=[{offset:0,opacity:"1",transform:"translate3d(0, 0, 0)"},{offset:1,opacity:"0",transform:"translate3d(100%, 100%, 0)"}],is=[{offset:0,opacity:"1"},{offset:1,opacity:"0",transform:"translate3d(0, 100%, 0)"}],as=[{offset:0,opacity:"1"},{offset:1,opacity:"0",transform:"translate3d(0, 2000px, 0)"}],ss=[{offset:0,opacity:"1"},{offset:1,opacity:"0",transform:"translate3d(-100%, 0, 0)"}],ns=[{offset:0,opacity:"1"},{offset:1,opacity:"0",transform:"translate3d(-2000px, 0, 0)"}],ls=[{offset:0,opacity:"1"},{offset:1,opacity:"0",transform:"translate3d(100%, 0, 0)"}],cs=[{offset:0,opacity:"1"},{offset:1,opacity:"0",transform:"translate3d(2000px, 0, 0)"}],ds=[{offset:0,opacity:"1",transform:"translate3d(0, 0, 0)"},{offset:1,opacity:"0",transform:"translate3d(-100%, -100%, 0)"}],hs=[{offset:0,opacity:"1",transform:"translate3d(0, 0, 0)"},{offset:1,opacity:"0",transform:"translate3d(100%, -100%, 0)"}],us=[{offset:0,opacity:"1"},{offset:1,opacity:"0",transform:"translate3d(0, -100%, 0)"}],ps=[{offset:0,opacity:"1"},{offset:1,opacity:"0",transform:"translate3d(0, -2000px, 0)"}],fs=[{offset:0,transform:"perspective(400px) scale3d(1, 1, 1) translate3d(0, 0, 0) rotate3d(0, 1, 0, -360deg)",easing:"ease-out"},{offset:.4,transform:"perspective(400px) scale3d(1, 1, 1) translate3d(0, 0, 150px)\n      rotate3d(0, 1, 0, -190deg)",easing:"ease-out"},{offset:.5,transform:"perspective(400px) scale3d(1, 1, 1) translate3d(0, 0, 150px)\n      rotate3d(0, 1, 0, -170deg)",easing:"ease-in"},{offset:.8,transform:"perspective(400px) scale3d(0.95, 0.95, 0.95) translate3d(0, 0, 0)\n      rotate3d(0, 1, 0, 0deg)",easing:"ease-in"},{offset:1,transform:"perspective(400px) scale3d(1, 1, 1) translate3d(0, 0, 0) rotate3d(0, 1, 0, 0deg)",easing:"ease-in"}],ms=[{offset:0,transform:"perspective(400px) rotate3d(1, 0, 0, 90deg)",easing:"ease-in",opacity:"0"},{offset:.4,transform:"perspective(400px) rotate3d(1, 0, 0, -20deg)",easing:"ease-in"},{offset:.6,transform:"perspective(400px) rotate3d(1, 0, 0, 10deg)",opacity:"1"},{offset:.8,transform:"perspective(400px) rotate3d(1, 0, 0, -5deg)"},{offset:1,transform:"perspective(400px)"}],bs=[{offset:0,transform:"perspective(400px) rotate3d(0, 1, 0, 90deg)",easing:"ease-in",opacity:"0"},{offset:.4,transform:"perspective(400px) rotate3d(0, 1, 0, -20deg)",easing:"ease-in"},{offset:.6,transform:"perspective(400px) rotate3d(0, 1, 0, 10deg)",opacity:"1"},{offset:.8,transform:"perspective(400px) rotate3d(0, 1, 0, -5deg)"},{offset:1,transform:"perspective(400px)"}],gs=[{offset:0,transform:"perspective(400px)"},{offset:.3,transform:"perspective(400px) rotate3d(1, 0, 0, -20deg)",opacity:"1"},{offset:1,transform:"perspective(400px) rotate3d(1, 0, 0, 90deg)",opacity:"0"}],vs=[{offset:0,transform:"perspective(400px)"},{offset:.3,transform:"perspective(400px) rotate3d(0, 1, 0, -15deg)",opacity:"1"},{offset:1,transform:"perspective(400px) rotate3d(0, 1, 0, 90deg)",opacity:"0"}],ys=[{offset:0,transform:"translate3d(-100%, 0, 0) skewX(30deg)",opacity:"0"},{offset:.6,transform:"skewX(-20deg)",opacity:"1"},{offset:.8,transform:"skewX(5deg)"},{offset:1,transform:"translate3d(0, 0, 0)"}],ws=[{offset:0,transform:"translate3d(100%, 0, 0) skewX(-30deg)",opacity:"0"},{offset:.6,transform:"skewX(20deg)",opacity:"1"},{offset:.8,transform:"skewX(-5deg)"},{offset:1,transform:"translate3d(0, 0, 0)"}],_s=[{offset:0,opacity:"1"},{offset:1,transform:"translate3d(-100%, 0, 0) skewX(-30deg)",opacity:"0"}],xs=[{offset:0,opacity:"1"},{offset:1,transform:"translate3d(100%, 0, 0) skewX(30deg)",opacity:"0"}],ks=[{offset:0,transform:"rotate3d(0, 0, 1, -200deg)",opacity:"0"},{offset:1,transform:"translate3d(0, 0, 0)",opacity:"1"}],$s=[{offset:0,transform:"rotate3d(0, 0, 1, -45deg)",opacity:"0"},{offset:1,transform:"translate3d(0, 0, 0)",opacity:"1"}],Cs=[{offset:0,transform:"rotate3d(0, 0, 1, 45deg)",opacity:"0"},{offset:1,transform:"translate3d(0, 0, 0)",opacity:"1"}],zs=[{offset:0,transform:"rotate3d(0, 0, 1, 45deg)",opacity:"0"},{offset:1,transform:"translate3d(0, 0, 0)",opacity:"1"}],Ss=[{offset:0,transform:"rotate3d(0, 0, 1, -90deg)",opacity:"0"},{offset:1,transform:"translate3d(0, 0, 0)",opacity:"1"}],As=[{offset:0,opacity:"1"},{offset:1,transform:"rotate3d(0, 0, 1, 200deg)",opacity:"0"}],Ts=[{offset:0,opacity:"1"},{offset:1,transform:"rotate3d(0, 0, 1, 45deg)",opacity:"0"}],Es=[{offset:0,opacity:"1"},{offset:1,transform:"rotate3d(0, 0, 1, -45deg)",opacity:"0"}],Ds=[{offset:0,opacity:"1"},{offset:1,transform:"rotate3d(0, 0, 1, -45deg)",opacity:"0"}],Ls=[{offset:0,opacity:"1"},{offset:1,transform:"rotate3d(0, 0, 1, 90deg)",opacity:"0"}],Ms=[{offset:0,transform:"translate3d(0, -100%, 0)",visibility:"visible"},{offset:1,transform:"translate3d(0, 0, 0)"}],Os=[{offset:0,transform:"translate3d(-100%, 0, 0)",visibility:"visible"},{offset:1,transform:"translate3d(0, 0, 0)"}],Fs=[{offset:0,transform:"translate3d(100%, 0, 0)",visibility:"visible"},{offset:1,transform:"translate3d(0, 0, 0)"}],Is=[{offset:0,transform:"translate3d(0, 100%, 0)",visibility:"visible"},{offset:1,transform:"translate3d(0, 0, 0)"}],Bs=[{offset:0,transform:"translate3d(0, 0, 0)"},{offset:1,visibility:"hidden",transform:"translate3d(0, 100%, 0)"}],Ps=[{offset:0,transform:"translate3d(0, 0, 0)"},{offset:1,visibility:"hidden",transform:"translate3d(-100%, 0, 0)"}],Vs=[{offset:0,transform:"translate3d(0, 0, 0)"},{offset:1,visibility:"hidden",transform:"translate3d(100%, 0, 0)"}],Rs=[{offset:0,transform:"translate3d(0, 0, 0)"},{offset:1,visibility:"hidden",transform:"translate3d(0, -100%, 0)"}],Us=[{offset:0,easing:"ease-in-out"},{offset:.2,transform:"rotate3d(0, 0, 1, 80deg)",easing:"ease-in-out"},{offset:.4,transform:"rotate3d(0, 0, 1, 60deg)",easing:"ease-in-out",opacity:"1"},{offset:.6,transform:"rotate3d(0, 0, 1, 80deg)",easing:"ease-in-out"},{offset:.8,transform:"rotate3d(0, 0, 1, 60deg)",easing:"ease-in-out",opacity:"1"},{offset:1,transform:"translate3d(0, 700px, 0)",opacity:"0"}],Ns=[{offset:0,opacity:"0",transform:"scale(0.1) rotate(30deg)","transform-origin":"center bottom"},{offset:.5,transform:"rotate(-10deg)"},{offset:.7,transform:"rotate(3deg)"},{offset:1,opacity:"1",transform:"scale(1)"}],Hs=[{offset:0,opacity:"0",transform:"translate3d(-100%, 0, 0) rotate3d(0, 0, 1, -120deg)"},{offset:1,opacity:"1",transform:"translate3d(0, 0, 0)"}],qs=[{offset:0,opacity:"1"},{offset:1,opacity:"0",transform:"translate3d(100%, 0, 0) rotate3d(0, 0, 1, 120deg)"}],js=[{offset:0,opacity:"0",transform:"scale3d(0.3, 0.3, 0.3)"},{offset:.5,opacity:"1"}],Ks=[{offset:0,opacity:"0",transform:"scale3d(0.1, 0.1, 0.1) translate3d(0, -1000px, 0)",easing:"cubic-bezier(0.55, 0.055, 0.675, 0.19)"},{offset:.6,opacity:"1",transform:"scale3d(0.475, 0.475, 0.475) translate3d(0, 60px, 0)",easing:"cubic-bezier(0.175, 0.885, 0.32, 1)"}],Xs=[{offset:0,opacity:"0",transform:"scale3d(0.1, 0.1, 0.1) translate3d(-1000px, 0, 0)",easing:"cubic-bezier(0.55, 0.055, 0.675, 0.19)"},{offset:.6,opacity:"1",transform:"scale3d(0.475, 0.475, 0.475) translate3d(10px, 0, 0)",easing:"cubic-bezier(0.175, 0.885, 0.32, 1)"}],Ws=[{offset:0,opacity:"0",transform:"scale3d(0.1, 0.1, 0.1) translate3d(1000px, 0, 0)",easing:"cubic-bezier(0.55, 0.055, 0.675, 0.19)"},{offset:.6,opacity:"1",transform:"scale3d(0.475, 0.475, 0.475) translate3d(-10px, 0, 0)",easing:"cubic-bezier(0.175, 0.885, 0.32, 1)"}],Ys=[{offset:0,opacity:"0",transform:"scale3d(0.1, 0.1, 0.1) translate3d(0, 1000px, 0)",easing:"cubic-bezier(0.55, 0.055, 0.675, 0.19)"},{offset:.6,opacity:"1",transform:"scale3d(0.475, 0.475, 0.475) translate3d(0, -60px, 0)",easing:"cubic-bezier(0.175, 0.885, 0.32, 1)"}],Gs=[{offset:0,opacity:"1"},{offset:.5,opacity:"0",transform:"scale3d(0.3, 0.3, 0.3)"},{offset:1,opacity:"0"}],Zs=[{offset:.4,opacity:"1",transform:"scale3d(0.475, 0.475, 0.475) translate3d(0, -60px, 0)",easing:"cubic-bezier(0.55, 0.055, 0.675, 0.19)"},{offset:1,opacity:"0",transform:"scale3d(0.1, 0.1, 0.1) translate3d(0, 2000px, 0)",easing:"cubic-bezier(0.175, 0.885, 0.32, 1)"}],Qs=[{offset:.4,opacity:"1",transform:"scale3d(0.475, 0.475, 0.475) translate3d(42px, 0, 0)"},{offset:1,opacity:"0",transform:"scale(0.1) translate3d(-2000px, 0, 0)"}],Js=[{offset:.4,opacity:"1",transform:"scale3d(0.475, 0.475, 0.475) translate3d(-42px, 0, 0)"},{offset:1,opacity:"0",transform:"scale(0.1) translate3d(2000px, 0, 0)"}],tn=[{offset:.4,opacity:"1",transform:"scale3d(0.475, 0.475, 0.475) translate3d(0, 60px, 0)",easing:"cubic-bezier(0.55, 0.055, 0.675, 0.19)"},{offset:1,opacity:"0",transform:"scale3d(0.1, 0.1, 0.1) translate3d(0, -2000px, 0)",easing:"cubic-bezier(0.175, 0.885, 0.32, 1)"}],en={linear:"linear",ease:"ease",easeIn:"ease-in",easeOut:"ease-out",easeInOut:"ease-in-out",easeInSine:"cubic-bezier(0.47, 0, 0.745, 0.715)",easeOutSine:"cubic-bezier(0.39, 0.575, 0.565, 1)",easeInOutSine:"cubic-bezier(0.445, 0.05, 0.55, 0.95)",easeInQuad:"cubic-bezier(0.55, 0.085, 0.68, 0.53)",easeOutQuad:"cubic-bezier(0.25, 0.46, 0.45, 0.94)",easeInOutQuad:"cubic-bezier(0.455, 0.03, 0.515, 0.955)",easeInCubic:"cubic-bezier(0.55, 0.055, 0.675, 0.19)",easeOutCubic:"cubic-bezier(0.215, 0.61, 0.355, 1)",easeInOutCubic:"cubic-bezier(0.645, 0.045, 0.355, 1)",easeInQuart:"cubic-bezier(0.895, 0.03, 0.685, 0.22)",easeOutQuart:"cubic-bezier(0.165, 0.84, 0.44, 1)",easeInOutQuart:"cubic-bezier(0.77, 0, 0.175, 1)",easeInQuint:"cubic-bezier(0.755, 0.05, 0.855, 0.06)",easeOutQuint:"cubic-bezier(0.23, 1, 0.32, 1)",easeInOutQuint:"cubic-bezier(0.86, 0, 0.07, 1)",easeInExpo:"cubic-bezier(0.95, 0.05, 0.795, 0.035)",easeOutExpo:"cubic-bezier(0.19, 1, 0.22, 1)",easeInOutExpo:"cubic-bezier(1, 0, 0, 1)",easeInCirc:"cubic-bezier(0.6, 0.04, 0.98, 0.335)",easeOutCirc:"cubic-bezier(0.075, 0.82, 0.165, 1)",easeInOutCirc:"cubic-bezier(0.785, 0.135, 0.15, 0.86)",easeInBack:"cubic-bezier(0.6, -0.28, 0.735, 0.045)",easeOutBack:"cubic-bezier(0.175, 0.885, 0.32, 1.275)",easeInOutBack:"cubic-bezier(0.68, -0.55, 0.265, 1.55)"};var on,rn=class extends et{constructor(){super(...arguments),this.hasStarted=!1,this.name="none",this.play=!1,this.delay=0,this.direction="normal",this.duration=1e3,this.easing="linear",this.endDelay=0,this.fill="auto",this.iterations=1/0,this.iterationStart=0,this.playbackRate=1}get currentTime(){var t,e;return null!=(e=null==(t=this.animation)?void 0:t.currentTime)?e:0}set currentTime(t){this.animation&&(this.animation.currentTime=t)}connectedCallback(){super.connectedCallback(),this.createAnimation(),this.handleAnimationCancel=this.handleAnimationCancel.bind(this),this.handleAnimationFinish=this.handleAnimationFinish.bind(this)}disconnectedCallback(){super.disconnectedCallback(),this.destroyAnimation()}handleAnimationChange(){this.hasUpdated&&this.createAnimation()}handleAnimationFinish(){this.play=!1,this.hasStarted=!1,Nt(this,"sl-finish")}handleAnimationCancel(){this.play=!1,this.hasStarted=!1,Nt(this,"sl-cancel")}handlePlayChange(){return!!this.animation&&(this.play&&!this.hasStarted&&(this.hasStarted=!0,Nt(this,"sl-start")),this.play?this.animation.play():this.animation.pause(),!0)}handlePlaybackRateChange(){this.animation&&(this.animation.playbackRate=this.playbackRate)}handleSlotChange(){this.destroyAnimation(),this.createAnimation()}async createAnimation(){var t,e;const o=null!=(t=da.easings[this.easing])?t:this.easing,r=null!=(e=this.keyframes)?e:da[this.name],i=(await this.defaultSlot).assignedElements()[0];return!(!i||!r)&&(this.destroyAnimation(),this.animation=i.animate(r,{delay:this.delay,direction:this.direction,duration:this.duration,easing:o,endDelay:this.endDelay,fill:this.fill,iterationStart:this.iterationStart,iterations:this.iterations}),this.animation.playbackRate=this.playbackRate,this.animation.addEventListener("cancel",this.handleAnimationCancel),this.animation.addEventListener("finish",this.handleAnimationFinish),this.play?(this.hasStarted=!0,Nt(this,"sl-start")):this.animation.pause(),!0)}destroyAnimation(){this.animation&&(this.animation.cancel(),this.animation.removeEventListener("cancel",this.handleAnimationCancel),this.animation.removeEventListener("finish",this.handleAnimationFinish),this.hasStarted=!1)}cancel(){var t;null==(t=this.animation)||t.cancel()}finish(){var t;null==(t=this.animation)||t.finish()}render(){return O` <slot @slotchange=${this.handleSlotChange}></slot> `}};rn.styles=ca,Dt([(on="slot",Yt({descriptor:t=>({async get(){var t;return await this.updateComplete,null===(t=this.renderRoot)||void 0===t?void 0:t.querySelector(on)},enumerable:!0,configurable:!0})}))],rn.prototype,"defaultSlot",2),Dt([Kt()],rn.prototype,"name",2),Dt([Kt({type:Boolean,reflect:!0})],rn.prototype,"play",2),Dt([Kt({type:Number})],rn.prototype,"delay",2),Dt([Kt()],rn.prototype,"direction",2),Dt([Kt({type:Number})],rn.prototype,"duration",2),Dt([Kt()],rn.prototype,"easing",2),Dt([Kt({attribute:"end-delay",type:Number})],rn.prototype,"endDelay",2),Dt([Kt()],rn.prototype,"fill",2),Dt([Kt({type:Number})],rn.prototype,"iterations",2),Dt([Kt({attribute:"iteration-start",type:Number})],rn.prototype,"iterationStart",2),Dt([Kt({attribute:!1})],rn.prototype,"keyframes",2),Dt([Kt({attribute:"playback-rate",type:Number})],rn.prototype,"playbackRate",2),Dt([Ut("name"),Ut("delay"),Ut("direction"),Ut("duration"),Ut("easing"),Ut("endDelay"),Ut("fill"),Ut("iterations"),Ut("iterationsStart"),Ut("keyframes")],rn.prototype,"handleAnimationChange",1),Dt([Ut("play")],rn.prototype,"handlePlayChange",1),Dt([Ut("playbackRate")],rn.prototype,"handlePlaybackRateChange",1),rn=Dt([qt("sl-animation")],rn);var an=n`
  ${it}

  :host {
    --control-box-size: 3rem;
    --icon-size: calc(var(--control-box-size) * 0.625);
    display: inline-flex;
    position: relative;
    cursor: pointer;
  }

  img {
    display: block;
    width: 100%;
    height: 100%;
  }

  img[aria-hidden='true'] {
    display: none;
  }

  .animated-image__control-box {
    display: flex;
    position: absolute;
    align-items: center;
    justify-content: center;
    top: calc(50% - var(--control-box-size) / 2);
    right: calc(50% - var(--control-box-size) / 2);
    width: var(--control-box-size);
    height: var(--control-box-size);
    font-size: var(--icon-size);
    background: none;
    border: solid 2px currentColor;
    background-color: rgb(0 0 0 /50%);
    border-radius: var(--sl-border-radius-circle);
    color: white;
    pointer-events: none;
    transition: var(--sl-transition-fast) opacity;
  }

  :host([play]:hover) .animated-image__control-box {
    opacity: 1;
    transform: scale(1);
  }

  :host([play]:not(:hover)) .animated-image__control-box {
    opacity: 0;
  }
`,sn=class extends et{constructor(){super(...arguments),this.isLoaded=!1}handleClick(){this.play=!this.play}handleLoad(){const t=document.createElement("canvas"),{width:e,height:o}=this.animatedImage;t.width=e,t.height=o,t.getContext("2d").drawImage(this.animatedImage,0,0,e,o),this.frozenFrame=t.toDataURL("image/gif"),this.isLoaded||(Nt(this,"sl-load"),this.isLoaded=!0)}handleError(){Nt(this,"sl-error")}handlePlayChange(){this.play&&(this.animatedImage.src="",this.animatedImage.src=this.src)}handleSrcChange(){this.isLoaded=!1}render(){return O`
      <div class="animated-image">
        <img
          class="animated-image__animated"
          src=${this.src}
          alt=${this.alt}
          crossorigin="anonymous"
          aria-hidden=${this.play?"false":"true"}
          @click=${this.handleClick}
          @load=${this.handleLoad}
          @error=${this.handleError}
        />

        ${this.isLoaded?O`
              <img
                class="animated-image__frozen"
                src=${this.frozenFrame}
                alt=${this.alt}
                aria-hidden=${this.play?"true":"false"}
                @click=${this.handleClick}
              />

              <div part="control-box" class="animated-image__control-box">
                ${this.play?O`<sl-icon part="pause-icon" name="pause-fill" library="system"></sl-icon>`:O`<sl-icon part="play-icon" name="play-fill" library="system"></sl-icon>`}
              </div>
            `:""}
      </div>
    `}};sn.styles=an,Dt([Xt()],sn.prototype,"frozenFrame",2),Dt([Xt()],sn.prototype,"isLoaded",2),Dt([Gt(".animated-image__animated")],sn.prototype,"animatedImage",2),Dt([Kt()],sn.prototype,"src",2),Dt([Kt()],sn.prototype,"alt",2),Dt([Kt({type:Boolean,reflect:!0})],sn.prototype,"play",2),Dt([Ut("play",{waitUntilFirstUpdate:!0})],sn.prototype,"handlePlayChange",1),Dt([Ut("src")],sn.prototype,"handleSrcChange",1),sn=Dt([qt("sl-animated-image")],sn);var nn={init:function(){document.querySelectorAll("*").forEach((t=>{if(t.classList.value.startsWith("nr-")){var e="dom";t.classList[0].substring(3).split("-").forEach((t=>e+=t.substring(0,1).toUpperCase()+t.substring(1))),nn[e]=t}})),nn.navbarInit(),nn.event(),document.cookie.includes(".theme=dark")&&!document.documentElement.classList.contains("sl-theme-dark")&&nn.setTheme("dark"),nn.ready(),nn.changeSize(),window.addEventListener("resize",(function(){nn.changeSize()})),nn.setThemeGrid()},navbarInit:function(){nn.domNavbar&&(nn.domNavbarToggler.addEventListener("click",(function(){nn.domNavbarDrawer.style.display="",nn.domNavbarDrawer.show()})),nn.domNavbarDropdown&&nn.domNavbarDropdown.addEventListener("sl-show",(function(){nn.domNavbarDropdown.querySelector("sl-menu").style.display=""})),nn.domNavbar.querySelectorAll("sl-menu").forEach((t=>{t.addEventListener("sl-select",(function(t){var e=t.detail.item,o=e.getAttribute("data-href"),r=e.getAttribute("data-target");null!=o&&("_blank"==r?window.open(o):location.href=o)}))}))),nn.changeTheme()},event:function(){document.body.addEventListener("click",(function(t){switch(t.target.getAttribute("data-action")){case"back-to-top":document.documentElement.scrollTo(0,0),document.body.scrollTo(0,0);break;case"theme":nn.setTheme(nn.isDark()?"light":"dark")}}),!1)},keyId:null,ready:function(){nn.onReady()},onReady:function(){},changeSize:function(){var t=document.documentElement.clientHeight,e=document.documentElement.clientWidth;nn.onChangeSize(t,e)},onChangeSize:function(t,e){},isDark:function(){return document.documentElement.classList.contains("sl-theme-dark")},setTheme:function(t){var e="dark"==t?"light":"dark";document.documentElement.className=document.documentElement.className.replaceAll(e,t),nn.domNavbar.className=nn.domNavbar.className.replaceAll(e,t),nn.cookie(".theme",t,31536e6),nn.changeTheme()},setThemeGrid:function(t,e){(e=e||nn.domGrid)&&("dark"==(t=t||(nn.isDark()?"dark":"light"))?(e.classList.remove("ag-theme-alpine"),e.classList.add("ag-theme-alpine-dark")):(e.classList.remove("ag-theme-alpine-dark"),e.classList.add("ag-theme-alpine")))},changeTheme:function(){if(nn.domNavbar){var t=nn.domNavbar.querySelector('sl-menu-item[data-action="theme"]');t&&(t.checked=nn.isDark())}window.monaco&&(nn.isDark()?monaco.editor.setTheme("vs-dark"):monaco.editor.setTheme("vs")),nn.nmd&&nn.nmd.toggleTheme(nn.isDark()?"dark":"light"),nn.setThemeGrid(),nn.onChangeTheme()},onChangeTheme:function(){},toFormData:function(t){var e=new FormData;for(var o in t)e.append(o,t[o]);return e},toQueryString:function(t){var e=[];for(var o in t)e.push(o+"="+encodeURIComponent(t[o]));return e.join("&")},htmlEncode:t=>{var e=document.createElement("div");return e.innerText=t,e.innerHTML},htmlDecode:t=>{var e=document.createElement("div");return e.innerHTML=t,e.innerText},alert:function(t,e="bell",o=5e3){const r=Object.assign(document.createElement("sl-alert"),{closable:!0,duration:o,innerHTML:`<sl-icon name="${e}" slot="icon"></sl-icon>${t}`});return document.body.append(r),r.toast()},dialog:function(t){const e=Object.assign(document.createElement("sl-dialog"),{label:"Message",innerHTML:`${t}`});return document.body.append(e),e.show(),e},cookie:function(t,e,o){if(1==arguments.length){var r=document.cookie.match(new RegExp("(^| )"+t+"=([^;]*)(;|$)"));return null!=r?r[2]:null}var i=t+"="+e+";path=/";if(o){var a=new Date;a.setTime(a.getTime()+o),i+=";expires="+a.toGMTString()}document.cookie=i},findScript:function(t){var e=document.scripts;for(let o=0;o<e.length;o++){let r=e[o];if(r.src.includes(t))return r}},type:function(t){return{}.toString.call(t).split(" ")[1].replace("]","")},receiveFiles:function(t,e,o){(o=o||document).addEventListener("dragover",(t=>{e&&e.contains(t.target)||(t.preventDefault(),t.stopPropagation())})),o.addEventListener("drop",(o=>{if(!e||!e.contains(o.target)){o.preventDefault();var r=o.dataTransfer.items;nn.readDataTransferItems(r).then((e=>{e.length&&t(e,"drag")}))}})),e&&e.addEventListener("change",(function(){var e=this.files;e.length&&t(e,"change")})),document.addEventListener("paste",(function(e){var o=e.clipboardData.items,r=[];for(let t=0;t<o.length;t++){var i=o[t].getAsFile();i&&r.push(i)}r.length&&t(r,"paste")}))},readDataTransferItems:t=>new Promise((e=>{for(var o=[],r=[],i=0;i<t.length;i++){var a=t[i],s=a.webkitGetAsEntry();if(null!=s)o.push(nn.readDataTransferItemEntry(s));else{var n=a.getAsFile();n&&r.push(n)}}Promise.all(o).then((t=>{t.forEach((t=>{t.length?r=r.concat(t):r.push(t)})),e(r)}))})),readDataTransferItemEntry:(t,e)=>new Promise((o=>{if(e=e||"",t.isFile)t.file((t=>{""!=e&&(t.fullPath=e+t.name),o(t)}));else if(t.isDirectory){t.createReader().readEntries((r=>{for(var i=[],a=0;a<r.length;a++)i.push(nn.readDataTransferItemEntry(r[a],e+t.name+"/"));Promise.all(i).then((t=>{var e=[];t.forEach((t=>{t.length?e=e.concat(t):e.push(t)})),o(e)}))}))}})),ls:{},lsInit:function(){try{var t=localStorage.getItem(location.pathname);null!=t&&""!=t&&(nn.ls=JSON.parse(t))}catch(t){nn.ls={},console.debug("localStorage parse error",t)}},lsArr:function(t){return nn.ls[t]=nn.ls[t]||[]},lsObj:function(t){return nn.ls[t]=nn.ls[t]||{}},lsStr:function(t){return nn.ls[t]=nn.ls[t]||""},lsSave:function(){localStorage.setItem(location.pathname,JSON.stringify(nn.ls))},download:function(t,e){var o=document.createElement("a");if(o.download=e,1==t.nodeType)o.href=t.toDataURL();else{var r=new Blob([t]);o.href=URL.createObjectURL(r)}document.body.appendChild(o),o.click(),o.remove()},formatByteSize:function(t,e=2,o=1024){if(Math.abs(t)<o)return t+" B";const r=1e3==o?["KB","MB","GB","TB","PB","EB","ZB","YB"]:["KiB","MiB","GiB","TiB","PiB","EiB","ZiB","YiB"];let i=-1;const a=10**e;do{t/=o,++i}while(Math.round(Math.abs(t)*a)/a>=o&&i<r.length-1);return(1*t.toFixed(e)).toString()+" "+r[i]}};window.addEventListener("DOMContentLoaded",(function(){nn.lsInit(),nn.init()}),!1);var ln={agSetColumn:(t,e)=>Object.assign(t,{cellRenderer:t=>t.value in e?e[t.value]:t.value,filter:"agSetColumnFilter",filterParams:{values:Object.keys(e)}}),lk:function(){agGrid.LicenseManager.prototype.outputMissingLicenseKey=t=>{}},defaultColDef:t=>Object.assign({width:150,maxWidth:4e3,filter:!0,sortable:!0,resizable:!0,filter:"agMultiColumnFilter",menuTabs:["generalMenuTab","filterMenuTab","columnsMenuTab"]},t),autoGroupColumnDef:t=>Object.assign({width:300,maxWidth:4e3},t),optionDef:t=>Object.assign({localeText:ln.localeText,defaultColDef:ln.defaultColDef(),autoGroupColumnDef:ln.autoGroupColumnDef(),rowGroupPanelShow:"always",enableBrowserTooltips:!0,rowSelection:"multiple",suppressRowClickSelection:!0,enableRangeSelection:!0,autoSizePadding:40,headerHeight:32,pagination:!1,paginationPageSize:100,cacheBlockSize:100,suppressMoveWhenRowDragging:!0,animateRows:!0,isRowSelectable:t=>!0!==t.group,onSortChanged:t=>t.api.refreshCells(),onFilterChanged:t=>t.api.refreshCells(),onRowGroupOpened:t=>t.api.refreshCells()},t),numberCol:t=>Object.assign({headerName:"🆔",valueGetter:"node.rowIndex + 1",width:120,maxWidth:150,checkboxSelection:!0,headerCheckboxSelection:!0,headerCheckboxSelectionFilteredOnly:!0,sortable:!1,filter:!1,menuTabs:!1},t),localeText:{selectAll:"（全部）",selectAllSearchResults:"（全部搜索结果）",searchOoo:"搜索...",blanks:"（空）",noMatches:"未找到",filterOoo:"搜索...",equals:"等于",notEqual:"不等于",blank:"空",notBlank:"非空",empty:"选择一项",lessThan:"小于",greaterThan:"大于",lessThanOrEqual:"小于等于",greaterThanOrEqual:"大于等于",inRange:"范围",inRangeStart:"开始值",inRangeEnd:"结束值",contains:"包含",notContains:"不包含",startsWith:"头包含",endsWith:"尾包含",dateFormatOoo:"yyyy-mm-dd",andCondition:"和",orCondition:"或",applyFilter:"确定",resetFilter:"重置",clearFilter:"清除",cancelFilter:"取消",textFilter:"文本搜索",numberFilter:"数字搜索",dateFilter:"日期搜索",setFilter:"项搜索",columns:"列",filters:"搜索",pivotMode:"枢轴模式",groups:"行组",rowGroupColumnsEmptyMessage:"拖拽列到此处进行分组",values:"值",valueColumnsEmptyMessage:"拖拽到此处合计",pivots:"列标签",pivotColumnsEmptyMessage:"拖拽到此处设置列标签",group:"分组",rowDragRows:"行",loadingOoo:"加载中...",noRowsToShow:"（空）",enabled:"启用",pinColumn:"固定列",pinLeft:"左固定",pinRight:"右固定",noPin:"取消固定",valueAggregation:"合计",autosizeThiscolumn:"当前列大小自适应",autosizeAllColumns:"所有列大小自适应",groupBy:"分组",ungroupBy:"不分组",addToValues:"添加值 ${variable}",removeFromValues:"移除值 ${variable}",addToLabels:"添加到标签 ${variable}",removeFromLabels:"移除标签 ${variable}",resetColumns:"重置列",expandAll:"展开全部",collapseAll:"折叠全部",copy:"复制",ctrlC:"Ctrl+C",copyWithHeaders:"复制（带标题）",copyWithHeaderGroups:"复制（带分组）",paste:"粘贴",ctrlV:"Ctrl+V",export:"内置保存",csvExport:"CSV 导出",excelExport:"Excel 导出",sum:"求和",min:"最小",max:"最大",none:"无",count:"总数",avg:"平均",filteredRows:"过滤行",selectedRows:"选中",totalRows:"总行",totalAndFilteredRows:"搜索",more:"更多",to:"-",of:"，总共",page:"当前",nextPage:"下一页",lastPage:"尾页",firstPage:"首页",previousPage:"上一页",pivotColumnGroupTotals:"总",pivotChartAndPivotMode:"图表枢轴 & 枢轴模式",pivotChart:"图表枢轴",chartRange:"范围图表",columnChart:"柱状图",groupedColumn:"分组",stackedColumn:"堆叠柱形图",normalizedColumn:"100% 堆叠柱形图",barChart:"条形图",groupedBar:"分组",stackedBar:"堆叠柱形图",normalizedBar:"100% 堆叠柱形图",pieChart:"饼形图",pie:"饼图",doughnut:"环形图",line:"线图",xyChart:"散点图及气泡图",scatter:"散点图",bubble:"气泡图",areaChart:"面积图",area:"面积",stackedArea:"叠堆",normalizedArea:"100% 叠堆",histogramChart:"直方图",pivotChartTitle:"图表枢轴",rangeChartTitle:"范围图表",settings:"设置",data:"数据",format:"格式",categories:"类别",defaultCategory:"(无)",series:"系数",xyValues:"X Y 值",paired:"配对模式",axis:"轴",navigator:"导航",color:"颜色",thickness:"坐标宽度",xType:"X Type",automatic:"Automatic",category:"类别",number:"数字",time:"时间",xRotation:"X 旋转",yRotation:"Y 旋转",ticks:"Ticks",width:"宽",height:"高",length:"长",padding:"填充",spacing:"间距",chart:"图表",title:"标题",titlePlaceholder:"图表标题 - 双击编辑",background:"背景",font:"字体",top:"上",right:"右",bottom:"下",left:"左",labels:"标签",size:"大小",minSize:"最小",maxSize:"最大",legend:"指标项",position:"位置",markerSize:"Marker Size",markerStroke:"Marker Stroke",markerPadding:"Marker Padding",itemSpacing:"Item Spacing",itemPaddingX:"Item Padding X",itemPaddingY:"Item Padding Y",layoutHorizontalSpacing:"Horizontal Spacing",layoutVerticalSpacing:"Vertical Spacing",strokeWidth:"线条宽度",offset:"Offset",offsets:"Offsets",tooltips:"显示提示",callout:"Callout",markers:"标点",shadow:"阴影",blur:"发散",xOffset:"X 偏移",yOffset:"Y 偏移",lineWidth:"线条粗细",normal:"正常",bold:"加粗",italic:"斜体",boldItalic:"加粗斜体",predefined:"Predefined",fillOpacity:"填充透明度",strokeOpacity:"线条透明度",histogramBinCount:"Bin count",columnGroup:"柱状",barGroup:"条形",pieGroup:"饼状",lineGroup:"线",scatterGroup:"散点及气泡",areaGroup:"面积",histogramGroup:"直方",groupedColumnTooltip:"Grouped",stackedColumnTooltip:"Stacked",normalizedColumnTooltip:"100% Stacked",groupedBarTooltip:"Grouped",stackedBarTooltip:"Stacked",normalizedBarTooltip:"100% Stacked",pieTooltip:"Pie",doughnutTooltip:"Doughnut",lineTooltip:"Line",groupedAreaTooltip:"Area",stackedAreaTooltip:"Stacked",normalizedAreaTooltip:"100% Stacked",scatterTooltip:"Scatter",bubbleTooltip:"Bubble",histogramTooltip:"Histogram",noDataToChart:"No data available to be charted.",pivotChartRequiresPivotMode:"Pivot Chart requires Pivot Mode enabled.",chartSettingsToolbarTooltip:"Menu",chartLinkToolbarTooltip:"Linked to Grid",chartUnlinkToolbarTooltip:"Unlinked from Grid",chartDownloadToolbarTooltip:"Download Chart"}},cn=(o(371),{apiServer:"https://netnr.zme.ink",fetch:(t,e)=>new Promise(((o,r)=>{var i=["https://cors.eu.org/","https://cors.zme.ink/","https://netnr.zme.ink/api/v1/Proxy?url="],a=encodeURIComponent(t.url),s=t.encoding||"utf-8";delete t.url,delete t.encoding,null!=e?(a=i[e]+a,fetch(a,t).then((t=>t.blob())).then((t=>{var e=new FileReader;e.onload=function(t){var e=t.target.result;o(e)},e.readAsText(t,s)})).catch(r)):upstream(i,(function(e){a=e+a,fetch(a,t).then((t=>t.blob())).then((t=>{var e=new FileReader;e.onload=function(t){var e=t.target.result;o(e)},e.readAsText(t,s)})).catch(r)}),1)})),loading:function(t){var e=nr.domNavbarToggler.querySelector("sl-animation"),o=nr.domSlaSs;t?(e.duration=o.duration=1500,e.keyframes=o.keyframes=[{offset:0,easing:"cubic-bezier(0.250, 0.460, 0.450, 0.940)",fillMode:"both",transformOrigin:"center center",transform:"rotate(0)"},{offset:1,easing:"cubic-bezier(0.250, 0.460, 0.450, 0.940)",fillMode:"both",transformOrigin:"center center",transform:"rotate(360deg)"}]):(o.duration=1e4,e.keyframes=o.keyframes=[],e.cancel())}}),dn={init:()=>new Promise((t=>{meRequire(["vs/editor/editor.main"],(function(){monaco.languages.html.registerHTMLLanguageService("xml",{},{documentFormattingEdits:!0}),t()}))})),config:t=>Object.assign({value:"",theme:nr.isDark()?"vs-dark":"vs",language:"text/plain",fontSize:18,automaticLayout:!0,scrollbar:{verticalScrollbarSize:13,horizontalScrollbarSize:13},minimap:{enabled:!0}},t),create:(t,e)=>{var o=monaco.editor.create(t,dn.config(e));return dn.fullScreen(o),dn.wordWrap(o),o},formatter:t=>t.trigger("a","editor.action.formatDocument"),selectedValue:t=>t.getModel().getValueInRange(t.getSelection()),keepSetValue:(t,e)=>{var o=t.getPosition();t.executeEdits("",[{range:t.getModel().getFullModelRange(),text:e}]),t.setSelection(new monaco.Range(0,0,0,0)),t.setPosition(o)},setLanguage:(t,e)=>{monaco.editor.setModelLanguage(t.getModel(),e)},getLanguage:t=>t.getModel().getLanguageId(),onChange:(t,e,o)=>{o=o||500,t.onDidChangeModelContent((function(){clearTimeout(dn.defer_change),dn.defer_change=setTimeout((function(){e(t.getValue())}),o)}))},fullScreen:function(t){t.addAction({id:"meid-fullscreen",label:"全屏切换",keybindings:[monaco.KeyMod.CtrlCmd|monaco.KeyMod.Alt|monaco.KeyCode.KeyM],contextMenuGroupId:"me-01",run:function(t){t.getContainerDomNode().classList.toggle("nrc-fullscreen")}})},wordWrap:function(t){t.addAction({id:"meid-wordwrap",label:"换行切换",keybindings:[monaco.KeyMod.CtrlCmd|monaco.KeyMod.Alt|monaco.KeyCode.KeyN],contextMenuGroupId:"me-01",run:function(t){var e=t.getContainerDomNode(),o=e.getAttribute("wordWrap");null==o&&(o=t.getRawOptions().wordWrap),o="on"==o?"off":"on",e.setAttribute("wordWrap",o),t.updateOptions({wordWrap:o})}})}};o(246);wo("https://fastly.jsdelivr.net/npm/@shoelace-style/shoelace@2.0.0-beta.78/dist"),Object.assign(window,{nr:nn,ag:ln,ss:cn,me:dn})})()})();