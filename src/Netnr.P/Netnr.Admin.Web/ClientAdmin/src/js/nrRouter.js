// page.js
import page from 'page';
import { nrVary } from "./nrVary";
import { nrmLayout } from './nrmLayout';
import { pageMenu } from '../admin/pageMenu';
import { pageLog } from '../admin/pageLog';

window.addEventListener("DOMContentLoaded", function () {

  page('*', function (ctx, next) {
    document.title = nrVary.title;
    document.body.innerHTML = "";
    console.debug(ctx);

    nrmLayout.init(); //布局

    next();
  });

  page('/account/login', function (ctx, next) {
    document.title = "登录"
    console.debug(ctx.pathname)
  });

  page('/admin/menu', pageMenu.init);

  page('/admin/log', pageLog.init);

  page();

}, false);