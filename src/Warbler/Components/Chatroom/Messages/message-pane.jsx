import React from "react";
import ReactDOM from "react-dom";
import Message from "./message.jsx";
import MessageBox from "./message-box.jsx";

export default class MessagePane extends React.Component {
  scrollToBottom() {
    var el = this.refs.messageWrap;
    el.scrollTop = el.scrollHeight;
  }

  componentDidMount() {
    this.scrollToBottom();
  }

  componentDidUpdate() {
    this.scrollToBottom();
  }

  handleScrollEvent() {
    var node = this.refs.messageWrap
    if (node.scrollTop < 10) {
      console.log("xd");
    } 
  }

  render() {
    const messages = this.props.messages.map((message, i) =>
      <Message data={message} key={i}/>);

    return (
      <div className="message-pane">
        <h4 className="chat-current-channel">
          <i>#{this.props.channelName}</i>
        </h4>
        <div className="auto-scroll h-90" onScroll={this.handleScrollEvent.bind(this)} ref="messageWrap">
          <ul className="message-area">{messages}</ul>
        </div>
        <div className="pos-rel">
          <MessageBox onSubmit={this.props.onSend} />
        </div>
      </div>
    );
  }
}
