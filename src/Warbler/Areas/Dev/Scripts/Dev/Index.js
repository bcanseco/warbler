(function (ko, $) {
    "use strict";

    /**
     * @constructor
     * @summary Viewmodel for the developer view.
     * @desc Shows the JSON for all universities at full depth.
     * @requires KnockoutJS
     * @requires jQuery
     */
    function DeveloperViewModel() {
        function getUniversitiesAsync() {
            $.ajax({
                    type: "GET",
                    url: "/Dev/Dev/Universities",
                    dataType: "json"
                })
                .done(function(response) {
                    console.info("universities", response);
                    $("#universities").html(prettyPrint(response));
                })
                .fail(function(a, b, c) {
                    console.error(a, b, c);
                });
        }

        function prettyPrint(obj) {
            var jsonLine = /^( *)("[\w]+": )?("[^"]*"|[\w.+-]*)?([,[{])?$/mg;

            function replaceJson(match, pIndent, pKey, pVal, pEnd) {
                var key = "<span class=json-key>";
                var val = "<span class=json-value>";
                var str = "<span class=json-string>";
                var r = pIndent || "";
                if (pKey)
                    r = r + key + pKey.replace(/[": ]/g, "") + "</span>: ";
                if (pVal)
                    r = r + (pVal[0] === '"' ? str : val) + pVal + "</span>";
                return r + (pEnd || "");
            }

            return JSON.stringify(obj, null, 2)
                .replace(/&/g, "&amp;").replace(/\\"/g, "&quot;")
                .replace(/</g, "&lt;").replace(/>/g, "&gt;")
                .replace(jsonLine, replaceJson);
        }

        // start immediately on KnockoutJS load
        getUniversitiesAsync();
    }

    if (ko && $) {
        ko.applyBindings(new DeveloperViewModel());
    } else {
        throw new Error("KnockoutJS and jQuery are required.");
    }
})(window.ko, window.jQuery);
