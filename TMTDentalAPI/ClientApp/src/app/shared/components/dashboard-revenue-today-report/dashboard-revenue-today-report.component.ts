import { Component, OnInit } from '@angular/core';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-dashboard-revenue-today-report',
  templateUrl: './dashboard-revenue-today-report.component.html',
  styleUrls: ['./dashboard-revenue-today-report.component.css']
})
export class DashboardRevenueTodayReportComponent implements OnInit {

  public pieData: any[] = [
    { category: "Tiền mặt", value:  300000, color:"#0066cc"},
    { category: "Ngân hàng", value: 200000, color:"#99ccff" },
    { category: "Khác", value: 100000, color: "#b3b3b3" }, 
  ];

  constructor(private intlService: IntlService) { 
    this.labelContent = this.labelContent.bind(this);
  }

  ngOnInit() {
  }

  public labelContent(args: LegendLabelsContentArgs): string {
    return `${args.dataItem.category} years old: ${this.intlService.formatNumber(
      args.dataItem.value,
      "p2"
    )}`;
  }

}
