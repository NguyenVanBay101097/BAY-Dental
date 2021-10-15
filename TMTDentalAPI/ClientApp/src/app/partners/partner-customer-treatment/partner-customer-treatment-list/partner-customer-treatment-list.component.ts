import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-partner-customer-treatment-list',
  templateUrl: './partner-customer-treatment-list.component.html',
  styleUrls: ['./partner-customer-treatment-list.component.css']
})
export class PartnerCustomerTreatmentListComponent implements OnInit {

  partnerId: string;
  dateFrom: Date;
  dateTo: Date;
  skip: number = 0;
  limit: number = 20;
  pagerSettings: any;
  search: string;
  saleOrdersData: GridDataResult;
  searchUpdate = new Subject<string>();
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor(
    private saleOrderService: SaleOrderService,
    private authService: AuthService,
    private activeRoute: ActivatedRoute,
    private router: Router,
    private intlService: IntlService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loading = true;
    this.activeRoute.parent.params.subscribe(
      params => {
        this.partnerId = params.id;
        console.log(this.partnerId);
        
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
    this.saleOrderService.defaultGet({partnerId: this.partnerId}).subscribe(result => {
      var dateOrder = new Date(result.dateOrder);
      var val = {
        partnerId: this.partnerId,
        companyId: result.companyId,
        dateOrder: this.intlService.formatDate(dateOrder, 'yyyy-MM-ddTHH:mm:ss')
      };
      this.saleOrderService.create(val).subscribe(result2 => {
        this.router.navigate(['sale-orders/' + result2.id]);
      });
    });
  }

  onDeleteSaleOrder(){
    this.getSaleOrders();
  }

  getFormSaleOrder(id){
    this.router.navigate(['/sale-orders/form'], { queryParams: { id: id } });
    this.router.navigate(['/sale-orders/' + id]);
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
        this.saleOrdersData = result;
        this.loading = false;
      }
    )
  }

  viewSaleOrder(id){
    this.router.navigate(['/sale-orders/form'], { queryParams: { id: id } });
  }

  pageChange(event){
    this.skip = event.skip;
    this.limit = event.take;
    this.getSaleOrders();
  }

  onSearchDateChange(data){
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.getSaleOrders();
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu điều trị';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa phiếu điều trị?';
    modalRef.result.then(() => {
      this.saleOrderService.unlink([item.id]).subscribe(() => {
        this.notify("success","Xóa thành công");
        this.getSaleOrders();
      });
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

}
