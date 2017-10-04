import React from "react";
import Channel from "./channel.jsx";

export default class ChannelPane extends React.Component {
  render() {
    const channels = this.props.channels.map((channel, i) =>
      <Channel data={channel} key={i} onClick={() => this.props.onSelect(channel)}/>);

    return (
      <div className="col-xs-3 col-sm-3 col-md-3 col-lg-2">
        <div className="col card">
          <div className="row">
            <h4 className="col card-title text-center">
              Channels
            </h4>
          </div>
          <div className="row auto-scroll">
            <div className="col card-list">
              {channels}
            </div>
          </div>
        </div>
      </div>
    );
  }
}
