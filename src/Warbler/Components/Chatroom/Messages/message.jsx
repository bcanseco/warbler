import React from "react";

export default class Message extends React.Component {
  render() {
    return (
      <li className="list-group-item">{this.props.data.text}</li>
    );
  }
}
