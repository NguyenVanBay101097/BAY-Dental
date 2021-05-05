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
      couponCode: ['']
    });
  }

  onSave() {
    if(this.formGroup.value.couponCode.trim() == '') {
      this.errorMsg = 'Vui lòng nhập mã khuyến mãi';
      return;
    }
    var val = this.formGroup.value;
    val.id = this.lineId;
    this.saleOrderLineService.applyPromotionUsageCode(val).subscribe((res: any) => {
      if (res.success) {
        this.errorMsg = '';
        this.applySuccess.emit(null);
      } else {
        this.errorMsg = res.error;
      }
    });

  }
}
