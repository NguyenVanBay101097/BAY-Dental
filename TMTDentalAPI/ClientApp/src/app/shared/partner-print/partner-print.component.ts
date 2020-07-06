import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-partner-print',
  templateUrl: './partner-print.component.html',
  styleUrls: ['./partner-print.component.css']
})
export class PartnerPrintComponent implements OnInit {
  partnerPrint: any;

  constructor() { }

  ngOnInit() {
  }

  print(item: any) {
    console.log(item);
    this.partnerPrint = item;
    setTimeout(() => {
      var printContents = document.getElementById('printPartnerDiv').innerHTML;
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
      this.partnerPrint = null;
    }, 500);
  }

  getTeethDisplay(teeth) {
    return teeth.map(x => 'R' + x).join(', ');
  }
}
