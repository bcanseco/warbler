import React from "react";

export default class VerticalDivider extends React.Component {
  render() {
    var divStyle = {
      order: this.props.order
    };

    return (
      <div className="vertical-divider" style={divStyle}>
        <div className="vertical-divider-spacer"></div>
        <div className="vertical-divider-line"></div>
        <div className="vertical-divider-spacer"></div>
      </div>
    );
  }
}
