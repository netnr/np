const reqModule = require.context('../page/ss', true, /.js$/);
const nrPack_ss = Object.values(reqModule.keys().map(reqModule));
export { nrPack_ss };