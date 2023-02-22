import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-account-payment-print',
  templateUrl: './account-payment-print.component.html',
  styleUrls: ['./account-payment-print.component.css']
})
export class AccountPaymentPrintComponent implements OnInit {
  accountPaymentPrint: any;
  constructor() { }

  ngOnInit() {
  }

  print(item: any) {
    this.accountPaymentPrint = item;
    console.log(item);
    setTimeout(() => {
      var printContents = document.getElementById('printAccountPaymentDiv').innerHTML;
      var popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
      popupWin.document.open();
      popupWin.document.write(`
          <html>
            <head>
              <title>Print tab</title>
              <link rel="stylesheet" type="text/css" href="/assets/css/bootstrap.min.css" />
              <link rel="stylesheet" type="text/css" href="/assets/css/print.css" />
            </head>
        <body onload="window.print();window.close()">${printContents}</body>
          </html>`
      );
      popupWin.document.close();
      this.accountPaymentPrint = null;
    }, 500);
  }
}
