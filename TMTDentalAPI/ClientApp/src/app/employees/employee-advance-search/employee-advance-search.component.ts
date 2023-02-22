import { Component, OnInit, Output, EventEmitter, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-employee-advance-search',
  templateUrl: './employee-advance-search.component.html',
  styleUrls: ['./employee-advance-search.component.css'],
  host: {
    class: "o_advance_search"
  }
})
export class EmployeeAdvanceSearchComponent implements OnInit {
  formGroup: FormGroup;
  @Output() searchChange = new EventEmitter<any>();

  show = false;
  defaultFormGroup = {
    isDoctor: false,
    isAssistant: false,
  };
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group(this.defaultFormGroup);
  }

  toggleShow() {
    this.show = !this.show;
  }

  onSearch() {
    this.searchChange.emit(this.formGroup.value);
  }

  onClear() {
    this.formGroup = this.fb.group(this.defaultFormGroup);
    this.searchChange.emit(this.formGroup.value);
  }
}

