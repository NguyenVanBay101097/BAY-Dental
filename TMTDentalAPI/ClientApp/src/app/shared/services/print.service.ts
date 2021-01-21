import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PrintService {
  public renderer: Renderer2;

  constructor(rendererFactory: RendererFactory2) {
    this.renderer = rendererFactory.createRenderer(null, null);
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

  printHtml(html: string) {
    var body = document.querySelector('body');
    var iframe = this.renderer.createElement("iframe");
    this.renderer.setStyle(iframe, "visibility", "hidden");
    //gán vào body
    this.renderer.appendChild(body, iframe);
    iframe.onload = function () {
      iframe.contentWindow.print();
      // setTimeout(function() { iframe.remove(); }, 0);
    };
    var doc = iframe.contentWindow.document.open("text/html", "replace");
    doc.write(html);
    doc.close();
  }

  print2(html) {
    var popupWin = window.open('', '_blank', 'top=0,left=0,height=auto,width=auto');
    popupWin.document.open();
    popupWin.document.write(html);
    popupWin.document.close();
  }
}
