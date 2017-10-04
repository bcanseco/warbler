import React from "react";

export default class User extends React.Component {
  render() {
    return (
      <div className="list-group-item">
        <div className="row mw-100">
          <div className="col-3">
            <img className="mw-100" src={"dist/ava.png"}/>
          </div>
          <div className="col-9 text-truncate align-self-center">
            {this.props.data.userName}
          </div>
        </div>
      </div>
    );
  }
}
