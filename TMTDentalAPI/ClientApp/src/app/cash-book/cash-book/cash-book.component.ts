import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { map } from 'rxjs/operators';
import { CashBookCuDialogComponent } from '../cash-book-cu-dialog/cash-book-cu-dialog.component';
import { CashBookPaged, CashBookService } from '../cash-book.service';

@Component({
  selector: 'app-cash-book',
  templateUrl: './cash-book.component.html',
  styleUrls: ['./cash-book.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class CashBookComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  paged: CashBookPaged;
  quickOptionDate: string;
  
  constructor(    
    private modalService: NgbModal, 
    private cashBookService: CashBookService, 
    private intlService: IntlService, 
  ) { }

  ngOnInit() {
    this.paged = new CashBookPaged();
    this.quickOptionDate = "Tháng này"; // Auto Call this.searchChangeDate()


  }

  searchChangeDate(value) {
    this.paged.dateFrom = value.dateFrom ? this.intlService.formatDate(value.dateFrom, "yyyy-MM-dd") : null;
    this.paged.dateTo = value.dateTo ? this.intlService.formatDate(value.dateTo, "yyyy-MM-dd") : null;

    this.loadDataFromApi();
  }

  changeType(value) {
    this.paged.type = value;

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    this.paged.limit = this.limit;
    this.paged.offset = this.skip;
    console.log(this.paged);

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
        console.log(this.gridData);
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
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

  createItem(type) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.type = type;
    modalRef.componentInstance.item = null;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, er => { });
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
