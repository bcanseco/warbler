import React from "react";
import Avatar from "react-avatar";

export default class Message extends React.Component {
  formatDate(date) {
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
  }

  render() {
    return (
      <li className="message-container">
        <div className="ava-thumbnail message-content-box">
          <Avatar name={this.props.data.sender.userName} className="ava-img" size={30} textSizeRatio={2}/>
        </div>
        <div className="message-text" dangerouslySetInnerHTML={{__html: this.props.data.text}}></div>
        <div className="message-details">
          {this.props.data.sender.userName || "username_placeholder"}
          &nbsp;-&nbsp;
          {this.formatDate(new Date(this.props.data.sendDate))}
        </div>
      </li>
    );
  }
}
