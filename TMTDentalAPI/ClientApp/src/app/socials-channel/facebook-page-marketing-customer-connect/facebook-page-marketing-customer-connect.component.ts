import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerCuDialogComponent } from 'src/app/partners/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { map } from 'rxjs/operators';
import { PartnerService } from 'src/app/partners/partner.service';
import { PartnerPaged } from 'src/app/partners/partner-simple';

@Component({
  selector: 'app-facebook-page-marketing-customer-connect',
  templateUrl: './facebook-page-marketing-customer-connect.component.html',
  styleUrls: ['./facebook-page-marketing-customer-connect.component.css']
})
export class FacebookPageMarketingCustomerConnectComponent implements OnInit {
  loading: boolean;
  listPartners: any[];
  limit: number = 10;
  skip: number = 0;
  searchNamePhone: string;
  selectedPartner: any;
  defaultPartner = {           
    'id': null,
    'name': 'Chọn khách hàng...',
  };

  constructor(public activeModal: NgbActiveModal, private modalService: NgbModal,
    private partnerService: PartnerService) { }

  ngOnInit() {
    this.onGetPartnersList();
  }

  onSave() {
    this.activeModal.close(this.selectedPartner);
  }

  onGetPartnersList() {
    this.loading = true;
    var val = new PartnerPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.searchNamePhoneRef = this.searchNamePhone || '';
    val.customer = true;
    val.supplier = false;
    this.partnerService.getPartnerPaged(val).pipe(
      map(response => (<any>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      console.log('onGetPartnersList', res);
      var res_data = res.data;
      this.listPartners = [];
      for (var i = 0; i < res_data.length; i++) {
        this.listPartners.push({
          'id': res_data[i].id,
          'name': res_data[i].name,
        });
      }
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = true;
    }
    )
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
