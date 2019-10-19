import { Component, OnInit, Output, EventEmitter, ViewChild, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-appointment-date-filter',
  templateUrl: './appointment-date-filter.component.html',
  styleUrls: ['./appointment-date-filter.component.css'],
  host: {
    class: "o_advance_search_dropdown"
  }
})
export class AppointmentDateFilterComponent implements OnInit {
  formGroup: FormGroup;
  @Input() dateFrom: Date;
  @Input() dateTo: Date;
  @Output() searchChange = new EventEmitter<any>();

  defaultFormGroup = {
    dateFrom: null,
    dateTo: null
  };

  public today: Date = new Date(new Date().toDateString());
  public yesterday: Date = new Date(new Date(new Date().setDate(new Date().getDate() - 1)).toDateString())
  public weekStart: Date = new Date(new Date().setDate(new Date().getDate() - new Date().getDay() + (new Date().getDay() == 0 ? -6 : 1)));
  public weekEnd: Date = new Date(new Date().setDate(new Date().getDate() - new Date().getDay() + (new Date().getDay() == 0 ? -6 : 1) + 6));
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());
  public yearStart: Date = new Date(new Date().getFullYear(), 0, 1);
  public yearEnd: Date = new Date(new Date().getFullYear(), 11, 31);

  dateOption = 'all';

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group(this.defaultFormGroup);
    if (this.dateFrom) {
      this.formGroup.get('dateFrom').setValue(this.dateFrom);
    }
    if (this.dateTo) {
      this.formGroup.get('dateTo').setValue(this.dateTo);
    }
  }

  onSearch() {
    this.searchChange.emit(this.formGroup.value);
  }

  onClear() {
    this.formGroup = this.fb.group(this.defaultFormGroup);
    this.dateOption = 'all';
    this.searchChange.emit(this.formGroup.value);
  }

  changeDateOption(e) {
    var value = e.target.value;
    if (value == 'today') {
      this.formGroup.get('dateFrom').setValue(this.today);
      this.formGroup.get('dateTo').setValue(this.today);
    } else if (value == 'yesterday') {
      this.formGroup.get('dateFrom').setValue(this.yesterday);
      this.formGroup.get('dateTo').setValue(this.yesterday);
    } else if (value == 'this_week') {
      this.formGroup.get('dateFrom').setValue(this.weekStart);
      this.formGroup.get('dateTo').setValue(this.weekEnd);
    } else if (value == 'this_month') {
      this.formGroup.get('dateFrom').setValue(this.monthStart);
      this.formGroup.get('dateTo').setValue(this.monthEnd);
    } else if (value == 'this_year') {
      this.formGroup.get('dateFrom').setValue(this.yearStart);
      this.formGroup.get('dateTo').setValue(this.yearEnd);
    } else {
      this.formGroup.get('dateFrom').setValue(null);
      this.formGroup.get('dateTo').setValue(null);
    }
  }
}


