import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { CashBookCuDialogComponent } from '../cash-book-cu-dialog/cash-book-cu-dialog.component';
import { CashBookPaged, CashBookService, ReportDataResult } from '../cash-book.service';

@Component({
  selector: 'app-cash-book-tab-cash-bank',
  templateUrl: './cash-book-tab-cash-bank.component.html',
  styleUrls: ['./cash-book-tab-cash-bank.component.css']
})
export class CashBookTabCashBankComponent implements OnInit, OnChanges {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  reportData: ReportDataResult;
  changeDateFirst: boolean = true;

  @Input() paged: CashBookPaged;
  @Input() changeToLoadData: boolean;

  constructor(
    private modalService: NgbModal, 
    private cashBookService: CashBookService,
    private phieuThuChiService: PhieuThuChiService,
  ) { }

  ngOnChanges(changes:SimpleChanges): void { 
    if (this.changeDateFirst == false) {
      this.loadDataFromApi();
    } else {
      this.changeDateFirst = false;
    }
  }

  ngOnInit() {
    this.reportData = new ReportDataResult();
  }

  getType(type) {
    if (type == "inbound") {
      return "Phiếu thu";
    } else {
      return "Phiếu chi";
    }
  }

  getState(state) {
    if (state == "posted") {
      return "Đã xác nhận";
    } else {
      return "Nháp"
    }
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataGetMoney();
  }

  loadDataGetSumary() {
    this.loading = true;

    this.cashBookService.getSumary(this.paged).subscribe(
      (res) => {
        this.reportData = res;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadDataGetMoney() {
    this.loading = true;
    this.paged.limit = this.limit;
    this.paged.offset = this.skip;

    this.cashBookService.getMoney(this.paged).pipe(map(
      (response: any) =>
        <GridDataResult>{
          data: response.items,
          total: response.totalItems,
        }
      )
    ).subscribe(
      (res) => {
        this.gridData = res;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadDataFromApi() {
    this.loadDataGetSumary();
    this.loadDataGetMoney();
  }

  editItem(item) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.type = item.type;
    modalRef.componentInstance.itemId = item.resId;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, er => { });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = `Xóa ${this.getType(item.type).toLowerCase()}`;
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa ${this.getType(item.type).toLowerCase()}?`;

    modalRef.result.then(() => {
      this.phieuThuChiService.delete(item.resId).subscribe(() => {
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }

  seeItem(item) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.type = item.type;
    modalRef.componentInstance.type2 = item.type2;
    modalRef.componentInstance.itemId = item.resId;
    modalRef.componentInstance.resModel = item.resModel;
    modalRef.componentInstance.seeForm = true;
    modalRef.result.then(() => {

    }, er => { });
  }
}
