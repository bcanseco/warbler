import React from "react";
import ReactDOM from "react-dom"

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
      var style = {
        top: this.props.buttonTop - (this.div.offsetHeight + 5),
        left: this.props.buttonLeft,
        width: this.props.buttonWidth
      };

      this.setState({
        divStyle: {style}
      });
    }
  }


  render() {
    const servers = this.props.servers
      .map((server) => <li key={server.id}>{server.name}</li>);

    if (this.state.divStyle === null) {
      return (
        <div
          className="server-popout"
          ref={(div) => this.div = div}
        >
          <ul>
            {servers}
          </ul>
        </div>
      );
    } else {
      console.log(this.state.divStyle);
      return(
        <div
          className="server-popout"
          ref={(div) => this.div = div }
          style={this.state.divStyle}
        >
          <ul>
            {servers}
          </ul>
        </div>
      );
    }
  }
}
