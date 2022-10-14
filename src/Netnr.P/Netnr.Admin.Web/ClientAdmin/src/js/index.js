// @shoelace-style/shoelace
import '@shoelace-style/shoelace/dist/themes/light.css';
import '@shoelace-style/shoelace/dist/themes/dark.css';
import '@shoelace-style/shoelace';
import { setBasePath } from '@shoelace-style/shoelace/dist/utilities/base-path.js';
setBasePath(`${location.pathname.split('/').slice(0, -1).join('/')}/shoelace`);

// ag-grid
const agGrid = require('ag-grid-enterprise');
agGrid.LicenseManager.prototype.outputMissingLicenseKey = _ => { }
import 'ag-grid-enterprise/dist/styles/ag-grid.css';
import 'ag-grid-enterprise/dist/styles/ag-theme-alpine.css';
import 'ag-grid-enterprise/dist/styles/ag-theme-alpine-dark.css';
// import 'ag-grid-enterprise/dist/styles/ag-theme-balham.css';
// import 'ag-grid-enterprise/dist/styles/ag-theme-balham-dark.css';

// bootstrap
import 'bootstrap/dist/css/bootstrap.css'

import '../css/layout.css';

import './nrRouter';

Object.assign(window, { agGrid })