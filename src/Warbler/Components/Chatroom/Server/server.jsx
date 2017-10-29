import React from "react";

export default class Server extends React.Component {
  render() {
    return (
      <button
        type="button"
        className="server"
        onClick={this.props.onClick}
      >
      <span className="text-truncate">#{this.props.data.name}</span>
      </button>
    );
  }
}
