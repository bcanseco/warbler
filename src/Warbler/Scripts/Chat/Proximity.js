((ko, $, google, signalR) => {
  "use strict";

  /**
   * @constructor
   * @summary Viewmodel for the University selection view.
   * @desc Gets the user's location, shows every nearby university.
   * @requires KnockoutJS
   * @requires jQuery
   * @requires GoogleMaps
   * @requires SignalR
   */
  function ProximityViewModel() {
    const self = this;
    const connection = new signalR.HubConnection("/ProximityHub");

    let gracefulExit = false;
    let coordinates = null;

    this.universities = ko.observableArray();
    this.showMap = ko.observable(false);

    this.onSelection = (university) => {
      console.info("onSelection()", university);
      connection.invoke("selectUniversityAsync", university.place_id);
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

        google.maps.event.addListener(marker, "click", function () {
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
      connection.start()
        .then(() => {
          console.info("Successfully connected to SignalR hub.");
          connection.invoke("getNearbyUniversitiesAsync", JSON.stringify(userLocation));
        })
        .catch(() => console.error("Error connecting to SignalR hub. Please refresh the page to try again."));

      connection.onConnectionClosed((error) => {
        if (error) console.warn(error);
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

    connection.on("receiveNearbyUniversities", (universities) => {
      console.info("Received nearby universities:", universities);
      self.universities(universities);
      initializeMap(universities);
    });

    connection.on("successfulJoin", () => {
      console.info("Successfully joined university. Redirecting...");
      gracefulExit = true; // don't show error for disconnection
      connection.stop();
      location.href = $("#RedirectTo").val(); // go to chatroom view
    });

    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(onLocationSuccess, onLocationError);
    } else {
      onLocationError(true);
    }
  }

  if (google, ko && $ && signalR) {
    ko.applyBindings(new ProximityViewModel());
  } else {
    throw new Error("KnockoutJS, jQuery, Google Maps, and SignalR are required.");
  }
})(window.ko, window.jQuery, window.google, window.signalR);
