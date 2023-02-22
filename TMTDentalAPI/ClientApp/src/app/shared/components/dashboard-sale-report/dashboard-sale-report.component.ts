import { Component, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleReportItem, SaleReportSearch, SaleReportService } from 'src/app/sale-report/sale-report.service';

@Component({
  selector: 'app-dashboard-sale-report',
  templateUrl: './dashboard-sale-report.component.html',
  styleUrls: ['./dashboard-sale-report.component.css'],
  host: { class: 'w-100' }
})
export class DashboardSaleReportComponent implements OnInit {

  saleReport: SaleReportItem;
  
  constructor(private intlService: IntlService, 
    private authService: AuthService, 
    private saleReportService: SaleReportService
  ) { }

  ngOnInit() {
    this.loadSaleReport();
  }

  loadSaleReport() {
    var val = new SaleReportSearch();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
    val.companyId = this.authService.userInfo.companyId;
    val.isQuotation = false;
    val.state = 'sale,done';
    // val.groupBy = "customer"
    this.saleReportService.getReport(val).subscribe(
      result => {
        if (result.length) {
          this.saleReport = result[0];
        } else {
          this.saleReport = null;
        }
      },
      error => {
        
      }
    );
  }
}
