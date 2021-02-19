import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AccountPaymentPaged, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { CashBookPaged, CashBookService, CashBookSummarySearch, ReportDataResult } from '../cash-book.service';

@Component({
  selector: 'app-cash-book-tab-page-ca-bo',
  templateUrl: './cash-book-tab-page-ca-bo.component.html',
  styleUrls: ['./cash-book-tab-page-ca-bo.component.css']
})
export class CashBookTabPageCaBoComponent implements OnInit {
  loading = false;
  searchUpdate = new Subject<string>();
  search: string;

  totalCash = 0;
  totalBank = 0;

  summarySearchResult: any;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  gridData: GridDataResult;
  limit = 20;
  skip = 0;

  dateFrom: Date;
  dateTo: Date;
  resultSelection = 'cash';

  constructor(
    private modalService: NgbModal,
    private cashBookService: CashBookService,
    private intlService: IntlService,
    private authService: AuthService,
    private accountPaymentService: AccountPaymentService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.loadCashBankTotal();
    this.loadDataFromApi();
    this.loadGridData();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadGridData();
      });
  }

  loadCashBankTotal() {
    var companyId = this.authService.userInfo.companyId;
    let cash = this.cashBookService.getSumary({ resultSelection: "cash", companyId: companyId });
    let bank = this.cashBookService.getSumary({ resultSelection: "bank", companyId: companyId });

    forkJoin([cash, bank]).subscribe(results => {
      this.totalCash = results[0].totalAmount;
      this.totalBank = results[1].totalAmount;
    });
  }

  loadDataFromApi() {
    this.loading = true;
    var summarySearch = new CashBookSummarySearch();
    summarySearch.resultSelection = this.resultSelection;
    summarySearch.companyId = this.authService.userInfo.companyId;
    summarySearch.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    summarySearch.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
    this.cashBookService.getSumary(summarySearch)
      .subscribe(
        (res) => {
          this.summarySearchResult = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  loadGridData() {
    this.loading = true;
    var gridPaged = new AccountPaymentPaged();
    gridPaged.companyId = this.authService.userInfo.companyId;
    gridPaged.journalType = this.resultSelection == "cash_bank" ? '' : this.resultSelection;
    gridPaged.paymentDateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    gridPaged.paymentDateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
    gridPaged.offset = this.skip;
    gridPaged.limit = this.limit;
    gridPaged.search = this.search || '';

    this.accountPaymentService.getPaged(gridPaged)
      .pipe(
        map((response: any) =>
          <GridDataResult>{
            data: response.items,
            total: response.totalItems,
          })
      ).subscribe(
        (res) => {
          this.gridData = res;
          this.loading = false;
        },
        (err) => {
          this.loading = false;
        }
      );
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadGridData();
  }


  searchChangeDate(value) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    this.loadDataFromApi();
    this.loadGridData();
  }

  clickTab(value) {
    this.resultSelection = value; 
    this.loadGridData();
    this.loadDataFromApi();
  }

  exportExcelFile() {
    // this.cashBookService.exportExcelFile(this.paged).subscribe((res) => {
    //   let filename = "TongSoQuy";
    //   if (this.paged.resultSelection == "cash") {
    //     filename = "SoQuyTienMat";
    //   } else if (this.paged.resultSelection == "bank") {
    //     filename = "SoQuyNganHang";
    //   }

    //   let newBlob = new Blob([res], {
    //     type:
    //       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //   });
    //   console.log(res);

    //   let data = window.URL.createObjectURL(newBlob);
    //   let link = document.createElement("a");
    //   link.href = data;
    //   link.download = filename;
    //   link.click();
    //   setTimeout(() => {
    //     // For Firefox it is necessary to delay revoking the ObjectURL
    //     window.URL.revokeObjectURL(data);
    //   }, 100);
    // });
  }
}
