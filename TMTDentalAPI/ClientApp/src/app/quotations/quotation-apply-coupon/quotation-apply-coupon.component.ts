import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

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
  constructor(private fb: FormBuilder,
    //  private saleOrderService: SaleOrderService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      couponCode: ['']
    });
  }

  onSave() {
    // if (!this.formGroup.valid) {
    //   return false;
    // }

    if (this.formGroup.value.couponCode.trim() == '') {
      this.errorMsg = 'Vui lòng nhập mã khuyến mãi';
      return;
    }
    var val = this.formGroup.value;
    val.id = this.quotationId;
    // this.saleOrderService.applyCouponOnOrder(val).subscribe((res: any) => {
    //   if (res.success) {
    //     this.errorMsg = '';
    //     this.applySuccess.emit(null);
    //   } else {
    //     this.errorMsg = res.error;
    //   }
    // });
  }

}
