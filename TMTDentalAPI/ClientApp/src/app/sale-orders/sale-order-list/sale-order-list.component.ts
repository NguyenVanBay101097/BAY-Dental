import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { NgbDate, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserSimple } from 'src/app/users/user-simple';
import { AccountRegisterPaymentService, AccountRegisterPaymentDefaultGet } from 'src/app/account-payments/account-register-payment.service';
import { SaleOrderService, SaleOrderPaged } from '../sale-order.service';
import { SaleOrderBasic } from '../sale-order-basic';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';

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
    { text: 'Đơn hàng', value: 'sale,done' },
    { text: 'Nháp', value: 'draft,cancel' }
  ];

  selectedIds: string[] = [];

  constructor(private saleOrderService: SaleOrderService, private intlService: IntlService,
    private router: Router, private dialogService: DialogService,
    private modalService: NgbModal, private registerPaymentService: AccountRegisterPaymentService) { }

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
        return 'Đơn hàng';
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

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
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
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/sale-orders/create']);
  }

  editItem(item: SaleOrderBasic) {
    this.router.navigate(['/sale-orders/edit/', item.id]);
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu điều trị';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.saleOrderService.unlink([item.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}

