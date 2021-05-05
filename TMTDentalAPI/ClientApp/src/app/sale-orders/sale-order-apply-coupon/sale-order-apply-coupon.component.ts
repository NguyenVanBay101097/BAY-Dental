import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SaleOrderService } from '../../core/services/sale-order.service';

@Component({
  selector: 'app-sale-order-apply-coupon',
  templateUrl: './sale-order-apply-coupon.component.html',
  styleUrls: ['./sale-order-apply-coupon.component.css']
})
export class SaleOrderApplyCouponComponent implements OnInit {
  @Input() orderId: string;
  formGroup: FormGroup;
  errorMsg: string;
  @Output() applySuccess = new EventEmitter<any>();
  constructor(private fb: FormBuilder, private saleOrderService: SaleOrderService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      couponCode: ['']
    });
  }

  onSave() {
    // if (!this.formGroup.valid) {
    //   return false;
    // }

    if(this.formGroup.value.couponCode.trim() == '') {
      this.errorMsg = 'Vui lòng nhập mã khuyến mãi';
      return;
    }
    var val = this.formGroup.value;
    val.id = this.orderId;
    this.saleOrderService.applyCouponOnOrder(val).subscribe((res: any) => {
      if (res.success) {
        this.errorMsg = '';
        this.applySuccess.emit(null);
      } else {
        this.errorMsg = res.error;
      }
    });
  }
}

