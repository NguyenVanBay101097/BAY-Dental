import { AuthService } from 'src/app/auth/auth.service';
import { DashboardReportService, RevenueTodayReponse, RevenueTodayRequest } from './../../../core/services/dashboard-report.service';
import { Component, OnInit } from '@angular/core';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-dashboard-revenue-today-report',
  templateUrl: './dashboard-revenue-today-report.component.html',
  styleUrls: ['./dashboard-revenue-today-report.component.css']
})
export class DashboardRevenueTodayReportComponent implements OnInit {
  public today: Date = new Date(new Date().toDateString());
  loading = false;
  revenue = new RevenueTodayReponse;
  public pieData: any[] = [
    { category: "Tiền mặt", value: 0, color: "#0066cc" },
    { category: "Ngân hàng", value: 0, color: "#99ccff" },
    { category: "Khác", value: 0, color: "#b3b3b3" },
  ];

  constructor(private intlService: IntlService,
    private dashboardService: DashboardReportService,
    private authService: AuthService
  ) { }

  ngOnInit() {
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new RevenueTodayRequest();
    val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.companyId = this.authService.userInfo.companyId;
    this.dashboardService.getSumary(val).subscribe(result => {
      this.revenue = result;
      this.pieData = [];
      this.loadItems();
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  }

}
