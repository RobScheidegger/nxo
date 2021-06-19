﻿//All credit from https://github.com/limefrogyank/MathJaxBlazor

window.MathJax = {
    startup: {
        pageReady: () => {
            console.log("MathJax is loaded and initialized.");
        }
    }
};

window.mathJaxBlazor = {
    promise: Promise.resolve(),
    applySettings: function (texSettings) {
        MathJax.config.tex.inlineMath = texSettings.inlineMath;
        MathJax.config.tex.displayMath = texSettings.displayMath;
        MathJax.startup.getComponents();
    },
    undoTypset: function () {
        var list = MathJax.startup.document.math.toArray();
        for (let i = 0; i < list.length; i++) {
            list[i].start.node.outerHTML = list[i].start.delim + list[i].math + list[i].end.delim;
        }
    },

    typesetPromise: function () {
        this.promise = this.promise.then(() => {
            this.typesetClear();
            return MathJax.typesetPromise();
        }).catch(err => {
            console.log(err);
        });
    },
    typesetClear: function () {
        try {
            this.undoTypset();
            MathJax.texReset();
            MathJax.typesetClear();
            MathJax.startup.document.clear();
        } catch (ex) {
            console.log(ex);
        }
    },
    processMathML: async function (input) {
        return MathJax.mathml2chtmlPromise(input).then(function (node) {
            //
            //  The promise returns the typeset node, which we add to the output
            //  Then update the document to include the adjusted CSS for the
            //    content of the new equation.
            //
            MathJax.startup.document.clear();
            MathJax.startup.document.updateDocument();
            return node.outerHTML;

        }).catch(function (err) {
            //
            //  If there was an error, put the message into the output instead
            //
            return err.message;
        });
    },
    processLatex: async function (input, isDisplay) {
        return MathJax.tex2chtmlPromise(input, { display: isDisplay }).then(function (node) {
            //
            //  The promise returns the typeset node, which we add to the output
            //  Then update the document to include the adjusted CSS for the
            //    content of the new equation.
            //
            MathJax.startup.document.clear();
            MathJax.startup.document.updateDocument();
            return node.outerHTML;

        }).catch(function (err) {
            //
            //  If there was an error, put the message into the output instead
            //
            return err.message;
        });
    }
};