import React, {Component} from 'react';
import "./Controls.css";

export default class Classes extends Component{
  render() {
    return (
      <div className="controls-container">
        <h3>{this.props.title}</h3>

        <div className="controls-btn-container">
          {this.props.children}
        </div>
      </div>
    );
  }
}