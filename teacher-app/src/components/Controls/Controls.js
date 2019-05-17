import React, {Component} from 'react';
import "./Controls.css";

export default class Classes extends Component{
  render() {
    return (
      <div className="controls-container">
        <h2>{this.props.title}</h2>

        <div className="controls-btn-container">
          {this.props.children}
        </div>
      </div>
    );
  }
}