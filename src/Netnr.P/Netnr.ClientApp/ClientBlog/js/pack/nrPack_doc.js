const reqModule = require.context('../page/doc', true, /.js$/);
const nrPack_doc = Object.values(reqModule.keys().map(reqModule));
export { nrPack_doc };