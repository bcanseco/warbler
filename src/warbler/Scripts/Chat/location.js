(function (ko, $, json, geolocation) {
    var map;
    var infoWindow;

    function initMap() {
        if (geolocation) {
            geolocation.getCurrentPosition(function(position) {
                document.getElementById('map-canvas').style.display = 'block';
	            var pos = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };
		        var mapOptions = {
		            center: pos,
                    zoom: 12
		        }
                infoWindow = new google.maps.InfoWindow();
		
                map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
                var service = new google.maps.places.PlacesService(map);
                service.nearbySearch({
                location: pos,
                radius: 24141,
                type: ['university']
            }, callback);
		
            }, function() {
                handleLocationError(true, infoWindow, map.getCenter());
            });
          
        } else {
        // Browser doesn't support Geolocation
            handleLocationError(false, infoWindow, map.getCenter());
        }
    }

    function handleLocationError(browserHasGeolocation, infoWindow, pos) {
        infoWindow.setPosition(pos);
        infoWindow.setContent(browserHasGeolocation ?
                          'Error: The Geolocation service failed.' :
                          'Error: Your browser doesn\'t support geolocation.');
    }

  /*function callback(results, status) {
    if (status === google.maps.places.PlacesServiceStatus.OK) {
          var places = [];
		  
		  for (var i = 0; i < results.length; i++) { 
		    var locations = results[i].geometry.location;
		    var marker = new google.map.Marker({
              position: results[i].geometry.location
			});
			places.push(marker);
			google.maps.event.addListener(marker, 'click', function() {
              infoWindow.setContent(results[i].name);
              infoWindow.open(map, this);
            });
		  }
		  
		  var markercluster = new MarkerClusterer(map, places, {imagePath: 'C:\Users\William\Downloads\v3-utility-library-master\v3-utility-library-master\markerclusterer\images\m2.png'});
        }
      }*/
	  
    function callback(results, status) {
        if (status === google.maps.places.PlacesServiceStatus.OK) {
            for (var i = 0; i < results.length; i++) {
            createMarker(results[i]);
            }
        }
    }

    function createMarker(place) {
        var placeLoc = place.geometry.location;
        var marker = new google.maps.Marker({
            map: map,
            position: place.geometry.location
        });

        google.maps.event.addListener(marker, 'click', function() {
            infoWindow.setContent(place.name);
            infoWindow.open(map, this);
        });
    }

    function loadScript() {
	    var script = document.createElement('script');
    	script.type = 'text/javascript';
        script.src = "https://maps.googleapis.com/maps/api/js?v=3.exp&key=AIzaSyCo9UePsjcg75Y2ZtJVsM33xJaWM6D1Qno&libraries=places&callback=initMap"
        
	    document.body.appendChild(script);
	    //script.src = "C:/Users/William/Downloads/v3-utility-library-master/v3-utility-library-master/markerclusterer/src/markerclusterer.js"
    }

    if (ko && $) {
        ko.applyBindings(new loadScript(), document.getElementById("map-canvas"));
    } else {
        throw new Error("KnockoutJS and jQuery are required.");
    }
})(window.ko, window.jQuery, window.JSON, navigator.geolocation);
