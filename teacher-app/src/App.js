import React, { Component } from 'react';
import {BrowserRouter as Router,Route, Switch, withRouter, Redirect} from 'react-router-dom';
import AddQuestions from './components/AddQuestions';
import SeeScores from './components/SeeScores';
import Login from "./components/Login/Login";
import './App.css';
import "../node_modules/bootstrap/dist/css/bootstrap.min.css";
import * as firebase from 'firebase/app';
import "firebase/app";
import "firebase/database";
import config from "./firebase.json";
import Navbar from './components/TeacherNavbar/TeacherNavbar';
import Classes from './components/Classes/Classes';
import Class from './components/Class/Class';

class App extends Component {

  constructor(props) {
    super(props);

    this.state = {
      authenticated: false,
      userId: null
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
    if(this.state.authenticated) {
      return (
        <Switch>
          <Route path="/" exact render={() => <Classes userId={this.state.userId} />} />
          <Route path="/class/:id" exact render={() => <Class userId={this.state.userId}/>}/>

          <Redirect to="/"/>
        </Switch>
      );
    } else {
      return (
        <Switch>
          <Route path="/login" exact component={Login}/>

          <Redirect to="/login"/>
        </Switch>
      )
    }
  }

  initializeFirebase() {
    firebase.initializeApp(config);

    firebase.auth().onAuthStateChanged(this.handleAuthStateChanged);

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

      const userRef = db.ref(`users/${userId}`);
      
      userRef.once("value", (snap) => {
        if(!snap.exists()) {
          userRef.set({
            display_name: "New User",
            uid: userId
          });
        }
      });
    }
  }
}

export default withRouter(App);
