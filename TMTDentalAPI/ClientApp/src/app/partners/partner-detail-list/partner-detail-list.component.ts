import { Component, OnInit, Input } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AccountInvoicePaged } from 'src/app/account-invoices/account-invoice.service';
import { PartnerService } from '../partner.service';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'app-partner-detail-list',
  templateUrl: './partner-detail-list.component.html',
  styleUrls: ['./partner-detail-list.component.css']
})
export class PartnerDetailListComponent implements OnInit {

  @Input() public id: string; // ID của khách hàng

  gridLoading = false;
  gridView: GridDataResult;

  limit = 5;
  skip = 0;

  constructor(private service: PartnerService, private router: Router) { }

  ngOnInit() {
    this.loadInvoices();
  }

  loadInvoices() {
    this.gridLoading = true;
    var val = new AccountInvoicePaged();
    val.limit = this.limit;
    val.offset = this.skip
    val.partnerId = this.id;
    this.service.getCustomerInvoices(val).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(
      rs2 => {
        this.gridView = rs2;
        this.gridLoading = false;
      }
    )
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadInvoices();
  }

  stateGet(state) {
    switch (state) {
      case 'open':
        return 'Đã xác nhận';
      case 'paid':
        return 'Đã thanh toán';
      case 'cancel':
        return 'Đã hủy';
      default:
        return 'Nháp';
    }
  }

  rowSelectionChange(e) {
    console.log(12);
    this.router.navigate([]).then(res => { window.open('/customer-invoices/edit/' + e.selectedRows[0].dataItem.id, '_blank'); });
  }

}
