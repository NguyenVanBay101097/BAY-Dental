import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[appRedirectComponent]'
})
export class RedirectComponentDirective {

  constructor(public viewContainerRef: ViewContainerRef) { }

}
