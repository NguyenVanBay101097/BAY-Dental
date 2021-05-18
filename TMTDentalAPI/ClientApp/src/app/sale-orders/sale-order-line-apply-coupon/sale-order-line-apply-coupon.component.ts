import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';

@Component({
  selector: 'app-sale-order-line-apply-coupon',
  templateUrl: './sale-order-line-apply-coupon.component.html',
  styleUrls: ['./sale-order-line-apply-coupon.component.css']
})
export class SaleOrderLineApplyCouponComponent implements OnInit {
  @Input() lineId: string;
  formGroup: FormGroup;
  errorMsg: string;
  @Output() applySuccess = new EventEmitter<any>();
  constructor(private fb: FormBuilder, private saleOrderLineService: SaleOrderLineService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      couponCode: ['', Validators.required]
    });
  }

  onSave() {
    if(!this.formGroup.valid) {
      return false;
    }

    this.applySuccess.emit(this.formGroup.value);
  }
}
