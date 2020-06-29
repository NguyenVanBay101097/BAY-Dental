import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SaleOrderService } from '../sale-order.service';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-sale-order-apply-discount-default-dialog',
  templateUrl: './sale-order-apply-discount-default-dialog.component.html',
  styleUrls: ['./sale-order-apply-discount-default-dialog.component.css']
})
export class SaleOrderApplyDiscountDefaultDialogComponent implements OnInit {
  saleOrderId: string;
  formGroup: FormGroup;
  constructor(private fb: FormBuilder, private saleOrderService: SaleOrderService, public activeModal: NgbActiveModal,
    private modalService: NgbModal, private errorService: AppSharedShowErrorService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      discountType: 'percentage',
      discountPercent: 0,
      discountFixed:0,
    });
  }

  get discountTypeValue() {
    return this.formGroup.get('discountType').value;
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }
    var val = this.formGroup.value;
    val.saleOrderId = this.saleOrderId;
    this.saleOrderService.applyDiscountDefault(val).subscribe(() => {
      this.activeModal.close(true);
    }, (error) => {
      this.errorService.show(error);
    });
  }


}
