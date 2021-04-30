# device-detector-js

`device-detector-js` browser support

### Build
```
npm install -g browserify
npm install device-detector-js
browserify index.js -o dd.js
```

### Usage

```
<script src="dd.js" ></script>

dd(navigator.userAgent)
```