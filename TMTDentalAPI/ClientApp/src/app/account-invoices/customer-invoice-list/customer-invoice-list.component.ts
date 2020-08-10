import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AccountInvoiceService, AccountInvoicePaged, AccountInvoiceBasic } from '../account-invoice.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { NgbDate, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserSimple } from 'src/app/users/user-simple';
import { AccountInvoiceRegisterPaymentDialogComponent } from '../account-invoice-register-payment-dialog/account-invoice-register-payment-dialog.component';
import { AccountRegisterPaymentService, AccountRegisterPaymentDefaultGet } from 'src/app/account-payments/account-register-payment.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/shared/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';

@Component({
  selector: 'app-customer-invoice-list',
  templateUrl: './customer-invoice-list.component.html',
  styleUrls: ['./customer-invoice-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class CustomerInvoiceListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 10;
  skip = 0;
  loading = false;
  opened = false;
  searchInvoiceNumber: string;
  searchPartnerNamePhone: string;
  search: string;
  dateOrderFrom: Date;
  dateOrderTo: Date;
  searchUpdate = new Subject<string>();
  searchStates: string[] = [];
  searchUser: UserSimple;

  selectedIds: string[] = [];

  constructor(private accountInvoiceService: AccountInvoiceService, private intlService: IntlService,
    private router: Router, private dialogService: DialogService, private parserFormatter: NgbDateParserFormatter,
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
      case 'open':
        return 'Đã xác nhận';
      case 'paid':
        return 'Đã thanh toán';
      default:
        return 'Nháp';
    }
  }

  onKey(event: any) { // without type info
    if (event.keyCode === 13) {
      this.loadDataFromApi();
    }
  }

  onChangeDateOrder(value: Date) {
    setTimeout(() => {
      this.loadDataFromApi();
    }, 200);
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AccountInvoicePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.searchNumber = this.searchInvoiceNumber || '';
    val.search = this.search || '';
    val.searchPartnerNamePhone = this.searchPartnerNamePhone || '';
    val.dateOrderFrom = this.dateOrderFrom ? this.intlService.formatDate(this.dateOrderFrom, 'd', 'en-US') : '';
    val.dateOrderTo = this.dateOrderTo ? this.intlService.formatDate(this.dateOrderTo, 'd', 'en-US') : '';
    if (this.searchUser) {
      val.userId = this.searchUser.id;
    }
    if (this.searchStates.length) {
      val.state = this.searchStates.join(',');
    }

    this.accountInvoiceService.getPaged(val).pipe(
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

  registerPayment() {
    if (this.selectedIds.length == 0) {
      return false;
    }

    var val = new AccountRegisterPaymentDefaultGet();
    val.invoiceIds = this.selectedIds;
    this.registerPaymentService.defaultGet(val).subscribe(result => {
      let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Thanh toán';
      modalRef.componentInstance.defaultVal = result;
      modalRef.result.then(() => {
        this.selectedIds = [];
        this.loadDataFromApi();
      }, () => {
      });
    });
  }

  unlink() {
    if (this.selectedIds.length == 0) {
      return false;
    }

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa hóa đơn';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.accountInvoiceService.unlink(this.selectedIds).subscribe(() => {
        this.selectedIds = [];
        this.loadDataFromApi();
      });
    });
  }


  createItem() {
    this.router.navigate(['/customer-invoices/create']);
  }

  editItem(item: AccountInvoiceBasic) {
    this.router.navigate(['/customer-invoices/edit/', item.id]);
  }

  deleteItem(item) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa hóa đơn',
      content: 'Bạn có chắc chắn muốn xóa?',
      actions: [
        { text: 'Hủy bỏ', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ],
      width: 450,
      height: 200,
      minWidth: 250
    });

    dialog.result.subscribe((result) => {
      if (result instanceof DialogCloseResult) {
        console.log('close');
      } else {
        console.log('action', result);
        if (result['value']) {
          this.accountInvoiceService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }

  onAdvanceSearchChange(data) {
    this.dateOrderFrom = data.dateOrderFrom;
    this.dateOrderTo = data.dateOrderTo;
    var states = [];
    if (data.draftState) {
      states.push('draft');
    }

    if (data.openState) {
      states.push('open');
    }

    if (data.paidState) {
      states.push('paid');
    }

    if (data.cancelState) {
      states.push('cancel');
    }

    this.searchStates = states;
    this.searchUser = data.user;

    this.loadDataFromApi();
  }
}
