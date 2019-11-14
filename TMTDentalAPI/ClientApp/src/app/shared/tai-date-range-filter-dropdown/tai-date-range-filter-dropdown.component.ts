import { Component, OnInit, Output, EventEmitter, ViewChild, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-tai-date-range-filter-dropdown',
  templateUrl: './tai-date-range-filter-dropdown.component.html',
  styleUrls: ['./tai-date-range-filter-dropdown.component.css'],
  host: {
    class: "o_advance_search_dropdown"
  }
})
export class TaiDateRangeFilterDropdownComponent implements OnInit {
  formGroup: FormGroup;
  @Input() dateFrom: Date;
  @Input() title: string;
  @Input() dateTo: Date;
  @Output() searchChange = new EventEmitter<any>();
  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;
  quickOptions: {}[] = [];

  defaultFormGroup = {
    dateFrom: [null],
    dateTo: [null]
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

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group(this.defaultFormGroup);
    if (this.dateFrom) {
      this.formGroup.get('dateFrom').setValue(this.dateFrom);
    }
    if (this.dateTo) {
      this.formGroup.get('dateTo').setValue(this.dateTo);
    }

    this.quickOptions = [
      { text: 'Hôm nay', dateFrom: this.today, dateTo: this.today },
      { text: 'Hôm qua', dateFrom: this.yesterday, dateTo: this.yesterday },
      { text: 'Tuần ngày', dateFrom: this.weekStart, dateTo: this.weekEnd },
      { text: 'Tháng này', dateFrom: this.monthStart, dateTo: this.monthEnd },
      { text: 'Năm này', dateFrom: this.yearStart, dateTo: this.yearEnd }
    ];
  }

  quickOptionClick(option) {
    this.formGroup.get('dateFrom').setValue(option.dateFrom);
    this.formGroup.get('dateTo').setValue(option.dateTo);
    this.searchChange.emit(this.formGroup.value);
    this.myDrop.close();
  }

  onSearch() {
    this.searchChange.emit(this.formGroup.value);
    this.myDrop.close();
  }

  onClear() {
    this.formGroup = this.fb.group(this.defaultFormGroup);
    this.searchChange.emit(this.formGroup.value);
    this.myDrop.close();
  }
}



