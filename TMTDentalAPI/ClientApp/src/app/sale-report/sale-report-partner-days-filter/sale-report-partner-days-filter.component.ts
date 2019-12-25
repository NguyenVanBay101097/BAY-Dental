import { Component, OnInit, Output, EventEmitter, ViewChild, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-sale-report-partner-days-filter',
  templateUrl: './sale-report-partner-days-filter.component.html',
  styleUrls: ['./sale-report-partner-days-filter.component.css'],
  host: {
    class: "o_advance_search_dropdown"
  }
})
export class SaleReportPartnerDaysFilterComponent implements OnInit {
  formGroup: FormGroup;
  @Input() title: string = 'Chưa quay lại sau';
  @Input() monthsFrom: number;
  @Input() monthsTo: number;
  @Output() searchChange = new EventEmitter<any>();
  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;
  quickOptions: {}[] = [];

  defaultFormGroup = {
    monthsFrom: [null],
    monthsTo: [null]
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
    if (this.monthsFrom) {
      this.formGroup.get('monthsFrom').setValue(this.monthsFrom);
    }
    if (this.monthsTo) {
      this.formGroup.get('monthsTo').setValue(this.monthsTo);
    }

    this.quickOptions = [
      { text: '1 tới 3 tháng', monthsFrom: 1, monthsTo: 3 },
      { text: '3 tới 6 tháng', monthsFrom: 3, monthsTo: 6 },
      { text: '6 tới 12 tháng', monthsFrom: 6, monthsTo: 12 },
    ];
  }

  quickOptionClick(option) {
    this.formGroup.get('monthsFrom').setValue(option.monthsFrom);
    this.formGroup.get('monthsTo').setValue(option.monthsTo);
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




