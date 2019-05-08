import React, { Component } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import { Button } from 'react-bootstrap';
import Controls from "../../Controls/Controls";

const { SearchBar } = Search;

export default class ClassStudents extends Component {
  constructor(props) {
    super(props);

    this.state = {
      studentsInClass: this.props.studentsInClass
    };
  }

  componentDidUpdate(prevProps) {
    if(this.props.studentsInClass !== prevProps.studentsInClass) {
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


    // const questionColumns = [
    //   {
    //     dataField: "",
    //     text: "First Name",
    //     sort: true
    //   },
    //   {
    //     dataField: "lastName",
    //     text: "Last Name",
    //     sort: true
    //   },
    //   {
    //     dataField: "email",
    //     text: "Email",
    //     sort: true
    //   }
    // ];

    return (
      <div id="students-in-class">
        <Controls title="Questions">
          <Button variant="success" size="sm">Add Question</Button>
        </Controls>

        {/* <ToolkitProvider
          keyField="id"
          data={ this.state.studentsInClass }
          columns={ studentColumns }
          search
          sort
        >
          {
            props => (
              <div>
                <SearchBar { ...props.searchProps } />
                <hr />
                <BootstrapTable
                  { ...props.baseProps }
                  selectRow={selectRow}
                />
              </div>
            )
          }
        </ToolkitProvider> */}
      </div>
      
    );
  }
}

