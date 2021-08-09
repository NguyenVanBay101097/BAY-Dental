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
  @Input() cashBooks: any;
  cashBookData: CashBookReportItem[] = [];
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
    debugger
    this.cashBookData = this.cashBooks;
    this.cashbookSeries = [];
    this.loadCashbookGroupby();
    this.loadCashbookSeries();
  }

  public labelContent = (e: any) => {
    var res = this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(e.value), 'dd/MM') : this.intlService.formatDate(new Date(e.value), 'MM/yyyy');
    return res;
  };

  loadCashbookSeries(){
    var cashbookThu = {name: "Thu", type: "column", data : this.cashBookData.map(s => s.totalThu)};
    var cashbookChi = {name: "Chi", type: "column", data : this.cashBookData.map(s => s.totalChi)};
    var cashbookTotalAmount = {name: "Tồn sổ quỹ", type: "line", data : this.cashBookData.map(s => s.totalAmount)};
    this.cashbookSeries.push(cashbookThu,cashbookChi,cashbookTotalAmount);      
  }

  loadCashbookGroupby() { 
    this.cashbookCategs = this.cashBookData.map(s => s.date);
       
  }

}
