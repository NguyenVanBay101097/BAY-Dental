import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
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

  @Input() paged: CashBookPaged;
  @Input() changeToLoadData: boolean;

  constructor(
    private modalService: NgbModal, 
    private cashBookService: CashBookService
  ) { }

  ngOnChanges(changes:SimpleChanges): void { 
    this.loadDataGetSumary();
    this.loadDataGetMoney();
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

  loadDataGetSumary() {
    this.loading = true;

    this.cashBookService.getSumary(this.paged)
    .subscribe(
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
        console.log(res);
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  editItem(item) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.type = item.type;
    modalRef.componentInstance.itemId = item.resId;
    modalRef.result.then(() => {
      this.loadDataGetSumary();
      this.loadDataGetMoney();
    }, er => { });
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
