import React, { Component } from "react";
import { withRouter } from "react-router-dom";
import { Container, Modal, ModalTitle, ModalBody, ModalFooter, Button, FormGroup, FormLabel, FormControl } from "react-bootstrap";
import BootstrapTable from 'react-bootstrap-table-next';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import * as firebase from 'firebase/app';
import "firebase/database";
import Controls from "../Controls/Controls";
import Loading from "../Loading/Loading";
import { Link } from "react-router-dom";
import * as moment from "moment";
import "./Attempts.css";
import ModalHeader from "react-bootstrap/ModalHeader";

import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faCheck, faTimes } from '@fortawesome/free-solid-svg-icons'

library.add(faCheck, faTimes);

const { SearchBar } = Search;

class Attempts extends Component {

  constructor(props) {
    super(props);

    this.state = {
      classId: this.props.match.params.classId,
      studentId: this.props.match.params.studentId,
      studentData: {},
      loading: true,
      modalOpen: false,
      selectedAttempt: {}
    };

    this.loadStudentClassData = this.loadStudentClassData.bind(this);
    this.handleSelectAttempt = this.handleSelectAttempt.bind(this);
    this.handleShowAttemptModal = this.handleShowAttemptModal.bind(this);
    this.handleHideAttemptModal = this.handleHideAttemptModal.bind(this);
  }

  componentDidMount() {
    this.loadStudentClassData();
  }
  
  render(){
    const selectRow = {
      mode: "radio",
      style: { background: "#eee" },
      onSelect: this.handleSelectAttempt
    };

    const attemptColumns = [
      {
        dataField: "date",
        text: "Date",
        sort: true,
        formatter: (val, row, rowIndex) => {
          return <div>{moment.utc(val).local().format("M/D/YYYY \\a\\t hh:mm a")}</div>
        }
      },
      {
        dataField: "finishTime",
        text: "Finish Time",
        sort: true,
        formatter: (val, row, rowIndex) => {
          return (
            <div>
              {Math.floor(val / 60)}:{Math.floor(val % 60).toString().padStart(2, "0")}
            </div>
          )
        }
      },
      {
        dataField: "grade",
        text: "Grade",
        sort: true,
        formatter: (val, row, rowIndex) => {
          return <div>{val}%</div>
        }
      }
    ]

    if(this.state.loading) {
      return <Loading/>
    }

    if(this.state.studentData.classes[this.state.classId].attempts === undefined || this.state.studentData.classes[this.state.classId].attempts === null) {
      return (
        <Container className="page-content">
          <Controls title={"Attempts"}/>

          <div className="question-group" style={{marginBottom: "1rem"}}>
            <div>
              <strong>Student:</strong> {this.state.studentData.firstName} {this.state.studentData.lastName}
            </div>
            <div>
              <strong>Class:</strong> {this.state.studentData.classes[this.state.classId].name}
            </div>
          </div>
          <div style={{textAlign: "center", marginTop: "3rem"}}>
            <h4>No Attempts</h4>
          </div>
        </Container>
      )
    }
    
    const studentAttempts = Object.keys(this.state.studentData.classes[this.state.classId].attempts).map(key => {
      this.state.studentData.classes[this.state.classId].attempts[key].id = key;
      return this.state.studentData.classes[this.state.classId].attempts[key];
    });

    return (
      <Container className="page-content">
        <Controls title={"Attempts"}>
          {this.renderShowAttemptButton()}
        </Controls>

        <div className="question-group" style={{marginBottom: "1rem"}}>
          <div>
            <strong>Student:</strong> {this.state.studentData.firstName} {this.state.studentData.lastName}
          </div>
          <div>
            <strong>Class:</strong> {this.state.studentData.classes[this.state.classId].name}
          </div>
        </div>

        <ToolkitProvider
          keyField="id"
          data={studentAttempts}
          columns={attemptColumns}
          search
          sort
          bootstrap4
        >
          {
            props => (
              <div>
                <SearchBar {...props.searchProps} />
                <hr />
                <BootstrapTable 
                  {...props.baseProps}
                  selectRow={selectRow}
                  ref={this.questionsTable}
                />
              </div>
            )
          }
        </ToolkitProvider>

        <Modal show={this.state.modalOpen} onHide={this.handleHideAttemptModal} size="lg">
          <ModalHeader closeButton onHide={this.handleHideAttemptModal}>
            <ModalTitle>
              Attempt
            </ModalTitle>
          </ModalHeader>
          <ModalBody>
            <h4>Attempt Statistics</h4>
            <div style={{marginBottom: ".5rem"}} className="question-group">
              <div>
                <strong>Date:</strong> {moment.utc(this.state.selectedAttempt.date).local().format("M/D/YYYY \\a\\t hh:mm a")}
              </div>
              <div>
                <strong>Finish Time:</strong> {Math.floor(this.state.selectedAttempt.finishTime / 60)}:{Math.floor(this.state.selectedAttempt.finishTime % 60).toString().padStart(2, "0")}
              </div>
              <div>
                <strong>Grade:</strong> {this.state.selectedAttempt.grade}%
              </div>
            </div>
            <hr/>
            <h4>Questions:</h4>
            {this.renderSelectedAttemptModalBody()}
          </ModalBody>
          <ModalFooter>
            <Button variant="secondary" onClick={this.handleHideAttemptModal} size="sm">
              Close
            </Button>
          </ModalFooter>
        </Modal>

      </Container>
    );
  }

  renderShowAttemptButton() {
    if(Object.keys(this.state.selectedAttempt).length > 0) {
      return <Button onClick={this.handleShowAttemptModal} size="sm">Show</Button>
    }
  }

  renderSelectedAttemptModalBody() {
    if(Object.keys(this.state.selectedAttempt).length > 0) {

      const attempt = this.state.selectedAttempt;
      var attempts = [];
      attempt.questions.map((question, idx) => {
        attempts.push(
          <div key={idx}>
            <FormGroup className="question-group">
              <FormLabel style={{margin: "0"}}>
                <h5 style={{display: "inline-block"}}>Question {idx+1}</h5> 
                {this.renderQuestionCorrect(question)}
              </FormLabel>        
              <hr style={{marginTop: ".5rem", marginBottom: ".5rem"}}/>
              <div>
                <FormGroup>
                  <FormLabel><strong>Question Image</strong></FormLabel>
                  <div>
                    <img src={question.imageUrl} className="question-image-preview"/>
                  </div>
                </FormGroup>
                <FormGroup>
                  <FormLabel><strong>Question Solutions</strong></FormLabel>
                  <div style={{marginLeft: "1rem"}}>
                    {question.solutions.map((solution, index) => {
                      if(question.studentSelection === index) {
                        return (
                          <div key={index}>
                            <input type="radio" disabled checked/> <span style={{marginLeft: "5px"}}>{solution}</span>
                          </div>
                        );                  
                      }
                      return (
                        <div key={index}>
                          <input type="radio" disabled/> <span style={{marginLeft: "5px"}}>{solution}</span>
                        </div>
                      );       
                    })} 
                  </div>
                </FormGroup>
              </div>
            </FormGroup>
          </div>
        );
      });

      return attempts;
    }
  }

  renderQuestionCorrect(question) {
    if(question.correct) {
      return <FontAwesomeIcon icon="check" size="1x" style={{marginLeft: ".3rem", marginBottom: "3px", color: "var(--success)"}}/>
    } else {
      return <FontAwesomeIcon icon="times" size="1x" style={{marginLeft: ".3rem", marginBottom: "1px", color: "var(--danger)"}}/>
    }
  }

  loadStudentClassData() {
    const db = firebase.database();
    const studentId = this.state.studentId;
    const studentRef = db.ref(`students/${studentId}`);

    studentRef.on("value", snap => {
      this.setState({
        loading: false,
        studentData: snap.val()
      });
    });
  }

  handleSelectAttempt(attempt) {
    console.log(attempt);
    this.setState({
      selectedAttempt: attempt
    });
  }

  handleShowAttemptModal() {
    this.setState({
      modalOpen: true
    });
  }

  handleHideAttemptModal() {
    this.setState({
      modalOpen: false
    });
  }
}

export default withRouter(Attempts);