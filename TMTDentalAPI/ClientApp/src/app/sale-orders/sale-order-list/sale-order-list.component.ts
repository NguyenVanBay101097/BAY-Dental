import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/shared/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { UserSimple } from 'src/app/users/user-simple';
import { SaleOrderPaged, SaleOrderService } from '../../core/services/sale-order.service';
import { SaleOrderBasic } from '../sale-order-basic';

@Component({
  selector: 'app-sale-order-list',
  templateUrl: './sale-order-list.component.html',
  styleUrls: ['./sale-order-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleOrderListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
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

  constructor(private saleOrderService: SaleOrderService, 
    private intlService: IntlService, private router: Router, 
    private modalService: NgbModal, 
    private paymentService: AccountPaymentService, 
    private notificationService: NotificationService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
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
    val.search = this.search || '';
    val.isQuotation = false;
    if (this.dateOrderFrom) {
      val.dateOrderFrom = this.intlService.formatDate(this.dateOrderFrom, 'd', 'en-US');
    }
    if (this.dateOrderTo) {
      val.dateOrderTo = this.intlService.formatDate(this.dateOrderTo, 'd', 'en-US');
    }
    if (this.stateFilter) {
      val.state = this.stateFilter;
    }

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
    this.limit = event.take;
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
      let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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

