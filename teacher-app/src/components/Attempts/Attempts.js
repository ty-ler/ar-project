import React, { Component } from "react";
import { withRouter } from "react-router-dom";
import { Container } from "react-bootstrap";
import * as firebase from 'firebase/app';
import "firebase/database";
import Loading from "../Loading/Loading";

class Attempts extends Component {
  
  constructor(props) {
    super(props);

    this.state = {
      classId: this.props.match.params.classId,
      studentId: this.props.match.params.studentId,
      studentData: {},
      loading: true
    };

    this.loadStudentClassData = this.loadStudentClassData.bind(this);
  }

  componentDidMount() {
    this.loadStudentClassData();
  }
  
  render(){

    if(this.state.loading) {
      return <Loading/>
    }
    console.log(this.state.studentData);
    return (
      <Container className="page-content">
        <h3>{this.state.studentData.firstName} {this.state.studentData.lastName}'s Attempts</h3>

        
      </Container>
    );
  }

  loadStudentClassData() {
    const db = firebase.database();
    const classId = this.state.classId;
    const studentId = this.state.studentId;
    const studentRef = db.ref(`students/${studentId}`);

    studentRef.on("value", snap => {
      this.setState({
        loading: false,
        studentData: snap.val()
      });
    });
  }
}

export default withRouter(Attempts);