import { edit } from 'ace-builds/src-noconflict/ace'
import 'ace-builds/src-min-noconflict/ext-linking'
import 'ace-builds/src-noconflict/mode-markdown'
import 'ace-builds/src-noconflict/theme-github'
import 'ace-builds/src-noconflict/theme-tomorrow_night'
import 'ace-builds/src-noconflict/ext-searchbox'

class netnrmdAce {
    constructor(id) {
        var mew = edit(id, {
            mode: 'ace/mode/markdown', // markdown
            theme: 'ace/theme/github', // theme
            enableLinking: true, // click+link
            fontSize: 16,
            wrap: true, // wrap text
            printMargin: false, // print margin
            copyWithEmptySelection: true, // Ctrl+X 
        });

        // ctrl+click open link
        mew.on("linkClick", function (data) {
            if (data && data.token) {
                try {
                    new URL(data.token.value);
                    window.open(data.token.value, "_blank");
                } catch (ex) { }
            }
        });

        return mew;
    }
}

export { netnrmdAce };