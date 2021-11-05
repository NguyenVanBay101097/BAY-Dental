import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import {
  DotKhamDisplay
} from 'src/app/dot-khams/dot-khams';
import { LaboOrderQuickCreateDialogComponent } from 'src/app/labo-orders/labo-order-quick-create-dialog/labo-order-quick-create-dialog.component';
import {
  LaboOrderDisplay, LaboOrderService
} from 'src/app/labo-orders/labo-order.service';
import { SaleOrderCreateDotKhamDialogComponent } from 'src/app/sale-orders/sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { DotKhamCreateUpdateDialogComponent } from 'src/app/shared/dot-kham-create-update-dialog/dot-kham-create-update-dialog.component';
import { SaleOrderLineDialogComponent } from 'src/app/shared/sale-order-line-dialog/sale-order-line-dialog.component';

@Component({
  selector: 'app-partner-customer-treatment-payment-detail',
  templateUrl: './partner-customer-treatment-payment-detail.component.html',
  styleUrls: ['./partner-customer-treatment-payment-detail.component.css']
})
export class PartnerCustomerTreatmentPaymentDetailComponent implements OnInit {
  saleOrderId: string;
  listSaleOrderLines: SaleOrderLineDisplay[] = [];
  listTreatments: DotKhamDisplay[] = [];
  listLabos: LaboOrderDisplay[] = [];
  partner: any;
  constructor(
    private saleOrder: SaleOrderService,
    private modalService: NgbModal,
    private dotkhamService: DotKhamService,
    private laboOrderService: LaboOrderService,
  ) { }

  ngOnInit() {
    if (this.saleOrderId) {
      this.loadService();
      this.loadTreatment()
      this.loadLabo();
    }
    console.log(this.partner);
  }

  loadService() {
    this.saleOrder.getServiceBySaleOrderId(this.saleOrderId).subscribe(
      result => {
        this.listSaleOrderLines = result;
        console.log('saleOrderline', result);

      }
    )
  }
  loadTreatment() {
    this.saleOrder.getTreatmentBySaleOrderId(this.saleOrderId).subscribe(
      result => {
        this.listTreatments = result
        console.log('treatment', result);

      }
    )
  }

  loadLabo() {
    this.saleOrder.getLaboBySaleOrderId(this.saleOrderId).subscribe(
      result => {
        this.listLabos = result;
        console.log('labo', result);
      }
    )
  }

  addSaleOrderLine() {
    let modalRef = this.modalService.open(SaleOrderLineDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.saleOrderId = this.saleOrderId;
    modalRef.result.then(result => {
      if (result) {

      }
    })
  }



  addTreatment() {
    let modalRef = this.modalService.open(SaleOrderCreateDotKhamDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Tạo đợt khám';
    modalRef.componentInstance.saleOrderId = this.saleOrderId;
    modalRef.componentInstance.partner = this.partner;
    modalRef.result.then(res => {
      this.loadTreatment();
    });
  }

  viewTreatment(id) {
    let modalRef = this.modalService.open(DotKhamCreateUpdateDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Chi tiết đợt khám';
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.partner = this.partner;
    modalRef.result.then(res => {
      this.loadTreatment();
    });
  }

  deleteTreatment(id) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu điều trị';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.dotkhamService.delete(id).subscribe(() => {
        this.loadTreatment();
      });
    });
  }

  addLabo() {
    let modalRef = this.modalService.open(LaboOrderQuickCreateDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Tạo phiếu Labo';
    modalRef.componentInstance.saleOrderId = this.saleOrderId;
    modalRef.result.then(() => {
      this.loadLabo();
    });
  }

  clickLabo(id) {
    // let modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    // modalRef.componentInstance.title = 'Chi tiết phiếu Labo';
    // if (id) {
    //   modalRef.componentInstance.id = id;
    // }
    // modalRef.componentInstance.saleOrderId = this.saleOrderId;
    // modalRef.result.then(() => {
    //   this.loadLabo();
    // });
  }

  deleteLabo(id) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu labo';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.laboOrderService.unlink([id]).subscribe(() => {
        this.loadLabo();
      });
    });
  }


  get totalLabo() {
    if (this.listLabos) {
      var total = 0;
      this.listLabos.forEach(item => {
        total = total + item.amountTotal;
      });
      return total;
    } else { return 0; }
  }

  get totalSaleOrderLine() {
    if (this.listSaleOrderLines) {
      var total = 0;
      this.listSaleOrderLines.forEach(item => {
        total = total + item.priceUnit;
      });
      return total;
    } else { return 0; }
  }

  get sumary() {
    return this.totalLabo + this.totalSaleOrderLine;
  }

}
