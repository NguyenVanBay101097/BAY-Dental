import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { QuotationService } from '../quotation.service';

@Component({
  selector: 'app-quotation-apply-coupon',
  templateUrl: './quotation-apply-coupon.component.html',
  styleUrls: ['./quotation-apply-coupon.component.css']
})
export class QuotationApplyCouponComponent implements OnInit {
  @Input() quotationId: string;
  formGroup: FormGroup;
  errorMsg: string;
  @Output() applySuccess = new EventEmitter<any>();
  constructor(
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      couponCode: ['', Validators.required]
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    this.applySuccess.emit(this.formGroup.value)
  }

}
