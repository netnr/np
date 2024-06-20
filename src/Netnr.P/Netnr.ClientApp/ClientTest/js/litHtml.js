import { html, render } from "lit-html";

let litHtml = {
    init: async () => {

        let href = `static/banan_video/2024/203595344031744.html"ebv9r<a b=c>isdmh`
        let text = "<script>alert('XSS')</script>";
        let text2 = html`<a href="${href}">${href}</a>`
        let template = html`<a href="${href}">${text}</a><br>${text2}`;

        document.body.innerHTML = '<h2>123</h2>';
        console.debug(render(template, document.body))
    },
}

Object.assign(window, { litHtml });
export { litHtml };