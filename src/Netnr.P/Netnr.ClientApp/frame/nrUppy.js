import Uppy from '@uppy/core'
import Dashboard from '@uppy/dashboard'
import XHR from '@uppy/xhr-upload'
import Tus from '@uppy/tus'
import ImageEditor from '@uppy/image-editor'
import zh_CN from '@uppy/locales/lib/zh_CN'

import '@uppy/core/dist/style.css'
import '@uppy/dashboard/dist/style.css'
import '@uppy/image-editor/dist/style.css'

let nrUppy = {
    Uppy,
    Dashboard,
    XHR,
    Tus,
    ImageEditor,
    zh_CN,
    create: (option) => new Uppy(Object.assign({ locale: zh_CN }, option))
};

Object.assign(window, { nrUppy });
export { nrUppy }