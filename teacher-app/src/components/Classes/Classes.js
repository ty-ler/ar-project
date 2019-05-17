import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Jumbotron, Container, Button, FormControl, FormGroup, FormLabel, Modal, ModalTitle, ModalBody, ModalFooter, Form} from 'react-bootstrap';
import firebase from 'firebase/app';
import 'firebase/database';
import './Classes.css';
import Loading from '../Loading/Loading';
import ModalHeader from 'react-bootstrap/ModalHeader';
import Controls from "../Controls/Controls";

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
		this.handleAddClass = this.handleAddClass.bind(this);
		this.addClass = this.addClass.bind(this);

		this.classNameField = React.createRef();
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
				<Controls title="Classes">
					<Button size="sm" variant="success" onClick={this.showNewClassForm}>Add Class</Button>
				</Controls>
				
				<div className="classes-body">
					{this.renderClasses()}
				</div>

				<Modal show={this.state.newClass} onHide={this.hideNewClassForm}>
          <ModalHeader closeButton onHide={this.hideNewClassForm}>
            <ModalTitle>Add a Class</ModalTitle>
          </ModalHeader>
          <ModalBody>
						<Form>
							<FormGroup>
								<FormLabel><strong>Class Name</strong></FormLabel>
								<FormControl ref={this.classNameField} placeholder="Name"/>
							</FormGroup>
						</Form>
					</ModalBody>
          <ModalFooter>
            <Button variant="secondary" onClick={this.hideNewClassForm} size="sm">
              Close
            </Button>
            <Button variant="primary" onClick={this.handleAddClass} size="sm">
              Confirm
            </Button>
          </ModalFooter>
        </Modal>
			</Container>
		)
	}

	renderClasses() {
		const classes = this.state.classes;

		if(classes.length) {
			return classes;
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

		const userRef = db.ref(`teachers/${userId}`);

		const classesRef = userRef.child("classes");

		classesRef.on("value", (snap) => {
			if(snap.exists()) {
				const classesData = snap.val();

				var classes = [];

				Object.keys(classesData).map(key => {
					const c = classesData[key];

					classes.push(
						<Link key={key} to={`/class/${key}`} className="class-item">
							<strong>{c.name}</strong>
						</Link>
					);
				});

				this.setState({
					classes: classes,
					loading: false
				});
			} else {
				this.setState({
					loading: false
				});
			}
		});
	}

	handleAddClass() {
		const className = this.classNameField.current.value;

		if(className.trim() != "") {
			this.addClass(className);
		} else {
			this.hideNewClassForm();
		}
	}

	addClass(className) {
		const db = firebase.database();
		const userId = this.state.userId;
		const classId = uuidv4();

		const classesRef = db.ref(`teachers/${userId}/classes`);
		
		
		classesRef.child(classId).set({
			name: className
		}).then(res => {
			this.hideNewClassForm();
		});
	}
}