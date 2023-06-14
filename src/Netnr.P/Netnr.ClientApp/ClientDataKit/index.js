require.context('../frame/Shoelace', true, /.css$/);
require.context('./css', true, /.css$/);

import { ndkAction } from './js/ndkAction';
import { ndkEditor } from './js/ndkEditor';
import { ndkExecute } from './js/ndkExecute';
import { ndkFunction } from './js/ndkFunction';
import { ndkGenerateCode } from './js/ndkGenerateCode';
import { ndkGenerateSQL } from './js/ndkGenerateSQL';
import { ndkGenerateDDL } from './js/ndkGenerateDDL';
import { ndkI18n } from './js/ndkI18n';
import { ndkRequest } from './js/ndkRequest';
import { ndkNoteSQL } from './js/ndkNoteSQL';
import { ndkStep } from './js/ndkStep';
import { ndkStorage } from './js/ndkStorage';
import { ndkTab } from './js/ndkTab';
import { ndkVary } from './js/ndkVary';
import { ndkView } from './js/ndkView';

import { ndkInit } from './js/ndkInit';
import { nrGrid } from '../frame/nrGrid';
import { nrcBase } from '../frame/nrcBase';

let init = async () => {
  await import('bootstrap/dist/css/bootstrap.css');

  const sqlFormatter = await import('sql-formatter');
  const JSZip = (await import('jszip')).default;
  const magicBytes = await import('magic-bytes.js');

  // monaco-editor
  const monaco = await import('monaco-editor/esm/vs/editor/editor.api');

  // @shoelace-style/shoelace
  await import('@shoelace-style/shoelace/dist/themes/light.css');
  await import('@shoelace-style/shoelace/dist/themes/dark.css');
  await import('@shoelace-style/shoelace');
  const { setBasePath } = await import('@shoelace-style/shoelace/dist/utilities/base-path.js');
  setBasePath(`${location.pathname.split('/').slice(0, -1).join('/')}`);

  // @shoelace-style/shoelace register icon
  const { registerIconLibrary } = await import('@shoelace-style/shoelace/dist/utilities/icon-library.js');
  registerIconLibrary('nrg-icon', {
    resolver: name => `/assets/icons/${name}.svg`,
    mutator: svg => svg.setAttribute('fill', 'currentColor')
  });

  //重写
  nrGrid.init = async () => {
    let agGrid = window['agGrid'];
    if (!agGrid) {
      await import('ag-grid-enterprise/styles/ag-grid.css')
      await import('ag-grid-enterprise/styles/ag-theme-alpine.css')
      agGrid = await import('ag-grid-enterprise');
      Object.assign(window, { agGrid });
      nrGrid.err();
    }
  };
  await nrGrid.init();

  Object.assign(window, {
    sqlFormatter, JSZip, magicBytes, monaco, ndkInit, nrGrid, nrcBase,
    ndkAction, ndkEditor, ndkExecute, ndkFunction, ndkGenerateCode, ndkGenerateSQL, ndkGenerateDDL, ndkI18n, ndkRequest, ndkNoteSQL, ndkStep, ndkStorage, ndkTab, ndkVary, ndkView
  });

  await ndkInit.init();
}

document.readyState == "loading" ? document.addEventListener("DOMContentLoaded", init) : init();