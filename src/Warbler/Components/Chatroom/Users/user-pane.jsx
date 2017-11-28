import React from "react";
import User from "./user.jsx";

export default class UserPane extends React.Component {
  render() {
    const users = this.props.users
      .map((user, i) => {
        user.userName = this.props.nicknames[user.userName] || user.userName; 
        return <User data={user} key={i}/>;
      });

    return (
      <div className="user-pane">
        <div>
          <h4 className="chat-header text-center">
            Users
          </h4>
          <div className="auto-scroll">
            {users}
          </div>
        </div>
      </div>
    );
  }
}
