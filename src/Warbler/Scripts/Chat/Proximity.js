((ko, $, google) => {
    "use strict";

    /**
     * @constructor
     * @summary Viewmodel for the University selection view.
     * @desc Gets the user's location, shows every nearby university.
     * @requires KnockoutJS
     * @requires jQuery
     * @requires GoogleMaps
     */
    function ProximityViewModel() {
        const self = this;
        const hub = $.connection.proximityHub; // ProximityHub.cs

        let gracefulExit = false;
        let coordinates = null;

        this.universities = ko.observableArray();
        this.showMap = ko.observable(false);

        this.onSelection = (university) => {
            console.info("onSelection()", university);
            hub.server.selectUniversityAsync(university.place_id);
        };

        this.onClickPopup = (universityIndex) => {
            console.info("onClickPopup()", universityIndex);
            self.onSelection(self.universities()[universityIndex]);
        };

        const initializeMap = (universities) => {
            const infoWindow = new google.maps.InfoWindow();
            const map = new google.maps.Map(document.getElementById("map-canvas"), {
                center: coordinates,
                zoom: 10
            });

            self.showMap(true);

            universities.forEach((university, index) => {
                const marker = new google.maps.Marker({
                    map: map,
                    position: university.geometry.location
                });

                google.maps.event.addListener(marker, "click", function() {
                    infoWindow.setContent(`
                        <a href="#"
                           class="info-window"
                           data-bind="click: onClickPopup.bind($data, ${index})">
                            ${university.name}
                        </a>`);
                    infoWindow.open(map, this);
                    ko.applyBindings(self, $(".info-window")[0]);
                });
            });
        };

        const initializeHub = (userLocation) => {
            const connection = hub.connection;
            connection.logging = true;

            connection.start()
                .done(() => {
                    console.info("Successfully connected to SignalR hub.");
                    hub.server.getNearbyUniversitiesAsync(JSON.stringify(userLocation));
                })
                .fail(() => console.error("Error connecting to SignalR hub. Please refresh the page to try again."));

            connection.disconnected(() => {
                if (!gracefulExit) {
                    console.error("Lost connection to server. Please refresh the page.");
                }
            });
        };

        const onLocationSuccess = (response) => {
            initializeHub(coordinates = {
                lat: response.coords.latitude,
                lng: response.coords.longitude
            });
        };

        const onLocationError = (permissionDenied) => {
            if (permissionDenied) {
                alert("Please allow location services to use Warbler.");
            } else {
                alert("Please upgrade your browser to use Warbler.");
            }
        };

        hub.client.receiveNearbyUniversities = (universities) => {
            console.info("receiveNearbyUniversities()", universities);
            self.universities(universities);
            initializeMap(universities);
        };

        hub.client.onSuccessfulJoin = () => {
            console.info("onSuccessfulJoin()");
            gracefulExit = true; // don't show error for disconnection
            hub.connection.stop();
            location.href = $("#RedirectTo").val(); // go to chatroom view
        };

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(onLocationSuccess, onLocationError);
        } else {
            onLocationError(true);
        }
    }

    if (google, ko && $) {
        ko.applyBindings(new ProximityViewModel());
    } else {
        throw new Error("KnockoutJS, jQuery, and Google Maps are required.");
    }
})(window.ko, window.jQuery, window.google);
