import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-sale-order-line-discount-popover',
  templateUrl: './sale-order-line-discount-popover.component.html',
  styleUrls: ['./sale-order-line-discount-popover.component.css']
})
export class SaleOrderLineDiscountPopoverComponent implements OnInit {
  @Input() discountForm: any;
  @Output() discountFormGroup = new EventEmitter<any>();
  @ViewChild('popOver', { static: true }) public popover: NgbPopover;
  formGroup: FormGroup;
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.loadFormDiscount();
  }

  loadFormDiscount() {
    var res = this.discountForm.value;
    if (res) {      
      this.formGroup = this.fb.group({
        discountType: res.discountType,
        discountPercent: res.discount,
        discountFixed: res.discountFixed,
      });
    }else{
      this.formGroup = this.fb.group({
        discountType: 'percentage',
        discountPercent: 0,
        discountFixed: 0,
      });
    }
  }

  get discountTypeValue() {
    return this.formGroup.get('discountType').value;
  }

  get discountValue() {
    return this.formGroup.get('discountPercent').value;
  }

  get discountFixedValue() {
    return this.formGroup.get('discountFixed').value;
  }

  get valueButton() {
    var res = this.formGroup.value;
    if (res.discountType == 'percentage') {
      return res.discountPercent + '' + '%';
    } else {
      return res.discountFixed;
    }
  }

  applyDiscount() {
    this.discountFormGroup.emit(this.formGroup.value);
    this.popover.close();
    this.resetForm(this.formGroup.value);
  }

  onChangeDiscount() {
    this.discountFormGroup.emit(this.formGroup.value);
  }

  onChangeDiscountType() {
    var res = this.formGroup.value;
    res.discountFixed = 0;
    res.discountPercent = 0;
    this.discountFormGroup.emit(this.formGroup.value);
    this.resetForm(this.formGroup.value);
  }

  resetForm(value) {
    this.formGroup = this.fb.group({
      discountType: value.discountType,
      discountPercent: 0,
      discountFixed: 0,
    });
  }

}
