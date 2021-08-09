import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { AccountInvoiceReportService, RevenueReportFilter, RevenueReportItem } from 'src/app/account-invoice-reports/account-invoice-report.service';
import { RevenueReportService } from 'src/app/revenue-report/revenue-report.service';

@Component({
  selector: 'app-sale-dashboard-invoice-report',
  templateUrl: './sale-dashboard-invoice-report.component.html',
  styleUrls: ['./sale-dashboard-invoice-report.component.css']
})
export class SaleDashboardInvoiceReportComponent implements OnInit {
  @Input() groupby: string;
  @Input() revenues: RevenueReportItem[] = [];
  revenueData: RevenueReportItem[] = [];
  revenuCateg: any[] = [];


  constructor(private revenueReportService: AccountInvoiceReportService,
    private intlService: IntlService) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadDataApi();
  }

  ngOnInit() {
    this.loadDataApi();
  }

  loadDataApi() {
    this.revenueData = this.revenues;
    this.loadCashbookGroupby();
  }
  
  loadCashbookGroupby() {
    this.revenuCateg = this.revenueData.map(s => s.invoiceDate);
  }

  public labelContent = (e: any) => {
    var res = this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(e.value), 'dd/MM') : this.intlService.formatDate(new Date(e.value), 'MM/yyyy');
    return res;
  };


}
