import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-toa-thuoc-print',
  templateUrl: './toa-thuoc-print.component.html',
  styleUrls: ['./toa-thuoc-print.component.css']
})
export class ToaThuocPrintComponent implements OnInit {
  toaThuocPrint: any;
  constructor() { }

  ngOnInit() {
  }

  print(item: any) {
    this.toaThuocPrint = item;
    setTimeout(() => {
      var printContents = document.getElementById('printToaThuocDiv').innerHTML;
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
      this.toaThuocPrint = null;
    }, 500);
  }
}
