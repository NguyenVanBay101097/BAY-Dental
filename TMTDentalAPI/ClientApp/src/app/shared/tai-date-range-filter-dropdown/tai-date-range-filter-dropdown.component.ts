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
  @Input() quickOption: string;
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
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public yearStart: Date = new Date(new Date().getFullYear(), 0, 1);
  public yearEnd: Date = new Date(new Date().getFullYear(), 11, 31);

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
      { text: 'Tuần này', dateFrom: this.weekStart, dateTo: this.weekEnd },
      { text: 'Tháng này', dateFrom: this.monthStart, dateTo: this.monthEnd },
      { text: 'Năm này', dateFrom: this.yearStart, dateTo: this.yearEnd }
    ];

    setTimeout(() => {
      if (this.quickOption) {
        switch (this.quickOption) {
          case 'Hôm nay':
            this.quickOptionClick(this.quickOptions[0]);
            return;
          case 'Hôm qua':
            this.quickOptionClick(this.quickOptions[1]);
            return;
          case 'Tuần này':
            this.quickOptionClick(this.quickOptions[2]);
            return;
          case 'Tháng này':
            this.quickOptionClick(this.quickOptions[3]);
            return;
          case 'Năm này':
            this.quickOptionClick(this.quickOptions[4]);
            return;
          default:
            return;
        }
      }
    });

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

    var value = Object.assign({}, this.formGroup.value);

    if (value.dateFrom) {
      value.dateFrom = new Date(value.dateFrom.year, value.dateFrom.month - 1, value.dateFrom.day);
    }

    if (value.dateTo) {
      value.dateTo = new Date(value.dateTo.year, value.dateTo.month - 1, value.dateTo.day);
    }

    this.searchChange.emit(value);
    this.myDrop.close();
  }

  onClear(event) {
    event.stopPropagation();
    
    this.formGroup = this.fb.group(this.defaultFormGroup);
    this.searchChange.emit(this.formGroup.value);
    this.myDrop.close();
  }

  toggleDropdown() {
    this.myDrop.toggle();
  }
}



