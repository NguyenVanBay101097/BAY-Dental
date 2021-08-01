import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin } from 'rxjs';
import { RevenueReportSearch, RevenueReportService } from 'src/app/revenue-report/revenue-report.service';

@Component({
  selector: 'app-sale-dashboard-report-chart-flow-year',
  templateUrl: './sale-dashboard-report-chart-flow-year.component.html',
  styleUrls: ['./sale-dashboard-report-chart-flow-year.component.css']
})
export class SaleDashboardReportChartFlowYearComponent implements OnInit {

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
    private intlService: IntlService
  ) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    forkJoin(this.listYears.map(x => this.loadReportRevenueYear(x))).subscribe(results => {
      results.forEach(res => {
        var index = results.indexOf(res);
        var year = this.listYears[index];

        var data = [];
        for (var i = 1; i <= 12; i++) {
          var details = res.details.filter(x => x.month == i);
          if (details.length) {
            data.push({ month: details[0].month, year: year, balance: details[0].balance });
          } else {
            data.push({ month: i, year: year, balance: 0 });
          }
        }

        this.reportYearDicts[year] = data;
      });
    });
  }

  loadReportRevenueYear(year) {
    var month = new Date().getMonth();
    var monthEnd = new Date(year, month, new Date(year, month + 1, 0).getDate());
    var val = new RevenueReportSearch();
    val.companyId = this.companyId || '';
    val.groupBy = "date:month";
    val.dateFrom = `${year}-01-01`
    val.dateTo = this.intlService.formatDate(monthEnd, 'yyyy-MM-dd');
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
