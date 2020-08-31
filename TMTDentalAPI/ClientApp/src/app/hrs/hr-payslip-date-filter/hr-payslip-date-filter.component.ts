import { Component, OnInit, EventEmitter, ViewChild, Output } from '@angular/core';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-hr-payslip-date-filter',
  templateUrl: './hr-payslip-date-filter.component.html',
  styleUrls: ['./hr-payslip-date-filter.component.css']
})
export class HrPayslipDateFilterComponent implements OnInit {

  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;
  @Output() searchChange = new EventEmitter<any>();


  dateFrom: any;
  dateTo: any;
  monthOptionSelect: any;
  currentDate = new Date();
  lastDate: any;
  otherDate: any;

  optionMonths: any;

  constructor() { }

  ngOnInit() {
    this.lastDate = new Date();
    this.lastDate.setDate(1);
    this.lastDate.setMonth(this.lastDate.getMonth() - 1);

    this.optionMonths = [
      { text: 'Tất cả', value: '' },
      { text: 'Tháng này', value: this.currentDate },
      { text: 'Tháng trước', value: this.lastDate },
    ];
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
      return 'Tất cả';
    }
  }

  onChangeData() {
    this.searchChange.emit({ dateFrom: this.dateFrom, dateTo: this.dateTo });
    this.myDrop.close();
  }

}
