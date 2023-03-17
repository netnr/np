const reqModule = require.context('../page/user', true, /.js$/);
const nrPack_user = Object.values(reqModule.keys().map(reqModule));
export { nrPack_user };