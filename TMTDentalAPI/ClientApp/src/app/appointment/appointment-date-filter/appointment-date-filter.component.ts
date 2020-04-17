import { Component, OnInit, Output, EventEmitter, ViewChild, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';

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
  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;
  quickOptions: {}[] = [];

  defaultFormGroup = {
    dateFrom: [null, Validators.required],
    dateTo: [null, Validators.required]
  };

  public today: Date = new Date(new Date().toDateString());
  public yesterday: Date = new Date(new Date(new Date().setDate(new Date().getDate() - 1)).toDateString());
  public tomorrow: Date = new Date(new Date(new Date().setDate(new Date().getDate() + 1)).toDateString());
  public next3days: Date = new Date(new Date(new Date().setDate(new Date().getDate() + 3)).toDateString());
  public next7days: Date = new Date(new Date(new Date().setDate(new Date().getDate() + 7)).toDateString());
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
      this.formGroup.get('dateFrom').setValue({
        year: this.dateFrom.getFullYear(),
        month: this.dateFrom.getMonth() + 1,
        day: this.dateFrom.getDate(),
      });
    }
    if (this.dateTo) {
      this.formGroup.get('dateTo').setValue({
        year: this.dateTo.getFullYear(),
        month: this.dateTo.getMonth() + 1,
        day: this.dateTo.getDate(),
      });
    }

    this.quickOptions = [
      { text: 'Hôm nay', dateFrom: this.today, dateTo: this.today },
      { text: 'Hôm qua', dateFrom: this.yesterday, dateTo: this.yesterday },
      { text: 'Ngày mai', dateFrom: this.tomorrow, dateTo: this.tomorrow },
      { text: '3 ngày tới', dateFrom: this.today, dateTo: this.next3days },
      { text: '7 ngày tới', dateFrom: this.today, dateTo: this.next7days }
    ];
  }

  quickOptionClick(option) {
    this.formGroup.get('dateFrom').setValue({
      year: option.dateFrom.getFullYear(),
      month: option.dateFrom.getMonth() + 1,
      day: option.dateFrom.getDate(),
    });

    this.formGroup.get('dateTo').setValue({
      year: option.dateTo.getFullYear(),
      month: option.dateTo.getMonth() + 1,
      day: option.dateTo.getDate(),
    });

    this.onSearch();
  }

  onSearch() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;

    if (value.dateFrom) {
      value.dateFrom = new Date(value.dateFrom.year, value.dateFrom.month - 1, value.dateFrom.day);
    }

    if (value.dateTo) {
      value.dateTo = new Date(value.dateTo.year, value.dateTo.month - 1, value.dateTo.day);
    }

    this.searchChange.emit(value);
    this.myDrop.close();
  }

  onClear() {
    this.formGroup = this.fb.group(this.defaultFormGroup);
    this.dateOption = 'all';
    this.searchChange.emit(this.formGroup.value);
    this.myDrop.close();
  }

  changeDateOption(e) {
    var value = e.target.value;
    if (value == 'today') {
      this.formGroup.get('dateFrom').setValue(this.today);
      this.formGroup.get('dateTo').setValue(this.today);
    } else if (value == 'yesterday') {
      this.formGroup.get('dateFrom').setValue(this.yesterday);
      this.formGroup.get('dateTo').setValue(this.yesterday);
    } else if (value == 'tomorrow') {
      this.formGroup.get('dateFrom').setValue(this.tomorrow);
      this.formGroup.get('dateTo').setValue(this.tomorrow);
    } else if (value == 'next3days') {
      this.formGroup.get('dateFrom').setValue(this.today);
      this.formGroup.get('dateTo').setValue(this.next3days);
    } else if (value == 'next7days') {
      this.formGroup.get('dateFrom').setValue(this.today);
      this.formGroup.get('dateTo').setValue(this.next7days);
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


