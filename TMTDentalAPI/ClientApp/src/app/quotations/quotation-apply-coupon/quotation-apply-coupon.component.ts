import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NotifyService } from 'src/app/shared/services/notify.service';
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
    private notifyService: NotifyService,
    private notificationService: NotificationService
  ) { }

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

    this.applySuccess.emit(this.formGroup.value)
  }

  get f() {
    return this.formGroup.controls;
  }

}
