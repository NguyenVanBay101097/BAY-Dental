import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root'
})
export class PrintService {
    isPrinting = false;

    constructor(private router: Router) { }

    printDocument(documentName: string, documentData: string[]) {
        this.isPrinting = true;
        this.router.navigate(['/',
            {
                outlets: {
                    'print': ['print', documentName, documentData.join()]
                }
            }]);
    }

    onDataReady() {
        setTimeout(() => {
            window.print();
            this.isPrinting = false;
            this.router.navigate([{ outlets: { print: null } }]);
        });
    }

    print(html) {
        var popupWin = window.open('', '_blank', 'top=0,left=0,height=auto,width=auto');
        popupWin.document.open();
        popupWin.document.write(`
              <html>
                <head>
                  <title>Print tab</title>
                  <link rel="stylesheet" type="text/css" href="/assets/css/bootstrap.min.css" />
                  <link rel="stylesheet" type="text/css" href="/assets/css/print.css" />
                </head>
              <body>
              ${html}
              <script defer>
                function triggerPrint(event) {
                  window.removeEventListener('load', triggerPrint, false);
                  setTimeout(function() {
                    window.print();
                    setTimeout(function() { window.close(); }, 0);
                  }, 0);
                }
                window.addEventListener('load', triggerPrint, false);
              </script>
              </body>
              </html>`
        );
        popupWin.document.close();
    }
}
