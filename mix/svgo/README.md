### Build
```
npm install -g browserify
npm install -g terser

npm i svgo

browserify index.js -o svgo.js
terser svgo.js -o svgo.min.js
```