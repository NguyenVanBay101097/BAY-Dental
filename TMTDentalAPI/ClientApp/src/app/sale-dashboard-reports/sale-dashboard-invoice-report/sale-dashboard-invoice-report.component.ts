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
  @Input() dateTo: Date;
  @Input() dateFrom: Date;
  @Input() companyId: string;
  revenues: RevenueReportItem[] = [];
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
    if (!this.dateFrom || !this.dateTo) {
      return false;
    }

    var filter = new RevenueReportFilter();
    filter.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    filter.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    filter.companyId = this.companyId ? this.companyId : '';
    filter.groupBy = this.groupby;
    this.revenueReportService.getRevenueReport(filter).subscribe((result: any) => {
      this.revenues = result;
    });
  }

  loadCashbookGroupby() {
    this.revenuCateg = this.revenues.map(s => s.invoiceDate);
  }

  public labelContent = (e: any) => {
    var res = this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(e.value), 'dd/MM') : this.intlService.formatDate(new Date(e.value), 'MM/yyyy');
    return res;
  };


}
