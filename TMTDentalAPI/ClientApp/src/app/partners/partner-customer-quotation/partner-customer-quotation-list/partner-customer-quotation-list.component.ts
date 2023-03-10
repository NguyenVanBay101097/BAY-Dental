import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { QuotationService, QuotationsPaged } from 'src/app/quotations/quotation.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
// import { QuotationService, QuotationsPaged } from './quotation.service';

@Component({
  selector: 'app-partner-customer-quotation-list',
  templateUrl: './partner-customer-quotation-list.component.html',
  styleUrls: ['./partner-customer-quotation-list.component.css']
})
export class PartnerCustomerQuotationListComponent implements OnInit {
  partnerId: string;
  gridData: GridDataResult;
  dateFrom: Date;
  dateTo: Date;
  limit: number = 20;
  skip: number = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  selectedIds: any;
  constructor(
    private intlService: IntlService,
    private quotationService: QuotationService,
    private router: Router,
    private activeRoute: ActivatedRoute,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loading = true;
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.activeRoute.parent.params.subscribe(
      params => {
        this.partnerId = params.id;
        this.loadDataFromApi();
      }
    )
    // this.getPaged();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new QuotationsPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.partnerId ? this.partnerId : '';
    val.search = this.search ? this.search : '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.quotationService.getPaged(val).pipe(
      map(res => (<GridDataResult>{
        data: res.items,
        total: res.totalItems
      }))
    ).subscribe(
      result => {
        this.gridData = result;
        this.loading = false
      }
    )
  }

  createQuotation() {
    this.router.navigate(['quotations/form'], { queryParams: { partner_id: this.partnerId } });
  }

  editQuotation(item) {
    this.router.navigate(['quotations/form'], { queryParams: { id: item.id } });
  }

  deleteQuotation(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'X??a b??o gi??';
    modalRef.componentInstance.body = 'B???n c?? ch???c ch???n mu???n x??a b??o gi???';
    modalRef.result.then(() => {
      if (item.orders.length == 0) {
        this.quotationService.delete([item.id]).subscribe(
          () => {
            this.notify("success", "X??a th??nh c??ng");
            this.loadDataFromApi();
          }
        )
      }
      else {
        this.notify("error", "B???n kh??ng th??? x??a b??o gi?? ???? t???o phi???u ??i???u tr???");
      }
    });
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

  pageChange(event) {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  GetState(dateEndQuotation ) {
    var dateTo = new Date(dateEndQuotation);
    var today = new Date();
    if ( today <= dateTo) {
      return "C??n h???n";
    }   
    else {
      return "H???t h???n";
    }

  }
}
