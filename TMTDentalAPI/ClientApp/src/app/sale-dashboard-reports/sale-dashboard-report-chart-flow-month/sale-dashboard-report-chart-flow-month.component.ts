import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { RevenueReportSearch, RevenueReportService } from 'src/app/revenue-report/revenue-report.service';

@Component({
  selector: 'app-sale-dashboard-report-chart-flow-month',
  templateUrl: './sale-dashboard-report-chart-flow-month.component.html',
  styleUrls: ['./sale-dashboard-report-chart-flow-month.component.css']
})
export class SaleDashboardReportChartFlowMonthComponent implements OnInit, OnChanges {
  @Input() dateTo: string;
  @Input() dateFrom: string;
  @Input() companyId: string;

  reportViewDateOfMonth: any[] = [];
  constructor(
    private revenueReportService: RevenueReportService,
  ) { }
  ngOnChanges(changes: SimpleChanges): void {
    if (this.dateFrom && this.dateTo) {
      this.loadData();
    }
  }

  month: Date;

  ngOnInit() {
    this.month = new Date(this.dateFrom);
    if (this.dateFrom && this.dateTo) {
      this.loadData();
    }
  }

  loadData() {
    var val = new RevenueReportSearch();
    val.dateFrom = this.dateFrom;
    val.dateTo = this.dateTo;
    val.companyId = this.companyId ? this.companyId : '';
    val.groupBy = "date:day";
    this.revenueReportService.getReport(val).subscribe(
      result => {
        if (result && result.details) {
          this.defindDateOfMonth(result.details);
        }
        console.log(result);
      }
    )
  }

  defindDateOfMonth(details) {
    debugger
    this.reportViewDateOfMonth = [];
    for (let index = 1; index <= new Date(this.dateTo).getDate(); index++) {
      var obj = {
        day: index,
        data: 0,
        month: new Date(this.dateTo).getMonth() + 1,
        year: new Date(this.dateTo).getFullYear()
      }
      var model = details.find(x => x.day == index);
      if (model) {
        obj.data = model.balance;
        this.reportViewDateOfMonth.push(obj);
      } else {
        this.reportViewDateOfMonth.push(obj);
      }
    }
  }
}
