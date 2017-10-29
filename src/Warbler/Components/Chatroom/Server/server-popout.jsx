import React from "react";
import Server from "./server.jsx";

export default class ServerPopout extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      divStyle: null,
      firstRender: true
    }
  }

  componentDidMount() {
    if (this.state.divStyle === null) {
      console.log(this.div.style.offsetHeight);
      console.log(this.div.offsetHeight);
      var style = {
        top: this.props.buttonTop - (this.div.offsetHeight) - 70,
        left: this.props.buttonLeft,
        width: this.props.buttonWidth
      };

      this.setState({
        divStyle: {style}
      });
    }
  }

  componentWillReceiveProps(nextProps) {
    console.log(nextProps);

  }

  render() {
    const servers = this.props.servers
      .map((server) => <Server className="server" data={server} onClick={() => this.props.onSelect(server)} key={server.id} />);

    console.log("rendered", this.state.divStyle);

    if (this.state.divStyle === null || this.state.divStyle === undefined) {
      return (
        <div
          className="server-popout"
          ref={(div) => this.div = div}
        >
          <h4 className="chat-header text-center">
            Universities
          </h4>
          <div>
            {servers}
          </div>
        </div>
      );
    } else {
      console.log(this.state.divStyle);
      return(
        <div
          className="server-popout"
          ref={(div) => this.div = div }
          style={this.state.divStyle.style}
        >
          <h4 className="chat-header text-center">
            Universities
          </h4>
          <div>
            {servers}
          </div>
        </div>
      );
    }
  }
}
