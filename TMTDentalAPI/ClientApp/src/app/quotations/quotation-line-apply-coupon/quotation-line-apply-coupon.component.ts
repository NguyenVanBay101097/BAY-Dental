import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotifyService } from 'src/app/shared/services/notify.service';

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
    private notifyService : NotifyService,
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

    if(!this.formGroup.valid) {
      return false;
    }
    
    this.applySuccess.emit(this.formGroup.value);
  }

  get f() {
    return this.formGroup.controls;
  }
  
}
