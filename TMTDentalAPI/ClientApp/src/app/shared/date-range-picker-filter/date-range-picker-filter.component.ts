import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { DaterangepickerDirective } from './config/daterangepicker.directive';

@Component({
  selector: 'app-date-range-picker-filter',
  templateUrl: './date-range-picker-filter.component.html',
  styleUrls: ['./date-range-picker-filter.component.css'],
})
export class DateRangePickerFilterComponent implements OnInit {

  @Input() startDate: any;
  @Input() endDate: any;
  @Output() searchChange = new EventEmitter<any>();

  selected: any;
  ranges: any = {
    'Today': [moment(), moment()],
    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
    'This Month': [moment().startOf('month'), moment().endOf('month')],
    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
  }

  public options: any = {
    locale: { format: 'YYYY-MM-DD' },
    alwaysShowCalendars: false,
  }

  @ViewChild(DaterangepickerDirective, { static: false }) inputDr: DaterangepickerDirective;
  constructor() {
   
  }

  ngOnInit() {
    this.selected = {
      startDate: moment(this.startDate),
      endDate:moment(this.endDate)
    }
  }
  ngAfterViewInit() {
  }

  clear() {
    this.selected = null;
    this.onApply();
  }

  open() {
    this.inputDr.open();
  }

  onApply() {
    var value = {dateFrom: null, dateTo: null};
    if(!this.selected) {
    this.searchChange.emit(value);
    return;
    }
    if (this.selected.startDate) {
      value.dateFrom = this.selected.startDate.toDate();
    }

    if (this.selected.endDate) {
      value.dateTo = this.selected.endDate.toDate();
    }
    this.searchChange.emit(value);
  }
}