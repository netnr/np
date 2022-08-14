// sql-formatter
const sqlFormatter = require('sql-formatter');

// jszip
const JSZip = require('jszip');

// localforage
const localforage = require('localforage');

// ag-grid
const agGrid = require('ag-grid-enterprise');
import 'ag-grid-enterprise/dist/styles/ag-grid.css';
// import 'ag-grid-enterprise/dist/styles/ag-theme-alpine.css';
// import 'ag-grid-enterprise/dist/styles/ag-theme-alpine-dark.css';
import 'ag-grid-enterprise/dist/styles/ag-theme-balham.css';
import 'ag-grid-enterprise/dist/styles/ag-theme-balham-dark.css';

// monaco-editor
const monaco = require('monaco-editor/esm/vs/editor/editor.api');

// @shoelace-style/shoelace
import '@shoelace-style/shoelace/dist/themes/light.css';
import '@shoelace-style/shoelace/dist/themes/dark.css';
import '@shoelace-style/shoelace';
import { setBasePath } from '@shoelace-style/shoelace/dist/utilities/base-path.js';
setBasePath(`${location.pathname.split('/').slice(0, -1).join('/')}/shoelace`);

// @shoelace-style/shoelace register icon
import { registerIconLibrary } from '@shoelace-style/shoelace/dist/utilities/icon-library.js';
registerIconLibrary('nr-icon', {
  resolver: name => `/shoelace/assets/icons/${name}.svg`,
  mutator: svg => svg.setAttribute('fill', 'currentColor')
});

// dev
import '../css/fix-grid.css'
import '../css/fix-sl.css'
import '../css/fix.css'
import '../css/index.css'
import '../css/index-mobile.css'
import '../css/nrc.css'
import '../css/theme-dark.css'

import { agg } from './agg'
import { ndkAction } from './ndkAction';
import { ndkEditor } from './ndkEditor';
import { ndkExecute } from './ndkExecute';
import { ndkFunction } from './ndkFunction';
import { ndkGenerateCode } from './ndkGenerateCode';
import { ndkGenerateSQL } from './ndkGenerateSQL';
import { ndkGenerateDDL } from './ndkGenerateDDL';
import { ndkI18n } from './ndkI18n';
import { ndkRequest } from './ndkRequest';
import { ndkNoteSQL } from './ndkNoteSQL';
import { ndkStep } from './ndkStep';
import { ndkStorage } from './ndkStorage';
import { ndkTab } from './ndkTab';
import { ndkVary } from './ndkVary';
import { ndkView } from './ndkView';

import { ndkInit } from './ndkInit'

Object.assign(window, {
    sqlFormatter, JSZip, localforage, agGrid, monaco, agg, ndkInit,
    ndkAction, ndkEditor, ndkExecute, ndkFunction, ndkGenerateCode, ndkGenerateSQL, ndkGenerateDDL, ndkI18n, ndkRequest, ndkNoteSQL, ndkStep, ndkStorage, ndkTab, ndkVary, ndkView
})