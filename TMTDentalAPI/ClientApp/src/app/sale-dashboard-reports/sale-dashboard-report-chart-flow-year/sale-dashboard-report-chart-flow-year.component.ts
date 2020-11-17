import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { forkJoin } from 'rxjs';
import { RevenueReportSearch, RevenueReportService } from 'src/app/revenue-report/revenue-report.service';

@Component({
  selector: 'app-sale-dashboard-report-chart-flow-year',
  templateUrl: './sale-dashboard-report-chart-flow-year.component.html',
  styleUrls: ['./sale-dashboard-report-chart-flow-year.component.css']
})
export class SaleDashboardReportChartFlowYearComponent implements OnInit, OnChanges {

  @Input() companyId: string;
  curYear = new Date().getFullYear();
  oldYear = this.curYear - 1;
  public reportCurrentYear: any[] = [];
  public reportOldYear: any[] = [];
  public reportViewCurYears: any[] = [];
  public reportViewOldYears: any[] = [];
  constructor(
    private revenueReportService: RevenueReportService,
  ) { }
  ngOnChanges(changes: SimpleChanges): void {
    this.loadData();
  }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    var cur = this.loadReportRevenueCurrentYear();
    var old = this.loadReportRevenueOldYear();
    forkJoin([cur, old]).subscribe(results => {
      this.reportCurrentYear = results[0] ? results[0].details : [];
      this.reportOldYear = results[1] ? results[1].details : [];
      this.defindMonthOfYear();
    });
  }

  loadReportRevenueCurrentYear() {
    var val = new RevenueReportSearch();
    val.dateFrom = `${this.curYear}-01-01`
    val.dateTo = `${this.curYear}-12-31T23:59`
    val.companyId = this.companyId ? this.companyId : '';
    val.groupBy = "date:month";
    return this.revenueReportService.getReport(val);
  }

  loadReportRevenueOldYear() {
    var val = new RevenueReportSearch();
    val.dateFrom = `${this.oldYear}-01-01`
    val.dateTo = `${this.oldYear}-12-31`
    val.companyId = this.companyId ? this.companyId : '';
    val.groupBy = "date:month";
    return this.revenueReportService.getReport(val);
  }

  defindMonthOfYear() {
    this.reportViewOldYears = [];
    this.reportViewCurYears = [];
    for (let index = 1; index <= new Date().getMonth() + 1; index++) {
      var obj = {
        month: index,
        data: 0,
        year: 0
      }
      var model = this.reportCurrentYear.find(x => x.month == index);
      if (model) {
        this.reportViewCurYears.push({ month: index, data: model.balance, year: this.curYear });
      } else {
        obj.year = this.curYear;
        this.reportViewCurYears.push(obj);
      }

      var model2 = this.reportOldYear.find(x => x.month == index);
      if (model2) {
        this.reportViewOldYears.push({ month: index, data: model.balance, year: this.oldYear });
      }
      else {
        obj.year = this.oldYear;
        this.reportViewOldYears.push(obj);
      }
    }
  }
}
