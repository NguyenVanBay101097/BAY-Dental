import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { AccountBankCuDialogComponent } from '../account-bank-cu-dialog/account-bank-cu-dialog.component';
import { AccountAccountGetListCanPayOrReceiveRequest, CashBookDetailFilter, CashBookService, CashBookSummarySearch } from '../cash-book.service';

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
  @ViewChild("accountJournalCbx", { static: true }) accountVC: ComboBoxComponent;

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;

  dateFrom: Date;
  dateTo: Date;
  resultSelection = 'cash_bank';
  accountJournalSelectedId: string;
  listAccounts: AccountJournalSimple[] = [];
  cbxPopupSettings = {
    width: 320
  };
  filteredPaymentTypes: { [key: string]: string }[] = [
    { value: 'inbound', text: 'Phi???u thu' },
    { value: 'outbound', text: 'Phi???u chi' },
  ];

  paymentType: string = '';
  accountIds: string[];
  filteredAccountCode: any;
  constructor(
    private cashBookService: CashBookService,
    private intlService: IntlService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private modalService: NgbModal,
    private accountJournalService: AccountJournalService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.route.queryParamMap.subscribe( params => {
      var result_selection = params.get('result_selection'); 
      if (result_selection) {
        this.resultSelection = result_selection;
      }   
    });

    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.loadCashBankTotal();
    this.clickTab('cash_bank');
    this.loadAccounts();
    this.loadAutoCompleteAccount();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadGridData();
      });

    this.accountVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.accountVC.loading = true)),
        switchMap((value) => this.searchAccounts$(value)
        )
      )
      .subscribe((result: any) => {
        this.listAccounts = result;
        this.accountVC.loading = false;
      });
  }

  loadAutoCompleteAccount() {
    let val = new AccountAccountGetListCanPayOrReceiveRequest();
    val.limit = 0;
    val.offset = 0;
    val.companyId = this.authService.userInfo.companyId;

    this.cashBookService.getListCanPayOrReceive(val).subscribe((res: any) => {
      this.filteredAccountCode = res;
    }, error => console.log(error));
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
    summarySearch.journalId = this.accountJournalSelectedId;
    summarySearch.resultSelection = this.resultSelection;
    summarySearch.companyId = this.authService.userInfo.companyId;
    summarySearch.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    summarySearch.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
    summarySearch.accountIds = this.accountIds;
    summarySearch.paymentType = this.paymentType;
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
    var gridPaged = new CashBookDetailFilter();
    gridPaged.companyId = this.authService.userInfo.companyId;
    gridPaged.journalId = this.accountJournalSelectedId;;
    gridPaged.resultSelection = this.resultSelection;
    gridPaged.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    gridPaged.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
    gridPaged.offset = this.skip;
    gridPaged.limit = this.limit;
    gridPaged.search = this.search || '';
    gridPaged.accountIds = this.accountIds;
    gridPaged.paymentType = this.paymentType;
    this.cashBookService.getDetails(gridPaged)
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
    this.limit = event.take;
    this.loadGridData();
  }

  searchChangeDate(value) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
    this.loadGridData();
  }

  clickTab(value) {
    this.resultSelection = value;
    this.skip = 0;
    this.accountJournalSelectedId = null;
    this.loadDataFromApi();
    this.loadGridData();
  }

  getPartnerType(partnerType) {
    if (partnerType == "employee") {
      return "Nh??n vi??n";
    }
    else if (partnerType == "customer") {
      return "Kh??ch h??ng";
    }
    else if (partnerType == "supplier") {
      return "Nh?? cung c???p";
    } 
    return "";
  }

  getDifferentThuChi() {
    if (this.summarySearchResult) {
      var result = this.summarySearchResult.totalThu - this.summarySearchResult.totalChi;
      return result;
    }
    return 0;
  }

  searchAccounts$(search?) {
    return this.accountJournalService.getBankJournals({search: search || ''});
  }

  loadAccounts() {
    this.searchAccounts$().subscribe((result: any) => {
      this.listAccounts = result;
    })
  }

  editAccountBank() {
    let modalRef = this.modalService.open(AccountBankCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Ch???nh s???a t??i kho???n ng??n h??ng';
    modalRef.componentInstance.accountId = this.accountJournalSelectedId;
    modalRef.result.then((journal) => {
      if(!journal)
      this.accountJournalSelectedId = null;
      
      this.loadAccounts();
    });
  }

  addAccountBank() {
    let modalRef = this.modalService.open(AccountBankCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Th??m t??i kho???n ng??n h??ng';
    modalRef.result.then(() => {
      this.loadAccounts();
    });
  }

  onSelectAccount(journalObj) {
    this.accountJournalSelectedId = journalObj ? journalObj.id : '';
    this.loadGridData();
    this.loadDataFromApi();
  }

  exportExcelFile() {
    var gridPaged = new CashBookDetailFilter();
    gridPaged.companyId = this.authService.userInfo.companyId;
    gridPaged.resultSelection = this.resultSelection;
    gridPaged.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    gridPaged.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
    gridPaged.offset = this.skip;
    gridPaged.limit = this.limit;
    gridPaged.search = this.search || '';
    gridPaged.accountIds = this.accountIds;
    gridPaged.paymentType = this.paymentType;

    this.cashBookService.exportExcelFile(gridPaged).subscribe((res: any) => {
      let filename = "TongSoQuy";
      if (this.resultSelection == "cash") {
        filename = "SoQuyTienMat";
      } else if (this.resultSelection == "bank") {
        filename = "SoQuyNganHang";
      }

      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });

    // var paged = new ProductPaged();

    // paged.search = this.searchService || "";
    // paged.categId = this.cateId || "";
    // this.productService.excelServiceExport(paged).subscribe((rs) => {
    //   let filename = "danh_sach_dich_vu";
    //   let newBlob = new Blob([rs], {
    //     type:
    //       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //   });
    //   console.log(rs);

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

  changeData() {
    this.cashBookService.changeData()
      .subscribe(
        (res) => {

        },
        (err) => {

        }
      );
  }

  onChangePaymentType(event) {
    this.paymentType = event;
    this.loadGridData();
  }

  onAccountValueChange(event) {
    this.loadGridData();
  }
}
