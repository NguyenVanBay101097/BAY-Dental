import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-sale-order-line-discount-odata-popover',
  templateUrl: './sale-order-line-discount-odata-popover.component.html',
  styleUrls: ['./sale-order-line-discount-odata-popover.component.css']
})
export class SaleOrderLineDiscountOdataPopoverComponent implements OnInit {

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
        DiscountType: res.DiscountType,
        DiscountPercent: res.Discount,
        DiscountFixed: res.DiscountFixed,
      });
    }else{
      this.formGroup = this.fb.group({
        DiscountType: 'percentage',
        DiscountPercent: 0,
        DiscountFixed: 0,
      });
    }
  }

  get discountTypeValue() {
    return this.formGroup.get('DiscountType').value;
  }

  get discountValue() {
    return this.formGroup.get('DiscountPercent').value;
  }

  get discountFixedValue() {
    return this.formGroup.get('DiscountFixed').value;
  }

  get valueButton() {
    var res = this.formGroup.value;
    if (res.DiscountType == 'percentage') {
      return res.DiscountPercent + '' + '%';
    } else {
      return res.DiscountFixed;
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
    res.DiscountFixed = 0;
    res.DiscountPercent = 0;
    this.discountFormGroup.emit(this.formGroup.value);
    this.resetForm(this.formGroup.value);
  }

  resetForm(value) {
    this.formGroup = this.fb.group({
      DiscountType: value.DiscountType,
      DiscountPercent: 0,
      DiscountFixed: 0,
    });
  }

}
