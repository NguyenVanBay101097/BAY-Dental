import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { IntlService, load } from '@progress/kendo-angular-intl';
import { CashBookReportFilter, CashBookReportItem, CashBookService } from 'src/app/cash-book/cash-book.service';

@Component({
  selector: 'app-sale-dashboard-cashbook-report',
  templateUrl: './sale-dashboard-cashbook-report.component.html',
  styleUrls: ['./sale-dashboard-cashbook-report.component.css']
})
export class SaleDashboardCashbookReportComponent implements OnInit {
  @Input() groupby: string;
  @Input() dateTo: Date;
  @Input() dateFrom: Date;
  @Input() companyId: string;
  cashBooks: CashBookReportItem[] = [];
  cashbookThu: any[] = [];
  cashbookChi: any[] = [];
  cashbookTotal: any[] = [];
  cashbookCategs: any[] = [];
  cashbookSeries: any[] = [];


  constructor(private cashBookService: CashBookService,
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

    var filter = new CashBookReportFilter();
    filter.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    filter.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    filter.companyId = this.companyId ? this.companyId : '';
    filter.groupBy = this.groupby;
    this.cashBookService.getChartReport(filter).subscribe((result: any) => {
      this.cashBooks = result;
      this.cashbookSeries = [];
      this.loadCashbookGroupby();
      this.loadCashbookSeries();
    });
  }

  public labelContent = (e: any) => {
    var res = this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(e.value), 'dd/MM') : this.intlService.formatDate(new Date(e.value), 'MM/yyyy');
    return res;
  };

  loadCashbookSeries(){
    var cashbookThu = {name: "Thu", type: "column", data : this.cashBooks.map(s => s.totalThu)};
    var cashbookChi = {name: "Chi", type: "column", data : this.cashBooks.map(s => s.totalChi)};
    var cashbookTotalAmount = {name: "Tồn sổ quỹ", type: "line", data : this.cashBooks.map(s => s.totalAmount)};
    this.cashbookSeries.push(cashbookThu,cashbookChi,cashbookTotalAmount);      
  }

  loadCashbookGroupby() { 
    this.cashbookCategs = this.cashBooks.map(s => s.date);
       
  }

}
