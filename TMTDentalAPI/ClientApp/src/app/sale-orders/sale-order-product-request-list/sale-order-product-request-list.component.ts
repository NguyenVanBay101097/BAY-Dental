import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { map } from 'rxjs/operators';
import { ProductRequestService } from 'src/app/shared/product-request.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductRequestPaged } from '../product-request';
import { SaleOrderProductRequestDialogComponent } from '../sale-order-product-request-dialog/sale-order-product-request-dialog.component';

@Component({
  selector: 'app-sale-order-product-request-list',
  templateUrl: './sale-order-product-request-list.component.html',
  styleUrls: ['./sale-order-product-request-list.component.css']
})
export class SaleOrderProductRequestListComponent implements OnInit {
  
  @Input() saleOrderId: string;
  @Input() saleOrderState: string;
  
  loading = false;
  productRequests: any = [];

  states: any[] = [
    { value: 'draft', text: 'Nháp'},
    { value: 'confirmed', text: 'Đang yêu cầu'},
    { value: 'done', text: 'Đã xuất'},
  ]

  constructor(
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private productRequestService: ProductRequestService
  ) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    if (!this.saleOrderId) {
      return;
    }

    const val = new ProductRequestPaged();
    val.saleOrderId = this.saleOrderId;
    val.offset = 0;
    val.limit = 0;
    
    this.productRequestService.getPaged(val).subscribe(res => {
      console.log(res);
      this.productRequests = res.items;
      console.log(this.productRequests);
    }, err => {
      console.log(err);
    })
  }

  createItem() {
    let modalRef = this.modalService.open(SaleOrderProductRequestDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Yêu cầu vật tư';
    modalRef.componentInstance.saleOrderId = this.saleOrderId;
    modalRef.result.then((result) => {
      this.loadData();
    }, () => {
    });
  }

  editItem(item) {
    let modalRef = this.modalService.open(SaleOrderProductRequestDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Yêu cầu vật tư';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.saleOrderId = this.saleOrderId;
    modalRef.result.then((result) => {
      // this.notify('success','Lưu thành công');
      this.loadData();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa yêu cầu vật tư';
    modalRef.componentInstance.body = 'Bạn có chắc chắn xóa yêu cầu vật tư?';
    modalRef.result.then(() => {
      this.productRequestService.delete(item.id).subscribe(() => {
      this.notify('success','Xóa thành công');
        this.loadData();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

  notify(type, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: type, icon: true }
    });
  }

  getStateDisplay(state) {
    switch (state) {
      case 'draft':
        return 'Nháp';
      case 'confirmed':
        return 'Đang yêu cầu';
      case 'done':
        return 'Đã xuất';
      default:
        return 'Nháp';
    }
  }
}
