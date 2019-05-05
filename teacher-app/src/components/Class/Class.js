import React, { Component } from 'react';
import {BrowserRouter as Router,Route, Switch, withRouter, Redirect} from 'react-router-dom';
import * as firebase from 'firebase/app';
import "firebase/database";
import { Container } from 'react-bootstrap';
import Loading from '../Loading/Loading';

class Class extends Component {

  constructor(props) {
    super(props);

    this.state = {
      loading: true,
      userId: this.props.userId,
      classId: this.props.match.params.id,
      classData: {}
    }

    this.loadClassData = this.loadClassData.bind(this);
  }

  componentDidMount() {
    console.log(this.props);
    this.loadClassData();
  }

  render() {

    if(this.state.loading) {
      return <Loading/>
    }

    return (
      <Container className="page-content">
        {this.state.classData.name}
      </Container>
    );
  }

  loadClassData() {
    const db = firebase.database();
    const userId = this.state.userId;
    const classId = this.state.classId;

    const classRef = db.ref(`users/${userId}/classes/${classId}`);

    classRef.once("value", (snap) => {
      const classData = snap.val();

      this.setState({
        classData: classData,
        loading: false
      });
    });
  }
}

export default withRouter(Class);
