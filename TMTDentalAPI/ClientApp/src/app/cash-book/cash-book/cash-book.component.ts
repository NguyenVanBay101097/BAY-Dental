import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
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

  constructor(    
    private modalService: NgbModal, 
    private cashBookService: CashBookService, 
    private intlService: IntlService, 
  ) { }

  ngOnInit() {
    this.paged = new CashBookPaged();
    this.paged.resultSelection = "cash";
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
    this.cashBookService.getSumary({resultSelection: "cash"})
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

    let cash =  this.cashBookService.getSumary({resultSelection: "cash"});
    let bank =  this.cashBookService.getSumary({resultSelection: "bank"});

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
    modalRef.result.then(() => {
      this.changeToLoadData = !this.changeToLoadData;
    }, er => { });
  }
}
