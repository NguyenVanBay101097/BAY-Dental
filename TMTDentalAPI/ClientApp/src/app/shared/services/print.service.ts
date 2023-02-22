import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PrintService {
  public renderer: Renderer2;

  constructor(rendererFactory: RendererFactory2) {
    this.renderer = rendererFactory.createRenderer(null, null);
  }

  printHtml(html: string) {
    var body = document.querySelector('body');
    var iframe = this.renderer.createElement("iframe");
    this.renderer.setStyle(iframe, "visibility", "hidden");
    this.renderer.setStyle(iframe, "display", "none");
    //gán vào body
    this.renderer.appendChild(body, iframe);
    iframe.onload = function () {
      iframe.contentWindow.print();
      setTimeout(function () { iframe.remove(); }, 0);
    };
    var doc = iframe.contentWindow.document.open("text/html", "replace");
    doc.write(html);
    doc.close();
  }
}
