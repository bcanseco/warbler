import React from "react";
import { withScriptjs, withGoogleMap, GoogleMap, Marker, InfoWindow } from "react-google-maps";

export default class Map extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      markerStatus: new Array(props.universities.length).fill(false)
    };

    this.key = "AIzaSyCo9UePsjcg75Y2ZtJVsM33xJaWM6D1Qno";
    this.GoogleMapWrapper = withScriptjs(withGoogleMap(() => (
      <GoogleMap
        defaultZoom={10}
        defaultCenter={this.props.coordinates}
      >
        {this.props.universities.map((uni, i) => (
          <Marker key={i}
                  position={uni.geometry.location}
                  onClick={() => this.toggleMarker(i)}
          >
            {(this.state.markerStatus[i]) && (
              <InfoWindow onCloseClick={() => this.toggleMarker(i)}>
                <a href="#"
                   onClick={(e) => this.props.onClickPopup(e, uni)}
                >
                  {uni.name}
                </a>
              </InfoWindow>
            )}
          </Marker>
        ))}
      </GoogleMap>
    )));
  }

  toggleMarker(markerIndex) {
    const modifiedMarkerStatus = this.state.markerStatus;
    modifiedMarkerStatus[markerIndex] = !modifiedMarkerStatus[markerIndex];
    this.setState({ markerStatus: modifiedMarkerStatus });
  }

  render() {
    const GoogleMapWrapper = this.GoogleMapWrapper;

    return (
      <div className="col">
        <GoogleMapWrapper
          containerElement={<div style={{ height: "100%" }} />}
          mapElement={<div style={{ height: "100%" }} />}
          loadingElement={<div>Loading Google Maps library...</div>}
          googleMapURL={`https://maps.googleapis.com/maps/api/js?key=${this.key}`}
        />
      </div>
    );
  }
}
