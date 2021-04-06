import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { ToothDiagnosisSave } from 'src/app/tooth-diagnosis/tooth-diagnosis.service';

@Component({
  selector: 'app-discount-popover-share',
  templateUrl: './discount-popover-share.component.html',
  styleUrls: ['./discount-popover-share.component.css']
})
export class DiscountPopoverShareComponent implements OnInit {

  @Input() line: any; //object
  @Output() lineDiscountEvent = new EventEmitter<any>();
  @ViewChild('popOver', { static: true }) public popover: NgbPopover;
  formGroup: FormGroup;
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.loadFormDiscount();
  }
  loadFormDiscount() {
    if (this.line) {
      this.formGroup = this.fb.group({
        discountType: this.line.discountType,
        discountPercent: this.line.discount,
        discountFixed: this.line.discount,
      });
    } else {
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
    if (this.formGroup.get('discountType').value === 'fixed') {
      return this.formGroup.get('discountFixed').value;
    }
    else if (this.formGroup.get('discountType').value === "percentage") {
      return this.formGroup.get('discountPercent').value;
    }
  }

  get valueButton() {
    var res = this.formGroup.value;
    if (res.discountType == 'percentage') {
      return res.discountPercent + '' + '%';
    } else {
      return res.discountFixed;
    }
  }

  getEmitValue() {
    var formData = this.formGroup.value;
    var res =
    {
      discountType: formData.discountType,
      discount: formData.discountType == 'fixed' ? formData.discountFixed : formData.discountPercent
    }
    res.discount = res.discount ? res.discount : 0;
    return res;
  }

  onChangeDiscount() {
    this.lineDiscountEvent.emit(this.getEmitValue());
  }

  onChangeDiscountType() {
    var res = this.formGroup.value;
    res.discountFixed = 0;
    res.discountPercent = 0;
    this.lineDiscountEvent.emit(this.getEmitValue());
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
