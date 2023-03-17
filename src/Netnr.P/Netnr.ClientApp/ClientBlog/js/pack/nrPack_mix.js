const reqModule = require.context('../page/mix', true, /.js$/);
const nrPack_mix = Object.values(reqModule.keys().map(reqModule));
export { nrPack_mix };