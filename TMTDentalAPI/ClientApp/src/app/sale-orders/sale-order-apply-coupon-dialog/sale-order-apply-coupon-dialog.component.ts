import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { SaleOrderService } from '../../core/services/sale-order.service';

@Component({
  selector: 'app-sale-order-apply-coupon-dialog',
  templateUrl: './sale-order-apply-coupon-dialog.component.html',
  styleUrls: ['./sale-order-apply-coupon-dialog.component.css']
})
export class SaleOrderApplyCouponDialogComponent implements OnInit {
  orderId: string;
  formGroup: FormGroup;
  constructor(private fb: FormBuilder,
     private saleOrderService: SaleOrderService,
     public activeModal: NgbActiveModal,
     private errorService: AppSharedShowErrorService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      couponCode: ['', Validators.required]
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.id = this.orderId;
    this.saleOrderService.applyCoupon(val).subscribe(() => {
      this.activeModal.close(true);
    }, (error) => {
      this.errorService.show(error);
    });
  }
}
