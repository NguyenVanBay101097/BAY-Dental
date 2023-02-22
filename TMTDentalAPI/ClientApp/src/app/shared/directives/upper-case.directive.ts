import { Directive, ElementRef, HostListener } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
  selector: 'input[upperCase]'
})
export class UpperCaseDirective {

  constructor(private el: ElementRef,private control: NgControl) {

  }

  ngOnInit() {
    var val = this.el.nativeElement.value;
    this.el.nativeElement.value = val.toUpperCase();
  }

  @HostListener('ngModelChange')
  @HostListener('input')
   input() {
    var val = this.el.nativeElement.value;
    if (val == val.toUpperCase()) {
      return;
    } else {
      this.el.nativeElement.value = val.toUpperCase();
      this.control.control.setValue(val.toUpperCase());
    }
  }
}
