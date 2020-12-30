import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { CashBookCuDialogComponent } from '../cash-book-cu-dialog/cash-book-cu-dialog.component';
import { CashBookPaged, CashBookService } from '../cash-book.service';

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

  @Input() paged: CashBookPaged;
  @Input() changeToLoadData: boolean;

  constructor(
    private modalService: NgbModal, 
    private cashBookService: CashBookService
  ) { }

  ngOnChanges(changes:SimpleChanges): void { 
    this.loadDataFromApi();
  }

  ngOnInit() {

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

  loadDataFromApi() {
    this.loading = true;
    this.paged.limit = this.limit;
    this.paged.offset = this.skip;

    this.cashBookService.getPaged(this.paged).pipe(map(
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

  editItem(type, item) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.type = type;
    modalRef.componentInstance.item = item;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, er => { });
  }
}
