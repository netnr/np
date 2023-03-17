const reqModule = require.context('../page/note', true, /.js$/);
const nrPack_note = Object.values(reqModule.keys().map(reqModule));
export { nrPack_note };