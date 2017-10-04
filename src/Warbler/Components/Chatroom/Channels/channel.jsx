import React from "react";

export default class Channel extends React.Component {
  render() {
    return (
      <button
        type="button"
        className="list-group-item list-group-item-action"
        onClick={this.props.onClick}
      >
        <span className="text-truncate">#{this.props.data.name}</span>
      </button>
    );
  }
}
