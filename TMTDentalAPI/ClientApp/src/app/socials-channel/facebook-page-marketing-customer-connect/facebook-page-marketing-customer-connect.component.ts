import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerCuDialogComponent } from 'src/app/partners/partner-customer-cu-dialog/partner-customer-cu-dialog.component';

@Component({
  selector: 'app-facebook-page-marketing-customer-connect',
  templateUrl: './facebook-page-marketing-customer-connect.component.html',
  styleUrls: ['./facebook-page-marketing-customer-connect.component.css']
})
export class FacebookPageMarketingCustomerConnectComponent implements OnInit {

  constructor(public activeModal: NgbActiveModal, private modalService: NgbModal,) { }

  ngOnInit() {
  }

  onSave() {

  }

  showModalCreatePartner() {
    let modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm khách hàng';

    modalRef.result.then(res => {
      console.log(res);
    }, () => {
    });
  }
}
