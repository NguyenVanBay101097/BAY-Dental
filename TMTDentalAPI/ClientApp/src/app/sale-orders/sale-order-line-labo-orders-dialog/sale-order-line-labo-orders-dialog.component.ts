import { Component, ElementRef, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderLineDisplay } from '../../sale-orders/sale-order-line-display';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LaboOrderService, LaboOrderBasic, LaboOrderPaged } from 'src/app/labo-orders/labo-order.service';
import { SaleOrderLineService } from '../../core/services/sale-order-line.service';
import { LaboOrderCuDialogComponent } from 'src/app/shared/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { PrintService } from 'src/app/shared/services/print.service';
import { NavigationStart, Router, RouterEvent } from '@angular/router';
import { filter, take, tap } from 'rxjs/operators';

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
    private printService: PrintService,
    private router: Router,
  ) {

    // Close any opened dialog when route changes
    router.events.pipe(
      filter((event: RouterEvent) => event instanceof NavigationStart),
      tap(() => this.activeModal.dismiss()),
      take(1),
    ).subscribe(rs => {});
      }
  saleOrderLineId: string;
  laboOrders: LaboOrderBasic[] = [];
  title: string;

  ngOnInit() {
    setTimeout(() => {
      this.loadLaboOrderList();
    });
  }

  loadLaboOrderList() {
    const val = new LaboOrderPaged();
    val.saleOrderLineId = this.saleOrderLineId;
    val.limit = 2000;
    return this.laboOrderService.getPaged(val).subscribe((result: any) => {
      this.laboOrders = result.items;
    });
  }

  GetTeeth(val) {
    return val.teeth.map(x => x.name).join(',');
  }

  stateGet(state) {
    switch (state) {
      case 'confirmed':
        return 'Đơn hàng';
      default:
        return 'Nháp';
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  actionLabo(item?) {
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    if (item && item.id) {
      modalRef.componentInstance.title = 'Cập nhật phiếu Labo';
      modalRef.componentInstance.id = item.id;
    } else {
      modalRef.componentInstance.title = 'Tạo phiếu Labo';
    }

    modalRef.componentInstance.saleOrderLineId = this.saleOrderLineId;
    modalRef.result.then(res => {
      this.loadLaboOrderList();
    }, () => {
    });
  }

  printLabo(item: any) {
    this.laboOrderService.getPrint(item.id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

  deleteLabo(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu Labo';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa phiếu Labo?';
    modalRef.result.then(() => {
      this.laboOrderService.delete(item.id).subscribe(res => {
        this.loadLaboOrderList();
      }, (err) => {
      });
    }, () => { });
  }
}

