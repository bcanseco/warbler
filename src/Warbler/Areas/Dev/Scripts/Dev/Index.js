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
        var self = this;
        
        this.universities = ko.observable();

        function getUniversitiesAsync() {
            $.ajax({
                    type: "GET",
                    url: "/Dev/Dev/GetEverything",
                    dataType: "json"
                })
                .done(function(response) {
                    console.info("universities", response);
                    self.universities(JSON.stringify(response, undefined, 2));
                })
                .fail(function(a, b, c) {
                    console.error(a, b, c);
                });
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
