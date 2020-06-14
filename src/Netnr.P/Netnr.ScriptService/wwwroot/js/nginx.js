/**
 * Ported by Yosef on 24/08/2016.
 * from project:
 *  https://github.com/1connect/nginx-config-formatter
 * from file:
 * nginxfmt.py
 *
 */

"use strict"; function extractTextBySeperator(t, e, r) { void 0 == r && (r = e); var n = new RegExp(e), i = new RegExp(r), o = new RegExp(e + "(.*?)" + r); return n.test(t) && i.test(t) ? t.match(o)[1] : "" } function extractAllPossibleText(t, e, r) { void 0 == r && (r = e); for (var n, i = {}, o = 0, s = e.length > 0 ? e.charCodeAt(0) : "", a = r.length > 0 ? r.charCodeAt(0) : ""; "" != (n = extractTextBySeperator(t, e, r));) { var p = "#$#%#$#placeholder" + o + s + a + "#$#%#$#"; i[p] = e + n + r, t = t.replace(i[p], p), o++ } return { filteredInput: t, extracted: i, getRestored: function () { var t = this.filteredInput; for (var e in i) t = t.replace(e, i[e]); return t } } } function strip_line(t) { var e = t.trim(), r = extractAllPossibleText(e, '"', '"'); return r.filteredInput = r.filteredInput.replace(/\s\s+/g, " "), r.getRestored() } function clean_lines(t) { for (var e = t.split(/\r\n|\r|\n/g), r = 0, n = 0; r < e.length; r++)if (e[r] = e[r].trim(), e[r].startsWith("#") || "" == e[r]) "" == e[r] && n++ >= 2 && (e.splice(r, 1), r--); else { n = 0; var i = e[r] = strip_line(e[r]); if ("}" != i && "{" != i && !i.includes("('{") && !i.includes("}')")) { var o = i.indexOf("#"), s = (o >= 0 && i.slice(o), o >= 0 ? i.slice(0, o) : i), a = s.indexOf("}"); if (a >= 0) { a > 0 && (e[r] = strip_line(s.slice(0, a - 1)), e.insert(r + 1, "}")); var p = strip_line(s.slice(a + 1)); "" != p && e.insert(r + 2, p), s = e[r] } var l = s.indexOf("{"); if (l >= 0) { e[r] = strip_line(s.slice(0, l)), e.insert(r + 1, "{"); var p = strip_line(s.slice(l + 1)); "" != p && e.insert(r + 2, p) } i = s } } return e } function join_opening_bracket(t) { for (var e = 0; e < t.length; e++) { "{" == t[e] && e >= 1 && (t[e] = t[e - 1] + " {", options.trailingBlankLines && t.length > e + 1 && t[e + 1].length > 0 && t.insert(e + 1, ""), t.remove(e - 1)) } return t } function perform_indentation(t) { var e, r, n; e = [], r = 0; for (var i = t, o = 0; o < i.length; o++)n = i[o], !n.startsWith("#") && n.endsWith("}") && r > 0 && (r -= 1), "" !== n ? e.push(options.INDENTATION.repeat(r) + n) : e.push(""), !n.startsWith("#") && n.endsWith("{") && (r += 1); return e } function perform_alignment(t) { for (var e, r = [], n = [], i = t, o = 0, s = 0; s < i.length; s++) { if (!("" === (e = i[s]) || e.endsWith("{") || e.startsWith("#") || e.endsWith("}") || e.trim().startsWith("upstream") || e.trim().contains("location"))) { var a = e.match(/\S+/g); if (a.length > 1) { n.push(e); var p = e.indexOf(a[1]) + 1; o < p && (o = p) } } r.push(e) } for (var l = 0; l < r.length; l++)if (e = r[l], n.includes(e)) { var c = e.match(/\S+/g), f = e.match(/\s+/g)[0]; e = f + c[0] + " ".repeat(o - c[0].length - f.length) + c.slice(1, c.length).join(" "), r[l] = e } return r } function walkSync(t, e, r) { var n = n || require("fs"), i = n.readdirSync(t); return r = r || [], e = e || "", i.forEach(function (i) { n.statSync(t + "/" + i).isDirectory() ? r = walkSync(t + "/" + i, e, r) : i.endsWith(e) && r.push(t + "/" + i) }), r } function modifyOptions(t) { for (var e in t) options[e] = t[e] } String.prototype.trim || (String.prototype.trim = function () { return this.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, "") }), String.prototype.startsWith || (String.prototype.startsWith = function (t, e) { return e = e || 0, this.substr(e, t.length) === t }), String.prototype.endsWith || (String.prototype.endsWith = function (t, e) { var r = this.toString(); ("number" != typeof e || !isFinite(e) || Math.floor(e) !== e || e > r.length) && (e = r.length), e -= t.length; var n = r.indexOf(t, e); return -1 !== n && n === e }), String.prototype.includes || (String.prototype.includes = function (t, e) { return "number" != typeof e && (e = 0), !(e + t.length > this.length) && -1 !== this.indexOf(t, e) }), String.prototype.repeat || (String.prototype.repeat = function (t) { if (null == this) throw new TypeError("can't convert " + this + " to object"); var e = "" + this; if (t = +t, t != t && (t = 0), t < 0) throw new RangeError("repeat count must be non-negative"); if (t == 1 / 0) throw new RangeError("repeat count must be less than infinity"); if (t = Math.floor(t), 0 == e.length || 0 == t) return ""; if (e.length * t >= 1 << 28) throw new RangeError("repeat count must not overflow maximum string size"); for (var r = ""; 1 == (1 & t) && (r += e), 0 != (t >>>= 1);)e += e; return r }), Array.prototype.remove || (Array.prototype.remove = function (t, e) { this.splice(t, 1) }), String.prototype.contains || (String.prototype.contains = String.prototype.includes), Array.prototype.insert || (Array.prototype.insert = function (t, e) { this.splice(t, 0, e) }); var INDENTATION = "\t", options = { INDENTATION: INDENTATION }; "undefined" != typeof module && (module.exports = { walkSync: walkSync, perform_alignment: perform_alignment, perform_indentation: perform_indentation, join_opening_bracket: join_opening_bracket, clean_lines: clean_lines, modifyOptions: modifyOptions, strip_line: strip_line });


/** nginx.js */

var ebox = $('#ebox');

var cme = CodeMirror.fromTextArea(ebox.children()[0], {
    mode: 'nginx',
    lineNumbers: true
})
cme.setValue(ss.lsStr("txt"));
cme.on("change", function () {
    ss.ls.txt = cme.getValue();
    ss.lsSave();
});

$(window).on('load resize', function () {
    cme.setSize("100%", $(this).height() - ebox.offset().top - 15);
}).click(function (e) {
    var target = e.target || window.event.srcElement;
    if (target.nodeName == "I" && target.className.indexOf('fa-arrows-alt') >= 0) {
        cme.setSize("100%", $(this).height() - ebox.offset().top - 15);
    }
})

$('#btnFormatterNginxConf').click(function () {
    var indent = $('#seindent').val();
    modifyOptions({ INDENTATION: indent });
    var cleanLines = clean_lines(cme.getValue());
    var newline = $('#senewline').val();
    if (newline == 1) {
        modifyOptions({ trailingBlankLines: false });
        cleanLines = join_opening_bracket(cleanLines);
    }
    cleanLines = perform_indentation(cleanLines, indent);
    cme.setValue(cleanLines.join("\n"));
});