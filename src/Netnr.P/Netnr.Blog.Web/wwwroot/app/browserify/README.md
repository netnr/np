### Build
```
npm install -g browserify
npm install -g terser
npm install -g npm-check-updates

npm install svgo
npm install js-yaml
npm install clean-css
npm install fast-xml-parser
npm install device-detector-js

ncu --loglevel verbose --packageFile package.json
ncu -u

npm run prod

browserify libs/svgo.js -o dist/svgo.js
terser dist/svgo.js -o dist/svgo.min.js
rm dist/svgo.js

browserify libs/clean-css.js -o dist/clean-css.js
terser dist/clean-css.js -o dist/clean-css.min.js
rm dist/clean-css.js

```