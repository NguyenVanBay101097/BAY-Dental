import { Component, Inject, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ServiceCardOrderPaged } from '../service-card-order-paged';
import { ServiceCardOrderService } from '../service-card-order.service';
import { Router } from '@angular/router';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-service-card-order-list',
  templateUrl: './service-card-order-list.component.html',
  styleUrls: ['./service-card-order-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})

export class ServiceCardOrderListComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  title = 'Đơn bán thẻ tiền mặt';

  // permission 
  canServiceCardOrderUpdate = this.checkPermissionService.check(["ServiceCard.Order.Update"]);
  canServiceCardOrderDelete = this.checkPermissionService.check(["ServiceCard.Order.Delete"]);

  constructor(private cardOrderService: ServiceCardOrderService,
    private modalService: NgbModal, private router: Router, 
    private checkPermissionService: CheckPermissionService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

      this.loadDataFromApi();
  }

  loadDataFromApi() {
   
    var val = new ServiceCardOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    this.loading = true;
    
    this.cardOrderService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/service-card-orders/form']);
  }

  posItem() {
    this.router.navigate(['/service-card-orders/pos']);
  }

  editItem(item: any) {
    this.router.navigate(['/service-card-orders/form'], { queryParams: { id: item.id } });
  }

  stateGet(state) {
    switch (state) {
      case 'sale':
        return 'Đã xác nhận';
      case 'done':
        return 'Đã khóa';
      case 'cancel':
        return 'Đã hủy';
      default:
        return 'Nháp';
    }
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.cardOrderService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}




