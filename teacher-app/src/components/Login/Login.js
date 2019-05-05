import React, {Component} from 'react';
import {Jumbotron, Container, Button, FormControl, FormLabel, FormGroup} from 'react-bootstrap';
import firebase from 'firebase/app';
import 'firebase/auth';
import "./Login.css";

export default class Login extends Component{

	constructor(props) {
		super(props);

		this.login = this.login.bind(this);

		this.emailField = React.createRef();
		this.passwordField = React.createRef();
	}

	render(){
		return(
			<Container className="login-container">
				<form className="login-form">
					<h4 className="login-title">Login</h4>
					<FormGroup>
						<FormLabel><strong>Email</strong></FormLabel>
						<FormControl type="text" ref={this.emailField} autoComplete="email"/>
					</FormGroup>
					<FormGroup>
						<FormLabel><strong>Password</strong></FormLabel>
						<FormControl type="password" ref={this.passwordField} autoComplete="current-password"/>
					</FormGroup>
					<div className="justify-children-end">
						<Button onClick={this.login}>Login</Button>
					</div>
				</form>
			</Container>
		)
	}

	login() {
		const email = this.emailField.current.value;
		const password = this.passwordField.current.value;
		
		firebase.auth().signInWithEmailAndPassword(email, password);
	}
}