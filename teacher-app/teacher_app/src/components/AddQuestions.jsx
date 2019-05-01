import React, {Component} from 'react';
import {Form, Row, Col, Button} from 'react-bootstrap';
import "../../node_modules/bootstrap/dist/css/bootstrap.min.css";
import QuestionRow from './QuestionRow';
import { timingSafeEqual } from 'crypto';

class AddQs extends Component{

    constructor(props) {
        super(props);

        this.state = {
            endpoint: "localhost:1337/api",
            data: [
                {
                    ProblemID: "",
                    Chapter: "",
                    Section: "",
                    Problem: "",
                    Solution: "",
                    ClassID: ""
                }
            ],
            rows: []
        }

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleRowChange = this.handleRowChange.bind(this);
    }

    componentDidMount() {
        const data = this.state.data;
        var rows = this.state.rows;

        data.map((val, index) => {
            rows.push(
                <QuestionRow index={index} key={index} handleRowChange={this.handleRowChange}/>
            );
        });

        //i have no idea what to do with this but im fairly sure its why nothing is going into the db
        data.push(
            {}
        )

        this.setState({
            rows: rows
        });
    }

    render(){
        return(
            <div className="wrapper">
                <div className="formWrapper">
                    <h1> Add Questions </h1>
                    <Form onSubmit={this.handleSubmit}>
                        <div className="chapter">
                            {this.state.rows}
                        </div>                       
                        <h1>     </h1>
                        <Button variant="success" type="submit">
                            Submit
                        </Button>
                    </Form>
                </div>
            </div>
        )
    }

    handleSubmit() {
        const endpoint = this.state.endpoint;

        fetch(`${endpoint}/add_questions`, {
             method: "POST",
             body: {
                chapter: this.state.Chapter,
                section: this.state.Section,
                question: this.state.Question,
                solution: this.state.Solution
             }
         })
    }

    handleRowChange(index, key, value) {
        var data = this.state.data;

        data[index][key] = value;

        console.log(data);
    }
}

export default AddQs;