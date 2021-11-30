import { Component, forwardRef, Input, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-number-input',
  templateUrl: './number-input.component.html',
  styleUrls: ['./number-input.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => NumberInputComponent),
      multi: true
    }
  ]
})
export class NumberInputComponent implements OnInit , ControlValueAccessor {
  options: any = {};
  userInputValue: number;
  @Input() max: number;
  constructor() {

  }
  ngOnInit(): void {
    // if (this.max) {
    //   this.options.maximumValue = this.max;
    // }
  }

  propagateChange: any = () => {};

  get inputValue() {
    return this.userInputValue;
  }

  set inputValue(value) {
    value = Number(value);
    if (this.max && value > this.max) {
      this.userInputValue = this.max;
      this.propagateChange(this.max);
    }
    else {
      this.userInputValue = Number(value);
      this.propagateChange(Number(value));
    }
  }

  writeValue(value: any): void {
    if (value) {
      this.inputValue = value;
    }
  }

  registerOnChange(fn: any): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: any): void {
  }

  setDisabledState?(isDisabled: boolean): void {
  }
}
