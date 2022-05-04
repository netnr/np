"use strict";(self.webpackChunknetnr_blog=self.webpackChunknetnr_blog||[]).push([[50],{2442:(e,t,r)=>{var o=r(1936),a=r(674),i=r(2676),s=r(9138),n=r(6645),l=r(2288),c=r(6910),h=r(6557),d=r(1703),u=class extends h.s{constructor(){super(...arguments),this.position=50}handleDrag(e){const{width:t}=this.base.getBoundingClientRect();e.preventDefault(),(0,a.o)(this.base,(e=>{this.position=parseFloat((0,i.u)(e/t*100,0,100).toFixed(2))}))}handleKeyDown(e){if(["ArrowLeft","ArrowRight","Home","End"].includes(e.key)){const t=e.shiftKey?10:1;let r=this.position;e.preventDefault(),"ArrowLeft"===e.key&&(r-=t),"ArrowRight"===e.key&&(r+=t),"Home"===e.key&&(r=0),"End"===e.key&&(r=100),r=(0,i.u)(r,0,100),this.position=r}}handlePositionChange(){(0,l.j)(this,"sl-change")}render(){return h.$`
      <div part="base" id="image-comparer" class="image-comparer" @keydown=${this.handleKeyDown}>
        <div class="image-comparer__image">
          <div part="before" class="image-comparer__before">
            <slot name="before"></slot>
          </div>

          <div
            part="after"
            class="image-comparer__after"
            style=${(0,s.i)({clipPath:`inset(0 ${100-this.position}% 0 0)`})}
          >
            <slot name="after"></slot>
          </div>
        </div>

        <div
          part="divider"
          class="image-comparer__divider"
          style=${(0,s.i)({left:`${this.position}%`})}
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
              <sl-icon class="image-comparer__handle-icon" name="grip-vertical" library="system"></sl-icon>
            </slot>
          </div>
        </div>
      </div>
    `}};u.styles=o.L,(0,d.u2)([(0,c.i)(".image-comparer")],u.prototype,"base",2),(0,d.u2)([(0,c.i)(".image-comparer__handle")],u.prototype,"handle",2),(0,d.u2)([(0,c.e)({type:Number,reflect:!0})],u.prototype,"position",2),(0,d.u2)([(0,n.Y)("position",{waitUntilFirstUpdate:!0})],u.prototype,"handlePositionChange",1),u=(0,d.u2)([(0,c.n)("sl-image-comparer")],u)},4710:(e,t,r)=>{var o=r(5135),a=r(9063),i=r(6645),s=r(6910),n=r(6557),l=r(1703),c=class extends n.s{constructor(){super(...arguments),this.hasError=!1,this.image="",this.label="",this.initials="",this.shape="circle"}handleImageChange(){this.hasError=!1}render(){return n.$`
      <div
        part="base"
        class=${(0,a.o)({avatar:!0,"avatar--circle":"circle"===this.shape,"avatar--rounded":"rounded"===this.shape,"avatar--square":"square"===this.shape})}
        role="img"
        aria-label=${this.label}
      >
        ${this.initials?n.$` <div part="initials" class="avatar__initials">${this.initials}</div> `:n.$`
              <div part="icon" class="avatar__icon" aria-hidden="true">
                <slot name="icon">
                  <sl-icon name="person-fill" library="system"></sl-icon>
                </slot>
              </div>
            `}
        ${this.image&&!this.hasError?n.$`
              <img
                part="image"
                class="avatar__image"
                src="${this.image}"
                alt=""
                @error="${()=>this.hasError=!0}"
              />
            `:""}
      </div>
    `}};c.styles=o.a,(0,l.u2)([(0,s.t)()],c.prototype,"hasError",2),(0,l.u2)([(0,s.e)()],c.prototype,"image",2),(0,l.u2)([(0,s.e)()],c.prototype,"label",2),(0,l.u2)([(0,s.e)()],c.prototype,"initials",2),(0,l.u2)([(0,s.e)({reflect:!0})],c.prototype,"shape",2),(0,l.u2)([(0,i.Y)("image")],c.prototype,"handleImageChange",1),c=(0,l.u2)([(0,s.n)("sl-avatar")],c)},449:(e,t,r)=>{r.d(t,{J:()=>i});var o=r(6133),a=r(9736),i=r(6557).r`
  ${a.N}
  ${o.n}

  :host {
    display: block;
  }

  .select {
    display: block;
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
    box-shadow: var(--sl-focus-ring);
    outline: none;
    color: var(--sl-input-color-focus);
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
    outline: none;
    background-color: var(--sl-input-filled-background-color-focus);
    box-shadow: var(--sl-focus-ring);
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
    margin-left: var(--sl-spacing-2x-small);
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
    margin-left: var(--sl-input-spacing-small);
  }

  .select--small .select__label {
    margin: 0 var(--sl-input-spacing-small);
  }

  .select--small .select__clear {
    margin-right: var(--sl-input-spacing-small);
  }

  .select--small .select__suffix ::slotted(*) {
    margin-right: var(--sl-input-spacing-small);
  }

  .select--small .select__icon {
    margin-right: var(--sl-input-spacing-small);
  }

  .select--small .select__tags {
    padding-bottom: 2px;
  }

  .select--small .select__tags sl-tag {
    padding-top: 2px;
  }

  .select--small .select__tags sl-tag:not(:last-of-type) {
    margin-right: var(--sl-spacing-2x-small);
  }

  .select--small.select--has-tags .select__label {
    margin-left: 0;
  }

  /* Medium */
  .select--medium .select__control {
    border-radius: var(--sl-input-border-radius-medium);
    font-size: var(--sl-input-font-size-medium);
    min-height: var(--sl-input-height-medium);
  }

  .select--medium .select__prefix ::slotted(*) {
    margin-left: var(--sl-input-spacing-medium);
  }

  .select--medium .select__label {
    margin: 0 var(--sl-input-spacing-medium);
  }

  .select--medium .select__clear {
    margin-right: var(--sl-input-spacing-medium);
  }

  .select--medium .select__suffix ::slotted(*) {
    margin-right: var(--sl-input-spacing-medium);
  }

  .select--medium .select__icon {
    margin-right: var(--sl-input-spacing-medium);
  }

  .select--medium .select__tags {
    padding-bottom: 3px;
  }

  .select--medium .select__tags sl-tag {
    padding-top: 3px;
  }

  .select--medium .select__tags sl-tag:not(:last-of-type) {
    margin-right: var(--sl-spacing-2x-small);
  }

  .select--medium.select--has-tags .select__label {
    margin-left: 0;
  }

  /* Large */
  .select--large .select__control {
    border-radius: var(--sl-input-border-radius-large);
    font-size: var(--sl-input-font-size-large);
    min-height: var(--sl-input-height-large);
  }

  .select--large .select__prefix ::slotted(*) {
    margin-left: var(--sl-input-spacing-large);
  }

  .select--large .select__label {
    margin: 0 var(--sl-input-spacing-large);
  }

  .select--large .select__clear {
    margin-right: var(--sl-input-spacing-large);
  }

  .select--large .select__suffix ::slotted(*) {
    margin-right: var(--sl-input-spacing-large);
  }

  .select--large .select__icon {
    margin-right: var(--sl-input-spacing-large);
  }

  .select--large .select__tags {
    padding-bottom: 4px;
  }
  .select--large .select__tags sl-tag {
    padding-top: 4px;
  }

  .select--large .select__tags sl-tag:not(:last-of-type) {
    margin-right: var(--sl-spacing-2x-small);
  }

  .select--large.select--has-tags .select__label {
    margin-left: 0;
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
`},7419:(e,t,r)=>{r.d(t,{y:()=>a});var o=r(9736),a=r(6557).r`
  ${o.N}

  :host {
    display: inline-block;
  }

  .dropdown {
    position: relative;
  }

  .dropdown__trigger {
    display: block;
  }

  .dropdown__positioner {
    position: absolute;
    z-index: var(--sl-z-index-dropdown);
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

  .dropdown__positioner[data-placement^='top'] .dropdown__panel {
    transform-origin: bottom;
  }

  .dropdown__positioner[data-placement^='bottom'] .dropdown__panel {
    transform-origin: top;
  }

  .dropdown__positioner[data-placement^='left'] .dropdown__panel {
    transform-origin: right;
  }

  .dropdown__positioner[data-placement^='right'] .dropdown__panel {
    transform-origin: left;
  }
`},6246:(e,t,r)=>{r.d(t,{e:()=>a,i:()=>i,t:()=>o});var o={ATTRIBUTE:1,CHILD:2,PROPERTY:3,BOOLEAN_ATTRIBUTE:4,EVENT:5,ELEMENT:6},a=e=>(...t)=>({_$litDirective$:e,values:t}),i=class{constructor(e){}get _$AU(){return this._$AM._$AU}_$AT(e,t,r){this._$Ct=e,this._$AM=t,this._$Ci=r}_$AS(e,t){return this.update(e,t)}update(e,t){return this.render(...t)}}}
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */,7851:(e,t,r)=>{r.d(t,{Y:()=>a});var o=r(9736),a=r(6557).r`
  ${o.N}

  :host {
    display: contents;
  }
`},5067:(e,t,r)=>{r.d(t,{F:()=>a,r:()=>o});var o=class{constructor(e,...t){this.slotNames=[],(this.host=e).addController(this),this.slotNames=t,this.handleSlotChange=this.handleSlotChange.bind(this)}hasDefaultSlot(){return[...this.host.childNodes].some((e=>{if(e.nodeType===e.TEXT_NODE&&""!==e.textContent.trim())return!0;if(e.nodeType===e.ELEMENT_NODE){const t=e;if("sl-visually-hidden"===t.tagName.toLowerCase())return!1;if(!t.hasAttribute("slot"))return!0}return!1}))}hasNamedSlot(e){return null!==this.host.querySelector(`:scope > [slot="${e}"]`)}test(e){return"[default]"===e?this.hasDefaultSlot():this.hasNamedSlot(e)}hostConnected(){this.host.shadowRoot.addEventListener("slotchange",this.handleSlotChange)}hostDisconnected(){this.host.shadowRoot.removeEventListener("slotchange",this.handleSlotChange)}handleSlotChange(e){const t=e.target;(this.slotNames.includes("[default]")&&!t.name||t.name&&this.slotNames.includes(t.name))&&this.host.requestUpdate()}};function a(e){if(!e)return"";const t=e.assignedNodes({flatten:!0});let r="";return[...t].forEach((e=>{e.nodeType===Node.TEXT_NODE&&(r+=e.textContent)})),r}},2676:(e,t,r)=>{function o(e,t,r){return e<t?t:e>r?r:e}r.d(t,{u:()=>o})},7823:(e,t,r)=>{r.d(t,{f:()=>a});var o=r(9736),a=r(6557).r`
  ${o.N}

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
    display: block;
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
`},8749:(e,t,r)=>{r.d(t,{D:()=>a});var o=r(9736),a=r(6557).r`
  ${o.N}

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
`},2152:(e,t,r)=>{r.d(t,{R:()=>a});var o=r(9736),a=r(6557).r`
  ${o.N}

  :host {
    --track-color: var(--sl-color-neutral-200);
    --indicator-color: var(--sl-color-primary-600);

    display: block;
  }

  .tab-group {
    display: flex;
    border: solid 1px transparent;
    border-radius: 0;
  }

  .tab-group .tab-group__tabs {
    display: flex;
    position: relative;
  }

  .tab-group .tab-group__indicator {
    position: absolute;
    left: 0;
    transition: var(--sl-transition-fast) transform ease, var(--sl-transition-fast) width ease;
  }

  .tab-group--has-scroll-controls .tab-group__nav-container {
    position: relative;
    padding: 0 var(--sl-spacing-x-large);
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
    border-bottom: solid 2px var(--track-color);
  }

  .tab-group--top .tab-group__indicator {
    bottom: -2px;
    border-bottom: solid 2px var(--indicator-color);
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
    border-top: solid 2px var(--track-color);
  }

  .tab-group--bottom .tab-group__indicator {
    top: calc(-1 * 2px);
    border-top: solid 2px var(--indicator-color);
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
    border-right: solid 2px var(--track-color);
  }

  .tab-group--start .tab-group__indicator {
    right: calc(-1 * 2px);
    border-right: solid 2px var(--indicator-color);
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
    border-left: solid 2px var(--track-color);
  }

  .tab-group--end .tab-group__indicator {
    left: calc(-1 * 2px);
    border-left: solid 2px var(--indicator-color);
  }

  .tab-group--end .tab-group__body {
    flex: 1 1 auto;
    order: 1;
  }

  .tab-group--end ::slotted(sl-tab-panel) {
    --padding: 0 var(--sl-spacing-medium);
  }
`},6317:(e,t,r)=>{var o=r(1664),a=r(2676),i=r(9138),s=r(9063),n=r(1690),l=r(6645),c=r(2288),h=r(6910),d=r(6557),u=r(1703),p=class extends d.s{constructor(){super(...arguments),this.hoverValue=0,this.isHovering=!1,this.value=0,this.max=5,this.precision=1,this.readonly=!1,this.disabled=!1,this.getSymbol=()=>'<sl-icon name="star-fill" library="system"></sl-icon>'}focus(e){this.rating.focus(e)}blur(){this.rating.blur()}getValueFromMousePosition(e){return this.getValueFromXCoordinate(e.clientX)}getValueFromTouchPosition(e){return this.getValueFromXCoordinate(e.touches[0].clientX)}getValueFromXCoordinate(e){const t=this.rating.getBoundingClientRect().left,r=this.rating.getBoundingClientRect().width;return(0,a.u)(this.roundToPrecision((e-t)/r*this.max,this.precision),0,this.max)}handleClick(e){this.setValue(this.getValueFromMousePosition(e))}setValue(e){this.disabled||this.readonly||(this.value=e===this.value?0:e,this.isHovering=!1)}handleKeyDown(e){if(!this.disabled&&!this.readonly){if("ArrowLeft"===e.key){const t=e.shiftKey?1:this.precision;this.value=Math.max(0,this.value-t),e.preventDefault()}if("ArrowRight"===e.key){const t=e.shiftKey?1:this.precision;this.value=Math.min(this.max,this.value+t),e.preventDefault()}"Home"===e.key&&(this.value=0,e.preventDefault()),"End"===e.key&&(this.value=this.max,e.preventDefault())}}handleMouseEnter(){this.isHovering=!0}handleMouseMove(e){this.hoverValue=this.getValueFromMousePosition(e)}handleMouseLeave(){this.isHovering=!1}handleTouchStart(e){this.hoverValue=this.getValueFromTouchPosition(e),e.preventDefault()}handleTouchMove(e){this.isHovering=!0,this.hoverValue=this.getValueFromTouchPosition(e)}handleTouchEnd(e){this.isHovering=!1,this.setValue(this.hoverValue),e.preventDefault()}handleValueChange(){(0,c.j)(this,"sl-change")}roundToPrecision(e,t=.5){const r=1/t;return Math.ceil(e*r)/r}render(){const e=Array.from(Array(this.max).keys());let t=0;return t=this.disabled||this.readonly?this.value:this.isHovering?this.hoverValue:this.value,d.$`
      <div
        part="base"
        class=${(0,s.o)({rating:!0,"rating--readonly":this.readonly,"rating--disabled":this.disabled})}
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
          ${e.map((e=>d.$`
              <span
                class=${(0,s.o)({rating__symbol:!0,"rating__symbol--hover":this.isHovering&&Math.ceil(t)===e+1})}
                role="presentation"
                @mouseenter=${this.handleMouseEnter}
              >
                ${(0,n.o)(this.getSymbol(e+1))}
              </span>
            `))}
        </span>

        <span class="rating__symbols rating__symbols--indicator">
          ${e.map((e=>d.$`
              <span
                class=${(0,s.o)({rating__symbol:!0,"rating__symbol--hover":this.isHovering&&Math.ceil(t)===e+1})}
                style=${(0,i.i)({clipPath:t>e+1?"none":`inset(0 ${100-(t-e)/1*100}% 0 0)`})}
                role="presentation"
              >
                ${(0,n.o)(this.getSymbol(e+1))}
              </span>
            `))}
        </span>
      </div>
    `}};p.styles=o.n,(0,u.u2)([(0,h.i)(".rating")],p.prototype,"rating",2),(0,u.u2)([(0,h.t)()],p.prototype,"hoverValue",2),(0,u.u2)([(0,h.t)()],p.prototype,"isHovering",2),(0,u.u2)([(0,h.e)({type:Number})],p.prototype,"value",2),(0,u.u2)([(0,h.e)({type:Number})],p.prototype,"max",2),(0,u.u2)([(0,h.e)({type:Number})],p.prototype,"precision",2),(0,u.u2)([(0,h.e)({type:Boolean,reflect:!0})],p.prototype,"readonly",2),(0,u.u2)([(0,h.e)({type:Boolean,reflect:!0})],p.prototype,"disabled",2),(0,u.u2)([(0,h.e)()],p.prototype,"getSymbol",2),(0,u.u2)([(0,l.Y)("value",{waitUntilFirstUpdate:!0})],p.prototype,"handleValueChange",1),p=(0,u.u2)([(0,h.n)("sl-rating")],p)},6452:(e,t,r)=>{r.d(t,{y:()=>a});var o=r(9736),a=r(6557).r`
  ${o.N}

  :host {
    --size: 25rem;
    --header-spacing: var(--sl-spacing-large);
    --body-spacing: var(--sl-spacing-large);
    --footer-spacing: var(--sl-spacing-large);

    display: contents;
  }

  .drawer {
    top: 0;
    left: 0;
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
    right: auto;
    bottom: auto;
    left: 0;
    width: 100%;
    height: var(--size);
  }

  .drawer--end .drawer__panel {
    top: 0;
    right: 0;
    bottom: auto;
    left: auto;
    width: var(--size);
    height: 100%;
  }

  .drawer--bottom .drawer__panel {
    top: auto;
    right: auto;
    bottom: 0;
    left: 0;
    width: 100%;
    height: var(--size);
  }

  .drawer--start .drawer__panel {
    top: 0;
    right: auto;
    bottom: auto;
    left: 0;
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
    margin-right: var(--sl-spacing-x-small);
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
`},3146:(e,t,r)=>{var o=r(4747),a=r(6995),i=r(6910),s=r(6557),n=r(1703),l=class extends s.s{constructor(){super(...arguments),this.attrId=(0,a.w)(),this.componentId=`sl-tab-panel-${this.attrId}`,this.name="",this.active=!1}connectedCallback(){super.connectedCallback(),this.id=this.id.length>0?this.id:this.componentId}render(){return this.style.display=this.active?"block":"none",s.$`
      <div part="base" class="tab-panel" role="tabpanel" aria-hidden=${this.active?"false":"true"}>
        <slot></slot>
      </div>
    `}};l.styles=o.r,(0,n.u2)([(0,i.e)({reflect:!0})],l.prototype,"name",2),(0,n.u2)([(0,i.e)({type:Boolean,reflect:!0})],l.prototype,"active",2),l=(0,n.u2)([(0,i.n)("sl-tab-panel")],l)},1693:(e,t,r)=>{var o=r(4093),a=r(9063),i=r(6910),s=r(6557),n=r(1703),l=class extends s.s{constructor(){super(...arguments),this.variant="primary",this.pill=!1,this.pulse=!1}render(){return s.$`
      <span
        part="base"
        class=${(0,a.o)({badge:!0,"badge--primary":"primary"===this.variant,"badge--success":"success"===this.variant,"badge--neutral":"neutral"===this.variant,"badge--warning":"warning"===this.variant,"badge--danger":"danger"===this.variant,"badge--pill":this.pill,"badge--pulse":this.pulse})}
        role="status"
      >
        <slot></slot>
      </span>
    `}};l.styles=o.I,(0,n.u2)([(0,i.e)({reflect:!0})],l.prototype,"variant",2),(0,n.u2)([(0,i.e)({type:Boolean,reflect:!0})],l.prototype,"pill",2),(0,n.u2)([(0,i.e)({type:Boolean,reflect:!0})],l.prototype,"pulse",2),l=(0,n.u2)([(0,i.n)("sl-badge")],l)},2765:(e,t,r)=>{r.d(t,{m:()=>a});var o=r(9736),a=r(6557).r`
  ${o.N}

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
`},9283:(e,t,r)=>{var o=r(8848),a=r(7419),i=r(2386),s=r(568),n=r(5946),l=r(93),c=r(9063),h=r(6645),d=r(2288),u=r(6910),p=r(6557),g=r(1703),b=class extends p.s{constructor(){super(...arguments),this.open=!1,this.placement="bottom-start",this.disabled=!1,this.stayOpenOnSelect=!1,this.distance=0,this.skidding=0,this.hoist=!1}connectedCallback(){super.connectedCallback(),this.handleMenuItemActivate=this.handleMenuItemActivate.bind(this),this.handlePanelSelect=this.handlePanelSelect.bind(this),this.handleDocumentKeyDown=this.handleDocumentKeyDown.bind(this),this.handleDocumentMouseDown=this.handleDocumentMouseDown.bind(this),this.containingElement||(this.containingElement=this)}async firstUpdated(){this.panel.hidden=!this.open,this.open&&(await this.updateComplete,this.addOpenListeners(),this.startPositioner())}disconnectedCallback(){super.disconnectedCallback(),this.removeOpenListeners(),this.hide(),this.stopPositioner()}focusOnTrigger(){const e=this.trigger.querySelector("slot").assignedElements({flatten:!0})[0];"function"==typeof(null==e?void 0:e.focus)&&e.focus()}getMenu(){return this.panel.querySelector("slot").assignedElements({flatten:!0}).find((e=>"sl-menu"===e.tagName.toLowerCase()))}handleDocumentKeyDown(e){var t;if("Escape"===e.key)return this.hide(),void this.focusOnTrigger();if("Tab"===e.key){if(this.open&&"sl-menu-item"===(null==(t=document.activeElement)?void 0:t.tagName.toLowerCase()))return e.preventDefault(),this.hide(),void this.focusOnTrigger();setTimeout((()=>{var e,t,r;const o=(null==(e=this.containingElement)?void 0:e.getRootNode())instanceof ShadowRoot?null==(r=null==(t=document.activeElement)?void 0:t.shadowRoot)?void 0:r.activeElement:document.activeElement;this.containingElement&&(null==o?void 0:o.closest(this.containingElement.tagName.toLowerCase()))===this.containingElement||this.hide()}))}}handleDocumentMouseDown(e){const t=e.composedPath();this.containingElement&&!t.includes(this.containingElement)&&this.hide()}handleMenuItemActivate(e){const t=e.target;(0,s.zT)(t,this.panel)}handlePanelSelect(e){const t=e.target;this.stayOpenOnSelect||"sl-menu"!==t.tagName.toLowerCase()||(this.hide(),this.focusOnTrigger())}handlePopoverOptionsChange(){this.updatePositioner()}handleTriggerClick(){this.open?this.hide():this.show()}handleTriggerKeyDown(e){if("Escape"===e.key)return this.focusOnTrigger(),void this.hide();if([" ","Enter"].includes(e.key))return e.preventDefault(),void this.handleTriggerClick();const t=this.getMenu();if(t){const r=t.defaultSlot.assignedElements({flatten:!0}),o=r[0],a=r[r.length-1];["ArrowDown","ArrowUp","Home","End"].includes(e.key)&&(e.preventDefault(),this.open||this.show(),r.length>0&&requestAnimationFrame((()=>{"ArrowDown"!==e.key&&"Home"!==e.key||(t.setCurrentItem(o),o.focus()),"ArrowUp"!==e.key&&"End"!==e.key||(t.setCurrentItem(a),a.focus())})));const i=["Tab","Shift","Meta","Ctrl","Alt"];this.open&&!i.includes(e.key)&&t.typeToSelect(e)}}handleTriggerKeyUp(e){" "===e.key&&e.preventDefault()}handleTriggerSlotChange(){this.updateAccessibleTrigger()}updateAccessibleTrigger(){const e=this.trigger.querySelector("slot").assignedElements({flatten:!0}).find((e=>(0,i.C)(e).start));let t;if(e){switch(e.tagName.toLowerCase()){case"sl-button":case"sl-icon-button":t=e.button;break;default:t=e}t.setAttribute("aria-haspopup","true"),t.setAttribute("aria-expanded",this.open?"true":"false")}}async show(){if(!this.open)return this.open=!0,(0,d.m)(this,"sl-after-show")}async hide(){if(this.open)return this.open=!1,(0,d.m)(this,"sl-after-hide")}reposition(){this.updatePositioner()}addOpenListeners(){this.panel.addEventListener("sl-activate",this.handleMenuItemActivate),this.panel.addEventListener("sl-select",this.handlePanelSelect),document.addEventListener("keydown",this.handleDocumentKeyDown),document.addEventListener("mousedown",this.handleDocumentMouseDown)}removeOpenListeners(){this.panel.removeEventListener("sl-activate",this.handleMenuItemActivate),this.panel.removeEventListener("sl-select",this.handlePanelSelect),document.removeEventListener("keydown",this.handleDocumentKeyDown),document.removeEventListener("mousedown",this.handleDocumentMouseDown)}async handleOpenChange(){if(this.disabled)this.open=!1;else if(this.updateAccessibleTrigger(),this.open){(0,d.j)(this,"sl-show"),this.addOpenListeners(),await(0,n.U_)(this),this.startPositioner(),this.panel.hidden=!1;const{keyframes:e,options:t}=(0,l.O8)(this,"dropdown.show");await(0,n.nv)(this.panel,e,t),(0,d.j)(this,"sl-after-show")}else{(0,d.j)(this,"sl-hide"),this.removeOpenListeners(),await(0,n.U_)(this);const{keyframes:e,options:t}=(0,l.O8)(this,"dropdown.hide");await(0,n.nv)(this.panel,e,t),this.panel.hidden=!0,this.stopPositioner(),(0,d.j)(this,"sl-after-hide")}}startPositioner(){this.stopPositioner(),this.updatePositioner(),this.positionerCleanup=(0,o.Me)(this.trigger,this.positioner,this.updatePositioner.bind(this))}updatePositioner(){this.open&&this.trigger&&this.positioner&&(0,o.oo)(this.trigger,this.positioner,{placement:this.placement,middleware:[(0,o.cv)({mainAxis:this.distance,crossAxis:this.skidding}),(0,o.RR)(),(0,o.uY)(),(0,o.dp)({apply:({width:e,height:t})=>{Object.assign(this.panel.style,{maxWidth:`${e}px`,maxHeight:`${t}px`})},padding:8})],strategy:this.hoist?"fixed":"absolute"}).then((({x:e,y:t,placement:r})=>{this.positioner.setAttribute("data-placement",r),Object.assign(this.positioner.style,{position:this.hoist?"fixed":"absolute",left:`${e}px`,top:`${t}px`})}))}stopPositioner(){this.positionerCleanup&&(this.positionerCleanup(),this.positionerCleanup=void 0,this.positioner.removeAttribute("data-placement"))}render(){return p.$`
      <div
        part="base"
        id="dropdown"
        class=${(0,c.o)({dropdown:!0,"dropdown--open":this.open})}
      >
        <span
          part="trigger"
          class="dropdown__trigger"
          @click=${this.handleTriggerClick}
          @keydown=${this.handleTriggerKeyDown}
          @keyup=${this.handleTriggerKeyUp}
        >
          <slot name="trigger" @slotchange=${this.handleTriggerSlotChange}></slot>
        </span>

        <!-- Position the panel with a wrapper since the popover makes use of translate. This let's us add animations
        on the panel without interfering with the position. -->
        <div class="dropdown__positioner">
          <div
            part="panel"
            class="dropdown__panel"
            aria-hidden=${this.open?"false":"true"}
            aria-labelledby="dropdown"
          >
            <slot></slot>
          </div>
        </div>
      </div>
    `}};b.styles=a.y,(0,g.u2)([(0,u.i)(".dropdown__trigger")],b.prototype,"trigger",2),(0,g.u2)([(0,u.i)(".dropdown__panel")],b.prototype,"panel",2),(0,g.u2)([(0,u.i)(".dropdown__positioner")],b.prototype,"positioner",2),(0,g.u2)([(0,u.e)({type:Boolean,reflect:!0})],b.prototype,"open",2),(0,g.u2)([(0,u.e)({reflect:!0})],b.prototype,"placement",2),(0,g.u2)([(0,u.e)({type:Boolean})],b.prototype,"disabled",2),(0,g.u2)([(0,u.e)({attribute:"stay-open-on-select",type:Boolean,reflect:!0})],b.prototype,"stayOpenOnSelect",2),(0,g.u2)([(0,u.e)({attribute:!1})],b.prototype,"containingElement",2),(0,g.u2)([(0,u.e)({type:Number})],b.prototype,"distance",2),(0,g.u2)([(0,u.e)({type:Number})],b.prototype,"skidding",2),(0,g.u2)([(0,u.e)({type:Boolean})],b.prototype,"hoist",2),(0,g.u2)([(0,h.Y)("distance"),(0,h.Y)("hoist"),(0,h.Y)("placement"),(0,h.Y)("skidding")],b.prototype,"handlePopoverOptionsChange",1),(0,g.u2)([(0,h.Y)("open",{waitUntilFirstUpdate:!0})],b.prototype,"handleOpenChange",1),b=(0,g.u2)([(0,u.n)("sl-dropdown")],b),(0,l.jx)("dropdown.show",{keyframes:[{opacity:0,transform:"scale(0.9)"},{opacity:1,transform:"scale(1)"}],options:{duration:100,easing:"ease"}}),(0,l.jx)("dropdown.hide",{keyframes:[{opacity:1,transform:"scale(1)"},{opacity:0,transform:"scale(0.9)"}],options:{duration:100,easing:"ease"}})},93:(e,t,r)=>{r.d(t,{O8:()=>n,jx:()=>s});r(1703);var o=new Map,a=new WeakMap;function i(e){return null!=e?e:{keyframes:[],options:{duration:0}}}function s(e,t){o.set(e,i(t))}function n(e,t){const r=a.get(e);if(null==r?void 0:r[t])return r[t];const i=o.get(t);return i||{keyframes:[],options:{duration:0}}}},6910:(e,t,r)=>{r.d(t,{e:()=>s,e2:()=>d,i:()=>h,n:()=>a,t:()=>n});var o=r(1703),a=e=>t=>{return"function"==typeof t?(r=e,o=t,window.customElements.define(r,o),o):((e,t)=>{const{kind:r,elements:o}=t;return{kind:r,elements:o,finisher(t){window.customElements.define(e,t)}}})(e,t);var r,o},i=(e,t)=>"method"===t.kind&&t.descriptor&&!("value"in t.descriptor)?(0,o.EZ)((0,o.ih)({},t),{finisher(r){r.createProperty(t.key,e)}}):{kind:"field",key:Symbol(),placement:"own",descriptor:{},originalKey:t.key,initializer(){"function"==typeof t.initializer&&(this[t.key]=t.initializer.call(this))},finisher(r){r.createProperty(t.key,e)}};function s(e){return(t,r)=>{return void 0!==r?(o=e,a=r,void t.constructor.createProperty(a,o)):i(e,t);var o,a}}function n(e){return s((0,o.EZ)((0,o.ih)({},e),{state:!0}))}var l,c=({finisher:e,descriptor:t})=>(r,a)=>{var i;if(void 0===a){const a=null!==(i=r.originalKey)&&void 0!==i?i:r.key,s=null!=t?{kind:"method",placement:"prototype",key:a,descriptor:t(r.key)}:(0,o.EZ)((0,o.ih)({},r),{key:a});return null!=e&&(s.finisher=function(t){e(t,a)}),s}{const o=r.constructor;void 0!==t&&Object.defineProperty(r,a,t(a)),null==e||e(o,a)}};function h(e,t){return c({descriptor:r=>{const o={get(){var t,r;return null!==(r=null===(t=this.renderRoot)||void 0===t?void 0:t.querySelector(e))&&void 0!==r?r:null},enumerable:!0,configurable:!0};if(t){const t="symbol"==typeof r?Symbol():"__"+r;o.get=function(){var r,o;return void 0===this[t]&&(this[t]=null!==(o=null===(r=this.renderRoot)||void 0===r?void 0:r.querySelector(e))&&void 0!==o?o:null),this[t]}}return o}})}function d(e){return c({descriptor:t=>({async get(){var t;return await this.updateComplete,null===(t=this.renderRoot)||void 0===t?void 0:t.querySelector(e)},enumerable:!0,configurable:!0})})}null===(l=window.HTMLSlotElement)||void 0===l||l.prototype.assignedElements;
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
/**
 * @license
 * Copyright 2021 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */},9063:(e,t,r)=>{r.d(t,{o:()=>i});var o=r(6246),a=r(6557),i=(0,o.e)(class extends o.i{constructor(e){var t;if(super(e),e.type!==o.t.ATTRIBUTE||"class"!==e.name||(null===(t=e.strings)||void 0===t?void 0:t.length)>2)throw Error("`classMap()` can only be used in the `class` attribute and must be the only part in the attribute.")}render(e){return" "+Object.keys(e).filter((t=>e[t])).join(" ")+" "}update(e,[t]){var r,o;if(void 0===this.st){this.st=new Set,void 0!==e.strings&&(this.et=new Set(e.strings.join(" ").split(/\s/).filter((e=>""!==e))));for(const e in t)t[e]&&!(null===(r=this.et)||void 0===r?void 0:r.has(e))&&this.st.add(e);return this.render(t)}const i=e.element.classList;this.st.forEach((e=>{e in t||(i.remove(e),this.st.delete(e))}));for(const e in t){const r=!!t[e];r===this.st.has(e)||(null===(o=this.et)||void 0===o?void 0:o.has(e))||(r?(i.add(e),this.st.add(e)):(i.remove(e),this.st.delete(e)))}return a.b}})}
/**
 * @license
 * Copyright 2018 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */,8642:(e,t,r)=>{r.d(t,{g:()=>i});var o=r(7803),a=r(9736),i=r(6557).r`
  ${a.N}

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
  .switch:not(.switch--checked):not(.switch--disabled) .switch__input${o.v} ~ .switch__control {
    background-color: var(--sl-color-neutral-400);
    border-color: var(--sl-color-neutral-400);
  }

  .switch:not(.switch--checked):not(.switch--disabled)
    .switch__input${o.v}
    ~ .switch__control
    .switch__thumb {
    background-color: var(--sl-color-neutral-0);
    border-color: var(--sl-color-primary-600);
    box-shadow: var(--sl-focus-ring);
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
  .switch.switch--checked:not(.switch--disabled) .switch__input${o.v} ~ .switch__control {
    background-color: var(--sl-color-primary-600);
    border-color: var(--sl-color-primary-600);
  }

  .switch.switch--checked:not(.switch--disabled)
    .switch__input${o.v}
    ~ .switch__control
    .switch__thumb {
    background-color: var(--sl-color-neutral-0);
    border-color: var(--sl-color-primary-600);
    box-shadow: var(--sl-focus-ring);
  }

  /* Disabled */
  .switch--disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .switch__label {
    line-height: var(--height);
    margin-left: 0.5em;
    user-select: none;
  }
`},4093:(e,t,r)=>{r.d(t,{I:()=>a});var o=r(9736),a=r(6557).r`
  ${o.N}

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
`},2608:(e,t,r)=>{var o=r(4302),a=r(5067),i=r(7803),s=r(2288),n=r(6910),l=r(6557),c=r(1703),h=class extends l.s{constructor(){super(...arguments),this.typeToSelectString=""}firstUpdated(){this.setAttribute("role","menu")}getAllItems(e={includeDisabled:!0}){return[...this.defaultSlot.assignedElements({flatten:!0})].filter((t=>"menuitem"===t.getAttribute("role")&&!(!e.includeDisabled&&t.disabled)))}getCurrentItem(){return this.getAllItems({includeDisabled:!1}).find((e=>"0"===e.getAttribute("tabindex")))}setCurrentItem(e){const t=this.getAllItems({includeDisabled:!1}),r=e.disabled?t[0]:e;t.forEach((e=>{e.setAttribute("tabindex",e===r?"0":"-1")}))}typeToSelect(e){var t;const r=this.getAllItems({includeDisabled:!1});clearTimeout(this.typeToSelectTimeout),this.typeToSelectTimeout=window.setTimeout((()=>this.typeToSelectString=""),1e3),"Backspace"===e.key?e.metaKey||e.ctrlKey?this.typeToSelectString="":this.typeToSelectString=this.typeToSelectString.slice(0,-1):this.typeToSelectString+=e.key.toLowerCase(),i.u||r.forEach((e=>e.classList.remove("sl-focus-invisible")));for(const e of r){const r=null==(t=e.shadowRoot)?void 0:t.querySelector("slot:not([name])");if((0,a.F)(r).toLowerCase().trim().startsWith(this.typeToSelectString)){this.setCurrentItem(e),e.focus();break}}}handleClick(e){const t=e.target.closest("sl-menu-item");!1===(null==t?void 0:t.disabled)&&(0,s.j)(this,"sl-select",{detail:{item:t}})}handleKeyUp(){if(!i.u){this.getAllItems().forEach((e=>{e.classList.remove("sl-focus-invisible")}))}}handleKeyDown(e){if("Enter"===e.key){const t=this.getCurrentItem();e.preventDefault(),null==t||t.click()}if(" "===e.key&&e.preventDefault(),["ArrowDown","ArrowUp","Home","End"].includes(e.key)){const t=this.getAllItems({includeDisabled:!1}),r=this.getCurrentItem();let o=r?t.indexOf(r):0;if(t.length>0)return e.preventDefault(),"ArrowDown"===e.key?o++:"ArrowUp"===e.key?o--:"Home"===e.key?o=0:"End"===e.key&&(o=t.length-1),o<0&&(o=t.length-1),o>t.length-1&&(o=0),this.setCurrentItem(t[o]),void t[o].focus()}this.typeToSelect(e)}handleMouseDown(e){const t=e.target;"menuitem"===t.getAttribute("role")&&(this.setCurrentItem(t),i.u||t.classList.add("sl-focus-invisible"))}handleSlotChange(){const e=this.getAllItems({includeDisabled:!1});e.length>0&&this.setCurrentItem(e[0])}render(){return l.$`
      <div
        part="base"
        class="menu"
        @click=${this.handleClick}
        @keydown=${this.handleKeyDown}
        @keyup=${this.handleKeyUp}
        @mousedown=${this.handleMouseDown}
      >
        <slot @slotchange=${this.handleSlotChange}></slot>
      </div>
    `}};h.styles=o.B,(0,c.u2)([(0,n.i)(".menu")],h.prototype,"menu",2),(0,c.u2)([(0,n.i)("slot")],h.prototype,"defaultSlot",2),h=(0,c.u2)([(0,n.n)("sl-menu")],h)},1032:(e,t,r)=>{r.d(t,{k:()=>i});var o=r(7803),a=r(9736),i=r(6557).r`
  ${a.N}

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

  :host(:not([disabled])) .divider${o.v} {
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
`},7105:(e,t,r)=>{var o=r(3096),a=r(6910),i=r(6557),s=r(1703),n=class extends i.s{render(){return i.$` <slot></slot> `}};n.styles=o.Y,n=(0,s.u2)([(0,a.n)("sl-visually-hidden")],n)},146:(e,t,r)=>{var o=r(674),a=r(2676),i=r(9138),s=r(3895),n=r(5868),l=r(5631),c=r(1167),h=r(9063),d=r(8830),u=r(6645),p=r(2288),g=r(6910),b=r(6557),m=r(1703),v=(0,m.Ee)({"node_modules/color-name/index.js"(e,t){t.exports={aliceblue:[240,248,255],antiquewhite:[250,235,215],aqua:[0,255,255],aquamarine:[127,255,212],azure:[240,255,255],beige:[245,245,220],bisque:[255,228,196],black:[0,0,0],blanchedalmond:[255,235,205],blue:[0,0,255],blueviolet:[138,43,226],brown:[165,42,42],burlywood:[222,184,135],cadetblue:[95,158,160],chartreuse:[127,255,0],chocolate:[210,105,30],coral:[255,127,80],cornflowerblue:[100,149,237],cornsilk:[255,248,220],crimson:[220,20,60],cyan:[0,255,255],darkblue:[0,0,139],darkcyan:[0,139,139],darkgoldenrod:[184,134,11],darkgray:[169,169,169],darkgreen:[0,100,0],darkgrey:[169,169,169],darkkhaki:[189,183,107],darkmagenta:[139,0,139],darkolivegreen:[85,107,47],darkorange:[255,140,0],darkorchid:[153,50,204],darkred:[139,0,0],darksalmon:[233,150,122],darkseagreen:[143,188,143],darkslateblue:[72,61,139],darkslategray:[47,79,79],darkslategrey:[47,79,79],darkturquoise:[0,206,209],darkviolet:[148,0,211],deeppink:[255,20,147],deepskyblue:[0,191,255],dimgray:[105,105,105],dimgrey:[105,105,105],dodgerblue:[30,144,255],firebrick:[178,34,34],floralwhite:[255,250,240],forestgreen:[34,139,34],fuchsia:[255,0,255],gainsboro:[220,220,220],ghostwhite:[248,248,255],gold:[255,215,0],goldenrod:[218,165,32],gray:[128,128,128],green:[0,128,0],greenyellow:[173,255,47],grey:[128,128,128],honeydew:[240,255,240],hotpink:[255,105,180],indianred:[205,92,92],indigo:[75,0,130],ivory:[255,255,240],khaki:[240,230,140],lavender:[230,230,250],lavenderblush:[255,240,245],lawngreen:[124,252,0],lemonchiffon:[255,250,205],lightblue:[173,216,230],lightcoral:[240,128,128],lightcyan:[224,255,255],lightgoldenrodyellow:[250,250,210],lightgray:[211,211,211],lightgreen:[144,238,144],lightgrey:[211,211,211],lightpink:[255,182,193],lightsalmon:[255,160,122],lightseagreen:[32,178,170],lightskyblue:[135,206,250],lightslategray:[119,136,153],lightslategrey:[119,136,153],lightsteelblue:[176,196,222],lightyellow:[255,255,224],lime:[0,255,0],limegreen:[50,205,50],linen:[250,240,230],magenta:[255,0,255],maroon:[128,0,0],mediumaquamarine:[102,205,170],mediumblue:[0,0,205],mediumorchid:[186,85,211],mediumpurple:[147,112,219],mediumseagreen:[60,179,113],mediumslateblue:[123,104,238],mediumspringgreen:[0,250,154],mediumturquoise:[72,209,204],mediumvioletred:[199,21,133],midnightblue:[25,25,112],mintcream:[245,255,250],mistyrose:[255,228,225],moccasin:[255,228,181],navajowhite:[255,222,173],navy:[0,0,128],oldlace:[253,245,230],olive:[128,128,0],olivedrab:[107,142,35],orange:[255,165,0],orangered:[255,69,0],orchid:[218,112,214],palegoldenrod:[238,232,170],palegreen:[152,251,152],paleturquoise:[175,238,238],palevioletred:[219,112,147],papayawhip:[255,239,213],peachpuff:[255,218,185],peru:[205,133,63],pink:[255,192,203],plum:[221,160,221],powderblue:[176,224,230],purple:[128,0,128],rebeccapurple:[102,51,153],red:[255,0,0],rosybrown:[188,143,143],royalblue:[65,105,225],saddlebrown:[139,69,19],salmon:[250,128,114],sandybrown:[244,164,96],seagreen:[46,139,87],seashell:[255,245,238],sienna:[160,82,45],silver:[192,192,192],skyblue:[135,206,235],slateblue:[106,90,205],slategray:[112,128,144],slategrey:[112,128,144],snow:[255,250,250],springgreen:[0,255,127],steelblue:[70,130,180],tan:[210,180,140],teal:[0,128,128],thistle:[216,191,216],tomato:[255,99,71],turquoise:[64,224,208],violet:[238,130,238],wheat:[245,222,179],white:[255,255,255],whitesmoke:[245,245,245],yellow:[255,255,0],yellowgreen:[154,205,50]}}}),f=(0,m.Ee)({"node_modules/simple-swizzle/node_modules/is-arrayish/index.js"(e,t){t.exports=function(e){return!(!e||"string"==typeof e)&&(e instanceof Array||Array.isArray(e)||e.length>=0&&(e.splice instanceof Function||Object.getOwnPropertyDescriptor(e,e.length-1)&&"String"!==e.constructor.name))}}}),y=(0,m.Ee)({"node_modules/simple-swizzle/index.js"(e,t){var r=f(),o=Array.prototype.concat,a=Array.prototype.slice,i=t.exports=function(e){for(var t=[],i=0,s=e.length;i<s;i++){var n=e[i];r(n)?t=o.call(t,a.call(n)):t.push(n)}return t};i.wrap=function(e){return function(){return e(i(arguments))}}}}),w=(0,m.Ee)({"node_modules/color-string/index.js"(e,t){var r,o=v(),a=y(),i=Object.hasOwnProperty,s={};for(r in o)i.call(o,r)&&(s[o[r]]=r);var n=t.exports={to:{},get:{}};function l(e,t,r){return Math.min(Math.max(t,e),r)}function c(e){var t=Math.round(e).toString(16).toUpperCase();return t.length<2?"0"+t:t}n.get=function(e){var t,r;switch(e.substring(0,3).toLowerCase()){case"hsl":t=n.get.hsl(e),r="hsl";break;case"hwb":t=n.get.hwb(e),r="hwb";break;default:t=n.get.rgb(e),r="rgb"}return t?{model:r,value:t}:null},n.get.rgb=function(e){if(!e)return null;var t,r,a,s=[0,0,0,1];if(t=e.match(/^#([a-f0-9]{6})([a-f0-9]{2})?$/i)){for(a=t[2],t=t[1],r=0;r<3;r++){var n=2*r;s[r]=parseInt(t.slice(n,n+2),16)}a&&(s[3]=parseInt(a,16)/255)}else if(t=e.match(/^#([a-f0-9]{3,4})$/i)){for(a=(t=t[1])[3],r=0;r<3;r++)s[r]=parseInt(t[r]+t[r],16);a&&(s[3]=parseInt(a+a,16)/255)}else if(t=e.match(/^rgba?\(\s*([+-]?\d+)(?=[\s,])\s*(?:,\s*)?([+-]?\d+)(?=[\s,])\s*(?:,\s*)?([+-]?\d+)\s*(?:[,|\/]\s*([+-]?[\d\.]+)(%?)\s*)?\)$/)){for(r=0;r<3;r++)s[r]=parseInt(t[r+1],0);t[4]&&(t[5]?s[3]=.01*parseFloat(t[4]):s[3]=parseFloat(t[4]))}else{if(!(t=e.match(/^rgba?\(\s*([+-]?[\d\.]+)\%\s*,?\s*([+-]?[\d\.]+)\%\s*,?\s*([+-]?[\d\.]+)\%\s*(?:[,|\/]\s*([+-]?[\d\.]+)(%?)\s*)?\)$/)))return(t=e.match(/^(\w+)$/))?"transparent"===t[1]?[0,0,0,0]:i.call(o,t[1])?((s=o[t[1]])[3]=1,s):null:null;for(r=0;r<3;r++)s[r]=Math.round(2.55*parseFloat(t[r+1]));t[4]&&(t[5]?s[3]=.01*parseFloat(t[4]):s[3]=parseFloat(t[4]))}for(r=0;r<3;r++)s[r]=l(s[r],0,255);return s[3]=l(s[3],0,1),s},n.get.hsl=function(e){if(!e)return null;var t=e.match(/^hsla?\(\s*([+-]?(?:\d{0,3}\.)?\d+)(?:deg)?\s*,?\s*([+-]?[\d\.]+)%\s*,?\s*([+-]?[\d\.]+)%\s*(?:[,|\/]\s*([+-]?(?=\.\d|\d)(?:0|[1-9]\d*)?(?:\.\d*)?(?:[eE][+-]?\d+)?)\s*)?\)$/);if(t){var r=parseFloat(t[4]);return[(parseFloat(t[1])%360+360)%360,l(parseFloat(t[2]),0,100),l(parseFloat(t[3]),0,100),l(isNaN(r)?1:r,0,1)]}return null},n.get.hwb=function(e){if(!e)return null;var t=e.match(/^hwb\(\s*([+-]?\d{0,3}(?:\.\d+)?)(?:deg)?\s*,\s*([+-]?[\d\.]+)%\s*,\s*([+-]?[\d\.]+)%\s*(?:,\s*([+-]?(?=\.\d|\d)(?:0|[1-9]\d*)?(?:\.\d*)?(?:[eE][+-]?\d+)?)\s*)?\)$/);if(t){var r=parseFloat(t[4]);return[(parseFloat(t[1])%360+360)%360,l(parseFloat(t[2]),0,100),l(parseFloat(t[3]),0,100),l(isNaN(r)?1:r,0,1)]}return null},n.to.hex=function(){var e=a(arguments);return"#"+c(e[0])+c(e[1])+c(e[2])+(e[3]<1?c(Math.round(255*e[3])):"")},n.to.rgb=function(){var e=a(arguments);return e.length<4||1===e[3]?"rgb("+Math.round(e[0])+", "+Math.round(e[1])+", "+Math.round(e[2])+")":"rgba("+Math.round(e[0])+", "+Math.round(e[1])+", "+Math.round(e[2])+", "+e[3]+")"},n.to.rgb.percent=function(){var e=a(arguments),t=Math.round(e[0]/255*100),r=Math.round(e[1]/255*100),o=Math.round(e[2]/255*100);return e.length<4||1===e[3]?"rgb("+t+"%, "+r+"%, "+o+"%)":"rgba("+t+"%, "+r+"%, "+o+"%, "+e[3]+")"},n.to.hsl=function(){var e=a(arguments);return e.length<4||1===e[3]?"hsl("+e[0]+", "+e[1]+"%, "+e[2]+"%)":"hsla("+e[0]+", "+e[1]+"%, "+e[2]+"%, "+e[3]+")"},n.to.hwb=function(){var e=a(arguments),t="";return e.length>=4&&1!==e[3]&&(t=", "+e[3]),"hwb("+e[0]+", "+e[1]+"%, "+e[2]+"%"+t+")"},n.to.keyword=function(e){return s[e.slice(0,3)]}}}),_=(0,m.Ee)({"node_modules/color-convert/conversions.js"(e,t){var r=v(),o={};for(const e of Object.keys(r))o[r[e]]=e;var a={rgb:{channels:3,labels:"rgb"},hsl:{channels:3,labels:"hsl"},hsv:{channels:3,labels:"hsv"},hwb:{channels:3,labels:"hwb"},cmyk:{channels:4,labels:"cmyk"},xyz:{channels:3,labels:"xyz"},lab:{channels:3,labels:"lab"},lch:{channels:3,labels:"lch"},hex:{channels:1,labels:["hex"]},keyword:{channels:1,labels:["keyword"]},ansi16:{channels:1,labels:["ansi16"]},ansi256:{channels:1,labels:["ansi256"]},hcg:{channels:3,labels:["h","c","g"]},apple:{channels:3,labels:["r16","g16","b16"]},gray:{channels:1,labels:["gray"]}};t.exports=a;for(const e of Object.keys(a)){if(!("channels"in a[e]))throw new Error("missing channels property: "+e);if(!("labels"in a[e]))throw new Error("missing channel labels property: "+e);if(a[e].labels.length!==a[e].channels)throw new Error("channel and label counts mismatch: "+e);const{channels:t,labels:r}=a[e];delete a[e].channels,delete a[e].labels,Object.defineProperty(a[e],"channels",{value:t}),Object.defineProperty(a[e],"labels",{value:r})}a.rgb.hsl=function(e){const t=e[0]/255,r=e[1]/255,o=e[2]/255,a=Math.min(t,r,o),i=Math.max(t,r,o),s=i-a;let n,l;i===a?n=0:t===i?n=(r-o)/s:r===i?n=2+(o-t)/s:o===i&&(n=4+(t-r)/s),n=Math.min(60*n,360),n<0&&(n+=360);const c=(a+i)/2;return l=i===a?0:c<=.5?s/(i+a):s/(2-i-a),[n,100*l,100*c]},a.rgb.hsv=function(e){let t,r,o,a,i;const s=e[0]/255,n=e[1]/255,l=e[2]/255,c=Math.max(s,n,l),h=c-Math.min(s,n,l),d=function(e){return(c-e)/6/h+.5};return 0===h?(a=0,i=0):(i=h/c,t=d(s),r=d(n),o=d(l),s===c?a=o-r:n===c?a=1/3+t-o:l===c&&(a=2/3+r-t),a<0?a+=1:a>1&&(a-=1)),[360*a,100*i,100*c]},a.rgb.hwb=function(e){const t=e[0],r=e[1];let o=e[2];const i=a.rgb.hsl(e)[0],s=1/255*Math.min(t,Math.min(r,o));return o=1-1/255*Math.max(t,Math.max(r,o)),[i,100*s,100*o]},a.rgb.cmyk=function(e){const t=e[0]/255,r=e[1]/255,o=e[2]/255,a=Math.min(1-t,1-r,1-o);return[100*((1-t-a)/(1-a)||0),100*((1-r-a)/(1-a)||0),100*((1-o-a)/(1-a)||0),100*a]},a.rgb.keyword=function(e){const t=o[e];if(t)return t;let a,i=1/0;for(const t of Object.keys(r)){const o=r[t],l=(n=o,((s=e)[0]-n[0])**2+(s[1]-n[1])**2+(s[2]-n[2])**2);l<i&&(i=l,a=t)}var s,n;return a},a.keyword.rgb=function(e){return r[e]},a.rgb.xyz=function(e){let t=e[0]/255,r=e[1]/255,o=e[2]/255;t=t>.04045?((t+.055)/1.055)**2.4:t/12.92,r=r>.04045?((r+.055)/1.055)**2.4:r/12.92,o=o>.04045?((o+.055)/1.055)**2.4:o/12.92;return[100*(.4124*t+.3576*r+.1805*o),100*(.2126*t+.7152*r+.0722*o),100*(.0193*t+.1192*r+.9505*o)]},a.rgb.lab=function(e){const t=a.rgb.xyz(e);let r=t[0],o=t[1],i=t[2];r/=95.047,o/=100,i/=108.883,r=r>.008856?r**(1/3):7.787*r+16/116,o=o>.008856?o**(1/3):7.787*o+16/116,i=i>.008856?i**(1/3):7.787*i+16/116;return[116*o-16,500*(r-o),200*(o-i)]},a.hsl.rgb=function(e){const t=e[0]/360,r=e[1]/100,o=e[2]/100;let a,i,s;if(0===r)return s=255*o,[s,s,s];a=o<.5?o*(1+r):o+r-o*r;const n=2*o-a,l=[0,0,0];for(let e=0;e<3;e++)i=t+1/3*-(e-1),i<0&&i++,i>1&&i--,s=6*i<1?n+6*(a-n)*i:2*i<1?a:3*i<2?n+(a-n)*(2/3-i)*6:n,l[e]=255*s;return l},a.hsl.hsv=function(e){const t=e[0];let r=e[1]/100,o=e[2]/100,a=r;const i=Math.max(o,.01);o*=2,r*=o<=1?o:2-o,a*=i<=1?i:2-i;return[t,100*(0===o?2*a/(i+a):2*r/(o+r)),100*((o+r)/2)]},a.hsv.rgb=function(e){const t=e[0]/60,r=e[1]/100;let o=e[2]/100;const a=Math.floor(t)%6,i=t-Math.floor(t),s=255*o*(1-r),n=255*o*(1-r*i),l=255*o*(1-r*(1-i));switch(o*=255,a){case 0:return[o,l,s];case 1:return[n,o,s];case 2:return[s,o,l];case 3:return[s,n,o];case 4:return[l,s,o];case 5:return[o,s,n]}},a.hsv.hsl=function(e){const t=e[0],r=e[1]/100,o=e[2]/100,a=Math.max(o,.01);let i,s;s=(2-r)*o;const n=(2-r)*a;return i=r*a,i/=n<=1?n:2-n,i=i||0,s/=2,[t,100*i,100*s]},a.hwb.rgb=function(e){const t=e[0]/360;let r=e[1]/100,o=e[2]/100;const a=r+o;let i;a>1&&(r/=a,o/=a);const s=Math.floor(6*t),n=1-o;i=6*t-s,0!=(1&s)&&(i=1-i);const l=r+i*(n-r);let c,h,d;switch(s){default:case 6:case 0:c=n,h=l,d=r;break;case 1:c=l,h=n,d=r;break;case 2:c=r,h=n,d=l;break;case 3:c=r,h=l,d=n;break;case 4:c=l,h=r,d=n;break;case 5:c=n,h=r,d=l}return[255*c,255*h,255*d]},a.cmyk.rgb=function(e){const t=e[0]/100,r=e[1]/100,o=e[2]/100,a=e[3]/100;return[255*(1-Math.min(1,t*(1-a)+a)),255*(1-Math.min(1,r*(1-a)+a)),255*(1-Math.min(1,o*(1-a)+a))]},a.xyz.rgb=function(e){const t=e[0]/100,r=e[1]/100,o=e[2]/100;let a,i,s;return a=3.2406*t+-1.5372*r+-.4986*o,i=-.9689*t+1.8758*r+.0415*o,s=.0557*t+-.204*r+1.057*o,a=a>.0031308?1.055*a**(1/2.4)-.055:12.92*a,i=i>.0031308?1.055*i**(1/2.4)-.055:12.92*i,s=s>.0031308?1.055*s**(1/2.4)-.055:12.92*s,a=Math.min(Math.max(0,a),1),i=Math.min(Math.max(0,i),1),s=Math.min(Math.max(0,s),1),[255*a,255*i,255*s]},a.xyz.lab=function(e){let t=e[0],r=e[1],o=e[2];t/=95.047,r/=100,o/=108.883,t=t>.008856?t**(1/3):7.787*t+16/116,r=r>.008856?r**(1/3):7.787*r+16/116,o=o>.008856?o**(1/3):7.787*o+16/116;return[116*r-16,500*(t-r),200*(r-o)]},a.lab.xyz=function(e){let t,r,o;r=(e[0]+16)/116,t=e[1]/500+r,o=r-e[2]/200;const a=r**3,i=t**3,s=o**3;return r=a>.008856?a:(r-16/116)/7.787,t=i>.008856?i:(t-16/116)/7.787,o=s>.008856?s:(o-16/116)/7.787,t*=95.047,r*=100,o*=108.883,[t,r,o]},a.lab.lch=function(e){const t=e[0],r=e[1],o=e[2];let a;a=360*Math.atan2(o,r)/2/Math.PI,a<0&&(a+=360);return[t,Math.sqrt(r*r+o*o),a]},a.lch.lab=function(e){const t=e[0],r=e[1],o=e[2]/360*2*Math.PI;return[t,r*Math.cos(o),r*Math.sin(o)]},a.rgb.ansi16=function(e,t=null){const[r,o,i]=e;let s=null===t?a.rgb.hsv(e)[2]:t;if(s=Math.round(s/50),0===s)return 30;let n=30+(Math.round(i/255)<<2|Math.round(o/255)<<1|Math.round(r/255));return 2===s&&(n+=60),n},a.hsv.ansi16=function(e){return a.rgb.ansi16(a.hsv.rgb(e),e[2])},a.rgb.ansi256=function(e){const t=e[0],r=e[1],o=e[2];if(t===r&&r===o)return t<8?16:t>248?231:Math.round((t-8)/247*24)+232;return 16+36*Math.round(t/255*5)+6*Math.round(r/255*5)+Math.round(o/255*5)},a.ansi16.rgb=function(e){let t=e%10;if(0===t||7===t)return e>50&&(t+=3.5),t=t/10.5*255,[t,t,t];const r=.5*(1+~~(e>50));return[(1&t)*r*255,(t>>1&1)*r*255,(t>>2&1)*r*255]},a.ansi256.rgb=function(e){if(e>=232){const t=10*(e-232)+8;return[t,t,t]}let t;e-=16;return[Math.floor(e/36)/5*255,Math.floor((t=e%36)/6)/5*255,t%6/5*255]},a.rgb.hex=function(e){const t=(((255&Math.round(e[0]))<<16)+((255&Math.round(e[1]))<<8)+(255&Math.round(e[2]))).toString(16).toUpperCase();return"000000".substring(t.length)+t},a.hex.rgb=function(e){const t=e.toString(16).match(/[a-f0-9]{6}|[a-f0-9]{3}/i);if(!t)return[0,0,0];let r=t[0];3===t[0].length&&(r=r.split("").map((e=>e+e)).join(""));const o=parseInt(r,16);return[o>>16&255,o>>8&255,255&o]},a.rgb.hcg=function(e){const t=e[0]/255,r=e[1]/255,o=e[2]/255,a=Math.max(Math.max(t,r),o),i=Math.min(Math.min(t,r),o),s=a-i;let n,l;return n=s<1?i/(1-s):0,l=s<=0?0:a===t?(r-o)/s%6:a===r?2+(o-t)/s:4+(t-r)/s,l/=6,l%=1,[360*l,100*s,100*n]},a.hsl.hcg=function(e){const t=e[1]/100,r=e[2]/100,o=r<.5?2*t*r:2*t*(1-r);let a=0;return o<1&&(a=(r-.5*o)/(1-o)),[e[0],100*o,100*a]},a.hsv.hcg=function(e){const t=e[1]/100,r=e[2]/100,o=t*r;let a=0;return o<1&&(a=(r-o)/(1-o)),[e[0],100*o,100*a]},a.hcg.rgb=function(e){const t=e[0]/360,r=e[1]/100,o=e[2]/100;if(0===r)return[255*o,255*o,255*o];const a=[0,0,0],i=t%1*6,s=i%1,n=1-s;let l=0;switch(Math.floor(i)){case 0:a[0]=1,a[1]=s,a[2]=0;break;case 1:a[0]=n,a[1]=1,a[2]=0;break;case 2:a[0]=0,a[1]=1,a[2]=s;break;case 3:a[0]=0,a[1]=n,a[2]=1;break;case 4:a[0]=s,a[1]=0,a[2]=1;break;default:a[0]=1,a[1]=0,a[2]=n}return l=(1-r)*o,[255*(r*a[0]+l),255*(r*a[1]+l),255*(r*a[2]+l)]},a.hcg.hsv=function(e){const t=e[1]/100,r=t+e[2]/100*(1-t);let o=0;return r>0&&(o=t/r),[e[0],100*o,100*r]},a.hcg.hsl=function(e){const t=e[1]/100,r=e[2]/100*(1-t)+.5*t;let o=0;return r>0&&r<.5?o=t/(2*r):r>=.5&&r<1&&(o=t/(2*(1-r))),[e[0],100*o,100*r]},a.hcg.hwb=function(e){const t=e[1]/100,r=t+e[2]/100*(1-t);return[e[0],100*(r-t),100*(1-r)]},a.hwb.hcg=function(e){const t=e[1]/100,r=1-e[2]/100,o=r-t;let a=0;return o<1&&(a=(r-o)/(1-o)),[e[0],100*o,100*a]},a.apple.rgb=function(e){return[e[0]/65535*255,e[1]/65535*255,e[2]/65535*255]},a.rgb.apple=function(e){return[e[0]/255*65535,e[1]/255*65535,e[2]/255*65535]},a.gray.rgb=function(e){return[e[0]/100*255,e[0]/100*255,e[0]/100*255]},a.gray.hsl=function(e){return[0,0,e[0]]},a.gray.hsv=a.gray.hsl,a.gray.hwb=function(e){return[0,100,e[0]]},a.gray.cmyk=function(e){return[0,0,0,e[0]]},a.gray.lab=function(e){return[e[0],0,0]},a.gray.hex=function(e){const t=255&Math.round(e[0]/100*255),r=((t<<16)+(t<<8)+t).toString(16).toUpperCase();return"000000".substring(r.length)+r},a.rgb.gray=function(e){return[(e[0]+e[1]+e[2])/3/255*100]}}}),k=(0,m.Ee)({"node_modules/color-convert/route.js"(e,t){var r=_();function o(e){const t=function(){const e={},t=Object.keys(r);for(let r=t.length,o=0;o<r;o++)e[t[o]]={distance:-1,parent:null};return e}(),o=[e];for(t[e].distance=0;o.length;){const e=o.pop(),a=Object.keys(r[e]);for(let r=a.length,i=0;i<r;i++){const r=a[i],s=t[r];-1===s.distance&&(s.distance=t[e].distance+1,s.parent=e,o.unshift(r))}}return t}function a(e,t){return function(r){return t(e(r))}}function i(e,t){const o=[t[e].parent,e];let i=r[t[e].parent][e],s=t[e].parent;for(;t[s].parent;)o.unshift(t[s].parent),i=a(r[t[s].parent][s],i),s=t[s].parent;return i.conversion=o,i}t.exports=function(e){const t=o(e),r={},a=Object.keys(t);for(let e=a.length,o=0;o<e;o++){const e=a[o];null!==t[e].parent&&(r[e]=i(e,t))}return r}}}),x=(0,m.Ee)({"node_modules/color-convert/index.js"(e,t){var r=_(),o=k(),a={};Object.keys(r).forEach((e=>{a[e]={},Object.defineProperty(a[e],"channels",{value:r[e].channels}),Object.defineProperty(a[e],"labels",{value:r[e].labels});const t=o(e);Object.keys(t).forEach((r=>{const o=t[r];a[e][r]=function(e){const t=function(...t){const r=t[0];if(null==r)return r;r.length>1&&(t=r);const o=e(t);if("object"==typeof o)for(let e=o.length,t=0;t<e;t++)o[t]=Math.round(o[t]);return o};return"conversion"in e&&(t.conversion=e.conversion),t}(o),a[e][r].raw=function(e){const t=function(...t){const r=t[0];return null==r?r:(r.length>1&&(t=r),e(t))};return"conversion"in e&&(t.conversion=e.conversion),t}(o)}))})),t.exports=a}}),$=(0,m.Ee)({"node_modules/color/index.js"(e,t){var r=w(),o=x(),a=[].slice,i=["keyword","gray","hex"],s={};for(const e of Object.keys(o))s[a.call(o[e].labels).sort().join("")]=e;var n={};function l(e,t){if(!(this instanceof l))return new l(e,t);if(t&&t in i&&(t=null),t&&!(t in o))throw new Error("Unknown model: "+t);let c,h;if(null==e)this.model="rgb",this.color=[0,0,0],this.valpha=1;else if(e instanceof l)this.model=e.model,this.color=e.color.slice(),this.valpha=e.valpha;else if("string"==typeof e){const t=r.get(e);if(null===t)throw new Error("Unable to parse color from string: "+e);this.model=t.model,h=o[this.model].channels,this.color=t.value.slice(0,h),this.valpha="number"==typeof t.value[h]?t.value[h]:1}else if(e.length>0){this.model=t||"rgb",h=o[this.model].channels;const r=a.call(e,0,h);this.color=u(r,h),this.valpha="number"==typeof e[h]?e[h]:1}else if("number"==typeof e)this.model="rgb",this.color=[e>>16&255,e>>8&255,255&e],this.valpha=1;else{this.valpha=1;const t=Object.keys(e);"alpha"in e&&(t.splice(t.indexOf("alpha"),1),this.valpha="number"==typeof e.alpha?e.alpha:0);const r=t.sort().join("");if(!(r in s))throw new Error("Unable to parse color from object: "+JSON.stringify(e));this.model=s[r];const a=o[this.model].labels,i=[];for(c=0;c<a.length;c++)i.push(e[a[c]]);this.color=u(i)}if(n[this.model])for(h=o[this.model].channels,c=0;c<h;c++){const e=n[this.model][c];e&&(this.color[c]=e(this.color[c]))}this.valpha=Math.max(0,Math.min(1,this.valpha)),Object.freeze&&Object.freeze(this)}l.prototype={toString(){return this.string()},toJSON(){return this[this.model]()},string(e){let t=this.model in r.to?this:this.rgb();t=t.round("number"==typeof e?e:1);const o=1===t.valpha?t.color:t.color.concat(this.valpha);return r.to[t.model](o)},percentString(e){const t=this.rgb().round("number"==typeof e?e:1),o=1===t.valpha?t.color:t.color.concat(this.valpha);return r.to.rgb.percent(o)},array(){return 1===this.valpha?this.color.slice():this.color.concat(this.valpha)},object(){const e={},t=o[this.model].channels,r=o[this.model].labels;for(let o=0;o<t;o++)e[r[o]]=this.color[o];return 1!==this.valpha&&(e.alpha=this.valpha),e},unitArray(){const e=this.rgb().color;return e[0]/=255,e[1]/=255,e[2]/=255,1!==this.valpha&&e.push(this.valpha),e},unitObject(){const e=this.rgb().object();return e.r/=255,e.g/=255,e.b/=255,1!==this.valpha&&(e.alpha=this.valpha),e},round(e){return e=Math.max(e||0,0),new l(this.color.map(function(e){return function(t){return function(e,t){return Number(e.toFixed(t))}(t,e)}}(e)).concat(this.valpha),this.model)},alpha(e){return arguments.length>0?new l(this.color.concat(Math.max(0,Math.min(1,e))),this.model):this.valpha},red:c("rgb",0,h(255)),green:c("rgb",1,h(255)),blue:c("rgb",2,h(255)),hue:c(["hsl","hsv","hsl","hwb","hcg"],0,(e=>(e%360+360)%360)),saturationl:c("hsl",1,h(100)),lightness:c("hsl",2,h(100)),saturationv:c("hsv",1,h(100)),value:c("hsv",2,h(100)),chroma:c("hcg",1,h(100)),gray:c("hcg",2,h(100)),white:c("hwb",1,h(100)),wblack:c("hwb",2,h(100)),cyan:c("cmyk",0,h(100)),magenta:c("cmyk",1,h(100)),yellow:c("cmyk",2,h(100)),black:c("cmyk",3,h(100)),x:c("xyz",0,h(100)),y:c("xyz",1,h(100)),z:c("xyz",2,h(100)),l:c("lab",0,h(100)),a:c("lab",1),b:c("lab",2),keyword(e){return arguments.length>0?new l(e):o[this.model].keyword(this.color)},hex(e){return arguments.length>0?new l(e):r.to.hex(this.rgb().round().color)},hexa(e){if(arguments.length>0)return new l(e);const t=this.rgb().round().color;let o=Math.round(255*this.valpha).toString(16).toUpperCase();return 1===o.length&&(o="0"+o),r.to.hex(t)+o},rgbNumber(){const e=this.rgb().color;return(255&e[0])<<16|(255&e[1])<<8|255&e[2]},luminosity(){const e=this.rgb().color,t=[];for(const[r,o]of e.entries()){const e=o/255;t[r]=e<=.03928?e/12.92:((e+.055)/1.055)**2.4}return.2126*t[0]+.7152*t[1]+.0722*t[2]},contrast(e){const t=this.luminosity(),r=e.luminosity();return t>r?(t+.05)/(r+.05):(r+.05)/(t+.05)},level(e){const t=this.contrast(e);return t>=7.1?"AAA":t>=4.5?"AA":""},isDark(){const e=this.rgb().color;return(299*e[0]+587*e[1]+114*e[2])/1e3<128},isLight(){return!this.isDark()},negate(){const e=this.rgb();for(let t=0;t<3;t++)e.color[t]=255-e.color[t];return e},lighten(e){const t=this.hsl();return t.color[2]+=t.color[2]*e,t},darken(e){const t=this.hsl();return t.color[2]-=t.color[2]*e,t},saturate(e){const t=this.hsl();return t.color[1]+=t.color[1]*e,t},desaturate(e){const t=this.hsl();return t.color[1]-=t.color[1]*e,t},whiten(e){const t=this.hwb();return t.color[1]+=t.color[1]*e,t},blacken(e){const t=this.hwb();return t.color[2]+=t.color[2]*e,t},grayscale(){const e=this.rgb().color,t=.3*e[0]+.59*e[1]+.11*e[2];return l.rgb(t,t,t)},fade(e){return this.alpha(this.valpha-this.valpha*e)},opaquer(e){return this.alpha(this.valpha+this.valpha*e)},rotate(e){const t=this.hsl();let r=t.color[0];return r=(r+e)%360,r=r<0?360+r:r,t.color[0]=r,t},mix(e,t){if(!e||!e.rgb)throw new Error('Argument to "mix" was not a Color instance, but rather an instance of '+typeof e);const r=e.rgb(),o=this.rgb(),a=void 0===t?.5:t,i=2*a-1,s=r.alpha()-o.alpha(),n=((i*s==-1?i:(i+s)/(1+i*s))+1)/2,c=1-n;return l.rgb(n*r.red()+c*o.red(),n*r.green()+c*o.green(),n*r.blue()+c*o.blue(),r.alpha()*a+o.alpha()*(1-a))}};for(const e of Object.keys(o)){if(i.includes(e))continue;const t=o[e].channels;l.prototype[e]=function(){if(this.model===e)return new l(this);if(arguments.length>0)return new l(arguments,e);const r="number"==typeof arguments[t]?t:this.valpha;return new l(d(o[this.model][e].raw(this.color)).concat(r),e)},l[e]=function(r){return"number"==typeof r&&(r=u(a.call(arguments),t)),new l(r,e)}}function c(e,t,r){e=Array.isArray(e)?e:[e];for(const o of e)(n[o]||(n[o]=[]))[t]=r;return e=e[0],function(o){let a;return arguments.length>0?(r&&(o=r(o)),a=this[e](),a.color[t]=o,a):(a=this[e]().color[t],r&&(a=r(a)),a)}}function h(e){return function(t){return Math.max(0,Math.min(e,t))}}function d(e){return Array.isArray(e)?e:[e]}function u(e,t){for(let r=0;r<t;r++)"number"!=typeof e[r]&&(e[r]=0);return e}t.exports=l}}),C=(0,m.v)($(),1),M="EyeDropper"in window,E=class extends b.s{constructor(){super(...arguments),this.formSubmitController=new c.K(this),this.isSafeValue=!1,this.localize=new n.Ve(this),this.inputValue="",this.hue=0,this.saturation=100,this.lightness=100,this.alpha=100,this.value="#ffffff",this.label="",this.format="hex",this.inline=!1,this.size="medium",this.noFormatToggle=!1,this.name="",this.disabled=!1,this.invalid=!1,this.hoist=!1,this.opacity=!1,this.uppercase=!1,this.swatches=["#d0021b","#f5a623","#f8e71c","#8b572a","#7ed321","#417505","#bd10e0","#9013fe","#4a90e2","#50e3c2","#b8e986","#000","#444","#888","#ccc","#fff"]}connectedCallback(){super.connectedCallback(),this.setColor(this.value)||this.setColor("#ffff"),this.inputValue=this.value,this.lastValueEmitted=this.value,this.syncValues()}getFormattedValue(e="hex"){const t=this.parseColor(`hsla(${this.hue}, ${this.saturation}%, ${this.lightness}%, ${this.alpha/100})`);if(null===t)return"";switch(e){case"hex":return t.hex;case"hexa":return t.hexa;case"rgb":return t.rgb.string;case"rgba":return t.rgba.string;case"hsl":return t.hsl.string;case"hsla":return t.hsla.string;default:return""}}reportValidity(){return!this.inline&&this.input.invalid?new Promise((e=>{this.dropdown.addEventListener("sl-after-show",(()=>{this.input.reportValidity(),e()}),{once:!0}),this.dropdown.show()})):this.input.reportValidity()}setCustomValidity(e){this.input.setCustomValidity(e),this.invalid=this.input.invalid}handleCopy(){this.input.select(),document.execCommand("copy"),this.previewButton.focus(),this.previewButton.classList.add("color-picker__preview-color--copied"),this.previewButton.addEventListener("animationend",(()=>{this.previewButton.classList.remove("color-picker__preview-color--copied")}))}handleFormatToggle(){const e=["hex","rgb","hsl"],t=(e.indexOf(this.format)+1)%e.length;this.format=e[t]}handleAlphaDrag(e){const t=this.shadowRoot.querySelector(".color-picker__slider.color-picker__alpha"),r=t.querySelector(".color-picker__slider-handle"),{width:i}=t.getBoundingClientRect();r.focus(),e.preventDefault(),(0,o.o)(t,(e=>{this.alpha=(0,a.u)(e/i*100,0,100),this.syncValues()}))}handleHueDrag(e){const t=this.shadowRoot.querySelector(".color-picker__slider.color-picker__hue"),r=t.querySelector(".color-picker__slider-handle"),{width:i}=t.getBoundingClientRect();r.focus(),e.preventDefault(),(0,o.o)(t,(e=>{this.hue=(0,a.u)(e/i*360,0,360),this.syncValues()}))}handleGridDrag(e){const t=this.shadowRoot.querySelector(".color-picker__grid"),r=t.querySelector(".color-picker__grid-handle"),{width:i,height:s}=t.getBoundingClientRect();r.focus(),e.preventDefault(),(0,o.o)(t,((e,t)=>{this.saturation=(0,a.u)(e/i*100,0,100),this.lightness=(0,a.u)(100-t/s*100,0,100),this.syncValues()}))}handleAlphaKeyDown(e){const t=e.shiftKey?10:1;"ArrowLeft"===e.key&&(e.preventDefault(),this.alpha=(0,a.u)(this.alpha-t,0,100),this.syncValues()),"ArrowRight"===e.key&&(e.preventDefault(),this.alpha=(0,a.u)(this.alpha+t,0,100),this.syncValues()),"Home"===e.key&&(e.preventDefault(),this.alpha=0,this.syncValues()),"End"===e.key&&(e.preventDefault(),this.alpha=100,this.syncValues())}handleHueKeyDown(e){const t=e.shiftKey?10:1;"ArrowLeft"===e.key&&(e.preventDefault(),this.hue=(0,a.u)(this.hue-t,0,360),this.syncValues()),"ArrowRight"===e.key&&(e.preventDefault(),this.hue=(0,a.u)(this.hue+t,0,360),this.syncValues()),"Home"===e.key&&(e.preventDefault(),this.hue=0,this.syncValues()),"End"===e.key&&(e.preventDefault(),this.hue=360,this.syncValues())}handleGridKeyDown(e){const t=e.shiftKey?10:1;"ArrowLeft"===e.key&&(e.preventDefault(),this.saturation=(0,a.u)(this.saturation-t,0,100),this.syncValues()),"ArrowRight"===e.key&&(e.preventDefault(),this.saturation=(0,a.u)(this.saturation+t,0,100),this.syncValues()),"ArrowUp"===e.key&&(e.preventDefault(),this.lightness=(0,a.u)(this.lightness+t,0,100),this.syncValues()),"ArrowDown"===e.key&&(e.preventDefault(),this.lightness=(0,a.u)(this.lightness-t,0,100),this.syncValues())}handleInputChange(e){const t=e.target;this.setColor(t.value),t.value=this.value,e.stopPropagation()}handleInputKeyDown(e){"Enter"===e.key&&(this.setColor(this.input.value),this.input.value=this.value,setTimeout((()=>this.input.select())))}normalizeColorString(e){if(/rgba?/i.test(e)){const t=e.replace(/[^\d.%]/g," ").split(" ").map((e=>e.trim())).filter((e=>e.length));return t.length<4&&(t[3]="1"),t[3].indexOf("%")>-1&&(t[3]=(parseFloat(t[3].replace(/%/g,""))/100).toString()),`rgba(${t[0]}, ${t[1]}, ${t[2]}, ${t[3]})`}if(/hsla?/i.test(e)){const t=e.replace(/[^\d.%]/g," ").split(" ").map((e=>e.trim())).filter((e=>e.length));return t.length<4&&(t[3]="1"),t[3].indexOf("%")>-1&&(t[3]=(parseFloat(t[3].replace(/%/g,""))/100).toString()),`hsla(${t[0]}, ${t[1]}, ${t[2]}, ${t[3]})`}return/^[0-9a-f]+$/i.test(e)?`#${e}`:e}parseColor(e){let t;e=this.normalizeColorString(e);try{t=(0,C.default)(e)}catch(e){return null}const r=t.hsl(),o={h:r.hue(),s:r.saturationl(),l:r.lightness(),a:r.alpha()},a=t.rgb(),i={r:a.red(),g:a.green(),b:a.blue(),a:a.alpha()},s=z(i.r),n=z(i.g),l=z(i.b),c=z(255*i.a);return{hsl:{h:o.h,s:o.s,l:o.l,string:this.setLetterCase(`hsl(${Math.round(o.h)}, ${Math.round(o.s)}%, ${Math.round(o.l)}%)`)},hsla:{h:o.h,s:o.s,l:o.l,a:o.a,string:this.setLetterCase(`hsla(${Math.round(o.h)}, ${Math.round(o.s)}%, ${Math.round(o.l)}%, ${o.a.toFixed(2).toString()})`)},rgb:{r:i.r,g:i.g,b:i.b,string:this.setLetterCase(`rgb(${Math.round(i.r)}, ${Math.round(i.g)}, ${Math.round(i.b)})`)},rgba:{r:i.r,g:i.g,b:i.b,a:i.a,string:this.setLetterCase(`rgba(${Math.round(i.r)}, ${Math.round(i.g)}, ${Math.round(i.b)}, ${i.a.toFixed(2).toString()})`)},hex:this.setLetterCase(`#${s}${n}${l}`),hexa:this.setLetterCase(`#${s}${n}${l}${c}`)}}setColor(e){const t=this.parseColor(e);return null!==t&&(this.hue=t.hsla.h,this.saturation=t.hsla.s,this.lightness=t.hsla.l,this.alpha=this.opacity?100*t.hsla.a:100,this.syncValues(),!0)}setLetterCase(e){return"string"!=typeof e?"":this.uppercase?e.toUpperCase():e.toLowerCase()}async syncValues(){const e=this.parseColor(`hsla(${this.hue}, ${this.saturation}%, ${this.lightness}%, ${this.alpha/100})`);null!==e&&("hsl"===this.format?this.inputValue=this.opacity?e.hsla.string:e.hsl.string:"rgb"===this.format?this.inputValue=this.opacity?e.rgba.string:e.rgb.string:this.inputValue=this.opacity?e.hexa:e.hex,this.isSafeValue=!0,this.value=this.inputValue,await this.updateComplete,this.isSafeValue=!1)}handleAfterHide(){this.previewButton.classList.remove("color-picker__preview-color--copied")}handleEyeDropper(){if(!M)return;(new EyeDropper).open().then((e=>this.setColor(e.sRGBHex))).catch((()=>{}))}handleFormatChange(){this.syncValues()}handleOpacityChange(){this.alpha=100}handleValueChange(e,t){if(!this.isSafeValue&&void 0!==e){const r=this.parseColor(t);null!==r?(this.inputValue=this.value,this.hue=r.hsla.h,this.saturation=r.hsla.s,this.lightness=r.hsla.l,this.alpha=100*r.hsla.a):this.inputValue=e}this.value!==this.lastValueEmitted&&((0,p.j)(this,"sl-change"),this.lastValueEmitted=this.value)}render(){const e=this.saturation,t=100-this.lightness,r=b.$`
      <div
        part="base"
        class=${(0,h.o)({"color-picker":!0,"color-picker--inline":this.inline,"color-picker--disabled":this.disabled})}
        aria-disabled=${this.disabled?"true":"false"}
        aria-labelledby="label"
        tabindex=${this.inline?"0":"-1"}
      >
        ${this.inline?b.$`
              <sl-visually-hidden id="label">
                <slot name="label">${this.label}</slot>
              </sl-visually-hidden>
            `:null}

        <div
          part="grid"
          class="color-picker__grid"
          style=${(0,i.i)({backgroundColor:`hsl(${this.hue}deg, 100%, 50%)`})}
          @mousedown=${this.handleGridDrag}
          @touchstart=${this.handleGridDrag}
        >
          <span
            part="grid-handle"
            class="color-picker__grid-handle"
            style=${(0,i.i)({top:`${t}%`,left:`${e}%`,backgroundColor:`hsla(${this.hue}deg, ${this.saturation}%, ${this.lightness}%)`})}
            role="application"
            aria-label="HSL"
            tabindex=${(0,d.l)(this.disabled?void 0:"0")}
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
                style=${(0,i.i)({left:(0===this.hue?0:100/(360/this.hue))+"%"})}
                role="slider"
                aria-label="hue"
                aria-orientation="horizontal"
                aria-valuemin="0"
                aria-valuemax="360"
                aria-valuenow=${`${Math.round(this.hue)}`}
                tabindex=${(0,d.l)(this.disabled?void 0:"0")}
                @keydown=${this.handleHueKeyDown}
              ></span>
            </div>

            ${this.opacity?b.$`
                  <div
                    part="slider opacity-slider"
                    class="color-picker__alpha color-picker__slider color-picker__transparent-bg"
                    @mousedown="${this.handleAlphaDrag}"
                    @touchstart="${this.handleAlphaDrag}"
                  >
                    <div
                      class="color-picker__alpha-gradient"
                      style=${(0,i.i)({backgroundImage:`linear-gradient(\n                          to right,\n                          hsl(${this.hue}deg, ${this.saturation}%, ${this.lightness}%, 0%) 0%,\n                          hsl(${this.hue}deg, ${this.saturation}%, ${this.lightness}%) 100%\n                        )`})}
                    ></div>
                    <span
                      part="slider-handle"
                      class="color-picker__slider-handle"
                      style=${(0,i.i)({left:`${this.alpha}%`})}
                      role="slider"
                      aria-label="alpha"
                      aria-orientation="horizontal"
                      aria-valuemin="0"
                      aria-valuemax="100"
                      aria-valuenow=${Math.round(this.alpha)}
                      tabindex=${(0,d.l)(this.disabled?void 0:"0")}
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
            style=${(0,i.i)({"--preview-color":`hsla(${this.hue}deg, ${this.saturation}%, ${this.lightness}%, ${this.alpha/100})`})}
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
            .value=${(0,l.l)(this.inputValue)}
            ?disabled=${this.disabled}
            aria-label=${this.localize.term("currentValue")}
            @keydown=${this.handleInputKeyDown}
            @sl-change=${this.handleInputChange}
          ></sl-input>

          <sl-button-group>
            ${this.noFormatToggle?"":b.$`
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
            ${M?b.$`
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

        ${this.swatches.length>0?b.$`
              <div part="swatches" class="color-picker__swatches">
                ${this.swatches.map((e=>b.$`
                    <div
                      part="swatch"
                      class="color-picker__swatch color-picker__transparent-bg"
                      tabindex=${(0,d.l)(this.disabled?void 0:"0")}
                      role="button"
                      aria-label=${e}
                      @click=${()=>!this.disabled&&this.setColor(e)}
                      @keydown=${t=>!this.disabled&&"Enter"===t.key&&this.setColor(e)}
                    >
                      <div class="color-picker__swatch-color" style=${(0,i.i)({backgroundColor:e})}></div>
                    </div>
                  `))}
              </div>
            `:""}
      </div>
    `;return this.inline?r:b.$`
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
          class=${(0,h.o)({"color-dropdown__trigger":!0,"color-dropdown__trigger--disabled":this.disabled,"color-dropdown__trigger--small":"small"===this.size,"color-dropdown__trigger--medium":"medium"===this.size,"color-dropdown__trigger--large":"large"===this.size,"color-picker__transparent-bg":!0})}
          style=${(0,i.i)({color:`hsla(${this.hue}deg, ${this.saturation}%, ${this.lightness}%, ${this.alpha/100})`})}
          type="button"
        >
          <sl-visually-hidden>
            <slot name="label">${this.label}</slot>
          </sl-visually-hidden>
        </button>
        ${r}
      </sl-dropdown>
    `}};function z(e){const t=Math.round(e).toString(16);return 1===t.length?`0${t}`:t}E.styles=s.t,(0,m.u2)([(0,g.i)('[part="input"]')],E.prototype,"input",2),(0,m.u2)([(0,g.i)('[part="preview"]')],E.prototype,"previewButton",2),(0,m.u2)([(0,g.i)(".color-dropdown")],E.prototype,"dropdown",2),(0,m.u2)([(0,g.t)()],E.prototype,"inputValue",2),(0,m.u2)([(0,g.t)()],E.prototype,"hue",2),(0,m.u2)([(0,g.t)()],E.prototype,"saturation",2),(0,m.u2)([(0,g.t)()],E.prototype,"lightness",2),(0,m.u2)([(0,g.t)()],E.prototype,"alpha",2),(0,m.u2)([(0,g.e)()],E.prototype,"value",2),(0,m.u2)([(0,g.e)()],E.prototype,"label",2),(0,m.u2)([(0,g.e)()],E.prototype,"format",2),(0,m.u2)([(0,g.e)({type:Boolean,reflect:!0})],E.prototype,"inline",2),(0,m.u2)([(0,g.e)()],E.prototype,"size",2),(0,m.u2)([(0,g.e)({attribute:"no-format-toggle",type:Boolean})],E.prototype,"noFormatToggle",2),(0,m.u2)([(0,g.e)()],E.prototype,"name",2),(0,m.u2)([(0,g.e)({type:Boolean,reflect:!0})],E.prototype,"disabled",2),(0,m.u2)([(0,g.e)({type:Boolean,reflect:!0})],E.prototype,"invalid",2),(0,m.u2)([(0,g.e)({type:Boolean})],E.prototype,"hoist",2),(0,m.u2)([(0,g.e)({type:Boolean})],E.prototype,"opacity",2),(0,m.u2)([(0,g.e)({type:Boolean})],E.prototype,"uppercase",2),(0,m.u2)([(0,g.e)({attribute:!1})],E.prototype,"swatches",2),(0,m.u2)([(0,g.e)()],E.prototype,"lang",2),(0,m.u2)([(0,u.Y)("format")],E.prototype,"handleFormatChange",1),(0,m.u2)([(0,u.Y)("opacity")],E.prototype,"handleOpacityChange",1),(0,m.u2)([(0,u.Y)("value")],E.prototype,"handleValueChange",1),E=(0,m.u2)([(0,g.n)("sl-color-picker")],E)},7096:(e,t,r)=>{r.d(t,{D:()=>a});var o=r(9736),a=r(6557).r`
  ${o.N}

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
`},499:(e,t,r)=>{r.d(t,{M:()=>s});var o=r(6133),a=r(7803),i=r(9736),s=r(6557).r`
  ${i.N}
  ${o.n}

  :host {
    --thumb-size: 20px;
    --tooltip-offset: 10px;
    --track-color-active: var(--sl-color-neutral-200);
    --track-color-inactive: var(--sl-color-neutral-200);
    --track-height: 6px;

    display: block;
  }

  .range {
    position: relative;
  }

  .range__control {
    -webkit-appearance: none;
    border-radius: 3px;
    width: 100%;
    height: var(--track-height);
    background: transparent;
    line-height: var(--sl-input-height-medium);
    vertical-align: middle;
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

  .range__control:enabled${a.v}::-webkit-slider-thumb {
    background-color: var(--sl-color-primary-500);
    border-color: var(--sl-color-primary-500);
    box-shadow: var(--sl-focus-ring);
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

  .range__control:enabled${a.v}::-moz-range-thumb {
    background-color: var(--sl-color-primary-500);
    border-color: var(--sl-color-primary-500);
    box-shadow: var(--sl-focus-ring);
  }

  .range__control:enabled::-moz-range-thumb:active {
    background-color: var(--sl-color-primary-500);
    border-color: var(--sl-color-primary-500);
    cursor: grabbing;
  }

  /* States */
  .range__control${a.v} {
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
    margin-left: calc(-1 * var(--sl-tooltip-arrow-size));
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
`},8309:(e,t,r)=>{var o=r(1347),a=r(5868),i=r(9063),s=r(2288),n=r(6910),l=r(6557),c=r(1703),h=class extends l.s{constructor(){super(...arguments),this.localize=new a.Ve(this),this.variant="neutral",this.size="medium",this.pill=!1,this.removable=!1}handleRemoveClick(){(0,s.j)(this,"sl-remove")}render(){return l.$`
      <span
        part="base"
        class=${(0,i.o)({tag:!0,"tag--primary":"primary"===this.variant,"tag--success":"success"===this.variant,"tag--neutral":"neutral"===this.variant,"tag--warning":"warning"===this.variant,"tag--danger":"danger"===this.variant,"tag--text":"text"===this.variant,"tag--small":"small"===this.size,"tag--medium":"medium"===this.size,"tag--large":"large"===this.size,"tag--pill":this.pill,"tag--removable":this.removable})}
      >
        <span part="content" class="tag__content">
          <slot></slot>
        </span>

        ${this.removable?l.$`
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
    `}};h.styles=o.l,(0,c.u2)([(0,n.e)({reflect:!0})],h.prototype,"variant",2),(0,c.u2)([(0,n.e)({reflect:!0})],h.prototype,"size",2),(0,c.u2)([(0,n.e)({type:Boolean,reflect:!0})],h.prototype,"pill",2),(0,c.u2)([(0,n.e)({type:Boolean})],h.prototype,"removable",2),h=(0,c.u2)([(0,n.n)("sl-tag")],h)},8329:(e,t,r)=>{var o=r(5868),a=r(6910),i=r(6557),s=r(1703),n=class extends i.s{constructor(){super(...arguments),this.localize=new o.Ve(this),this.value=0,this.unit="byte",this.display="short"}render(){if(isNaN(this.value))return"";const e="bit"===this.unit?["","kilo","mega","giga","tera"]:["","kilo","mega","giga","tera","peta"],t=Math.max(0,Math.min(Math.floor(Math.log10(this.value)/3),e.length-1)),r=e[t]+this.unit,o=parseFloat((this.value/Math.pow(1e3,t)).toPrecision(3));return this.localize.number(o,{style:"unit",unit:r,unitDisplay:this.display})}};(0,s.u2)([(0,a.e)({type:Number})],n.prototype,"value",2),(0,s.u2)([(0,a.e)()],n.prototype,"unit",2),(0,s.u2)([(0,a.e)()],n.prototype,"display",2),(0,s.u2)([(0,a.e)()],n.prototype,"lang",2),n=(0,s.u2)([(0,a.n)("sl-format-bytes")],n)},5447:(e,t,r)=>{var o=r(2588),a=r(6910),i=r(6557),s=r(1703),n=class extends i.s{constructor(){super(...arguments),this.label="Breadcrumb"}getSeparator(){const e=this.separatorSlot.assignedElements({flatten:!0})[0].cloneNode(!0);return[e,...e.querySelectorAll("[id]")].forEach((e=>e.removeAttribute("id"))),e.slot="separator",e}handleSlotChange(){const e=[...this.defaultSlot.assignedElements({flatten:!0})].filter((e=>"sl-breadcrumb-item"===e.tagName.toLowerCase()));e.forEach(((t,r)=>{null===t.querySelector('[slot="separator"]')&&t.append(this.getSeparator()),r===e.length-1?t.setAttribute("aria-current","page"):t.removeAttribute("aria-current")}))}render(){return i.$`
      <nav part="base" class="breadcrumb" aria-label=${this.label}>
        <slot @slotchange=${this.handleSlotChange}></slot>
      </nav>

      <slot name="separator" hidden aria-hidden="true">
        <sl-icon name="chevron-right" library="system"></sl-icon>
      </slot>
    `}};n.styles=o.K,(0,s.u2)([(0,a.i)("slot")],n.prototype,"defaultSlot",2),(0,s.u2)([(0,a.i)('slot[name="separator"]')],n.prototype,"separatorSlot",2),(0,s.u2)([(0,a.e)()],n.prototype,"label",2),n=(0,s.u2)([(0,a.n)("sl-breadcrumb")],n)}}]);