import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { IntlService } from '@progress/kendo-angular-intl';
import { CustomerReceiptReportFilter } from 'src/app/customer-receipt-reports/customer-receipt-report.service';

@Component({
  selector: 'app-sale-dashboard-ap-cr-chart',
  templateUrl: './sale-dashboard-ap-cr-chart.component.html',
  styleUrls: ['./sale-dashboard-ap-cr-chart.component.css']
})
export class SaleDashboardApCrChartComponent implements OnInit {
  // Pie
  @Input() dataCustomer: any[] = [];
  @Input() dataNoTreatment: any[] = [];

  loading = false;
  limit = 20;
  skip = 0;
  total: number;
  state: string;
  pieDataCustomer: any[] = [];
  pieDataNoTreatment: any[] = [];

  constructor(
    private intlService: IntlService,
    private router: Router,
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadPieDataCustomer();
  }

  ngOnInit() {
    this.loadPieDataCustomer();
  }


  loadPieDataCustomer() {
    this.pieDataCustomer = [];
    for (let i = 0; i < this.dataCustomer.length; i++) {
      this.pieDataCustomer.push({ category: this.dataCustomer[i].text, value: this.dataCustomer[i].count, color: this.dataCustomer[i].text == 'old' ? '#1A6DE3' : '#95C8FF' })
    };
  }

  get countdataCustomer() {
    var count = 0;
    this.dataCustomer.forEach(x =>
      count += x.count
    );
    return count;
  }

  get countdataNoTreatment() {
    var count = 0;
    this.dataNoTreatment.forEach(x =>
      count += x.countCustomerReceipt
    );
    return count;
  }

  getCusType(val) {
    switch (val) {
      case 'old':
        return 'Khách quay lại';
      case 'new':
        return 'Khách mới';
    }
  }

  redirectTo(value) {
    switch (value) {
      case 'partner-report-overview':
        this.router.navigateByUrl("report-account-common/partner-report-overview");
        break;
      case 'customer-receipt-overview':
        this.router.navigateByUrl("customer-receipt-reports/customer-receipt-overview");
        break;
      default:
        break;
    }
  }



}


