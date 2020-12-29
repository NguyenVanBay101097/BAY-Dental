import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { CashBookCuDialogComponent } from '../cash-book-cu-dialog/cash-book-cu-dialog.component';
import { CashBookService } from '../cash-book.service';

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
  
  constructor(    
    private modalService: NgbModal, 
    private cashBookService: CashBookService
  ) { }

  ngOnInit() {
  }

  loadDataFromApi() {

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
