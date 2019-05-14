import React, {Component} from "react";
import { FormControl, FormGroup, InputGroup, Button } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from '@fortawesome/fontawesome-svg-core'
import { faTrash } from '@fortawesome/free-solid-svg-icons'

library.add(faTrash);

export default class SolutionListItem extends Component {
  constructor(props) {
    super(props);

    this.state = {
      value: this.props.value
    };

    this.onChange = this.onChange.bind(this);
    this.handleRemoveListItem = this.handleRemoveListItem.bind(this);
  }

  componentDidUpdate() {
    if(this.props.value !== this.state.value) {
      this.setState({
        value: this.props.value
      });
    }
  }

  render() {
    return (
      <li className="mb-2">
        <InputGroup>
          <FormControl size="sm" type="text" value={this.state.value} onChange={this.onChange} onBlur={this.onBlur}/>
          <InputGroup.Append>
            <Button size="sm" variant="danger" onClick={this.handleRemoveListItem}>
              <FontAwesomeIcon icon="trash"/>
            </Button>
          </InputGroup.Append>
        </InputGroup>
      </li>
    )
  }

  handleRemoveListItem() {
    this.props.removeSolutionItem(this.props.index);
  }
  
  onBlur(e) {
    if(e.target.value.trim() === "") this.props.removeSolutionItem(this.props.index);
  }

  onChange(e) {
    this.setState({
      value: e.target.value
    }, () => this.props.updateSolutionItem(this.props.index, this.state.value));
  }
}