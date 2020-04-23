import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerCuDialogComponent } from 'src/app/partners/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PartnerService } from 'src/app/partners/partner.service';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { Subject } from 'rxjs';

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
  selectedPartner: any;
  searchNamePhoneRef: string;
  searchNamePhoneRefUpdate = new Subject<string>();

  constructor(public activeModal: NgbActiveModal, private modalService: NgbModal,
    private partnerService: PartnerService) { }

  ngOnInit() {
    this.getPartnersList();
    this.searchChange();
  }

  onSave() {
    console.log(this.selectedPartner);
    this.activeModal.close(this.selectedPartner);
  }

  getPartnersList() {
    this.loading = true;
    var val = new PartnerPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.searchNamePhoneRef || '';
    val.customer = true;
    val.supplier = false;
    this.partnerService.getPartnerPaged(val).pipe(
      map(response => (<any>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      // console.log('getPartnersList', res);
      var res_data = res.data;
      this.listPartners = [];
      for (var i = 0; i < res_data.length; i++) {
        this.listPartners.push({
          'id': res_data[i].id,
          'name': res_data[i].name,
          'phone': res_data[i].phone,
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
      this.listPartners.push({
        'id': res.id,
        'name': res.name,
        'phone': res.phone,
      });
      this.selectedPartner = res.id;
    }, () => {
    });
  }

  handleFilter(value) {
    this.searchNamePhoneRef = value;
  }

  searchChange() {
    this.searchNamePhoneRefUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.getPartnersList();
      });
  }
}
