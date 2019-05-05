import React, { Component } from 'react';
import {Jumbotron, Container, Table} from 'react-bootstrap';

class SeeScores extends Component{
    render(){
        return(
            <Container>
                <Jumbotron>
                    <h1> See Student Scores </h1>
                </Jumbotron>
                <Table striped bordered hover variant="dark">
                    <thead>
                    <tr>
                    <th>#</th>
                    <th>First Name</th>
                    <th>Last Name</th>
                    <th>Overall Grade</th>
                    </tr>
                    </thead>
                    <tbody>
                        <tr>
                        <td>1</td>
                        <td>Albert</td>
                        <td>Einstein</td>
                        <td>93%</td>
                        </tr>
                        <tr>
                        <td>2</td>
                        <td>George</td>
                        <td>Washington</td>
                        <td>82%</td>
                        </tr>
                        <tr>
                        <td>3</td>
                        <td>Bruce</td>
                        <td>Springsteen</td>
                        <td>98%</td>
                        </tr>
                    </tbody>
                </Table>;
            </Container>    
        )
    }
}

export default SeeScores;