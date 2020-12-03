import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { ToaThuocCuDialogSaveComponent } from 'src/app/shared/toa-thuoc-cu-dialog-save/toa-thuoc-cu-dialog-save.component';
import { ToaThuocPaged, ToaThuocService } from 'src/app/toa-thuocs/toa-thuoc.service';

@Component({
  selector: 'app-partner-customer-toathuoc-list',
  templateUrl: './partner-customer-toathuoc-list.component.html',
  styleUrls: ['./partner-customer-toathuoc-list.component.css']
})
export class PartnerCustomerToathuocListComponent implements OnInit {
  @Input() saleOrderId: string;

  toathuocs;
  constructor(
    private toaThuocService: ToaThuocService,
    private printService: PrintService,
    private modalService: NgbModal,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.loadData();
  }
  loadData() {
    if (!this.saleOrderId) {
      return;
    }
    const val = new ToaThuocPaged();
    val.limit = 0;
    val.saleOrderId = this.saleOrderId;

    this.toaThuocService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.toathuocs = res.data;
    }, err => {
      console.log(err);
    })
  }

  printToaThuoc(item) {
    this.toaThuocService.getPrint(item.id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

  editProductToaThuoc(item: any) {
    let modalRef = this.modalService.open(ToaThuocCuDialogSaveComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: Đơn Thuốc';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.partnerId = item.partnerId;
    modalRef.result.then((result) => {
      this.notify('success','thành công');
      this.loadData();
      if (result.print) {
        this.printToaThuoc(item);
      }
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
  deleteProductToaThuoc(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: Đơn Thuốc';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa đơn thuốc này?';
    modalRef.result.then(() => {
      this.toaThuocService.delete(item.id).subscribe(() => {
      this.notify('success','thành công');
        this.loadData();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
}
