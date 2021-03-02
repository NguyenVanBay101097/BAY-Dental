import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ProductRequestLineDisplay } from '../product-request-line';

@Component({
  selector: 'app-sale-order-product-request-dialog',
  templateUrl: './sale-order-product-request-dialog.component.html',
  styleUrls: ['./sale-order-product-request-dialog.component.css']
})
export class SaleOrderProductRequestDialogComponent implements OnInit {
  title: string = null;

  productRequestLineDisplay: ProductRequestLineDisplay = new ProductRequestLineDisplay();

  reload = false;

  constructor(
    public activeModal: NgbActiveModal,
  ) { }

  ngOnInit() {
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
