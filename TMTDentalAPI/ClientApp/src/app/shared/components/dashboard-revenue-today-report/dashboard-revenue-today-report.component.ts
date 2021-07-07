import { AuthService } from 'src/app/auth/auth.service';
import { DashboardReportService, ReportTodayRequest, RevenueTodayReponse } from './../../../core/services/dashboard-report.service';
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

  // Pie
  public pieData: any[] = [];


  constructor(private intlService: IntlService,
    private dashboardService: DashboardReportService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ReportTodayRequest();
    val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.companyId = this.authService.userInfo.companyId;
    this.dashboardService.getSumary(val).subscribe(result => {
      this.revenue = result;
      this.loadPieData();
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  }

  loadPieData() {
    var cash = new Object({ category: "cash", value: this.revenue.totalCash, color: "#0066cc" });
    var bank = new Object({ category: "bank", value: this.revenue.totalBank, color: "#99ccff" });
    var other = new Object({ category: "other", value: this.totalOther, color: "#b3b3b3" });
    this.pieData.push(cash, bank, other);
  }

  get isIncrease() {
    if (this.revenue.totalAmount >= this.revenue.totalAmountYesterday) {
      return true;
    }
    return false;
  }

  get percentRevenue() {
    if (this.revenue) {
      return (this.revenue.totalAmount - this.revenue.totalAmountYesterday / this.revenue.totalAmountYesterday) * 100;
    }

    return 0;
  }

  get totalOther() {
    if (this.revenue) {
      return (this.revenue.totalAmount - (this.revenue.totalBank + this.revenue.totalCash));
    }

    return 0;
  }

  getType(value) {
    switch (value) {
      case 'cash':
        return 'Tiền mặt';
      case 'bank':
        return 'Ngân hàng';
      case 'other':
        return 'Khác';
    }
  }

}
