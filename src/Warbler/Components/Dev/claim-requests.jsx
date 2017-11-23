import React from "react";
import "isomorphic-fetch";

export default class ClaimRequests extends React.Component {
  /**
   * Resolves a claim request.
   * @param {*} claim The claim request.
   * @param {bool} decision True to accept, false to reject
   */
  resolveClaim(claim, decision) {
    claim.IsAccepted = decision;
    fetch("/Dev/ResolveClaim",
      {
        method: "POST",
        headers: {
          'Accept': "application/json, text/plain, */*",
          'Content-Type': "application/json"
        },
        body: JSON.stringify(claim)
      })
      .then(() => location.reload());
  }

  render() {
    const claims = this.props.data && this.props.data.map((claim, i) =>
      <tr key={i}>
        <td className="align-middle">{claim.University.Name}</td>
        <td className="align-middle">{`${claim.FirstName} ${claim.LastName}`}</td>
        <td className="align-middle">{claim.PositionTitle}</td>
        <td className="align-middle">{claim.Comments}</td>
        <td className="align-middle">
          <button
            className="btn btn-sm btn-float btn-success waves-circle mr-1"
            type="button"
            onClick={() => this.resolveClaim(this.props.data[i], true)}
          >
            <i className="material-icons fa fa-check"></i>
          </button>
          <button
            className="btn btn-sm btn-float btn-danger waves-circle ml-1"
            type="button"
            onClick={() => this.resolveClaim(this.props.data[i], false)}
          >
            <i className="material-icons fa fa-times"></i>
          </button>
        </td>
      </tr>
    );

    if (!!claims) {
      return claims.length !== 0 && (
        <div className="row">
          <div className="col-md-12">
            <label>Unresolved claim requests:</label>
            <table className="table table-striped table-bordered claims-table">
              <thead>
                <tr>
                  <th>University</th>
                  <th>Requestor name</th>
                  <th>Requestor position</th>
                  <th>Comments</th>
                  <th>Accept or deny</th>
                </tr>
              </thead>
              <tbody>
                {claims}
              </tbody>
            </table>
          </div>
        </div>
      );
    } else {
      return (
        <div className="row">
          <div className="col-md-12">
            <span>Loading claim requests...</span>
          </div>
        </div>
      );
    }
  }
}
