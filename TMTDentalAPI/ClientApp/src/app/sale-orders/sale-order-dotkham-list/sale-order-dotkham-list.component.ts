import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { DotKhamBasic } from 'src/app/dot-khams/dot-khams';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { DotKhamCreateUpdateDialogComponent } from 'src/app/shared/dot-kham-create-update-dialog/dot-kham-create-update-dialog.component';
import { SaleOrderCreateDotKhamDialogComponent } from '../sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';

@Component({
  selector: 'app-sale-order-dotkham-list',
  templateUrl: './sale-order-dotkham-list.component.html',
  styleUrls: ['./sale-order-dotkham-list.component.css']
})
export class SaleOrderDotkhamListComponent implements OnInit {
  @Input() saleOrder;

  dotKhams: DotKhamBasic[] = [];

  constructor(
    private saleOrderService: SaleOrderService,
    private modalService: NgbModal,
    private dotKhamService: DotKhamService
  ) { }

  ngOnInit() {
    this.loadDotKhamList();
  }

  loadDotKhamList() {
    if (this.saleOrder.Id) {
      return this.saleOrderService.getDotKhamList(this.saleOrder.Id).subscribe(result => {
        this.dotKhams = result;
      });
    }
  }

  actionCreateDotKham() {
    if (this.saleOrder.Id) {
      let modalRef = this.modalService.open(SaleOrderCreateDotKhamDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Tạo đợt khám';
      modalRef.componentInstance.saleOrderId = this.saleOrder.Id;

      modalRef.result.then(res => {
        if (res.view) {
          this.actionEditDotKham(res.result);
          this.loadDotKhamList();
        } else {
          this.loadDotKhamList();
          // $('#myTab a[href="#profile"]').tab('show');
        }
      }, () => {
      });
    }
  }

  actionEditDotKham(item) {
    let modalRef = this.modalService.open(DotKhamCreateUpdateDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật đợt khám';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.partnerId = this.saleOrder.Partner.Id;
    if (this.saleOrder.Partner)
      modalRef.componentInstance.partner = this.saleOrder.Partner;
    modalRef.result.then(() => {
      this.loadDotKhamList();
    }, () => {
    });
  }

  deleteDotKham(dotKham) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa đợt khám';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.dotKhamService.delete(dotKham.id).subscribe(() => {
        this.loadDotKhamList();
      });
    }, () => { });
  }


}
