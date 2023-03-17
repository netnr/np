const reqModule = require.context('../page/run', true, /.js$/);
const nrPack_run = Object.values(reqModule.keys().map(reqModule));
export { nrPack_run };