import React from "react";

export default class University extends React.Component {
  render() {
    const data = this.props.data;

    return (
      <a href="#"
         className="list-group-item list-group-item-action flex-column align-items-start active"
         onClick={(e) => this.props.onSelection(e, this.props.data)}
      >
        <div className="d-flex w-100 justify-content-between">
          <h5 className="mb-1">{data.name}</h5>
        </div>
        <small>{data.vicinity}</small>
      </a>
    );
  }
}
