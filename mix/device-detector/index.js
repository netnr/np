const DeviceDetector = require("device-detector-js");
const deviceDetector = new DeviceDetector();

window.dd = function (userAgent) {
    return deviceDetector.parse(userAgent);
}