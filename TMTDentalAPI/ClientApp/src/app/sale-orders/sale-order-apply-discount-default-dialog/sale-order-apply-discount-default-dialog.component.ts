import { Component, OnInit, Output, EventEmitter, Input, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-sale-order-apply-discount-default-dialog',
  templateUrl: './sale-order-apply-discount-default-dialog.component.html',
  styleUrls: ['./sale-order-apply-discount-default-dialog.component.css']
})
export class SaleOrderApplyDiscountDefaultDialogComponent implements OnInit {
  @Output() discountFormGroup = new EventEmitter<any>();
  @ViewChild('popOver',{static: true}) public popover: NgbPopover;
  formGroup: FormGroup;
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      discountType: 'percentage',
      discountPercent: 0,
      discountFixed:0,
    });
  }

  get discountTypeValue() {
    return this.formGroup.get('discountType').value;
  }

  applyDiscount() {
    this.discountFormGroup.emit(this.formGroup.value);
    this.popover.close();
    this.resetForm();
  }

   resetForm(){
    this.formGroup = this.fb.group({
      discountType: 'percentage',
      discountPercent: 0,
      discountFixed:0,
    });
  }

}
