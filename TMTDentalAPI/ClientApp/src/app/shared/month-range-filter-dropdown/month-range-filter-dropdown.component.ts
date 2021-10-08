import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { PopupSettings } from '@progress/kendo-angular-dropdowns';
import * as moment from 'moment';

@Component({
  selector: 'app-month-range-filter-dropdown',
  templateUrl: './month-range-filter-dropdown.component.html',
  styleUrls: ['./month-range-filter-dropdown.component.css']
})
export class MonthRangeFilterDropdownComponent implements OnInit {
  formGroup: FormGroup;
  @Input() dateFrom: Date;
  @Input() dateTo: Date;
  @Output() searchChange = new EventEmitter<any>();
  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;

  public popupSettings: PopupSettings = {
    appendTo: "component",
  };

  monthFrom: any;
  monthTo: any;
  currentYear = new Date().getFullYear();
  monthStart = new Date(this.currentYear, 0, 1);
  monthEnd = new Date(this.currentYear, 11, 1);
  constructor() { }

  ngOnInit() {
    this.monthFrom = new Date(this.dateFrom)
    this.monthTo = new Date(this.dateTo)
  }

  onApply() {
    var value = { dateFrom: null, dateTo: null };
    this.dateFrom = this.monthFrom;
    this.dateTo = this.monthTo;

    if (this.monthFrom) {
      value.dateFrom = new Date(this.monthFrom.getFullYear(), this.monthFrom.getMonth(), 1);
    }
    if (this.monthTo) {
      value.dateTo = new Date(this.monthTo.getFullYear(), this.monthTo.getMonth(), 1);
    }
    console.log(value);

    this.searchChange.emit(value);
    this.myDrop.close();
  }

  showMonthRange() {
    let dateFromStr = this.dateFrom ? moment(this.dateFrom).format('MM/YYYY') : '';
    let dateToStr = this.dateTo ? moment(this.dateTo).format('MM/YYYY') : '';
    let title = '';
    if (dateFromStr && !dateToStr) {
      title = `${dateFromStr} - ...`;
    }
    if (!dateFromStr && dateToStr) {
      title = `... - ${dateToStr}`;
    }
    if (dateFromStr && dateToStr) {
      title = `${dateFromStr} - ${dateToStr}`;
    }
    return title;
  }

  onClose() {
    this.monthFrom = new Date(this.dateFrom)
    this.monthTo = new Date(this.dateTo)
    this.myDrop.close();
  }
}
