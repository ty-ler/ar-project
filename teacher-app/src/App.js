import React, { Component } from 'react';
import {BrowserRouter as Router,Route, Switch, withRouter, Redirect} from 'react-router-dom';
import AddQuestions from './components/AddQuestions';
import SeeScores from './components/SeeScores';
import Login from "./components/Login/Login";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap-table/dist/bootstrap-table.min.css";
import 'react-bootstrap-table-next/dist/react-bootstrap-table2.min.css';
import './App.css';

import * as firebase from 'firebase/app';
import "firebase/app";
import "firebase/database";
import config from "./firebase.json";
import Navbar from './components/TeacherNavbar/TeacherNavbar';
import Classes from './components/Classes/Classes';
import Class from './components/Class/Class';
import Students from './components/Students/Students';
import Loading from './components/Loading/Loading';
import Attempts from './components/Attempts/Attempts';

class App extends Component {

  constructor(props) {
    super(props);

    this.state = {
      authenticated: false,
      userId: null,
      loading: true,
      authUser: null
    };

    this.handleAuthStateChanged = this.handleAuthStateChanged.bind(this);
    this.initializeFirebase = this.initializeFirebase.bind(this);
    this.initializeUser = this.initializeUser.bind(this);
  }

  componentDidMount() {
    this.initializeFirebase();
  }

  render() {
    return (
      <div className="App">
        <Navbar authenticated={this.state.authenticated}/>        
        {this.renderApp()}
      </div>
    );
  }

  renderApp() {
    if(this.state.loading) {
      return <Loading/>
    }

    if(this.state.authenticated) {
      return (
        <Switch>
          <Route path="/" exact render={() => <Classes userId={this.state.userId} />} />
          <Route path="/class/:classId" exact render={() => <Class userId={this.state.userId}/>}/>
          <Route path="/class/:classId/:studentId/attempts" exact render={() => <Attempts/>}/>
          <Route path="/students" exact render={() => <Students/>} />

          {/* 404 redirect */}
          <Redirect to="/"/>
        </Switch>
      );
    } else {
      return <Login/>;
    }
  }

  initializeFirebase() {
    firebase.initializeApp(config);

    firebase.auth().onAuthStateChanged(this.handleAuthStateChanged);

    this.setState({
      loading: false
    });
  }

  handleAuthStateChanged(user) {
    if(user) {
      this.initializeUser(user);
      this.setState({
        authenticated: true,
        userId: user.uid
      });
    } else {
      this.setState({
        authenticated: false,
        userId: null
      });
    }
  }

  initializeUser(user) {
    if(user) {
      const db = firebase.database();
      const userId = user.uid;

      const userRef = db.ref(`teachers/${userId}`);
      
      userRef.once("value", (snap) => {
        if(!snap.exists()) {
          userRef.set({
            display_name: "New Teacher",
            uid: userId,
            type: "teacher"
          });
        }
      });
    }
  }
}

export default withRouter(App);
