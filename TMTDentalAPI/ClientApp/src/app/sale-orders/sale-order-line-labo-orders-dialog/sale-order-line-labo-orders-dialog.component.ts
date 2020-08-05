import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderLineDisplay } from '../../sale-orders/sale-order-line-display';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LaboOrderService, LaboOrderBasic, LaboOrderPaged } from 'src/app/labo-orders/labo-order.service';
import { LaboOrderCuDialogComponent } from 'src/app/labo-orders/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { SaleOrderLineService } from '../../core/services/sale-order-line.service';

@Component({
  selector: 'app-sale-order-line-labo-orders-dialog',
  templateUrl: './sale-order-line-labo-orders-dialog.component.html',
  styleUrls: ['./sale-order-line-labo-orders-dialog.component.css']
})
export class SaleOrderLineLaboOrdersDialogComponent implements OnInit {

  constructor(
    private laboOrderService: LaboOrderService,
    public activeModal: NgbActiveModal,
    public modalService: NgbModal,
    private showErrorService: AppSharedShowErrorService,
    private saleLineService: SaleOrderLineService
  ) { }

  saleOrderLineId: string;
  laboOrders: LaboOrderBasic[] = [];
  title: string;

  ngOnInit() {
    setTimeout(() => {
      this.loadLaboOrderList();
    });
  }

  loadLaboOrderList() {
    return this.saleLineService.getLaboOrders(this.saleOrderLineId).subscribe((result: any) => {
      this.laboOrders = result;
    });
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  actionLabo(item?) {
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    if (item && item.id) {
      modalRef.componentInstance.title = 'Cập nhật phiếu labo';
      modalRef.componentInstance.id = item.id;
    } else {
      modalRef.componentInstance.title = 'Tạo phiếu labo';
    }

    modalRef.componentInstance.saleOrderLineId = this.saleOrderLineId;
    modalRef.result.then(res => {
      if (res) {
        this.loadLaboOrderList();
      }
    }, () => {
    });
  }

  deleteLabo(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu labo';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.laboOrderService.delete(item.id).subscribe(res => {
        this.loadLaboOrderList();
      }, (err) => {
        this.showErrorService.show(err);
      });
    }, () => {});
  }
}

