import React, { Component } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import { Button, Form, Modal, ModalTitle, ModalBody, ModalFooter, FormLabel, FormGroup, FormControl } from 'react-bootstrap';
import Controls from "../../Controls/Controls";
import * as firebase from 'firebase/app';
import "firebase/database";
import "firebase/storage";
import { storage } from 'firebase';
import ModalHeader from 'react-bootstrap/ModalHeader';
import { faWindowRestore } from '@fortawesome/free-solid-svg-icons';

const { SearchBar } = Search;

export default class ClassStudents extends Component {
  constructor(props) {
    super(props);

    this.state = {
      studentsInClass: this.props.studentsInClass,
      modalOpen: false,
      image: null,
      addQuestion: false,

    };
    this.handleHideAddQuestionModal = this.handleHideAddQuestionModal.bind(this);
    this.handleShowAddQuestionModal = this.handleShowAddQuestionModal.bind(this);
    this.handleUploadPicture = this.handleUploadPicture.bind(this);
    this.handleChange = this.handleChange.bind(this);
  }

  componentDidMount() {

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
      onSelect: this.props.handleStudentInClassSelect
    };


    const questionColumns = [
      {
        dataField: "",
        text: "First Name",
        sort: true
      },
      {
        dataField: "lastName",
        text: "Last Name",
        sort: true
      },
      {
        dataField: "email",
        text: "Email",
        sort: true
      }
    ];


    return (
      <div id="students-in-class">
        <Controls title="Questions">
          <Button variant="success" size="sm" onClick={this.handleShowAddQuestionModal}>Add Question</Button>
        </Controls>
        <Form>
          {/* <ToolkitProvider
            keyField="id"
            data={this.state.image}
            columns={questionColumns}
            search
            sort
          >
            {
              props => (
                <div>
                  <SearchBar {...props.searchProps} />
                  <hr />
                  <BootstrapTable
                    {...props.baseProps}
                    selectRow={selectRow}
                  />
                </div>
              )
            }
          </ToolkitProvider> */}
          <Modal show={this.state.modalOpen} onHide={this.handleHideAddQuestionModal}>
            <ModalHeader closeButton onHide={this.handleHideAddQuestionModal}>
              <ModalTitle>Upload Question(s)</ModalTitle>
            </ModalHeader>
            <ModalBody>
              <FormGroup>
                <FormControl type="file" accept="image/x-png,image/gif,image/jpeg" onChange={this.handleChange} required></FormControl>
              </FormGroup>

            </ModalBody>
            <ModalFooter>
              <Button variant="secondary" onClick={this.handleHideAddQuestionModal} size="sm">
                Close
            </Button>
              <Button type="submit" variant="primary" onClick={this.handleUploadPicture} size="sm">
                Confirm
            </Button>
            </ModalFooter>
          </Modal>
        </Form>
      </div>

    );
  }
  handleHideAddQuestionModal() {
    this.setState({
      modalOpen: false
    })
  }
  handleShowAddQuestionModal() {
    this.setState({
      modalOpen: true
    })
  }
  handleChange = e => {
    const image = e.target.files[0];
    console.log(image);
    this.setState({
      image
    })
  }
  handleUploadPicture = () => {
    const { image } = this.state;
    const metadata = {
      contentType: 'image/jpeg'
    };
    const storageRef = firebase.storage().ref();
    const imageRef = storageRef.child(`${image.name}`).put(image, metadata);
    imageRef.on('state_changed', (snapshot) => {
      const progress = Math.round((snapshot.bytesTransferred / snapshot.totalBytes) * 100);
      console.log(progress)
      this.setState({ progress });
    },
      (error) => {
        // error function ....
        console.log(error);
      },
    )
  }
}

