import React from "react";
import Result from "./university-selector-result.jsx";

export default class UniversitySelector extends React.Component {
  render() {
    const items = this.props.universities.map((university, i) =>
      <Result data={university} onSelection={this.props.onClickResult} key={i}/>);

    return <div className="col">{items}</div>;
  }
}
