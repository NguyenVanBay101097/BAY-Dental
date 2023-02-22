import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
    selector: '[anchorHost]'
})
export class AnchorHostDirective {
    constructor(public viewContainerRef: ViewContainerRef) { }
}