import { AccountPaymentPaged } from './../../account-payments/account-payment.service';
import { Component, Inject, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { HistoryPartnerAdvanceFilter, SaleOrderPaymentPaged, SaleOrderPaymentService } from 'src/app/core/services/sale-order-payment.service';
import { PartnerService } from 'src/app/partners/partner.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-partner-advance-history-list',
  templateUrl: './partner-advance-history-list.component.html',
  styleUrls: ['./partner-advance-history-list.component.css']
})
export class PartnerAdvanceHistoryListComponent implements OnInit {
  partnerId: string;
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  
  search: string;
  limit = 20;
  offset = 0;
  pagerSettings: any;
  edit = false;
  dateFrom: Date;
  dateTo: Date;
  loading = false;
  
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor( private intlService: IntlService,
    private modalService: NgbModal,
    private partnerService: PartnerService,
    private router: Router,
    private route: ActivatedRoute,
    private notificationService: NotificationService,
    private saleOrderPaymentService: SaleOrderPaymentService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.partnerId = this.route.parent.parent.snapshot.paramMap.get('id');
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.offset = 0;
        this.loadDataFromApi();
      });


    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var paged = new HistoryPartnerAdvanceFilter();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.partnerId = this.partnerId;
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.saleOrderPaymentService.getHistoryPartnerAdvance(paged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;

      this.loading = false;
    }, err => {
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.offset = 0;
    this.loadDataFromApi();
  }

  getFormSaleOrder(id){
    this.router.navigate(['/sale-orders/form'], { queryParams: { id: id } });
  }

}
