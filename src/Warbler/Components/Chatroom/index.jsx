import React from "react";
import ReactDOM from "react-dom";
import { AppContainer } from "react-hot-loader";
import { HubConnection } from "@aspnet/signalr-client";
import ChannelPane from "./Channels/channel-pane.jsx";
import MessagePane from "./Messages/message-pane.jsx";
import UserPane from "./Users/user-pane.jsx";
import "./styles.less";

export default class Chatroom extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      universities: null,
      selectedUniversity: null,
      selectedChannel: null,
      error: null
    };

    this.log = console.log.bind(console, `%c${this.constructor.name}`, "color: #1f82e4");
    this.onSelectChannel = this.onSelectChannel.bind(this);
    this.onSendMessage = this.onSendMessage.bind(this);

    this.connection = new HubConnection("/ChatHub");
    this.connection.on("receiveInitialPayload", this.onInitialPayload.bind(this));
    this.connection.on("messageReceived", this.onMessageReceived.bind(this));
    this.connection.on("onJoin", this.onJoin.bind(this));
    this.connection.on("onLeave", this.onLeave.bind(this));
    this.connection.onClosed = this.onConnectionClosed.bind(this);

    this.connection.start()
      .then(() => {
        this.log("Successfully connected to SignalR hub.");
      })
      .catch((error) => {
        console.error(error);
        this.setState({ error: "Uh oh. You broke Warbler. How could you :'(" });
      });
  }

  onInitialPayload(payload) {
    this.log("Initial payload:", payload);
    this.setState({
      universities: payload,
      selectedUniversity: payload[0],
      selectedChannel: payload[0].server.channels[0]
    });
  }

  onMessageReceived(payload) { }

  onJoin(user, channel, server, university) {
    this.log("onJoin", user, channel, server, university);
    const universities = this.state.universities;

    const otherUsers = universities
      .find(u => u.id === university.id).server.channels
      .find(c => c.id === channel.id).users;

    const trackedUser = otherUsers.find(u => u.userName === user.userName);

    if (trackedUser) {
      trackedUser.isOnline = true; // User previously joined, set them online
    } else {
      otherUsers.push(user); // User just registered, add to the list
    }

    this.setState({ universities: universities });
  }

  onLeave(user, channel, server, university) {
    this.log("onLeave", user, channel, server, university);

    const universities = this.state.universities;

    universities
      .find(u => u.id === university.id).server.channels
      .find(c => c.id === channel.id).users
      .find(u => u.userName === user.userName)
      .isOnline = false;

    this.setState({ universities: universities });
  }

  onSelectChannel(channel) {
    this.log("Selected channel:", channel);
    this.setState({
      selectedChannel: channel
    });
  }

  onSendMessage(message) {
    this.log("Sent message:", message);
    // TODO: Wait for backend to catch up
  }

  onConnectionClosed(error) {
    if (!error) return;
    console.error(error);
    this.setState({ error: "Lost connection to server. Please refresh the page." });
  }

  render() {
    let content;
    if (!!this.state.error) {
      content = <div>{this.state.error}</div>;
    } else if (!this.state.selectedUniversity) {
      content = <div>Loading...</div>;
    } else { // No errors and all data available
      content = (
        <div className="row chatroom-container">
          <ChannelPane
            channels={this.state.selectedUniversity.server.channels}
            onSelect={this.onSelectChannel}
          />
          <MessagePane
            messages={this.state.selectedChannel.messages}
            onSend={this.onSendMessage}
          />
          <UserPane users={this.state.selectedChannel.users}/>
        </div>
      );
    }

    return <AppContainer>{content}</AppContainer>;
  }
}

ReactDOM.render(<Chatroom />, document.getElementById("react-root"));
if (module.hot) module.hot.accept(); // Allow Hot Module Replacement
