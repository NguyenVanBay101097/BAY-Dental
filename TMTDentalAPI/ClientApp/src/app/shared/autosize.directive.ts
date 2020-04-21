import { Directive, ElementRef, AfterViewInit } from '@angular/core';
declare var autosize: any;

@Directive({
    selector: '[my-autosize]',
})

export class MyAutosizeDirective implements AfterViewInit {
    constructor(private elementRef: ElementRef) { }

    ngAfterViewInit(): void {
        autosize(this.elementRef.nativeElement);
    }
}
