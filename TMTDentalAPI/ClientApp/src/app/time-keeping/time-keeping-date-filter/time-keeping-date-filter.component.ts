import { Component, OnInit, ViewChild, Output, EventEmitter, Input } from '@angular/core';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-time-keeping-date-filter',
  templateUrl: './time-keeping-date-filter.component.html',
  styleUrls: ['./time-keeping-date-filter.component.css']
})
export class TimeKeepingDateFilterComponent implements OnInit {

  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;
  @Output() searchChange = new EventEmitter<any>();
  dateFrom: any;
  dateTo: any;
  monthOptionSelect: any;
  currentDate = new Date();
  lastDate: any;
  nextDate: any;
  otherDate: any;
  optionMonths: any;

  constructor() { }

  ngOnInit() {
    this.lastDate = new Date();
    this.lastDate.setDate(1);
    this.lastDate.setMonth(this.lastDate.getMonth() - 1);
    
    this.nextDate = new Date();
    this.nextDate.setDate(1);
    this.nextDate.setMonth(this.nextDate.getMonth() + 1);

    this.optionMonths = [
      { text: 'Tháng sau', value: this.nextDate },
      { text: 'Tháng này', value: this.currentDate },
      { text: 'Tháng trước', value: this.lastDate }
    ];
    this.otherDate = this.currentDate;
  }

  onSelectMonth(e) {
    if (e.value === '') {
      this.dateFrom = '';
      this.dateTo = '';
    } else {
      this.monthOptionSelect = e;
      this.dateFrom = new Date(e.value.getFullYear(), e.value.getMonth(), 1);
      this.dateTo = new Date(e.value.getFullYear(), e.value.getMonth() + 1, 0);
      this.otherDate = null;
    }
    this.onChangeData();
  }

  onOtherMonthChange(date: Date) {
    this.dateFrom = new Date(date.getFullYear(), date.getMonth(), 1);
    this.dateTo = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    this.onChangeData();
  }

  getMonthFilter() {
    const today = new Date();
    if (this.dateFrom && this.dateFrom !== '') {
      return (this.dateFrom.getMonth() + 1) + '/' + this.dateFrom.getFullYear();
    } else {
      return (this.currentDate.getMonth() + 1) + '/' + this.currentDate.getFullYear();
    }
  }

  onChangeData() {
    this.searchChange.emit({ dateFrom: this.dateFrom, dateTo: this.dateTo , date: this.otherDate });
    this.myDrop.close();
  }
}
