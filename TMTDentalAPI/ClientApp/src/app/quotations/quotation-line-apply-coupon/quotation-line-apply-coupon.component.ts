import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';
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
    private quotationLineService: QuotationLineService,
    private notificationService: NotificationService
    ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      couponCode: ['', Validators.required]
    });
  }

  onSave() {
    if(!this.formGroup.valid) {
      this.notificationService.show({
        content: 'Nhập mã khuyến mãi',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }
    
    this.applySuccess.emit(this.formGroup.value);
  }
  
}
