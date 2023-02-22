import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { RevenueReportSearch, RevenueReportService } from 'src/app/revenue-report/revenue-report.service';

@Component({
  selector: 'app-sale-dashboard-report-chart-flow-month',
  templateUrl: './sale-dashboard-report-chart-flow-month.component.html',
  styleUrls: ['./sale-dashboard-report-chart-flow-month.component.css']
})
export class SaleDashboardReportChartFlowMonthComponent implements OnInit {
  @Input() dateTo: Date;
  @Input() dateFrom: Date;
  @Input() companyId: string;

  reportViewDateOfMonth: any[] = [];
  constructor(
    private revenueReportService: RevenueReportService,
    private intlService: IntlService
  ) { }

  month: number;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    if (!this.dateFrom || !this.dateTo) {
      return false;
    }
    
    var val = new RevenueReportSearch();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    val.companyId = this.companyId ? this.companyId : '';
    val.groupBy = "date:day";
    this.revenueReportService.getReport(val).subscribe(
      result => {
        if (result && result.details) {
          this.defindDateOfMonth(result.details);
        }
      }
    )
  }

  defindDateOfMonth(details) {
    var list = [];
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
        list.push(obj);
      } else {
        list.push(obj);
      }
    }

    this.reportViewDateOfMonth = list;
  }
}
