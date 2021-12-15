import '../css/fix-grid.css'
import '../css/fix-sl.css'
import '../css/fix.css'
import '../css/index.css'
import '../css/index-mobile.css'
import '../css/nrc.css'
import '../css/theme-dark.css'

import '../icon/123.svg'
import '../icon/abc.svg'
import '../icon/clock.svg'
import '../icon/column.svg'
import '../icon/database.svg'
import '../icon/key.svg'
import '../icon/mariadb.svg'
import '../icon/mysql.svg'
import '../icon/oracle.svg'
import '../icon/plug.svg'
import '../icon/postgresql.svg'
import '../icon/sqlite.svg'
import '../icon/sqlserver.svg'
import '../icon/table.svg'

import { agg } from './agg'
import { ndkI18n } from './ndkI18n'
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
    agg, ndkI18n, ndkVary, ndkLs, ndkDb, ndkFn, ndkEditor, ndkTab, ndkStep, ndkBuild, ndkSqlNote, ndkInit
})