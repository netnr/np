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
    agg, ndkInit, ndkAction, ndkEditor, ndkExecute, ndkFunction, ndkGenerateCode, ndkGenerateSQL, ndkGenerateDDL, ndkI18n, ndkRequest, ndkNoteSQL, ndkStep, ndkStorage, ndkTab, ndkVary, ndkView
})