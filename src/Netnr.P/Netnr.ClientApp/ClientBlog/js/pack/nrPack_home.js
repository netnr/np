const reqModule = require.context('../page/home', true, /.js$/);
const nrPack_home = Object.values(reqModule.keys().map(reqModule));
export { nrPack_home };