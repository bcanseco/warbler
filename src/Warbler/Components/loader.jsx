import React from "react";

export default class Loader extends React.Component {
  render() {
    return (
      <div className="progress-circular progress-circular-primary mx-auto d-flex align-items-center loader">
        <div className="progress-circular-wrapper">
          <div className="progress-circular-inner">
            <div className="progress-circular-left">
              <div className="progress-circular-spinner"></div>
            </div>
            <div className="progress-circular-gap"></div>
            <div className="progress-circular-right">
              <div className="progress-circular-spinner"></div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
