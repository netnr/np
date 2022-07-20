import '@shoelace-style/shoelace/dist/themes/light.css';
import '@shoelace-style/shoelace/dist/themes/dark.css';

import '../css/global.css';
import '../css/dark.css';

import '@shoelace-style/shoelace';
import { setBasePath } from '@shoelace-style/shoelace/dist/utilities/base-path.js';
setBasePath('https://unpkg.com/@shoelace-style/shoelace@2.0.0-beta.78/dist');

import { nr } from './global';
import { ag } from './ag';
import './upstream';
import { ss } from './ss';
import { me } from './me';
import './fun';

Object.assign(window, { nr, ag, ss, me })