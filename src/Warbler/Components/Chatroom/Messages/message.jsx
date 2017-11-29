import React from "react";

export default class Message extends React.Component {
  formatDate(date) {
    var hours = date.getHours() % 12 === 0 ? 12 : date.getHours() % 12;
    var paddedMinutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
    var ampm = date.getHours >= 12 ? "pm" : "am";
    return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear() + " " + hours + ":" + paddedMinutes + " " + ampm
  };

  render() {
    return (
      <li className="message-container">
        <div className="ava-thumbnail message-content-box">
          <img className="ava-img" src={"dist/ava.png"} />
        </div>
        <div className="message-text">{this.props.data.text}</div>
        <div className="message-details">
          {this.props.data.sender.userName || "username_placeholder"}
          &nbsp;-&nbsp;
          {this.formatDate(new Date(this.props.data.sendDate))}
        </div>
      </li>
    );
  }
}
