import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
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
  quickOptionDate: string;
  paged: CashBookPaged;
  changeToLoadData: boolean = false;

  constructor(    
    private modalService: NgbModal, 
    private cashBookService: CashBookService, 
    private intlService: IntlService, 
  ) { }

  ngOnInit() {
    this.paged = new CashBookPaged();
    this.paged.resultSelection = "cash";
    this.quickOptionDate = "Tháng này"; // Auto Call this.searchChangeDate()

  }

  searchChangeDate(value) {
    this.paged.dateFrom = value.dateFrom ? this.intlService.formatDate(value.dateFrom, "yyyy-MM-dd") : null;
    this.paged.dateTo = value.dateTo ? this.intlService.formatDate(value.dateTo, "yyyy-MM-dd") : null;
    this.changeToLoadData = !this.changeToLoadData;
  }

  changeType(value) {
    this.paged.type = value;
    this.changeToLoadData = !this.changeToLoadData;
  }

  clickTab(value) {
    this.paged.resultSelection = value;
    this.changeToLoadData = !this.changeToLoadData;
  }

  createItem(type) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.type = type;
    modalRef.componentInstance.item = null;
    modalRef.result.then(() => {
      this.changeToLoadData = !this.changeToLoadData;
    }, er => { });
  }
}
