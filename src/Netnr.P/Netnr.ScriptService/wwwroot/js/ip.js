try {
    $('#txtSohu').val(returnCitySN.cip);
} catch (e) { }

try {
    fetch('https://api.zme.ink/ip').then(x => x.json()).then(res => {
        $('#txtZme').val(res.ip);
    })
} catch (e) { }

try {
    function findIP(onNewIP) {
        var myPeerConnection = window.RTCPeerConnection || window.mozRTCPeerConnection || window.webkitRTCPeerConnection;
        var pc = new myPeerConnection({ iceServers: [] }), noop = function () { }, localIPs = {},
            ipRegex = /([0-9]{1,3}(\.[0-9]{1,3}){3}|[a-f0-9]{1,4}(:[a-f0-9]{1,4}){7})/g, key;

        function ipIterate(ip) {
            if (!localIPs[ip]) onNewIP(ip);
            localIPs[ip] = true;
        }
        pc.createDataChannel("");
        pc.createOffer().then(function (sdp) {
            sdp.sdp.split('\n').forEach(function (line) {
                if (line.indexOf('candidate') < 0) return;
                line.match(ipRegex).forEach(ipIterate);
            });
            pc.setLocalDescription(sdp, noop, noop);
        });
        pc.onicecandidate = function (ice) {
            if (!ice || !ice.candidate || !ice.candidate.candidate || !ice.candidate.candidate.match(ipRegex)) return;
            ice.candidate.candidate.match(ipRegex).forEach(ipIterate);
        };
    }
    findIP(function (ip) {
        $('#txIntranet').val(ip);
    });
} catch (e) { }


function CallBack_Ipsb(data) {
    $('#txtIpsb').val(data.ip)
}

$(document).click(function (e) {
    e = e || window.event;
    var target = e.target || e.srcElement;
    if (target.nodeName == "INPUT" && target.className.indexOf('form-control') >= 0) {
        target.focus();
        target.select();
    }
})