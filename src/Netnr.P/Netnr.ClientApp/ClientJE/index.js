import { nrWeb } from './js/nrWeb';
document.readyState == "loading" ? document.addEventListener("DOMContentLoaded", nrWeb.init) : nrWeb.init();