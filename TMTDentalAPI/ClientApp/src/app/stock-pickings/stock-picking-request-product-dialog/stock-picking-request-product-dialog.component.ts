import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ProductRequestDisplay } from 'src/app/sale-orders/product-request';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductRequestService } from 'src/app/shared/product-request.service';

@Component({
  selector: 'app-stock-picking-request-product-dialog',
  templateUrl: './stock-picking-request-product-dialog.component.html',
  styleUrls: ['./stock-picking-request-product-dialog.component.css']
})
export class StockPickingRequestProductDialogComponent implements OnInit {
  id: string;
  productRequest: ProductRequestDisplay;

  constructor(
    private activateModal: NgbActiveModal,
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

  onRequest() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Yêu cầu vật tư';
    modalRef.componentInstance.body = 'Bạn có chắc chắn tạo phiếu xuất kho vật tư?';
    modalRef.result.then(() => {
      this.productRequestService.actionDone([this.id]).subscribe(
        () => {
          this.activateModal.close();
        }
      )
    })
  }
}
