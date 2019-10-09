import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AccountInvoiceService, AccountInvoicePaged, AccountInvoiceBasic } from '../account-invoice.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { NgbDate, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-customer-invoice-list',
  templateUrl: './customer-invoice-list.component.html',
  styleUrls: ['./customer-invoice-list.component.css']
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

  constructor(private accountInvoiceService: AccountInvoiceService, private intlService: IntlService,
    private router: Router, private dialogService: DialogService, private parserFormatter: NgbDateParserFormatter) { }

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

  onSearchChange(data) {
    if (data.dateOrderFrom) {
      this.dateOrderFrom = new Date(data.dateOrderFrom.year, data.dateOrderFrom.month - 1, data.dateOrderFrom.day);
    } else {
      this.dateOrderFrom = null;
    }

    if (data.dateOrderTo) {
      this.dateOrderTo = new Date(data.dateOrderTo.year, data.dateOrderTo.month - 1, data.dateOrderTo.day);
    } else {
      this.dateOrderTo = null;
    }

    this.loadDataFromApi();
  }

}
