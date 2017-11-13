import React from "react";
import ReactDOM from "react-dom";
import { AppContainer } from "react-hot-loader";
import { HubConnection } from "@aspnet/signalr-client";
import ChannelPane from "./Channels/channel-pane.jsx";
import MessagePane from "./Messages/message-pane.jsx";
import UserPane from "./Users/user-pane.jsx";
import VerticalDivider from "./vertical-divider.jsx";
import ServerPopout from "./Server/server-popout.jsx";
import "./styles.less";

export default class Chatroom extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      universities: null,
      selectedUniversity: null,
      selectedChannel: null,
      error: null,
      serverPopout: false,
      buttonTop: 0,
      buttonLeft: 0,
      buttonWidth: 0
    };

    this.log = console.log.bind(console, `%c${this.constructor.name}`, "color: #1f82e4");
    this.onSelectServer = this.onSelectServer.bind(this);
    this.onSelectChannel = this.onSelectChannel.bind(this);
    this.onSendMessage = this.onSendMessage.bind(this);

    this.onServerPopoutOpen = this.onServerPopoutOpen.bind(this);

    this.connection = new HubConnection("/ChatHub");
    this.connection.on("receiveInitialPayload", this.onInitialPayload.bind(this));
    this.connection.on("onMessageReceived", this.onMessageReceived.bind(this));
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

  onMessageReceived(message, user, channel, server, university) {
    this.log("onMessageReceived", message, user, channel, server, university);
    const universities = this.state.universities;

    universities
      .find(u => u.id === university.id).server.channels
      .find(c => c.id === channel.id).messages
      .push(message);

    this.setState({ universities: universities });
  }

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

  onSelectServer(server) {
    this.log("Selected server:", server);
    this.setState({
      selectedServer: server
    });
    this.setState({
      selectedChannel: this.state.selectedUniversity.server.channels[0]
    });
  }

  onSelectChannel(channel) {
    this.log("Selected channel:", channel);
    this.setState({
      selectedChannel: channel
    });
  }

  onSendMessage(message) {
    this.log("Sent message:", message);
    const channel = this.state.selectedChannel;
    this.connection.invoke("sendMessageAsync", channel, message);
  }

  onConnectionClosed(error) {
    if (!error) return;
    console.error(error);
    this.setState({ error: "Lost connection to server. Please refresh the page." });
  }

  onServerPopoutOpen(buttonTop, buttonLeft, buttonWidth) {
    this.setState({
      buttonTop: buttonTop,
      buttonLeft: buttonLeft,
      buttonWidth: buttonWidth,
      serverPopout: !this.state.serverPopout
    });
  }

  render() {
    let content;
    if (!!this.state.error) {
      content = <div>{this.state.error}</div>;
    } else if (!this.state.selectedUniversity) {
      content = <div>Loading...</div>;
    } else { // No errors and all data available
      content = (
        <div>
          <div className="chatroom-container">
            <ChannelPane
              server={this.state.selectedUniversity.name}
              channels={this.state.selectedUniversity.server.channels}
              onSelect={this.onSelectChannel}
              popoutFunc={this.onServerPopoutOpen}
            />
            <VerticalDivider order={2}/>
            <MessagePane
              channelName={this.state.selectedChannel.name}
              messages={this.state.selectedChannel.messages}
              onSend={this.onSendMessage}
            />
            <VerticalDivider order={4} />
            <UserPane users={this.state.selectedChannel.users} />
          </div>
          <div>
            {this.state.serverPopout
              ? <ServerPopout
                  servers={this.state.universities}
                  buttonWidth={this.state.buttonWidth}
                  buttonTop={this.state.buttonTop}
                  buttonLeft={this.state.buttonLeft}
                  onSelect={this.onSelectServer}
                />
              : null}
          </div>
        </div>
      );
    }

    return <AppContainer>{content}</AppContainer>;
  }
}

ReactDOM.render(<Chatroom />, document.getElementById("react-root"));
if (module.hot) module.hot.accept(); // Allow Hot Module Replacement
