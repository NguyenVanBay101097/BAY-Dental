import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ProductRequestDisplay } from '../product-request';

@Component({
  selector: 'app-sale-order-product-request-dialog',
  templateUrl: './sale-order-product-request-dialog.component.html',
  styleUrls: ['./sale-order-product-request-dialog.component.css']
})
export class SaleOrderProductRequestDialogComponent implements OnInit {
  title: string = null;
  formGroup: FormGroup;
  productRequestDisplay: ProductRequestDisplay = new ProductRequestDisplay();
  reload = false;

  get f() { return this.formGroup.controls; }

  get seeForm() {
    var state = this.formGroup.get('state').value;
    var val = state == 'confirmed' || state == 'done';
    return val;
  }

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      amount: 0,
      communication: null,
      paymentDateObj: [null, Validators.required],
      journal: [null, Validators.required],
      loaiThuChi: [null, Validators.required],
      name: null,
      state: null,
      partnerType: ['customer', Validators.required],
      partner: [null],
    });

    this.productRequestDisplay.state = "draft";
  }

  onSave() {

  }

  onRequest() {

  }

  onCancel() {

  }

  onClose() {
    if (this.reload) {
      this.activeModal.close();
    } else {
      this.activeModal.dismiss();
    }
  }
}
