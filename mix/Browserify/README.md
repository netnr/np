### Build
```
npm install -g browserify
npm install -g terser
npm install -g npm-check-updates

npm install svgo
npm install clean-css
npm install fast-xml-parser
npm install device-detector-js

ncu --loglevel verbose --packageFile package.json # 查看版本更新
ncu -u # 更新版本

browserify svgo/index.js -o svgo/svgo.js
terser svgo/svgo.js -o svgo/svgo.min.js

browserify clean-css/index.js -o clean-css/clean-css.js
terser clean-css/clean-css.js -o clean-css/clean-css.min.js

browserify fast-xml-parser/index.js -o fast-xml-parser/fast-xml-parser.js
terser fast-xml-parser/fast-xml-parser.js -o fast-xml-parser/fast-xml-parser.min.js

browserify device-detector-js/index.js -o device-detector-js/device-detector-js.js
terser device-detector-js/device-detector-js.js -o device-detector-js/device-detector-js.min.js

```