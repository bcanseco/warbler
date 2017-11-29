import React from "react";

export default class Message extends React.Component {
  render() {
    return (
      <li className="message-container">
        <img className="ava-thumbnail" src={ "dist/ava.png"}/>
        <span className="d-inline-block" dangerouslySetInnerHTML={{__html: this.props.data.text}}/>
      </li>
    );
  }
}
