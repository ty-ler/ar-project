import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {Jumbotron, Container, Button} from 'react-bootstrap';
import "../../node_modules/bootstrap/dist/css/bootstrap.min.css";
import './Home.css';


export default class Home extends Component{
    render(){
        return(
            <Container>
                <Jumbotron>
                    <h1> Welcome to the teacher web app! </h1>
                </Jumbotron>
                <Link to="/AddQuestions">
                    <Button variant="light"> Add Questions </Button>
                </Link>
                <h1>      </h1>
                <Link to="/SeeScores">
                    <Button variant="light"> See Student Scores </Button>
                </Link>
            </Container>
        )
    }
}