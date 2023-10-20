import AirDatepicker from 'air-datepicker';
import localeZh from 'air-datepicker/locale/zh';
import 'air-datepicker/air-datepicker.css'

let nrDatepicker = {
    AirDatepicker,
    localeZh,
    create: (selector, options) => new AirDatepicker(selector, Object.assign({
        locale: localeZh
    }, options))
}

Object.assign(window, { nrDatepicker });
export { nrDatepicker }