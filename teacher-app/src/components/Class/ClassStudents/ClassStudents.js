import React, { Component } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import { Button, Modal, ModalTitle, ModalBody, ModalFooter } from 'react-bootstrap';
import Controls from "../../Controls/Controls";
import * as firebase from 'firebase/app';
import "firebase/database";
import ModalHeader from 'react-bootstrap/ModalHeader';

const { SearchBar } = Search;

export default class ClassStudents extends Component {
  constructor(props) {
    super(props);

    this.state = {
      selectedStudentInClass: null,
      addStudent: false,
      selectedStudents: [],
      studentsInClass: []
    };

    this.handleStudentInClassSelect = this.handleStudentInClassSelect.bind(this);
    this.handleShowAddStudentModal = this.handleShowAddStudentModal.bind(this);
    this.handleHideAddStudentModal = this.handleHideAddStudentModal.bind(this);
    this.handleStudentSelect = this.handleStudentSelect.bind(this);
    this.handleSelectAllStudents = this.handleSelectAllStudents.bind(this);
    this.removeSelectedStudentFromClass = this.removeSelectedStudentFromClass.bind(this);
    this.addStudentsToClass = this.addStudentsToClass.bind(this);

    this.studentsTable = React.createRef();
  }

  componentDidMount() {
    this.getStudentsInClass();
  }

  componentWillReceiveProps(nextProps) {
    if(nextProps.studentsData) {
      this.setState({
        studentsData: nextProps.studentsData
      }, this.getStudentsInClass);
    }
  }

  render() {
    const selectRow = {
      mode: "radio",
      style: { background: "#eee" },
      onSelect: this.handleStudentInClassSelect
    };

    const addStudentsRowSelect = {
      mode: "checkbox",
      style: { background: "#eee" },
      onSelect: this.handleStudentSelect,
      onSelectAll: this.handleSelectAllStudents
    };

    const studentColumns = [
      {
        dataField: "firstName",
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
        <Controls title="Students">
          {this.renderRemoveStudentButton()}
          <Button variant="success" size="sm" onClick={this.handleShowAddStudentModal}>Add Student</Button>
        </Controls>

        <ToolkitProvider
          keyField="id"
          data={ this.state.studentsInClass }
          columns={ studentColumns }
          search
          sort
          bootstrap4
        >
          {
            props => (
              <div>
                <SearchBar { ...props.searchProps } />
                <hr />
                <BootstrapTable
                  { ...props.baseProps }
                  selectRow={selectRow}
                  ref={this.studentsTable}
                />
              </div>
            )
          }
        </ToolkitProvider>

        <Modal show={this.state.addStudent} onHide={this.handleHideAddStudentModal} size="lg">
          <ModalHeader closeButton onHide={this.handleHideAddStudentModal}>
            <ModalTitle>Add a Student to {this.props.classData.name}</ModalTitle>
          </ModalHeader>
          <ModalBody>
            <ToolkitProvider
              keyField="id"
              data={ this.props.studentsData }
              columns={ studentColumns }
              search
              sort
              bootstrap4
            >
              {
                props => (
                  <div>
                    <SearchBar { ...props.searchProps } />
                    <hr />
                    <BootstrapTable
                      { ...props.baseProps }
                      selectRow={addStudentsRowSelect}
                    />
                  </div>
                )
              }
            </ToolkitProvider>
					</ModalBody>
          <ModalFooter>
            <Button variant="secondary" onClick={this.handleHideAddStudentModal} size="sm">
              Close
            </Button>
            <Button variant="primary" onClick={this.addStudentsToClass} size="sm">
              Confirm
            </Button>
          </ModalFooter>
        </Modal>
      </div>
    );
  }
  
  renderRemoveStudentButton() {
    if(this.state.selectedStudentInClass != null && this.state.selectedStudentInClass != undefined && this.state.selectedStudentInClass) {
      return <Button variant="danger" onClick={this.removeSelectedStudentFromClass} size="sm" className="mr-2">Remove</Button>
    }
  }

  getStudentsInClass() {
    if(this.props.classData.students) {
      const allStudents = this.props.studentsData;
      const studentIds = this.props.classData.students;
      var studentsInClass = []; 

      allStudents.map(student => {
        const studentId = student.id;
        if(studentIds.includes(studentId)) {
          studentsInClass.push(student);
        }
      });


      if(studentsInClass.length) {
        this.setState({
          studentsInClass: studentsInClass
        });
      } else {
        this.setState({
          studentsInClass: []
        })
      }
    } else {
      this.setState({
        studentsInClass: []
      });
    }
  }

  removeSelectedStudentFromClass() {
    const db = firebase.database();

    const userId = this.props.userId;
    const classId = this.props.classId;

    const studentsInClassRef = db.ref(`teachers/${userId}/classes/${classId}/students`);

    var studentsInClass = this.state.studentsInClass.map(val=>val);
    const selectedStudentInClass = this.state.selectedStudentInClass;

    const index = studentsInClass.findIndex(obj => obj.id === selectedStudentInClass.id);
    if(index !== -1) {
      studentsInClass.splice(index, 1);
      const students = studentsInClass.map(obj => obj.id);
      studentsInClassRef.set(students)
        .then(() => {
          this.studentsTable.current.selectionContext.selected = [];
          this.setState({
            studentsInClass: studentsInClass,
            selectedStudentInClass: null
          }, () => this.getStudentsInClass);
        });
    }
  }

  addStudentsToClass() {
    const db = firebase.database();
    
    const userId = this.props.userId;
    const classId = this.props.classId;

    const studentsInClassRef = db.ref(`teachers/${userId}/classes/${classId}/students`);

    const selectedStudent = this.state.selectedStudents;
    const studentsInClass = this.state.studentsInClass;
    
    var students = studentsInClass.map(val => val.id);
    var update = false;
    selectedStudent.map(selectedStudent => {
      if(!students.includes(selectedStudent.id)) {
        update = true;
        students.push(selectedStudent.id);
      }
    });
    const studentsData = this.props.studentsData;
    console.log("STUDENTS DATA: ", studentsData);

    if(update) {
      studentsInClassRef.set(students)
      .then(() => {
        var updateStudents = students.map(studentId => {
          const classData = {
            name: this.props.classData.name,
            id: classId,
            teacherId: this.props.userId
          };

          return db.ref(`students/${studentId}/classes/${classId}/`).set(classData);
        });
  
        Promise.all(updateStudents).then(() => {
          this.setState({
            addStudent: false,
            selectedStudents: []
          }, this.getStudentsInClass);
        });
      });
    } else {
      this.setState({
        addStudent: false
      });
    }
  }

  handleHideAddStudentModal() {
    this.setState({
      addStudent: false
    });
  }

  handleShowAddStudentModal() {
    this.setState({
      addStudent: true
    });
  }
  
  handleStudentInClassSelect(student) {
    if(student !== this.state.selectedStudentInClass) {
      this.setState({
        selectedStudentInClass: student
      });
    }
  }

  handleStudentSelect(student) {
    var found = false;
    var selectedStudents = this.state.selectedStudents;
    selectedStudents.map(obj => {
      if(obj === student) {
        found = true
      }
    });

    if(!found) {
      selectedStudents.push(student);
    } else {
      const index = selectedStudents.findIndex(selStudent => selStudent === student);
      selectedStudents.splice(index, 1);
    }

    this.setState({
      selectedStudents: selectedStudents
    });
  }

  handleSelectAllStudents(selectedAll) {
    if(selectedAll) {
      this.setState({
        selectedStudents: this.props.studentsData
      });
    } else {
      this.setState({
        selectedStudents: []
      });
    }
  }
}

