require.context('../frame/Shared', true, /.css$/);
import '../frame/Bootstrap/nrDark.css';
require.context('./css', true, /.css$/);

import { nrWeb } from './js/nrWeb';
document.readyState == "loading" ? document.addEventListener("DOMContentLoaded", nrWeb.init) : nrWeb.init();