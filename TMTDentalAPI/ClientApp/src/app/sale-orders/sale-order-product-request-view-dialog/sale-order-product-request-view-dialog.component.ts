import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ProductRequestDisplay } from 'src/app/sale-orders/product-request';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductRequestService } from 'src/app/shared/product-request.service';

@Component({
  selector: 'app-sale-order-product-request-view-dialog',
  templateUrl: './sale-order-product-request-view-dialog.component.html',
  styleUrls: ['./sale-order-product-request-view-dialog.component.css']
})
export class SaleOrderProductRequestViewDialogComponent implements OnInit {
  id: string;
  title: string;
  productRequest: ProductRequestDisplay;

  constructor(
    public activateModal: NgbActiveModal,
    private modalService: NgbModal,
    private productRequestService: ProductRequestService
  ) { }

  ngOnInit() {
    if (this.id) {
      this.loadDataFromApi();
    }
  }

  loadDataFromApi() {
    this.productRequestService.get(this.id).subscribe(
      result => {
        this.productRequest = result;
      }
    )
  }

  actionCancel() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Hủy yêu cầu';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn hủy phiếu yêu cầu vật tư?';
    modalRef.result.then(() => {
      this.productRequestService.actionCancel([this.id]).subscribe(
        () => {
          this.activateModal.close();
        }
      )
    })
  }
}
