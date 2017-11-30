import React from "react";
import Avatar from "react-avatar";

export default class User extends React.Component {
  render() {
    return (
      <button
        type="button"
        className="user-button"
        onClick={this.props.onClick}
      >
        <Avatar name={this.props.data.userName} className="ava-img" size={30} textSizeRatio={2}/>
        <span className="text-truncate">{this.props.data.userName}</span>
        {this.props.data.isOnline && <img className="user-online" src={"dist/circle-filled.png"} />}
      </button>
    );
  }
}
