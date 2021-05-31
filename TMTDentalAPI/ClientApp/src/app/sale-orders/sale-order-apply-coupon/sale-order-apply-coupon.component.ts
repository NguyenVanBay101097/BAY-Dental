import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { SaleOrderService } from '../../core/services/sale-order.service';

@Component({
  selector: 'app-sale-order-apply-coupon',
  templateUrl: './sale-order-apply-coupon.component.html',
  styleUrls: ['./sale-order-apply-coupon.component.css']
})
export class SaleOrderApplyCouponComponent implements OnInit {
  formGroup: FormGroup;
  @Output() applySuccess = new EventEmitter<any>();
  constructor(private fb: FormBuilder, private saleOrderService: SaleOrderService, private notifyService: NotifyService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      couponCode: ['', Validators.required]
    });
  }

  onSave() {
    if (this.formGroup.get('couponCode').value.trim() === '') {
      this.notifyService.notify('error', 'Nhập mã khuyến mãi');
      return false;
    }

    if (!this.formGroup.valid) {
      return false;
    }

    this.applySuccess.emit(this.formGroup.value);
  }

  get f() {
    return this.formGroup.controls;
  }
}

