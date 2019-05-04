import React, {Component} from 'react';
import {Form, Row, Col, Button} from 'react-bootstrap';
import "../../node_modules/bootstrap/dist/css/bootstrap.min.css";

class QuestionRow extends Component{
    constructor(props) {
        super(props);
       
        this.chapterField = React.createRef();
        this.sectionField = React.createRef();
        this.questionField = React.createRef();
        this.solutionField = React.createRef();
    }

    render(){
        return(
            <Row>
                <Col>
                    <input
                        type="text"
                        className="chapter"
                        placeholder="chapter"
                        name="chapter"
                        formNoValidate
                        onChange={() => this.props.handleRowChange(this.props.index, "Chapter", this.chapterField.current.value)}
                        ref={this.chapterField}
                   />
                </Col>
                <Col>
                    <input
                        type="text"
                        className="section"
                        placeholder="section"
                        name="section"
                        formNoValidate
                        onChange={() => this.props.handleRowChange(this.props.index, "Section", this.sectionField.current.value)}
                        ref={this.sectionField}
                    />
                </Col>
                <Col>
                    <input
                        type="text"
                        className="question"
                        placeholder="question"
                        name="question"
                        formNoValidate
                        onChange={() => this.props.handleRowChange(this.props.index, "Question", this.questionField.current.value)}
                        ref={this.questionField}
                    />
                </Col>
                <Col>
                    <input
                        type="int"
                        className="solution"
                        placeholder="solution"
                        name="solution"
                        formNoValidate
                        onChange={() => this.props.handleRowChange(this.props.index, "Solution", this.solutionField.current.value)}
                        ref={this.solutionField}
                    />
                </Col>                                
            </Row>
        )
    }

    /*handleSubmit() { i dont think this is needed?
        const endpoint = this.state.endpoint;

        // fetch(`${endpoint}/add_questions`, {
        //     method: "POST",
        //     body: 
        // })
    }*/
}

export default QuestionRow;