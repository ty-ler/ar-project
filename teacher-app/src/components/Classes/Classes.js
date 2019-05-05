import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Jumbotron, Container, Button, FormControl, FormGroup, FormLabel} from 'react-bootstrap';
import firebase from 'firebase/app';
import 'firebase/database';
import './Classes.css';
import Loading from '../Loading/Loading';

const uuidv4 = require("uuid/v4");

export default class Classes extends Component{

	constructor(props) {
		super(props);

		this.state = {
			loading: true,
			classes: [],
			userId: this.props.userId,
			newClass: false
		}

		this.loadClasses = this.loadClasses.bind(this);
		this.showNewClassForm = this.showNewClassForm.bind(this);
		this.hideNewClassForm = this.hideNewClassForm.bind(this);
		this.addNewClass = this.addNewClass.bind(this);
	}

	componentDidMount() {
		this.loadClasses();
	}

	render(){

		if(this.state.loading) {
			return <Loading/>;
		}

		return(
			<Container className="page-content">
				<div className="classes-controls">
					<h2>Classes</h2>

					<div className="add-class-btn-container">
						<Button size="sm" variant="success" onClick={this.showNewClassForm}>Add Class</Button>
					</div>
				</div>
				
				<div className="classes-body">
					{this.renderClasses()}
				</div>
			</Container>
		)
	}

	renderClasses() {
		const classes = this.state.classes;
		const newClass = this.state.newClass;

		if(classes.length && newClass) {
			return (
				<div>
					<NewClassForm hideNewClassForm={this.hideNewClassForm} addNewClass={this.addNewClass}/>
					{classes}
				</div>
			);
		} else if(classes.length) {
			return classes;
		} else if(newClass) {
			return (
				<NewClassForm hideNewClassForm={this.hideNewClassForm} addNewClass={this.addNewClass}/>
			);
		} else {
			return (
				<div className="no-classes">
					No classes added
				</div>
			);
		}
	}

	showNewClassForm() {
		this.setState({
			newClass: true
		});
	}

	hideNewClassForm() {
		this.setState({
			newClass: false
		});
	}

	loadClasses() {
		const db = firebase.database();
		const userId = this.state.userId;

		const userRef = db.ref(`users/${userId}`);

		const classesRef = userRef.child("classes");

		classesRef.on("value", (snap) => {
			if(snap.exists()) {
				const classesData = snap.val();

				var classes = [];

				Object.keys(classesData).map(key => {
					const c = classesData[key];

					classes.push(
						<Link to={`/class/${key}`} className="class-item">
							{c.name}
						</Link>
					);
				});

				this.setState({
					classes: classes,
					loading: false
				});
			}
		});
	}

	addNewClass(className) {
		const db = firebase.database();
		const userId = this.state.userId;
		const classId = uuidv4();

		const classesRef = db.ref(`users/${userId}/classes`);
		
		
		classesRef.child(classId).set({
			name: className
		}).then(res => {
			console.log(res);

			this.hideNewClassForm();
		});
	}
}

class NewClassForm extends Component {
	
	constructor(props) {
		super(props);

		this.handleAddNewClass = this.handleAddNewClass.bind(this);
		
		this.classNameField = React.createRef();
	}
	
	render() {
		return (
			<form className="new-class-form">
				<FormGroup>
					<FormLabel><strong>Class Name</strong></FormLabel>
					<FormControl ref={this.classNameField}/>
				</FormGroup>
				<div className="justify-children-end">
					<Button className="mr-2" onClick={this.handleAddNewClass}>Add</Button>
					<Button variant="danger" onClick={this.props.hideNewClassForm}>Cancel</Button>
				</div>
			</form>
		);
	}

	handleAddNewClass() {
		const className = this.classNameField.current.value;

		this.props.addNewClass(className);
	}
}