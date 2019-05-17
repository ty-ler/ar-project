import React, { Component } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import { Button, Form, Modal, ModalTitle, ModalBody, ModalFooter, FormLabel, FormGroup, FormControl, InputGroup } from 'react-bootstrap';
import Controls from "../../Controls/Controls";
import { Link } from "react-router-dom";
import * as firebase from 'firebase/app';
import "firebase/database";
import "firebase/storage";
import ModalHeader from 'react-bootstrap/ModalHeader';
import { faWindowRestore } from '@fortawesome/free-solid-svg-icons';
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSpinner } from '@fortawesome/free-solid-svg-icons'
import Loading from '../../Loading/Loading';

import "./ClassQuestions.css";
import SolutionListItem from './SolutionListItem';
import { POINT_CONVERSION_COMPRESSED } from 'constants';
const uuidv4 = require("uuid/v4");


library.add(faSpinner);

const { SearchBar } = Search;

const alphabet = 'abcdefghijklmnopqrstuvwxyz'.toUpperCase().split('');

export default class ClassQuestions extends Component {
  constructor(props) {
    super(props);

    this.state = {
      modalOpen: false,
      image: null,
      imageLoading: false,
      solutions: [],
      selectedSolution: null,
      questions: [],
      questionId: null,
      moveButtonsDisabled: false
    };
    this.handleHideAddQuestionModal = this.handleHideAddQuestionModal.bind(this);
    this.handleShowAddQuestionModal = this.handleShowAddQuestionModal.bind(this);
    this.handleUploadPicture = this.handleUploadPicture.bind(this);
    this.handleAddSolution = this.handleAddSolution.bind(this);
    this.handleAddQuestion = this.handleAddQuestion.bind(this);
    this.handleSelectQuestion = this.handleSelectQuestion.bind(this);
    this.removeSolutionItem = this.removeSolutionItem.bind(this);
    this.updateSolutionItem = this.updateSolutionItem.bind(this);
    this.loadQuestions = this.loadQuestions.bind(this);
    this.moveQuestionUp = this.moveQuestionUp.bind(this);
    this.moveQuestionDown = this.moveQuestionDown.bind(this);
    this.handleRemoveQuestion = this.handleRemoveQuestion.bind(this);
    this.renderQuestionImage = this.renderQuestionImage.bind(this);
    this.renderLoadingImage = this.renderLoadingImage.bind(this);
    this.renderQuestionControls = this.renderQuestionControls.bind(this);

    this.fileUploadField = React.createRef();
    this.addSolutionField = React.createRef();
    this.questionsTable = React.createRef();
  }

  componentDidMount() {
    this.loadQuestions();
  }

  componentDidUpdate(prevProps) {
    if (this.props.studentsInClass !== prevProps.studentsInClass) {
      this.setState({
        studentsInClass: this.props.studentsInClass
      });
    }
  }

  render() {
    const selectRow = {
      mode: "radio",
      style: { background: "#eee" },
      onSelect: this.handleSelectQuestion
    };

    const questionColumns = [
      {
        dataField: "order",
        text: "Question",
        formatter: (val, row, rowIndex) => {
          return <div>{rowIndex+1}</div>
        }
      },
      {
        dataField: "imageUrl",
        text: "Image",
        formatter: (val, row, rowIndex) => {
          return <a style={{color: "#007bff"}} href={val} target="_blank">{this.state.questions[rowIndex].imageName}</a>
        }
      }
    ];

    return (
      <div id="students-in-class">
        <Controls title="Questions">
          {this.renderQuestionControls()}
          <Button variant="success" size="sm" onClick={this.handleShowAddQuestionModal}>Add Question</Button>
        </Controls>

        <ToolkitProvider
          keyField="id"
          data={this.state.questions}
          columns={questionColumns}
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

        <Modal show={this.state.modalOpen} onHide={this.handleHideAddQuestionModal}>
          <ModalHeader closeButton onHide={this.handleHideAddQuestionModal}>
            <ModalTitle>Add a Question</ModalTitle>
          </ModalHeader>
          <ModalBody>
            <FormGroup>
              <FormLabel><strong>Upload Question Image</strong></FormLabel>
              <div style={{display: "flex"}}>
                <FormControl 
                  multiple={false} 
                  ref={this.fileUploadField} 
                  type="file" 
                  accept="image/x-png,image/jpeg" 
                  required
                />
                {this.renderLoadingImage()}
                <Button size="sm" onClick={this.handleUploadPicture}>Upload</Button>
              </div>
            </FormGroup>
            {this.renderQuestionImage()}
            <hr/>
            <FormGroup>
              <FormLabel><strong>Add Solutions</strong></FormLabel>
                <InputGroup size="sm">
                  <FormControl type="text" disabled={!this.state.image} ref={this.addSolutionField}/>
                  <InputGroup.Append>
                    <Button onClick={this.handleAddSolution} disabled={!this.state.image}>Add Solution</Button>
                  </InputGroup.Append>
                </InputGroup>
                {this.renderSolutions()}
            </FormGroup>
            <hr/>
            <FormGroup>
              <FormLabel><strong>Select Solution</strong></FormLabel>
              {this.renderSolutionSelect()}
            </FormGroup>
          </ModalBody>
          <ModalFooter>
            <Button variant="secondary" onClick={this.handleHideAddQuestionModal} size="sm">
              Close
          </Button>
            <Button type="submit" variant="primary" onClick={this.handleAddQuestion} size="sm" disabled={!this.state.image || this.state.solutions.length <= 0 || this.state.selectedSolution === null}>
              Confirm
          </Button>
          </ModalFooter>
        </Modal>
      </div>

    );
  }

  renderQuestionControls() {
    if(this.questionsTable.current && this.questionsTable.current.selectionContext.selected.length > 0) {
      if(this.state.questions.length > 1) {
        return (
          <div className="question-controls">
            <Button className="mr-2" size="sm" onClick={this.moveQuestionUp} disabled={this.state.moveButtonsDisabled}>Up</Button>
            <Button className="mr-2" size="sm" onClick={this.moveQuestionDown} disabled={this.state.moveButtonsDisabled}>Down</Button>
            <Button className="mr-2" variant="danger" size="sm" onClick={this.handleRemoveQuestion}>Remove</Button>
          </div>
        );
      } else {
        return (
          <div className="question-controls">
            <Button className="mr-2" variant="danger" size="sm" onClick={this.handleRemoveQuestion}>Remove</Button>
          </div>
        );
      }
    }
  }

  renderSolutionSelect() {
    const solutions = this.state.solutions;


    if(solutions && solutions.length) {
      var solutionRadioButtons = solutions.map((val, idx) => (
        <div key={idx}>
          <input 
            type="radio" 
            name="solution" 
            value={val}
            checked={this.state.solutions[this.state.selectedSolution] === val}
            onChange={() => this.handleSelectSolution(idx)}
          /> {alphabet[idx]}
        </div>
      ));
      
      return (
        <div style={{marginLeft: "1rem"}}>
          {solutionRadioButtons}
        </div>
      );
    }

    return (
      <div style={{textAlign: "center"}}>
        <small>Upload an image, then add solutions</small>
      </div>
    );
  }

  renderSolutions() {
    if(this.state.solutions && this.state.solutions.length) {
      const solutions = this.state.solutions;
      var solutionListItems = solutions.map((val, idx) => 
        <SolutionListItem 
          key={idx} 
          index={idx} 
          value={val} 
          removeSolutionItem={this.removeSolutionItem}
          updateSolutionItem={this.updateSolutionItem}
        />);
      
      return (
        <FormGroup style={{marginLeft: "1rem", marginTop: ".5rem"}}>
          <FormLabel><small><strong>Possible Solutions</strong></small></FormLabel>
          <ol type="A">
            {solutionListItems}
          </ol>  
        </FormGroup>
      );
    }

    return;
  }

  renderQuestionImage() {
    if(this.state.image && this.state.image.url) {
      return (
        <FormGroup>
          <FormLabel><strong>Question Image:</strong></FormLabel>
          <div style={{margin: "0 auto", minWidth: "100%"}}>
            <img src={this.state.image.url} className="question-image-preview"/>
          </div>
        </FormGroup>
      );
    }
  }

  renderLoadingImage() {
    if(this.state.imageLoading) {
      return (
        <Loading size="1x" className="mr-3"/>
      );
    }
  }

  handleHideAddQuestionModal() {
    this.setState({
      modalOpen: false,
      image: null,
      solutions: [],
      selectedSolution: null
    });
  }

  handleShowAddQuestionModal() {
    this.setState({
      modalOpen: true
    })
  }

  handleAddSolution() {
    const solution = this.addSolutionField.current.value;
    this.addSolutionField.current.value = "";
    this.addSolutionField.current.focus();
    var solutions = this.state.solutions.map(val => val);

    if(solution && solutions) {
      if(!solutions.includes(solution)) {
        solutions.push(solution);
        this.setState({
          solutions: solutions
        });
      }
    }
  }

  handleUploadPicture() { 
    const image = this.fileUploadField.current.files;

    if(image && image[0]) {
      this.setState({
        imageLoading: true
      }, () => {
        const imageFile = image[0];

        const metadata = {
          contentType: imageFile.type
        };
  
        const classId = this.props.classId;
  
        const storageRef = firebase.storage().ref();
        const imageRef = storageRef.child(`classes/${classId}/${imageFile.name}`);
  
        imageRef.put(imageFile, metadata).then(snap => {
          snap.ref.getDownloadURL().then(url => {
            this.setState({
              image: {
                file: imageFile,
                url: URL.createObjectURL(imageFile),
                public_url: url
              },
              imageLoading: false
            });
          });
        });
      });
      
    }
  }

  handleSelectSolution(index) {
    this.setState({
      selectedSolution: index
    });
  }

  updateSolutionItem(index, value) {
    var solutions = this.state.solutions.map(val => val);
    solutions[index] = value;
    this.setState({
      solutions: solutions
    });
  }

  removeSolutionItem(index) {
    var solutions = this.state.solutions.map(val => val);
    solutions.splice(index, 1);
    this.setState({
      solutions: solutions
    }, () => {
      if(this.state.selectedSolution === index) {
        this.setState({
          selectedSolution: null
        });
      }
    });
  }

  handleAddQuestion() {
    const db = firebase.database();
    const userId = this.props.userId;
    const classId = this.props.classId;
    var questionId;

    questionId = uuidv4();

    const questionRef = db.ref(`teachers/${userId}/classes/${classId}/questions`);
    console.log(this.state.image, this.state.solutions, this.state.solutions.length, this.state.selectedSolution);
    if(this.state.image != null && this.state.solutions.length >= 0 && this.state.selectedSolution != null) {
      const questionData = {
        order: this.state.questions.length+1,
        imageName: this.state.image.file.name,
        imageUrl: this.state.image.public_url,
        solutions: this.state.solutions,
        selectedSolution: this.state.selectedSolution,
        questionId: questionId
      };
  
      questionRef.child(questionId).set(questionData).then(this.handleHideAddQuestionModal);
    }
  }

  handleRemoveQuestion() {
    var selectedIndex = this.questionsTable.current.selectionContext.selected[0];
    var questions = this.state.questions.map(val => val);

    questions.splice(selectedIndex-1, 1);

    const questionData = {};
    questions.map(question => {
      questionData[question.id] = {
        imageName: question.imageName, 
        imageUrl: question.imageUrl,
        order: question.order,
        selectedSolution: question.selectedSolution,
        solutions: question.solutions
      };
    }); 

    const db = firebase.database();
    const userId = this.props.userId;
    const classId = this.props.classId;
    const questionsRef = db.ref(`teachers/${userId}/classes/${classId}/questions`);

    questionsRef.set(questionData).then(() => {
      this.questionsTable.current.selectionContext.selected = [];
      this.setState({
        questions: questions          
      });
    });
  }

  handleSelectQuestion(question) {
    this.setState({
      questionId: question.id
    });
  }

  loadQuestions() {
    const db = firebase.database();
    const userId = this.props.userId;
    const classId = this.props.classId;
    const questionsRef = db.ref(`teachers/${userId}/classes/${classId}/questions`);

    questionsRef.on("value", snap => {
      if(snap.exists()) {
        const data = snap.val();

        var questions = Object.keys(data).map((val, idx) => {
          var obj = data[val];
          obj.id = val;
          return obj;
        });

        console.log(questions.sort((a,b) => a.order - b.order));

        this.setState({
          questions: questions
        });
      }
    });
  }

  moveQuestionUp() {
    var questions = this.state.questions.map(val => val);
    var selectedIndex = this.questionsTable.current.selectionContext.selected[0];
    if(selectedIndex > 1) {
      this.setState({
        moveButtonsDisabled: true
      }, () =>{
        
        this.questionsTable.current.selectionContext.selected = [selectedIndex-1];
    
          var questionData = {}
          questions.map((question, idx) => {
            idx += 1; // Normalize this index?

            if(question.order === selectedIndex) {
              question.order -= 1;
            } else if(question.order === selectedIndex-1) {
              question.order = selectedIndex;
            }

            questionData[question.id] = {
              imageName: question.imageName, 
              imageUrl: question.imageUrl,
              order: question.order,
              selectedSolution: question.selectedSolution,
              solutions: question.solutions
            };
          });

          this.setState({
            questions: questions
          }, () => {
            const db = firebase.database();
            const userId = this.props.userId;
            const classId = this.props.classId;
      
            const questionsRef = db.ref(`teachers/${userId}/classes/${classId}/questions`);
      
            questionsRef.set(questionData).then(() => this.setState({
              moveButtonsDisabled: false
            }));
          })
      });
    }
  }

  moveQuestionDown() {
    var selectedIndex = this.questionsTable.current.selectionContext.selected[0];
    var questions = this.state.questions.map(val => val);

    if(selectedIndex < questions.length) {
      this.setState({
        moveButtonsDisabled: true
      }, () =>{
        this.questionsTable.current.selectionContext.selected = [selectedIndex+1];
    
        
          var questionData = {}
          questions.map((question, idx) => {
            idx += 1; // Normalize this index?

            if(question.order === selectedIndex) {
              question.order += 1;
            } else if(question.order === selectedIndex+1) {
              question.order = selectedIndex;
            }

            questionData[question.id] = {
              imageName: question.imageName, 
              imageUrl: question.imageUrl,
              order: question.order,
              selectedSolution: question.selectedSolution,
              solutions: question.solutions
            };
          });

          this.setState({
            questions: questions
          }, () => {
            const db = firebase.database();
            const userId = this.props.userId;
            const classId = this.props.classId;
      
            const questionsRef = db.ref(`teachers/${userId}/classes/${classId}/questions`);
      
            questionsRef.set(questionData).then(() => this.setState({
              moveButtonsDisabled: false
            }));
          });
      });
    }
  }
}

