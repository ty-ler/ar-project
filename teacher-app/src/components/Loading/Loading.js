import React, {Component} from 'react';
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSpinner } from '@fortawesome/free-solid-svg-icons'
import "./Loading.css";

library.add(faSpinner);

export default class Loading extends Component{

	render(){

    if(this.props.size) {
      return (
        <div className={`loading ${this.props.className ? `${this.props.className}` : ""}`}>
          <FontAwesomeIcon icon="spinner" spin size={this.props.size}/>
        </div>
      )
    }

    return (
      <div className={`loading-page ${this.props.className ? `${this.props.className}` : ""}`}>
        <FontAwesomeIcon icon="spinner" spin size="3x"/>
      </div>
    );
	}
}