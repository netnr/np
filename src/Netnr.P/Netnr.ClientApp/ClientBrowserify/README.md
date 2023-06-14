### Build
```
npm install -g browserify
npm install -g terser
npm install -g npm-check-updates

ncu --loglevel verbose --packageFile package.json
ncu -u

browserify clean-css.js -o clean-css.bundle.js
terser clean-css.bundle.js -o clean-css.min.js
rm clean-css.bundle.js

```