import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
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
    private cashBookService: CashBookService,
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
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

  getTypePayment(type) {
    if (type == "cash") {
      return "Tiền mặt";
    } else {
      return "Ngân hàng";
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
    this.paged.begin = true;
    this.cashBookService.getSumary(this.paged).subscribe(
      (res) => {
        this.reportData = res;
        this.loading = false;
      },
      (err) => {
        this.loading = false;
      }
    );
  }

  loadDataGetMoney() {
    this.loading = true;
    this.paged.limit = this.limit;
    this.paged.offset = this.skip;
    this.paged.begin = false;
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
        this.loading = false;
      }
    );
  }

  loadDataFromApi() {
    this.loadDataGetSumary();
    this.loadDataGetMoney();
  }

}
