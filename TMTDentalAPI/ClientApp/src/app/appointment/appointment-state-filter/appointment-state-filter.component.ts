import { Component, OnInit, Output, EventEmitter, ViewChild, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-appointment-state-filter',
  templateUrl: './appointment-state-filter.component.html',
  styleUrls: ['./appointment-state-filter.component.css'],
  host: {
    class: "o_advance_search_dropdown"
  }
})
export class AppointmentStateFilterComponent implements OnInit {
  formGroup: FormGroup;
  @Input() confirmedState: boolean;
  @Input() doneState: boolean;
  @Input() cancelState: boolean;

  @Output() searchChange = new EventEmitter<any>();

  defaultFormGroup = {
    confirmedState: false,
    doneState: false,
    cancelState: false,
  };

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group(this.defaultFormGroup);
    if (this.confirmedState) {
      this.formGroup.get('confirmedState').setValue(this.confirmedState);
    }
    if (this.doneState) {
      this.formGroup.get('doneState').setValue(this.doneState);
    }
    if (this.cancelState) {
      this.formGroup.get('cancelState').setValue(this.cancelState);
    }
  }

  onSearch() {
    this.searchChange.emit(this.formGroup.value);
  }

  onClear() {
    this.formGroup = this.fb.group(this.defaultFormGroup);
    this.searchChange.emit(this.formGroup.value);
  }

  getResult() {
    var list = [];
    if (this.confirmedState == true) {
      list.push('Đang hẹn');
    }
    if (this.doneState == true) {
      list.push('Đã tới');
    }
    if (this.cancelState == true) {
      list.push('Đã hủy');
    }
    return list.join(' - ');
  }
}



