import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SaleOrderService } from '../../core/services/sale-order.service';

@Component({
  selector: 'app-sale-order-apply-coupon',
  templateUrl: './sale-order-apply-coupon.component.html',
  styleUrls: ['./sale-order-apply-coupon.component.css']
})
export class SaleOrderApplyCouponComponent implements OnInit {
  formGroup: FormGroup;
  @Output() applySuccess = new EventEmitter<any>();
  constructor(private fb: FormBuilder, private saleOrderService: SaleOrderService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      couponCode: ['', Validators.required]
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    this.applySuccess.emit(this.formGroup.value);
  }
}

