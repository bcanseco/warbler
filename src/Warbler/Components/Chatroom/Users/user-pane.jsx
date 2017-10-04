import React from "react";
import User from "./user.jsx";

export default class UserPane extends React.Component {
  render() {
    const users = this.props.users
      .filter(u => u.isOnline)
      .map((user, i) => <User data={user} key={i}/>);

    return (
      <div className="hidden-sm-down col-md-3 col-lg-2">
        <div className="col card chat-card">
          <div className="row">
            <h4 className="col card-title text-center">
              Users
            </h4>
          </div>
          <div className="row auto-scroll">
            <div className="col card-list">
              {users}
            </div>
          </div>
        </div>
      </div>
    );
  }
}
