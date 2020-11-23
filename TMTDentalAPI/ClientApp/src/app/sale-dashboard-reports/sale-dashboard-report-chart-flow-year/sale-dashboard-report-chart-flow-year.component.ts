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
  public reportYearDicts: {} = {};
  public reportViewYearDicts: {} = {};
  public listYears: any[] = [this.curYear, this.oldYear];
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
    forkJoin([this.loadReportRevenueYear(this.curYear), this.loadReportRevenueYear(this.oldYear)]).subscribe(results => {
      results.forEach(res => {
        if (res) {
          if (res.details) {
            res.details.forEach(detail => {
              if (!this.reportYearDicts[detail.year]) {
                this.reportYearDicts[detail.year] = [];
              }
              this.reportYearDicts[detail.year].push(detail);
            });
          }
        }
      });
      this.reportCurrentYear = results[0] ? results[0].details : [];
      this.reportOldYear = results[1] ? results[1].details : [];
      this.defindMonthOfYear();
    });
  }

  loadReportRevenueYear(year) {
    var val = new RevenueReportSearch();
    val.companyId = this.companyId ? this.companyId : '';
    val.groupBy = "date:month";
    val.dateFrom = `${year}-01-01`
    val.dateTo = `${year}-12-31T23:59`
    return this.revenueReportService.getReport(val);
  }

  defindMonthOfYear() {
    this.reportViewOldYears = [];
    this.reportViewCurYears = [];
    for (let index = 1; index <= 12; index++) {
      var obj = {
        month: index,
        data: 0,
        year: 0
      }
      if (!this.reportViewYearDicts[this.oldYear]) {
        this.reportViewYearDicts[this.oldYear] = [];
      }

      if (!this.reportViewYearDicts[this.curYear]) {
        this.reportViewYearDicts[this.curYear] = [];
      }

      var model = this.reportYearDicts[this.curYear] && this.reportYearDicts[this.curYear].length ? this.reportYearDicts[this.curYear].find(x => x.month == index) : null;
      if (model) {
        this.reportViewYearDicts[this.curYear].push({ month: index, data: model.balance, year: this.curYear });
      } else {
        obj.year = this.curYear;
        this.reportViewYearDicts[this.curYear].push(obj);
      }

      var model2 = this.reportYearDicts[this.oldYear] && this.reportYearDicts[this.oldYear].length ? this.reportYearDicts[this.oldYear].find(x => x.month == index) : null;
      if (model2) {
        this.reportViewYearDicts[this.oldYear]({ month: index, data: model.balance, year: this.oldYear });
      }
      else {
        obj.year = this.oldYear;
        this.reportViewYearDicts[this.oldYear].push(obj);
      }
    }
  }
}
