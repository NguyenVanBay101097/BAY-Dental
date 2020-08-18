import { Component, OnInit, ViewChild, Input, Renderer2 } from '@angular/core';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { SharedService } from 'src/app/shared/shared.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';


@Component({
  selector: 'app-partner-customer-treatment-payment-child',
  templateUrl: './partner-customer-treatment-payment-child.component.html',
  styleUrls: ['./partner-customer-treatment-payment-child.component.css']
})
export class PartnerCustomerTreatmentPaymentChildComponent implements OnInit {

  @Input() saleOrder: SaleOrderBasic;
  @Input() partner: any;
  show = false;

  constructor(
    private renderer2: Renderer2,
    private sharedService: SharedService,
    private modalService: NgbModal,
    private saleOrderService: SaleOrderService

  ) { }

  ngOnInit() {
  }

  showTreatment(id) {
    var element = document.getElementById(id);
    document.getElementById(id + '_hide').classList.add('show')
    document.getElementById(id + '_show').classList.add('hide')
    document.getElementById(id + '_hide').classList.remove('hide')
    document.getElementById(id + '_show').classList.remove('show')
    this.renderer2.setStyle(element, 'height', 'auto');
    this.show = true;
  }

  hideTreatment(id) {
    document.getElementById(id + '_hide').classList.add('hide')
    document.getElementById(id + '_show').classList.add('show')
    document.getElementById(id + '_hide').classList.remove('show')
    document.getElementById(id + '_show').classList.remove('hide')
    var element = document.getElementById(id);
    this.renderer2.setStyle(element, 'height', '75px');
    this.show = false;
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu điều trị';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.saleOrderService.unlink([item.id]).subscribe(() => {
        this.saleOrder = null;
      });
    });
  }
}
