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
import { nrEditor } from '../frame/nrEditor';
import { nrcRely } from '../frame/nrcRely';

let init = async () => {
    const magicBytes = await import('magic-bytes.js');

    await nrEditor.init();
    await nrGrid.init();
    await nrcRely.remote('sql-formatter.js');
    await nrcRely.remote('jszip.js');

    Object.assign(window, {
        magicBytes, ndkInit,
        ndkAction, ndkEditor, ndkExecute, ndkFunction, ndkGenerateCode, ndkGenerateSQL, ndkGenerateDDL, ndkI18n, ndkRequest, ndkNoteSQL, ndkStep, ndkStorage, ndkTab, ndkVary, ndkView
    });

    await ndkInit.init();
}

document.readyState == "loading" ? document.addEventListener("DOMContentLoaded", init) : init();