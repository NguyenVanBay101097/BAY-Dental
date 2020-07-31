import { Component, OnInit } from '@angular/core';
import { LaboOrderPaged, LaboOrderService, LaboOrderBasic } from '../labo-order.service';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LaboOrderCuDialogComponent } from '../labo-order-cu-dialog/labo-order-cu-dialog.component';
import { SaleOrderLineDisplay } from '../../sale-orders/sale-order-line-display';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-labo-order-list-dialog',
  templateUrl: './labo-order-list-dialog.component.html',
  styleUrls: ['./labo-order-list-dialog.component.css']
})
export class LaboOrderListDialogComponent implements OnInit {

  constructor(
    private laboOrderService: LaboOrderService,
    public activeModal: NgbActiveModal,
    public modalService: NgbModal,
    private showErrorService: AppSharedShowErrorService
  ) { }

  saleOrderLineId: string;
  saleOrderId: string;
  saleOrderLine: SaleOrderLineDisplay;

  laboOrders: LaboOrderBasic[] = [];

  ngOnInit() {
    setTimeout(() => {
      this.LoadLaboOrderList();
    });
  }

  LoadLaboOrderList() {
    const val = new LaboOrderPaged();
    val.saleOrderLineId = this.saleOrderLineId;
    return this.laboOrderService.GetFromSaleOrder_OrderLine(val).subscribe(result => {
      this.laboOrders = result.items;
    });
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  actionLabo(item?) {
    if (this.saleOrderId) {
      const modalRef = this.modalService.open(LaboOrderCuDialogComponent,
        { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      if (item && item.id) {
        modalRef.componentInstance.title = 'Cập nhật phiếu labo';
        modalRef.componentInstance.id = item.id;
      } else {
        modalRef.componentInstance.title = 'Tạo phiếu labo';
      }

      modalRef.componentInstance.saleOrderId = this.saleOrderId;
      modalRef.componentInstance.saleOrderLineId = this.saleOrderLineId;
      modalRef.componentInstance.saleOrderLine = this.saleOrderLine;


      modalRef.result.then(res => {
        if (res) {
          this.LoadLaboOrderList();
        }
      }, () => {
      });
    }
  }

  deleteLabo(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa Labo';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.laboOrderService.delete(item.id).subscribe(res => {
        this.LoadLaboOrderList();
      }, (err) => {
        this.showErrorService.show(err);
      });
    });
  }
}
