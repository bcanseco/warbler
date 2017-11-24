import React from "react";

export default class UniversityDump extends React.Component {
  createMarkup(data) {
    const jsonLine = /^( *)("[\w]+": )?("[^"]*"|[\w.+-]*)?([,[{])?$/mg;

    const replaceJson = (match, pIndent, pKey, pVal, pEnd) => {
      const key = "<span class=json-key>";
      const val = "<span class=json-value>";
      const str = "<span class=json-string>";
      let r = pIndent || "";
      if (pKey)
        r = r + key + pKey.replace(/[": ]/g, "") + "</span>: ";
      if (pVal)
        r = r + (pVal[0] === '"' ? str : val) + pVal + "</span>";
      return r + (pEnd || "");
    }

    const prettyPrint = JSON.stringify(data, null, 2)
      .replace(/&/g, "&amp;").replace(/\\"/g, "&quot;")
      .replace(/</g, "&lt;").replace(/>/g, "&gt;")
      .replace(jsonLine, replaceJson);

    return { __html: prettyPrint };
  }

  render() {
    return (
      <div className="row">
        <div className="col-md-12">
          <label>University dump:</label>
          <pre>
            <code dangerouslySetInnerHTML={this.createMarkup(this.props.data)}></code>
          </pre>
        </div>
      </div>
    );
  }
}
