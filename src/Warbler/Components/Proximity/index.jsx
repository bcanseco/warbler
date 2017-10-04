import React from "react";
import ReactDOM from "react-dom";
import { AppContainer } from "react-hot-loader";
import { HubConnection } from "@aspnet/signalr-client";
import UniversitySelector from "./university-selector.jsx";
import Map from "./map.jsx";
import "./styles.less";

export default class Proximity extends React.Component {
  constructor(props) {
    super(props);

    this.log = console.log.bind(console, `%c${this.constructor.name}`, "color: #1f82e4");
    this.onLocationSuccess = this.onLocationSuccess.bind(this);
    this.onLocationError = this.onLocationError.bind(this);
    this.receiveNearbyUniversities = this.receiveNearbyUniversities.bind(this);
    this.onSuccessfulJoin = this.onSuccessfulJoin.bind(this);
    this.onConnectionClosed = this.onConnectionClosed.bind(this);

    this.state = {
      universities: null,
      coordinates: null,
      error: null
    };

    if (navigator.geolocation) {
      navigator.geolocation
        .getCurrentPosition(this.onLocationSuccess, this.onLocationError);
    } else {
      this.onLocationError(true);
    }

    this.connection = new HubConnection("/ProximityHub");
    this.connection.on("receiveNearbyUniversities", this.receiveNearbyUniversities);
    this.connection.on("successfulJoin", this.onSuccessfulJoin);
    this.connection.onConnectionClosed(this.onConnectionClosed);
  }

  receiveNearbyUniversities(universities) {
    this.log("Received nearby universities:", universities);
    this.setState({ universities: universities });
  }

  onLocationSuccess(response) {
    this.log("onLocationSuccess", response);
    this.setState({
      coordinates: {
        lat: response.coords.latitude,
        lng: response.coords.longitude
      }
    });

    this.connection.start()
      .then(() => {
        this.log("Successfully connected to SignalR hub.");
        this.connection.invoke("getNearbyUniversitiesAsync", JSON.stringify(this.state.coordinates));
      })
      .catch((error) => {
        console.error(error);
        this.setState({ error: "Uh oh. You broke Warbler. How could you :'(" });
      });
  }

  onLocationError(permissionWasDenied) {
    if (permissionWasDenied) {
      this.setState({ error: "Please allow location services to use Warbler." });
    } else {
      this.setState({ error: "Please upgrade your browser to use Warbler." });
    }
  }

  onConnectionClosed(error) {
    if (!error) return;
    console.error(error);
    this.setState({ error: "Lost connection to server. Please refresh the page." });
  }

  onSelection(event, university) {
    event.preventDefault();
    this.log("onSelection", university);
    this.connection.invoke("selectUniversityAsync", university.place_id);
  }

  onSuccessfulJoin() {
    this.log("Successfully joined university. Redirecting...");
    location.href = "/chat";
  }

  render() {
    let content;
    if (!!this.state.error) {
      content = <div>{this.state.error}</div>;
    } else if (!this.state.universities || !this.state.coordinates) {
      content = <div>Loading...</div>;
    } else { // No errors and all data available
      content = (
        <div className="row">
          <UniversitySelector
            universities={this.state.universities}
            onClickResult={this.onSelection.bind(this)} />
          <Map
            universities={this.state.universities}
            coordinates={this.state.coordinates}
            onClickPopup={this.onSelection.bind(this)} />
        </div>
      );
    }

    return <AppContainer>{content}</AppContainer>;
  }
}

ReactDOM.render(<Proximity />, document.getElementById("react-root"));
if (module.hot) module.hot.accept(); // Allow Hot Module Replacement
