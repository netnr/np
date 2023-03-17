const reqModule = require.context('../page/admin', true, /.js$/);
const nrPack_admin = Object.values(reqModule.keys().map(reqModule));
export { nrPack_admin };