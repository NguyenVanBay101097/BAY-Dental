import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-hr-payslip-date-filter',
  templateUrl: './hr-payslip-date-filter.component.html',
  styleUrls: ['./hr-payslip-date-filter.component.css']
})
export class HrPayslipDateFilterComponent implements OnInit {

  dateFrom: Date;
  dateTo: Date;
  constructor() { }

  ngOnInit() {

  }

}
