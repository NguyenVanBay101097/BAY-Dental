import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { ToaThuocCuDialogComponent } from 'src/app/toa-thuocs/toa-thuoc-cu-dialog/toa-thuoc-cu-dialog.component';
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
      // console.log(res);
      this.toathuocs = res.data;
    }, err => {
      console.log(err);
    })
  }

  printToaThuoc(item) {
    this.toaThuocService.getPrint(item.id).subscribe((result: any) => {
      console.log(result);
      this.printService.printHtml(result.html);
    });
  }

  editProductToaThuoc(item: any) {
    let modalRef = this.modalService.open(ToaThuocCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a: ????n Thu???c';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.partnerId = item.partnerId;
    modalRef.result.then((result) => {
      this.notify('success', 'L??u th??nh c??ng');
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
    modalRef.componentInstance.title = 'X??a: ????n Thu???c';
    modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n x??a ????n thu???c n??y?';
    modalRef.result.then(() => {
      this.toaThuocService.delete(item.id).subscribe(() => {
        this.notify('success', 'X??a th??nh c??ng');
        this.loadData();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
}
