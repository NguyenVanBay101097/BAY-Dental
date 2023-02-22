import { Directive, OnInit, ElementRef, Renderer2, Input } from "@angular/core";
import { NgControl } from '@angular/forms';

@Directive({
    selector: '[charCount]'
})

export class CharCountDirective implements OnInit {
    @Input() limit: number;
    countSpan: any;

    constructor(private elementRef: ElementRef, private renderer: Renderer2, private ngControl: NgControl) {
    }

    ngOnInit() {
        this.limit = this.limit || 100;
        this.countSpan = this.renderer.createElement('span');
        this.renderer.addClass(this.countSpan, 'char-count');

        var countText = this.renderer.createText(this.limit.toString());
        this.renderer.appendChild(this.countSpan, countText);

        this.renderer.appendChild(this.elementRef.nativeElement.parentElement, this.countSpan);

        this.ngControl.valueChanges.subscribe((val: string) => {
            if (val && val.length > this.limit) {
                var newVal = val.substr(0, this.limit);
                this.ngControl.control.setValue(newVal);
                this.renderTextCount(newVal);
            } else {
                this.renderTextCount(val);
            }
        });
    }

    renderTextCount(val: string) {
        this.renderer.removeChild(this.elementRef.nativeElement.parentElement, this.countSpan);

        this.countSpan = this.renderer.createElement('span');
        this.renderer.addClass(this.countSpan, 'char-count');

        var remainCount = this.limit - (val ? val.length : 0);
        var countText = this.renderer.createText(remainCount + '');
        this.renderer.appendChild(this.countSpan, countText);

        this.renderer.appendChild(this.elementRef.nativeElement.parentElement, this.countSpan);
    }
}