var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function(req, res, next) {
    //res.render('index', { title: 'Express' });
    res.sendfile('./public/index.html');
});

router.get('/swagger', function (req, res, next) {
    res.sendfile('./public/swagger.html');
});

module.exports = router;
