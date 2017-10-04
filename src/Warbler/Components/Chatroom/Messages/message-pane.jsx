import React from "react";
import Message from "./message.jsx";
import MessageBox from "./message-box.jsx";

export default class MessagePane extends React.Component {
  render() {
    const messages = this.props.messages.map((message, i) =>
      <Message data={message} key={i}/>);

    return (
      <div className="col-xs-6 col-sm-9 col-md-6 col-lg-8">
        <div className="col card">
          <div className="row h-100">
            <ul className="col message-container">{messages}</ul>
          </div>
          <div className="row pos-rel">
            <MessageBox onSubmit={this.props.onSend} />
          </div>
        </div>
      </div>
    );
  }
}
