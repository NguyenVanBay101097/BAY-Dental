import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerImageBasic } from 'src/app/partners/partner.service';
import { SaleOrderImagesLibraryPopupComponent } from '../sale-order-images-library-popup/sale-order-images-library-popup.component';

@Component({
  selector: 'app-sale-order-print-popup',
  templateUrl: './sale-order-print-popup.component.html',
  styleUrls: ['./sale-order-print-popup.component.css']
})
export class SaleOrderPrintPopupComponent implements OnInit {

  title: string;
  imagesPreview: PartnerImageBasic[] = [];
  id: string = '';
  constructor(public activeModal: NgbActiveModal,private modalService: NgbModal,) { }

  ngOnInit() {
  }

  onPrint(){
    this.activeModal.close(this.imagesPreview);
  }

  onCancel(){
    this.activeModal.close();
  }

  attachImg(){
    let modalRef = this.modalService.open(SaleOrderImagesLibraryPopupComponent, {size: 'md'});
    modalRef.componentInstance.id = this.id;
    modalRef.result.then(result =>{
      this.imagesPreview = result;
    });
  }

  deleteImage(id){
    let index = this.imagesPreview.findIndex(x => x.id == id);
    if (index != -1)
      this.imagesPreview.splice(index,1);
  }

}
