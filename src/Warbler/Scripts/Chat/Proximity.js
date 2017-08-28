(function(ko, $, json, geolocation) {
    "use strict";

    /**
     * @constructor
     * @summary Viewmodel for the University selection view.
     * @desc Gets the user's location, shows every nearby university.
     * @requires KnockoutJS
     * @requires jQuery
     */
    function ProximityViewModel() {
        var self = this;
        
        this.gracefulExit = false;
        this.universities = ko.observableArray();
        this.hub = $.connection.proximityHub; // ProximityHub.cs

        this.hub.client.receiveNearbyUniversities = function(universities) {
            console.info("receiveNearbyUniversities()", universities);
            self.universities(universities);
        };

        this.hub.client.onSuccessfulJoin = function () {
            console.info("onSuccessfulJoin()");
            self.gracefulExit = true; // don't show error for disconnection
            self.hub.connection.stop();
            location.href = $("#RedirectTo").val(); // Go to chatroom view
        };

        this.onSelection = function (university) {
            console.info("onSelection()", university);
            self.hub.server.selectUniversityAsync(university.place_id);
        };

        function getLocationAsync() {
            var options = {
                enableHighAccuracy: true,
                timeout: 10000
            };

            if (geolocation) {
                geolocation.getCurrentPosition(onSuccess, onError, options);
            } else {
                onError(true);
            }

            function onSuccess(response) {
                initHub({
                    lat: response.coords.latitude,
                    lng: response.coords.longitude
                });
            }

            function onError(permissionDenied) {
                if (permissionDenied) {
                    alert("Sorry, we encountered an error getting your location." +
                        " Please make sure you allow location services.");
                } else {
                    alert("Please upgrade your browser to use Warbler.");
                }
            }
        }

        function initHub(userLocation) {
            var connection = self.hub.connection;
            connection.logging = true;

            connection.start()
                .done(function() {
                    console.info("Successfully connected to SignalR hub.");
                    self.hub.server.getNearbyUniversitiesAsync(json.stringify(userLocation));
                })
                .fail(function () {
                    console.error("Error connecting to SignalR hub. Please refresh the page to try again.");
                });

            connection.disconnected(function () {
                if (!self.gracefulExit) {
                    console.error("Lost connection to server. Please refresh the page.");
                }
            });
        }

        // start immediately on KnockoutJS load
        getLocationAsync(); 
    }

    if (ko && $) {
        ko.applyBindings(new ProximityViewModel(), document.getElementById("university-list"));
    } else {
        throw new Error("KnockoutJS and jQuery are required.");
    }
})(window.ko, window.jQuery, window.JSON, navigator.geolocation);
