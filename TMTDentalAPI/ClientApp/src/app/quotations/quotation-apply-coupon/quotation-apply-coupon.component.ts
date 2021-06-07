import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';
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
    private fb: FormBuilder,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      couponCode: ['', Validators.required]
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      this.notificationService.show({
        content: 'Nhập mã khuyến mãi',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }

    this.applySuccess.emit(this.formGroup.value)
  }

}
