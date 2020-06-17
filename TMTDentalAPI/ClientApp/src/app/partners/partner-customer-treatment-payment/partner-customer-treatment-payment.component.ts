import { Component, OnInit, ViewChild } from '@angular/core';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/account-invoices/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { PageChangeEvent, GridComponent } from '@progress/kendo-angular-grid';
import { SaleOrderPaged, SaleOrderService } from 'src/app/sale-orders/sale-order.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { PartnerService } from '../partner.service';
import { PartnerDisplay } from '../partner-simple';

@Component({
  selector: 'app-partner-customer-treatment-payment',
  templateUrl: './partner-customer-treatment-payment.component.html',
  styleUrls: ['./partner-customer-treatment-payment.component.css']
})
export class PartnerCustomerTreatmentPaymentComponent implements OnInit {

  @ViewChild(GridComponent, { static: false }) grid: GridComponent;
  limit = 20;
  id: string;
  skip = 0;
  customerInfo: PartnerDisplay;
  listSaleOrder: SaleOrderBasic[] = []
  selectedIds: string[] = [];

  constructor(private saleOrderService: SaleOrderService,
    private router: Router,
    private partnerService: PartnerService,
    private modalService: NgbModal,
    private paymentService: AccountPaymentService,
    private notificationService: NotificationService,
    private activeRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.customerInfo = new PartnerDisplay();
    this.id = this.activeRoute.snapshot['_routerState']._root.children[0].value.params.id
    this.loadDataFromApi();
    this.loadCustomerInfo();
  }

  loadDataFromApi() {
    var val = new SaleOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.id;
    this.saleOrderService.getPaged(val).subscribe(res => {
      this.listSaleOrder = res.items;
      console.log(res);
    })
  }

  loadCustomerInfo() {
    this.partnerService.getPartner(this.id).subscribe(result => {
      this.customerInfo = result;
      console.log(this.customerInfo);
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/sale-orders/form']);
  }

  editItem(item: SaleOrderBasic) {
    this.router.navigate(['/sale-orders/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu điều trị';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.saleOrderService.unlink([item.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }

  actionPayment() {
    if (this.selectedIds.length == 0) {
      return false;
    }
    this.paymentService.saleDefaultGet(this.selectedIds).subscribe(rs2 => {
      let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Thanh toán';
      modalRef.componentInstance.defaultVal = rs2;
      modalRef.result.then(() => {
        this.notificationService.show({
          content: 'Thanh toán thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadDataFromApi();
      }, () => {
      });
    })
  }

}
