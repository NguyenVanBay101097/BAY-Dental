import { Component, OnInit } from '@angular/core';
import { PrintService } from 'src/app/print.service';
import { AccountInvoiceService, AccountInvoiceDisplay, AccountInvoicePrint } from '../account-invoice.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-customer-invoice-print',
  templateUrl: './customer-invoice-print.component.html',
  styleUrls: ['./customer-invoice-print.component.css']
})
export class CustomerInvoicePrintComponent implements OnInit {
  invoice: AccountInvoicePrint;
  constructor(private accountInvoiceService: AccountInvoiceService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    let invoiceId = this.route.snapshot.params['id'];
    if (invoiceId) {
      this.accountInvoiceService.getPrint(invoiceId).subscribe(result => {
        console.log(result);
        this.invoice = result;
        setTimeout(() => {
          window.print();
        }, 1000);
      });
    }
  }

}
