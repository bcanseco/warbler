import React from "react";

export default class Message extends React.Component {
  render() {
    return (
      <li className="message-container">
        <img className="ava-thumbnail" src={ "dist/ava.png"}/>
        {this.props.data.text}
      </li>
    );
  }
}
