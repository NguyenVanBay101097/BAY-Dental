import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SaleOrderService } from '../sale-order.service';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SharedErrorDialogComponent } from 'src/app/shared/shared-error-dialog/shared-error-dialog.component';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-sale-order-apply-coupon-dialog',
  templateUrl: './sale-order-apply-coupon-dialog.component.html',
  styleUrls: ['./sale-order-apply-coupon-dialog.component.css']
})
export class SaleOrderApplyCouponDialogComponent implements OnInit {
  orderId: string;
  formGroup: FormGroup;
  constructor(private fb: FormBuilder, private saleOrderService: SaleOrderService, public activeModal: NgbActiveModal,
    private modalService: NgbModal, private errorService: AppSharedShowErrorService) { }

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
