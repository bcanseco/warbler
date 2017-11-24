import React from "react";
import Popover from 'react-simple-popover';

export default class ServerButton extends React.Component {
  onButtonClick() {
    this.props.popoutFunc(this.button.getBoundingClientRect().top, this.button.getBoundingClientRect().left, this.button.offsetWidth)
  }

  render() {
    return (
      <div>
        <button
          type="button"
          className="server-button"
          ref={(button) => { this.button = button }}
          onClick={this.onButtonClick.bind(this)}
        >
          <span className="text-truncate">{this.props.server}</span>
        </button>
      </div>
    );
  }
}
