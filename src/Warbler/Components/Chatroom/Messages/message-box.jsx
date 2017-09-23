import React from "react";

export default class MessageBox extends React.Component {
  componentDidMount() {
    this.input.focus();
  }

  onSubmit(e) {
    e.preventDefault();
    this.props.onSubmit(this.input.value);
    this.input.value = null;
    this.input.focus();
  }

  render() {
    return (
      <div className="col pos-abs bottom">
        <form
          className="row"
          onSubmit={this.onSubmit.bind(this)}
        >
          <div className="col-sm-9 col-lg-10">
            <input
              type="text"
              className="form-control"
              placeholder="Enter a message"
              ref={(input) => { this.input = input }}
            />
          </div>
          <div className="col-sm-3 col-lg-2">
            <span className="input-group-btn">
              <button
                className="btn btn-secondary"
                type="button"
                onClick={this.onSubmit.bind(this)}
              >
                Send
              </button>
            </span>
          </div>
        </form>
      </div>
    );
  }
}
