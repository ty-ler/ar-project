import React, { Component } from 'react';
import * as firebase from 'firebase/app';
import "firebase/database";
import "firebase/app";
import { Container, Button, FormGroup, FormLabel, FormControl, Modal, ModalTitle, ModalBody, ModalFooter, Form } from 'react-bootstrap';
import Loading from '../Loading/Loading';
import "./Students.css";
import ModalHeader from 'react-bootstrap/ModalHeader';
import config from "../../firebase.json";
import BootstrapTable from 'react-bootstrap-table-next';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import Controls from "../Controls/Controls";

const { SearchBar } = Search;

export default class Students extends Component {
  constructor(props) {
    super(props);

    this.state = {
      loading: true,
      newStudent: false,
      students: [],
      selectedStudent: null
    }

    this.loadStudents = this.loadStudents.bind(this);
    this.showNewStudentForm = this.showNewStudentForm.bind(this);
    this.hideNewStudentForm = this.hideNewStudentForm.bind(this);
    this.handleCreateNewStudent = this.handleCreateNewStudent.bind(this);
    this.handleStudentSelect = this.handleStudentSelect.bind(this);

    this.studentFirstNameField = React.createRef();
    this.studentLastNameField = React.createRef();
    this.studentEmailField = React.createRef();
    this.studentPasswordField = React.createRef();
  }

  componentDidMount() {
    this.loadStudents();
  }

  render() {
    if(this.state.loading) {
      return <Loading/>
    }

    const columns = [
      {
        dataField: "firstName",
        text: "First Name",
        sort: true
      },
      {
        dataField: "lastName",
        text: "Last Name",
        sort: true
      },
      {
        dataField: "email",
        text: "Email",
        sort: true
      }
    ];

    const selectRow = {
      mode: "radio",
      style: { background: "#eee" },
      onSelect: this.handleStudentSelect
    };

    return (
      <Container className="page-content">

        <Controls title="Students">
          <Button size="sm" variant="success" onClick={this.showNewStudentForm}>New Student</Button>
        </Controls>

        <div className="students-body">
          <ToolkitProvider
            keyField="id"
            data={ this.state.students }
            columns={ columns }
            search
            sort
          >
            {
              props => (
                <div>
                  <SearchBar { ...props.searchProps } />
                  <hr />
                  <BootstrapTable
                    { ...props.baseProps }
                    selectRow={selectRow}
                  />
                </div>
              )
            }
          </ToolkitProvider>
        </div>

        <Modal show={this.state.newStudent} onHide={this.hideNewStudentForm}>
          <ModalHeader closeButton onHide={this.hideNewStudentForm}>
            <ModalTitle>New Student</ModalTitle>
          </ModalHeader>

          <ModalBody>
            <Form>
              <FormGroup>
                <FormLabel><strong>First Name</strong></FormLabel>
                <FormControl ref={this.studentFirstNameField} placeholder=""/>
              </FormGroup>
              <FormGroup>
                <FormLabel><strong>Last Name</strong></FormLabel>
                <FormControl ref={this.studentLastNameField} placeholder=""/>
              </FormGroup>
              <FormGroup>
                <FormLabel><strong>Student Email</strong></FormLabel>
                <FormControl ref={this.studentEmailField} type="email" placeholder="Email"/>
              </FormGroup>
              <FormGroup>
                <FormLabel><strong>Student Password</strong></FormLabel>
                <FormControl ref={this.studentPasswordField} type="password" placeholder="Password" autoComplete="new-password"/>
              </FormGroup>
            </Form>
          </ModalBody>

          <ModalFooter>
            <Button variant="secondary" onClick={this.hideNewStudentForm} size="sm">
              Close
            </Button>
            <Button variant="primary" onClick={this.handleCreateNewStudent} size="sm">
              Confirm
            </Button>
          </ModalFooter>

        </Modal>
      </Container>
    );
  }

  loadStudents() {
    const db = firebase.database();

    const studentsRef = db.ref(`students`);

    studentsRef.on("value", (snap) => {
      if(snap.exists()) {
        const studentsData = snap.val();
        var students = [];

        Object.keys(studentsData).map(key => {
          var student = studentsData[key];
          student.id = key;
          students.push(student);
        });

        console.log(students);

        this.setState({
          students: students,
          loading: false
        });

      } else {
        this.setState({
          loading: false
        });
      }
    });
  }

  showNewStudentForm() {
    this.setState({
      newStudent: true
    });
  }

  hideNewStudentForm() {
    this.setState({
      newStudent: false
    });
  }

  handleStudentSelect(student) {
    this.setState({
      selectedStudent: student
    });
  }

  handleCreateNewStudent() {
    const studentFirstName = this.studentFirstNameField.current.value;
    const studentLastName = this.studentLastNameField.current.value;
    const studentEmail = this.studentEmailField.current.value;
    const studentPassword = this.studentPasswordField.current.value;
    

    this.createNewStudent(studentFirstName, studentLastName, studentEmail, studentPassword);
  }

  createNewStudent(firstName, lastName, email, password) {
    // Create a second instance of our firebase app
    // because there is no way to create a user account
    // without logging into the account immediately after.
    const secondaryApp = firebase.initializeApp(config, "Secondary");
    const db = firebase.database();

    secondaryApp.auth().createUserWithEmailAndPassword(email, password)
      .then(res => {
        secondaryApp.auth().signOut(); // Log out of newly created account
        secondaryApp.delete(); // Clean up secondary instance which is not needed anymore
        const userId = res.user.uid;
        const studentRef = db.ref(`students/${userId}`);
        studentRef.set({
          firstName: firstName,
          lastName: lastName,
          email: email
        }).then(res => {
          this.setState({
            newStudent: false
          });
        });
      })
      .catch(err => {
        console.log(err);
      });
  }
}