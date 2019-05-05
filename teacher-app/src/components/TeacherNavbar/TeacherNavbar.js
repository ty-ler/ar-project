import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Navbar, NavbarBrand, Nav, Button} from 'react-bootstrap';
import firebase from 'firebase/app';
import 'firebase/auth';
import NavbarToggle from 'react-bootstrap/NavbarToggle';
import NavbarCollapse from 'react-bootstrap/NavbarCollapse';

export default class TeacherNavbar extends Component{

	constructor(props) {
		super(props);

		this.state = {
			authenticated: this.props.authenticated
		};

		this.logout = this.logout.bind(this);
	}

	componentDidUpdate(prevProps) {
		if(this.props.authenticated != prevProps.authenticated) {
			this.setState({
				authenticated: this.props.authenticated
			});
		}
	}

	render(){
		return(
			<Navbar bg="dark" expand="lg" variant="dark">
				<NavbarBrand>
					<NavbarLink to="/">MathAR Teacher</NavbarLink>
				</NavbarBrand>
				<NavbarToggle aria-controls="teacher-nav"/>
				{this.renderNav()}
			</Navbar>
		);
	}

	renderNav() {
		if(this.state.authenticated) {
			return (
				<NavbarCollapse id="teacher-nav">
					<Nav className="mr-auto">
						<NavbarLink to="/">Classes</NavbarLink>
					</Nav>
					<Button variant="danger" size="md" onClick={this.logout}>Logout</Button>
				</NavbarCollapse>
			)
		}
	}

	logout() {
		firebase.auth().signOut();
	}
}

class NavbarLink extends Component {
	render() {
		return (
			<Link to={this.props.to} className="nav-link" role="button">
				{this.props.children}
			</Link>
		);
	}
}