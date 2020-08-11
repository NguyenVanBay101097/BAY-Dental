import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Subject } from 'rxjs';
import { IntlService } from '@progress/kendo-angular-intl';
import { CommissionReportsService, CommissionReport, ReportFilterCommission } from '../commission-reports.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-commission-report-list',
  templateUrl: './commission-report-list.component.html',
  styleUrls: ['./commission-report-list.component.css']
})
export class CommissionReportListComponent implements OnInit {
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());
  gridData: GridDataResult;
  reportResults: CommissionReport[] = [];
  limit = 20;
  skip = 0;
  dateFrom: Date;
  formGroup: FormGroup;
  dateTo: Date;
  searchUpdate = new Subject<string>();
  constructor(private commissionReportService: CommissionReportsService,
    private fb: FormBuilder,
    private intl: IntlService) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new ReportFilterCommission();
    if (this.dateFrom) {
      val.dateFrom = this.intl.formatDate(this.dateFrom, 'd', 'en-US');
    }
    if (this.dateTo) {
      val.dateTo = this.intl.formatDate(this.dateTo, 'd', 'en-US');
    }
   
    this.loading = true;
    this.commissionReportService.getReport(val).subscribe(result => {
      this.reportResults = result;       
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
    
  }
  
}
