import React from "react";
import ReactQuill from "react-quill";
import "quill-emoji/dist/quill-emoji";

export default class MessageBox extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      text: null
    };
    this.log = console.log.bind(console, `%c${this.constructor.name}`, "color: #1f82e4");
    this.handleChange = this.handleChange.bind(this);

    this.style = {
      border: "1px solid grey",
      height: "44px",
      maxHeight: "44px"
    };

    this.modules = {
      toolbar: [
        ["bold", "italic", "underline", "strike"],
        ["link", "blockquote", "code"],
        ["clean"]
      ],
      keyboard: {
        bindings: {
          enter: {
            key: "Enter",
            handler: (range, context) => {
              if (!context.empty) this.onSubmit();
            }
          }
        }
      },
      toolbar_emoji: true,
      short_name_emoji: true,
      textarea_emoji: true
    };

    this.formats = this.modules.toolbar[0].concat(this.modules.toolbar[1]);
  }

  componentDidMount() {
    this.input.focus();
  }

  onSubmit() {
    this.log("onSubmit", this.state.text);
    this.props.onSubmit(this.state.text);
    this.setState({ text: null });
  }

  handleChange(value) {
    this.setState({ text: value });
  }

  render() {
    return (
      <div className="col pos-abs bottom">
        <div className="row">
          <div className="col-12">
            <ReactQuill
              theme="bubble"
              modules={this.modules}
              formats={this.formats}
              value={this.state.text}
              style={this.style}
              placeholder="Enter a message"
              ref={(input) => { this.input = input }}
              onChange={this.handleChange}
            />
          </div>
        </div>
      </div>
    );
  }
}
