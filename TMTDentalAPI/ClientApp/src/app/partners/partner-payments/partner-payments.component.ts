import { Component, OnInit, Input } from '@angular/core';
import { AccountPaymentPaged } from 'src/app/account-payments/account-payment.service';
import { PartnerService } from '../partner.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-partner-payments',
  templateUrl: './partner-payments.component.html',
  styleUrls: ['./partner-payments.component.css']
})
export class PartnerPaymentsComponent implements OnInit {

  @Input() public partnerId: string;//Id KH
  @Input() public saleOrderId: string;//Id phiếu điều trị
  gridLoading = false;
  gridView: GridDataResult;

  limit = 5;
  skip = 0;
  constructor(private service: PartnerService) { }

  ngOnInit() {
    this.loadPayments();
  }

  loadPayments() {
    this.gridLoading = true;
    var apPaged = new AccountPaymentPaged;
    apPaged.partnerId = this.partnerId ? this.partnerId : '';
    apPaged.saleOrderId = this.saleOrderId ? this.saleOrderId : '';
    apPaged.limit = this.limit;
    apPaged.offset = this.skip;
    this.service.getPayments(apPaged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      this.gridLoading = false;
    }, er => {
      this.gridLoading = true;
      console.log(er);
    }
    );
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadPayments();
  }

}
