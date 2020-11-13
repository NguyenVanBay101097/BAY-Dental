import { Component, Input, OnInit, SimpleChange } from '@angular/core';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';

@Component({
  selector: 'app-print-sale-order',
  templateUrl: './print-sale-order.component.html',
  styleUrls: ['./print-sale-order.component.css']
})
export class PrintSaleOrderComponent implements OnInit {
  @Input() saleOrderId;
  saleOrderPrint: any;
  constructor(private saleOrderService: SaleOrderService,) { }

  ngOnInit() {
    this.printFastSaleOrder(this.saleOrderId);
  }

  ngOnChanges(change: SimpleChange) {
    this.printFastSaleOrder(this.saleOrderId);
  }

  printFastSaleOrder(saleOrderId) {
    debugger
    if (saleOrderId) {
      this.saleOrderService.getPrint(saleOrderId).subscribe((result: any) => {
        this.saleOrderPrint = result;
        debugger
        setTimeout(() => {
          var printContents = document.getElementById('printSaleOrderDiv').innerHTML;
          var popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
          popupWin.document.open();
          popupWin.document.write(`
              <html>
                <head>
                  <title>Print tab</title>
                  <link rel="stylesheet" type="text/css" href="/assets/css/bootstrap.min.css" />
                  <link rel="stylesheet" type="text/css" href="/assets/css/print.css" />
                </head>
                 <body onload="window.print();">${printContents}</body>
              </html>`
          );
          popupWin.document.close();
          this.saleOrderPrint = null;
        });
      });
    }
  }

}
