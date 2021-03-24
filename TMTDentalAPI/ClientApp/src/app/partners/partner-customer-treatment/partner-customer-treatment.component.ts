import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';

@Component({
  selector: 'app-partner-customer-treatment',
  templateUrl: './partner-customer-treatment.component.html',
  styleUrls: ['./partner-customer-treatment.component.css']
})
export class PartnerCustomerTreatmentComponent implements OnInit {
  partnerId : string;
  dateFrom : Date;
  dateTo : Date;
  skip : number = 0;
  limit : number = 10;
  search : string;
  saleOrdersData : GridDataResult;
  searchUpdate = new Subject<string>();
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor(
    private saleOrderService: SaleOrderService,
    private authService: AuthService,
    private activeRoute: ActivatedRoute,
    private router: Router,
    private intlService: IntlService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.activeRoute.parent.params.subscribe(
      params => {
        this.partnerId = params.id;
        this.getSaleOrders();
      }
    )
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.skip = 0;
        this.getSaleOrders();
      });
  }

  createNewSaleOrder(){
    this.router.navigate(['sale-orders/form'], { queryParams: { partner_id: this.partnerId } });
  }

  onDeleteSaleOrder(){
    this.getSaleOrders();
  }

  getSaleOrders() {
    this.loading = true;
    var val = new SaleOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search? this.search : '';
    val.companyId = this.authService.userInfo.companyId;
    val.partnerId = this.partnerId;
    val.dateOrderFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd")
    val.dateOrderTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59")
    this.saleOrderService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(
      result => {
        this.loading = true;
        this.saleOrdersData = result;
      }
    )
  }

  pageChange(skip){
    this.skip = skip;
    this.getSaleOrders();
  }

  onSearchDateChange(data){
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.getSaleOrders();
  }
}
