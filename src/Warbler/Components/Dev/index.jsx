import React from "react";
import ReactDOM from "react-dom";
import { AppContainer } from "react-hot-loader";
import UniversityDump from "./university-dump.jsx";
import "isomorphic-fetch";
import "./styles.less";

export default class Dev extends React.Component {
  constructor(props) {
    super(props);
    this.log = console.log.bind(console, `%c${this.constructor.name}`, "color: #1f82e4");

    this.state = {
      universities: null
    };

    this.getUniversities();
  }

  getUniversities() {
    fetch("/Dev/Universities")
      .then(response => response.json())
      .then(universities => {
        this.log("getUniversities()", universities);
        this.setState({ universities: universities});
      });
  }

  render() {
    return (
      <AppContainer>
        <UniversityDump data={this.state.universities} />
      </AppContainer>
    );
  }
}

ReactDOM.render(<Dev />, document.getElementById("react-root"));
if (module.hot) module.hot.accept(); // Allow Hot Module Replacement
