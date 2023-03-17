import { nrApp } from '../../../../frame/Bootstrap/nrApp';
import { nrcRely } from '../../../../frame/nrcRely';

let nrPage = {
    pathname: ['/gist/code/*', '/gist/user/*', '/gist/discover'],

    init: async () => {
        let domMarkdownBody = document.querySelector('.markdown-body');
        let domPlayer = document.querySelector('asciinema-player');

        // markdown
        if (domMarkdownBody) {
            //markdown 编辑器
            await nrcRely.remote("netnrmd");

            //查看一篇 markdown
            if (domMarkdownBody.dataset.language == "markdown" && location.pathname.toLocaleLowerCase().includes("code")) {
                domMarkdownBody.innerHTML = netnrmd.render(domMarkdownBody.children[0].innerHTML);
            } else {
                document.querySelectorAll("div>pre>code").forEach(domCode => {
                    let domPre = domCode.parentElement;
                    if (domCode.dataset.language != "markdown") {
                        let domV = document.createElement("div");
                        domV.innerHTML = netnrmd.render('```\r\n' + domCode.innerText + '\r\n```');

                        domPre.innerHTML = domV.children[0].innerHTML;
                        domPre.children[0].contentEditable = true;
                    }
                })
            }
        } else if (domPlayer) {
            domPlayer.innerHTML = nrApp.tsLoadingHtml;
            // asciinema-player
            await nrcRely.remote('asciinema-player');
            domPlayer.innerHTML = "";

            AsciinemaPlayer.create(domPlayer.dataset.source, domPlayer, {
                poster: "npt:0:44", fit: "height"
            });
        }
    },

}

export { nrPage };