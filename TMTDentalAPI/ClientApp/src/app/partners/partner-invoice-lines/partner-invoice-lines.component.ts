import { Component, OnInit, Input } from '@angular/core';
import { AccountInvoiceLinePaged } from 'src/app/account-invoices/account-invoice-line-display';
import { PartnerService, SaleOrderLinePaged } from '../partner.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-partner-invoice-lines',
  templateUrl: './partner-invoice-lines.component.html',
  styleUrls: ['./partner-invoice-lines.component.css']
})
export class PartnerInvoiceLinesComponent implements OnInit {

  @Input() public id: string; // ID của khách hàng
  constructor(private service: PartnerService) { }

  gridView: GridDataResult;
  loading = false;
  skip = 0;
  pageSize = 20;
  gridLoading = false;

  ngOnInit() {
    this.loadSaleOrderLines();
  }

  loadInvoiceLines() {
    this.gridLoading = true;
    var ilPaged = new AccountInvoiceLinePaged;
    ilPaged.partnerId = this.id;
    this.service.getInvoiceLineByPartner(ilPaged).pipe(
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

  loadSaleOrderLines() {
    this.gridLoading = true;
    var slPaged = new SaleOrderLinePaged;
    slPaged.orderPartnerId = this.id;
    this.service.getSaleOrderLineByPartner(slPaged).pipe(
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
    this.pageSize = event.take;
    this.loadInvoiceLines();
  }

  lineTeeth(teeth: any[]) {
    return teeth.map(x => x.name).join(',');
  }
}
