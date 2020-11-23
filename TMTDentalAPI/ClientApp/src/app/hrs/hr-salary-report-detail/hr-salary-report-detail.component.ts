import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-hr-salary-report-detail',
  templateUrl: './hr-salary-report-detail.component.html',
  styleUrls: ['./hr-salary-report-detail.component.css']
})
export class HrSalaryReportDetailComponent implements OnInit {

  @Input() item: any;

  constructor() { }

  ngOnInit() {
  }

}
