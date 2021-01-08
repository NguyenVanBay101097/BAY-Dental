import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { CashBookCuDialogComponent } from '../cash-book-cu-dialog/cash-book-cu-dialog.component';
import { CashBookPaged, CashBookService, ReportDataResult } from '../cash-book.service';

@Component({
  selector: 'app-cash-book',
  templateUrl: './cash-book.component.html',
  styleUrls: ['./cash-book.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class CashBookComponent implements OnInit {
  quickOptionDate: string;
  paged: CashBookPaged;
  loading = false;
  searchUpdate = new Subject<string>();
  reportCashData: ReportDataResult;
  reportBankData: ReportDataResult;
  changeToLoadData: boolean = false;
  listType = [{ name: 'Phiếu chi', value: 'outbound' }, { name: 'Phiếu thu', value: 'inbound' }];

  constructor(
    private modalService: NgbModal,
    private cashBookService: CashBookService,
    private intlService: IntlService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.paged = new CashBookPaged();
    this.paged.resultSelection = "cash";
    this.paged.companyId = this.authService.userInfo.companyId
    this.paged.begin = true;
    this.quickOptionDate = "Tháng này"; // Auto Call this.searchChangeDate()
    this.reportCashData = new ReportDataResult();
    this.reportBankData = new ReportDataResult();
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.changeToLoadData = !this.changeToLoadData;
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var companyId = this.authService.userInfo.companyId;
    this.cashBookService.getSumary({ resultSelection: "cash", companyId: companyId })
      .subscribe(
        (res) => {
          this.reportCashData = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );

    let cash = this.cashBookService.getSumary({ resultSelection: "cash", companyId: companyId });
    let bank = this.cashBookService.getSumary({ resultSelection: "bank", companyId: companyId });

    forkJoin([cash, bank]).subscribe(results => {
      this.reportCashData = results[0];
      this.reportBankData = results[1];
    });
  }

  searchChangeDate(value) {
    this.paged.dateFrom = value.dateFrom ? this.intlService.formatDate(value.dateFrom, "yyyy-MM-dd") : null;
    this.paged.dateTo = value.dateTo ? this.intlService.formatDate(value.dateTo, "yyyy-MM-ddT23:59") : null;
    this.changeToLoadData = !this.changeToLoadData;
  }

  changeType(type) {
    this.paged.type = type.value;
    this.changeToLoadData = !this.changeToLoadData;
  }

  clickTab(value) {
    this.paged.resultSelection = value;
    this.changeToLoadData = !this.changeToLoadData;
  }

  createItem(type) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.type = type;
    modalRef.result.then(() => {
      this.changeToLoadData = !this.changeToLoadData;
      this.loadDataFromApi();
    }, (err) => { });
  }

  exportExcelFile() {
    this.cashBookService.exportExcelFile(this.paged).subscribe((res) => {
      let filename = "TongSoQuy";
      if (this.paged.resultSelection == "cash") {
        filename = "SoQuyTienMat";
      } else if (this.paged.resultSelection == "bank") {
        filename = "SoQuyNganHang";
      }

      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      console.log(res);

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
  }
}
