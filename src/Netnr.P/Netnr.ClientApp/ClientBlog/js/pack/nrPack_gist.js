const reqModule = require.context('../page/gist', true, /.js$/);
const nrPack_gist = Object.values(reqModule.keys().map(reqModule));
export { nrPack_gist };