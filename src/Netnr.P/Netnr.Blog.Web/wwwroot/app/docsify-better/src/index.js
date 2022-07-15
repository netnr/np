import './pure.css';
import './docs.css';
import '@shoelace-style/shoelace/dist/themes/light.css';
import '@shoelace-style/shoelace/dist/themes/dark.css';

import '../../md/src/css/nmd-hljs.css';
import '../../md/src/css/nmd-markdown.css';
import '../../md/src/css/nmd-toc.css';

import tocbot from 'tocbot';
import hljs from 'highlight.js/lib/common';
import Docsify from 'docsify/lib/docsify';

Object.assign(window, { tocbot, hljs, Docsify });