const fs = require('fs');
const request = require('request');
const multiparty = require('multiparty');

module.exports = (req, res) => {

    let vm = { code: 0, msg: null };

    try {
        if (req.method === "POST") {
            new multiparty.Form().parse(req, (err, fields, files) => {

                if (err) {
                    vm.msg = err;
                    res.json(vm);
                } else {
                    let or = fields.or[0], path, token = fields.token[0], name = fields.name[0], file = files.file[0];

                    if (or && or.length > 2 && or.indexOf('/') > 0) {

                        if (token && token.length > 30) {

                            if (fields.path == null) {
                                let ext = name.split('.').pop();

                                path = new Date(new Date().valueOf() + 8 * 3600000).toISOString()
                                    .replace(/-/g, '/').replace('T', '/').replace(/:/g, '').replace('.', '').replace('Z', '')
                                    + Math.random().toString().substring(3, 4) + '.' + ext;
                            } else {
                                path = fields.path[0];
                            }

                            let uri = "https://api.github.com/repos/" + or + "/contents/" + path;
                            let bitfile = fs.readFileSync(file.path);
                            let content = Buffer.from(bitfile, 'binary').toString('base64');

                            request({
                                url: uri,
                                method: "PUT",
                                json: true,
                                headers: {
                                    'User-Agent': req.headers['user-agent'],
                                    Accept: 'application/vnd.github.v3+json',
                                    Authorization: 'token ' + token
                                },
                                body: {
                                    message: 'a',
                                    content: content
                                }
                            }, function (error, response, body) {
                                if (error) {
                                    vm.msg = error + "";
                                } else {
                                    vm.code = response.statusCode
                                    vm.data = body;
                                }
                                res.json(vm);
                            });
                        } else {
                            vm.msg = "token Parameter is invalid";
                            res.json(vm);
                        }
                    } else {
                        vm.msg = "or Parameter is invalid";
                        res.json(vm);
                    }
                }
            });
        } else {
            vm.msg = "Method not allowed. Send a POST request.";
            res.json(vm);
        }
    } catch (e) {
        vm.code = -1;
        vm.msg = e + "";
        res.json(vm);
    }
}