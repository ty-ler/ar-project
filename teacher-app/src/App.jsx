import React, { Component } from 'react';
import {BrowserRouter as Router,Route, Switch} from 'react-router-dom';
import AddQuestions from './components/AddQuestions';
import SeeScores from './components/SeeScores';
import Home from './components/Home';
import './App.css';
import "../node_modules/bootstrap/dist/css/bootstrap.min.css";

class App extends Component {
  render() {
    return (
      <div>
        <div className="App">
          <Switch>
            <Route exact path="/" component={Home}/>
            <Route path="/AddQuestions" component={AddQuestions}/>
            <Route path="/SeeScores" component={SeeScores}/>
          </Switch>
        </div>
      </div>
    );
  }
}

export default App;
