import React, {Component} from 'react';
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSpinner } from '@fortawesome/free-solid-svg-icons'
import "./Loading.css";

library.add(faSpinner);

export default class Loading extends Component{

	constructor(props) {
		super(props);

	}

	render(){
    return (
      <div className="loading">
        <FontAwesomeIcon icon="spinner" spin size="3x"/>
      </div>
    );
	}
}