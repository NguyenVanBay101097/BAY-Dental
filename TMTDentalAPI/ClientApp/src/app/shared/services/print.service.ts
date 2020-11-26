import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PrintService {
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

  print2(html) {
    var popupWin = window.open('', '_blank', 'top=0,left=0,height=auto,width=auto');
    popupWin.document.open();
    popupWin.document.write(html);
    popupWin.document.close();
  }
}
