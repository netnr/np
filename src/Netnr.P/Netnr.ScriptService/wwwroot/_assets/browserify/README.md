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

browserify svgo/index.js -o svgo/svgo.js
terser svgo/svgo.js -o svgo/svgo.min.js
rm svgo/svgo.js

browserify js-yaml/index.js -o js-yaml/js-yaml.js
terser js-yaml/js-yaml.js -o js-yaml/js-yaml.min.js
rm js-yaml/js-yaml.js

browserify clean-css/index.js -o clean-css/clean-css.js
terser clean-css/clean-css.js -o clean-css/clean-css.min.js
rm clean-css/clean-css.js

browserify fast-xml-parser/index.js -o fast-xml-parser/fast-xml-parser.js
terser fast-xml-parser/fast-xml-parser.js -o fast-xml-parser/fast-xml-parser.min.js
rm fast-xml-parser/fast-xml-parser.js

browserify device-detector-js/index.js -o device-detector-js/device-detector-js.js
terser device-detector-js/device-detector-js.js -o device-detector-js/device-detector-js.min.js
rm device-detector-js/device-detector-js.js
```