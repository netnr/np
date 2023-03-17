### Build
```
npm install -g browserify
npm install -g terser
npm install -g npm-check-updates

npm install clean-css

ncu --loglevel verbose --packageFile package.json
ncu -u

browserify clean-css.js -o clean-css.js
terser clean-css.js -o clean-css.min.js
rm clean-css.js

```