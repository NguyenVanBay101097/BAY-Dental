import { Component, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { AuthService } from 'src/app/auth/auth.service';
import { PartnerCustomerReportInput } from 'src/app/partners/partner.service';
import { PartnerOldNewReport, PartnerOldNewReportService } from 'src/app/sale-report/partner-old-new-report.service';

@Component({
  selector: 'app-dashboard-partner-customer-report',
  templateUrl: './dashboard-partner-customer-report.component.html',
  styleUrls: ['./dashboard-partner-customer-report.component.css']
})
export class DashboardPartnerCustomerReportComponent implements OnInit {

  partnerOldNewReport: PartnerOldNewReport;
  
  constructor(private intlService: IntlService, 
    private authService: AuthService, 
    private partnerOldNewReportService: PartnerOldNewReportService,
  ) { }

  ngOnInit() {
    this.loadPartnerCustomerReport();
  }

  loadPartnerCustomerReport() {
    var val = new PartnerCustomerReportInput();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
    val.companyId = this.authService.userInfo.companyId;
    this.partnerOldNewReportService.getSumaryPartnerOldNewReport(val).subscribe(
      result => {
        this.partnerOldNewReport = result;
      },
      error => {

      }
    );
  }
}
