import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-apply-discount-sale-order-line-popover',
  templateUrl: './apply-discount-sale-order-line-popover.component.html',
  styleUrls: ['./apply-discount-sale-order-line-popover.component.css']
})
export class ApplyDiscountSaleOrderLinePopoverComponent implements OnInit {

  @Output() discountFormGroup = new EventEmitter<any>();
  @ViewChild('popOver', { static: true }) public popover: any;
  formGroup: FormGroup;
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      discountType: 'percentage',
      discountPercent: 0,
      discountFixed: 0,
    });



  }

  toggle(popover) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      popover.open();
    }
  }

  get discountTypeValue() {
    return this.formGroup.get('discountType').value;
  }

  applyDiscount() {
    this.discountFormGroup.emit(this.formGroup.value);
    this.popover.close();
    this.resetForm();
  }

  resetForm() {
    this.formGroup = this.fb.group({
      discountType: 'percentage',
      discountPercent: 0,
      discountFixed: 0,
    });
  }

}
