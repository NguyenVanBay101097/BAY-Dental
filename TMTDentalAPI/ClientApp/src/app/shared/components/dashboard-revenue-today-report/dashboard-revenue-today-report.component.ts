import { AuthService } from 'src/app/auth/auth.service';
import { DashboardReportService, ReportTodayRequest, RevenueTodayReponse } from './../../../core/services/dashboard-report.service';
import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-dashboard-revenue-today-report',
  templateUrl: './dashboard-revenue-today-report.component.html',
  styleUrls: ['./dashboard-revenue-today-report.component.css']
})
export class DashboardRevenueTodayReportComponent implements OnInit {
  @Input() revenueToday: RevenueTodayReponse;
  public today: Date = new Date(new Date().toDateString());
  loading = false;
  revenue: RevenueTodayReponse;

  // Pie
  public pieData: any[] = [];


  constructor(private intlService: IntlService,
    private dashboardService: DashboardReportService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.loadData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadData();
  }

  loadData() {
    this.revenue = this.revenueToday;
    this.loadPieData();
  }

  loadPieData() {
    if (this.revenue) {
      var cash = new Object({ category: "cash", value: this.revenue.totalCash, color: "#0066cc" });
      var bank = new Object({ category: "bank", value: this.revenue.totalBank, color: "#99ccff" });
      var other = new Object({ category: "other", value: this.totalOther, color: "#b3b3b3" });
      this.pieData.push(cash, bank, other);
    }

  }

  get isIncrease() {
    if (this.revenue.totalAmount >= this.revenue.totalAmountYesterday) {
      debugger
      return true;
    }
    return false;
  }

  get percentRevenue() {
    if (this.revenue) {
      if (this.revenue.totalAmountYesterday == 0) {
        return 0;
      }

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
