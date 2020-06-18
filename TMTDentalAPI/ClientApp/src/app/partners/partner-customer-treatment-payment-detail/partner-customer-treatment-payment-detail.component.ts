import { Component, OnInit, Input } from '@angular/core';
import { PartnerService, SaleOrderLineBasic } from '../partner.service';
import { SaleOrderService } from 'src/app/sale-orders/sale-order.service';
import { DotKhamBasic, DotKhamDisplay } from 'src/app/dot-khams/dot-khams';
import { LaboOrderBasic, LaboOrderDisplay } from 'src/app/labo-orders/labo-order.service';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderLineDialogComponent } from 'src/app/sale-orders/sale-order-line-dialog/sale-order-line-dialog.component';
import { SaleOrderCreateDotKhamDialogComponent } from 'src/app/sale-orders/sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { DotKhamCreateUpdateDialogComponent } from 'src/app/dot-khams/dot-kham-create-update-dialog/dot-kham-create-update-dialog.component';

@Component({
  selector: 'app-partner-customer-treatment-payment-detail',
  templateUrl: './partner-customer-treatment-payment-detail.component.html',
  styleUrls: ['./partner-customer-treatment-payment-detail.component.css']
})
export class PartnerCustomerTreatmentPaymentDetailComponent implements OnInit {
  @Input() saleOrderId: string;
  listSaleOrderLines: SaleOrderLineDisplay[] = [];
  listTreatments: DotKhamDisplay[] = [];
  listLabos: LaboOrderDisplay[] = [];

  constructor(
    private saleOrder: SaleOrderService,
    private modalService: NgbModal,
    private dotkhamService: DotKhamService
  ) { }

  ngOnInit() {
    if (this.saleOrderId) {
      this.loadService();
      this.loadTreatment()
      this.loadLabo();
    }

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
    let modalRef = this.modalService.open(SaleOrderLineDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.saleOrderId = this.saleOrderId;
    modalRef.result.then(result => {
      if (result) {

      }
    })
  }

  addTreatment() {
    let modalRef = this.modalService.open(SaleOrderCreateDotKhamDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Tạo đợt khám';
    modalRef.componentInstance.saleOrderId = this.saleOrderId;
    modalRef.result.then(res => {
      this.loadTreatment();
    });
  }

  viewTreatment(id) {
    let modalRef = this.modalService.open(DotKhamCreateUpdateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Chi tiết đợt khám';
    modalRef.componentInstance.idSend = id;
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



}
