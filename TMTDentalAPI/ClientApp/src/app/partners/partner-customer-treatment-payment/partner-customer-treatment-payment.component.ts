// import { Component, OnInit, ViewChild, Renderer2 } from '@angular/core';
// import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/account-invoices/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
// import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
// import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
// import { PageChangeEvent, GridComponent } from '@progress/kendo-angular-grid';
// import { SaleOrderPaged, SaleOrderService } from 'src/app/sale-orders/sale-order.service';
// import { IntlService } from '@progress/kendo-angular-intl';
// import { Router, ActivatedRoute } from '@angular/router';
// import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
// import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
// import { NotificationService } from '@progress/kendo-angular-notification';
// import { PartnerService } from '../partner.service';
// import { PartnerDisplay } from '../partner-simple';
// import { RedirectComponentComponent } from 'src/app/shared/redirect-component/redirect-component.component';
// import { SharedService } from 'src/app/shared/shared.service';
// import { PartnerCustomerTreatmentPaymentDetailComponent } from '../partner-customer-treatment-payment-detail/partner-customer-treatment-payment-detail.component';

// @Component({
//   selector: 'app-partner-customer-treatment-payment',
//   templateUrl: './partner-customer-treatment-payment.component.html',
//   styleUrls: ['./partner-customer-treatment-payment.component.css']
// })
// export class PartnerCustomerTreatmentPaymentComponent implements OnInit {


//   // @ViewChild(PartnerCustomerTreatmentPaymentDetailComponent, { static: true }) cmp: PartnerCustomerTreatmentPaymentDetailComponent;
//   limit = 20;
//   id: string;
//   skip = 0;
//   customerInfo: PartnerDisplay;
//   listSaleOrder: SaleOrderBasic[] = []
//   selectedIds: string[] = [];
//   show = false;
//   constructor(private saleOrderService: SaleOrderService,
//     private router: Router,
//     private partnerService: PartnerService,
//     private modalService: NgbModal,
//     private paymentService: AccountPaymentService,
//     private notificationService: NotificationService,
//     private activeRoute: ActivatedRoute,
//     private renderer2: Renderer2,
//     private sharedService: SharedService
//   ) { }

//   ngOnInit() {
//     this.id = this.activeRoute.snapshot['_routerState']._root.children[0].value.params.id
//     this.loadDataFromApi();
//     this.loadCustomerInfo();
//   }

//   loadDataFromApi() {
//     var val = new SaleOrderPaged();
//     val.limit = this.limit;
//     val.offset = this.skip;
//     val.partnerId = this.id;
//     this.saleOrderService.getPaged(val).subscribe(res => {
//       this.listSaleOrder = res.items;
//       console.log(res);
//     })
//   }

//   loadCustomerInfo() {
//     this.partnerService.getPartner(this.id).subscribe(result => {
//       this.customerInfo = new PartnerDisplay();
//       this.customerInfo = result;
//       console.log(this.customerInfo);
//     })
//   }

//   createItem() {
//     this.router.navigate(['/sale-orders/form'], { queryParams: { partner_id: this.customerInfo.id } });
//   }

//   deleteItem(item) {
//     let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
//     modalRef.componentInstance.title = 'Xóa phiếu điều trị';
//     modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
//     modalRef.result.then(() => {
//       this.saleOrderService.unlink([item.id]).subscribe(() => {
//         this.loadDataFromApi();
//       });
//     });
//   }



// }


import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { GridDataResult, PageChangeEvent, GridComponent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { NgbDate, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserSimple } from 'src/app/users/user-simple';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/account-invoices/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { SaleOrderService, SaleOrderPaged } from 'src/app/sale-orders/sale-order.service';

@Component({
  selector: 'app-partner-customer-treatment-payment',
  templateUrl: './partner-customer-treatment-payment.component.html',
  styleUrls: ['./partner-customer-treatment-payment.component.css']
})
export class PartnerCustomerTreatmentPaymentComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  searchInvoiceNumber: string;
  searchPartnerNamePhone: string;
  search: string;
  dateOrderFrom: Date;
  dateOrderTo: Date;
  stateFilter: string;
  searchUpdate = new Subject<string>();
  searchStates: string[] = [];
  searchUser: UserSimple;

  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Tất cả', value: '' },
    { text: 'Đã xác nhận', value: 'sale,done' },
    { text: 'Nháp', value: 'draft,cancel' }
  ];

  selectedIds: string[] = [];

  partnerId: string;

  constructor(private saleOrderService: SaleOrderService, private intlService: IntlService, private router: Router, private dialogService: DialogService,
    private modalService: NgbModal, private paymentService: AccountPaymentService, private notificationService: NotificationService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.partnerId = this.route.parent.snapshot.paramMap.get('id');
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
  }

  stateGet(state) {
    switch (state) {
      case 'sale':
        return 'Đã xác nhận';
      case 'done':
        return 'Đã khóa';
      case 'cancel':
        return 'Đã hủy';
      default:
        return 'Nháp';
    }
  }

  onKey(event: any) { // without type info
    if (event.keyCode === 13) {
      this.loadDataFromApi();
    }
  }

  onDateSearchChange(data) {
    this.dateOrderFrom = data.dateFrom;
    this.dateOrderTo = data.dateTo;
    this.loadDataFromApi();
  }

  onStateSelectChange(data: TmtOptionSelect) {
    this.stateFilter = data.value;
    this.loadDataFromApi();
  }

  onChangeDateOrder(value: Date) {
    setTimeout(() => {
      this.loadDataFromApi();
    }, 200);
  }

  unlink() {
    if (this.selectedIds.length == 0) {
      return false;
    }

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu điều trị';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.saleOrderService.unlink(this.selectedIds).subscribe(() => {
        this.selectedIds = [];
        this.loadDataFromApi();
      });
    });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SaleOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.partnerId;
    val.isQuotation = false;

    this.saleOrderService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/sale-orders/form'], { queryParams: { partner_id: this.partnerId } });
  }

  editItem(item: any) {
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


