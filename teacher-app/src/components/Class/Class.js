import React, { Component } from 'react';
import {BrowserRouter as Router,Route, Switch, withRouter, Redirect} from 'react-router-dom';
import * as firebase from 'firebase/app';
import "firebase/database";
import { Container, Button, Modal, ModalTitle, ModalBody, Form, FormGroup, FormControl, FormLabel, ModalFooter } from 'react-bootstrap';
import Loading from '../Loading/Loading';
import Controls from "../Controls/Controls";
import ModalHeader from 'react-bootstrap/ModalHeader';
import BootstrapTable from 'react-bootstrap-table-next';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import ClassStudents from './ClassStudents/ClassStudents';
import ClassQuestions from './ClassQuestions/ClassQuestions';

const { SearchBar } = Search;

class Class extends Component {

  constructor(props) {
    super(props);

    this.state = {
      loading: true,
      userId: this.props.userId,
      classId: this.props.match.params.id,
      classData: {},
      addStudent: false,
      loadedStudents: false,
      studentsData: [],
      selectedStudents: [],
      studentsInClass: []
    }

    this.loadClassData = this.loadClassData.bind(this);
    this.loadStudents = this.loadStudents.bind(this);
  }

  componentDidMount() {
    this.loadClassData();
    this.loadStudents();
  }

  render() {
    if(this.state.loading) {
      return <Loading/>
    }

    return (
      <Container className="page-content">
        <h2>{this.state.classData.name}</h2>
        <hr/>

        <ClassQuestions
          userId={this.state.userId}
          classId={this.state.classId}
        />
        
        <hr/>

        <ClassStudents 
          studentsData={this.state.studentsData} 
          classData={this.state.classData}
          userId={this.state.userId}
          classId={this.state.classId}
        />
      </Container>
    );
  }

  loadClassData() {
    const db = firebase.database();
    const userId = this.state.userId;
    const classId = this.state.classId;

    const classRef = db.ref(`teachers/${userId}/classes/${classId}`);

    classRef.on("value", (snap) => {
      const classData = snap.val();

      this.setState({
        classData: classData,
        loading: false
      });
    });
  }

  loadStudents() {
    const db = firebase.database();
    
    const studentsRef = db.ref(`students`);

    studentsRef.on("value", (snap) => {
      const studentsData = snap.val();
      var students = [];

      Object.keys(studentsData).map(key => {
        var student = studentsData[key];
        student.id = key;

        students.push(student);
      });

      this.setState({
        studentsData: students,
        loadedStudents: true
      });
    });
  }
}

export default withRouter(Class);
