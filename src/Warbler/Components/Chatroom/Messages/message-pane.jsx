import React from "react";
import Message from "./message.jsx";
import MessageBox from "./message-box.jsx";

export default class MessagePane extends React.Component {
  render() {
    const messages = this.props.messages.map((message, i) =>
      <Message data={message} key={i}/>);

    return (
      <div className="message-pane">
        <h4 className="chat-current-channel">
          <i>#{this.props.channelName}</i>
        </h4>
        <div className="auto-scroll h-81">
          <ul className="auto-scroll message-area">{messages}</ul>
        </div>
        <div className="pos-rel">
          <MessageBox onSubmit={this.props.onSend} />
        </div>
      </div>
    );
  }
}
