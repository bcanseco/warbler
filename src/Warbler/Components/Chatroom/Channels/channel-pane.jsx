import React from "react";
import Channel from "./channel.jsx";
import ServerButton from "../Server/server-button.jsx";

export default class ChannelPane extends React.Component {
  render() {
    const channels = this.props.channels.map((channel, i) =>
      <Channel data={channel} key={i} onClick={() => this.props.onSelect(channel)}/>);

    return (
      <div className="channel-pane">
        <div className="channel-box">
          <h4 className="chat-header text-center">
            Channels
          </h4>
          <div className="auto-scroll">
            <div>
              {channels}
            </div>
          </div>
        </div>
        <div className="divider-box">
          <div className="divider-horizontal" />
        </div>
        <div className="server-box">
          <ServerButton server={this.props.server} popoutFunc={this.props.popoutFunc} />
        </div>
      </div>
    );
  }
}
