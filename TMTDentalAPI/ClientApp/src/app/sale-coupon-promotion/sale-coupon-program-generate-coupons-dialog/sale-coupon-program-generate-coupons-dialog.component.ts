import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleCouponProgramService } from '../sale-coupon-program.service';

@Component({
  selector: 'app-sale-coupon-program-generate-coupons-dialog',
  templateUrl: './sale-coupon-program-generate-coupons-dialog.component.html',
  styleUrls: ['./sale-coupon-program-generate-coupons-dialog.component.css']
})
export class SaleCouponProgramGenerateCouponsDialogComponent implements OnInit {
  formGroup: FormGroup;
  programId: string;
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, private programService: SaleCouponProgramService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      number: [1, Validators.required]
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    value.id = this.programId;
    this.programService.generateCoupons(value).subscribe(() => {
      this.activeModal.close(true);
    });
  }
}
