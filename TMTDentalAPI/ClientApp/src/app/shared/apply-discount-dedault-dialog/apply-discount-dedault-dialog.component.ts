import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-apply-discount-dedault-dialog',
  templateUrl: './apply-discount-dedault-dialog.component.html',
  styleUrls: ['./apply-discount-dedault-dialog.component.css']
})
export class ApplyDiscountDedaultDialogComponent implements OnInit {
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
