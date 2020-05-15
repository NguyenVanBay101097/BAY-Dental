import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[appSharedComponent]'
})
export class SharedComponentDirective {

  constructor(public viewContainerRef: ViewContainerRef) { }
}
