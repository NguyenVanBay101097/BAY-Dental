import { Injectable } from '@angular/core';
declare var $: any;

@Injectable({ providedIn: 'root' })
export class PrintService {

  public print(html) {
    debugger;
    var hiddenFrame = $('<iframe style="visibility: hidden"></iframe>').appendTo('body')[0];
    hiddenFrame.onload = function () {
      hiddenFrame.contentWindow.print();
      $(hiddenFrame).remove();
    };
    var doc = hiddenFrame.contentWindow.document.open("text/html", "replace");
    doc.write(html);
    doc.close();
  }
}
