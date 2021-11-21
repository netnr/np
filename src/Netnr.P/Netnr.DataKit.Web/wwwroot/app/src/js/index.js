import '../css/fix-grid.css'
import '../css/fix-sl.css'
import '../css/fix.css'
import '../css/index.css'
import '../css/nrc.css'
import '../css/theme-dark.css'
import { agg } from './agg'
import { ndkVary } from './ndkVary'
import { ndkLs } from './ndkLs'
import { ndkDb } from './ndkDb'
import { ndkFn } from './ndkFn'
import { ndkEditor } from './ndkEditor'
import { ndkTab } from './ndkTab'
import { ndkStep } from './ndkStep'
import { ndkBuild } from './ndkBuild'
import { ndkSqlNote } from './ndkSqlNote'

import { ndkInit } from './ndkInit'
Object.assign(window, {
    agg, ndkVary, ndkLs, ndkDb, ndkFn, ndkEditor, ndkTab, ndkStep, ndkBuild, ndkSqlNote, ndkInit
})