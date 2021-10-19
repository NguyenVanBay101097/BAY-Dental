import { AuthService } from 'src/app/auth/auth.service';
import { DashboardReportService, ReportTodayRequest, RevenueTodayReponse } from './../../../core/services/dashboard-report.service';
import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { ChartDataset, ChartOptions, ChartType } from 'chart.js';

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

  // Pie
  public pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false
      }
    }
  };

  public pieChartLabels: string[] = ['Tiền mặt',  'Ngân hàng' , 'Khác'];
  public pieChartData: ChartDataset[] = [
    { 
      data: [],
      backgroundColor: [ 'rgb(255, 99, 132)', 'rgb(54, 162, 235)', 'rgb(255, 205, 86)']
    }
  ];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = false;
  public pieChartPlugins = [this.pieChartLabels];
  public pieChartColors = [
    {
      backgroundColor: ['#0066cc', '#99ccff', '#b3b3b3'],
    },
  ];


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
      this.pieChartData[0].data = [this.revenue.totalCash, this.revenue.totalBank, this.totalOther];
      this.pieData.push(cash, bank, other);
    }

  }

  get isIncrease() {
    if (this.revenue.totalAmount > this.revenue.totalAmountYesterday) {
      return true;
    }
    return false;
  }

  get percentRevenue() {
    if (this.revenue) {
      if (this.revenue.totalAmountYesterday == 0) {
        return 0;
      }
      return Math.abs(((this.revenue.totalAmount - this.revenue.totalAmountYesterday) / this.revenue.totalAmountYesterday) * 100);
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
