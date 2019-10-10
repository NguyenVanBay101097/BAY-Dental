import { Component, OnInit, Output, EventEmitter, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-labo-order-line-advance-search',
  templateUrl: './labo-order-line-advance-search.component.html',
  styleUrls: ['./labo-order-line-advance-search.component.css'],
  host: {
    class: "o_advance_search"
  }
})

export class LaboOrderLineAdvanceSearchComponent implements OnInit {
  formGroup: FormGroup;
  @Output() searchChange = new EventEmitter<any>();

  show = false;
  defaultFormGroup = {
    sentDateFrom: null,
    sentDateTo: null,
    receivedDateFrom: null,
    receivedDateTo: null
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



