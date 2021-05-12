import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { QuotationLineService } from '../quotation-line.service';

@Component({
  selector: 'app-quotation-line-apply-coupon',
  templateUrl: './quotation-line-apply-coupon.component.html',
  styleUrls: ['./quotation-line-apply-coupon.component.css']
})
export class QuotationLineApplyCouponComponent implements OnInit {

  @Input() lineId: string;
  formGroup: FormGroup;
  errorMsg: string;
  @Output() applySuccess = new EventEmitter<any>();
  constructor(
    private fb: FormBuilder,
    private quotationLineService: QuotationLineService
    ) { }

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
    this.quotationLineService.applyPromotionUsageCode(val).subscribe((res: any) => {
      if (res.success) {
        this.errorMsg = '';
        this.applySuccess.emit(null);
      } else {
        this.errorMsg = res.error;
      }
    });
  }
  
}
