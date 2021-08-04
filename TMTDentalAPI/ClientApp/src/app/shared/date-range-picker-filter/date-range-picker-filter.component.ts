import { Component, ElementRef, EventEmitter, Input, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
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
  @Input() opens: string = 'auto';
  @Input() drops: string = 'auto';

  @Input() selected: any;
  ranges: any = {
    'Hôm nay': [moment(new Date()), moment(new Date())],
    'Hôm qua': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
    '7 ngày qua': [moment().subtract(6, 'days'), moment()],
    '30 ngày qua': [moment().subtract(29, 'days'), moment()],
    'Tháng này': [moment().startOf('month'), moment().endOf('month')],
    'Tháng trước': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
  }

  public options: any = {
    locale: { format: 'YYYY-MM-DD' },
    alwaysShowCalendars: false,
  }

  @ViewChild(DaterangepickerDirective, { static: false }) inputDr: DaterangepickerDirective;
  constructor() {

  }

  ngOnInit() {
    this.selected = this.selected || (
      (!this.startDate && !this.endDate) ? null :
        {
          startDate: moment(this.startDate),
          endDate: moment(this.endDate)
        })
  }
  ngAfterViewInit() {
  }
  // ngOnChanges(changes: SimpleChanges): void {
  //   //Called before any other lifecycle hook. Use it to inject dependencies, but avoid any serious work here.
  //   //Add '${implements OnChanges}' to the class.
  //   for (const change in changes) {
  //     if (changes.hasOwnProperty(change)) {
  //         if (['startDate','endDate'].indexOf(change) !== -1) {
  //           this.selected = 
  //               {
  //                 startDate: moment(this.startDate),
  //                 endDate: moment(this.endDate)
  //               };   
  //         }
  //     }
  // }
    
  // }

  clear() {
    this.selected = null;
    this.onApply();
  }

  open() {
    // this.inputDr.open();
  }

  onApply() {
    var value = { dateFrom: null, dateTo: null };
    if (!this.selected) {
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