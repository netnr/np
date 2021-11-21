import '../css/fix-grid.css'
import '../css/fix-sl.css'
import '../css/fix.css'
import '../css/index.css'
import '../css/nrc.css'
import '../css/theme-dark.css'
import { agg } from './agg'
import { build } from './build'
import { db } from './db'
import { fn } from './fn'
import { init } from './init'
import { ls } from './ls'
import { me } from './me'
import { sqlFor } from './sqlFor'
import { step } from './step'
import { tab } from './tab'
import { vary } from './vary'

window['agg'] = agg
window['vary'] = vary
window['ls'] = ls
window['db'] = db
window['fn'] = fn
window['me'] = me
window['tab'] = tab
window['step'] = step
window['build'] = build
window['sqlFor'] = sqlFor
window['init'] = init