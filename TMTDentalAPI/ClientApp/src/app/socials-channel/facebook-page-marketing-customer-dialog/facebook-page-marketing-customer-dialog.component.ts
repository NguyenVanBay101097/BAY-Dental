import { Component, OnInit } from '@angular/core';
import { FacebookUserProfilesService } from '../facebook-user-profiles.service';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookTagsPaged, FacebookTagsService } from '../facebook-tags.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject, Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PartnerService } from 'src/app/partners/partner.service';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerCustomerCuDialogComponent } from 'src/app/partners/partner-customer-cu-dialog/partner-customer-cu-dialog.component';

@Component({
  selector: 'app-facebook-page-marketing-customer-dialog',
  templateUrl: './facebook-page-marketing-customer-dialog.component.html',
  styleUrls: ['./facebook-page-marketing-customer-dialog.component.css']
})
export class FacebookPageMarketingCustomerDialogComponent implements OnInit {
  customerId: string;
  loading: boolean;
  dataCustomer: any;

  selectedTags: any[] = [];

  listPartners: any[];
  selectedPartner: any;
  searchNamePhoneRef: string;
  searchNamePhoneRefUpdate = new Subject<string>();

  constructor(
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private facebookUserProfilesService: FacebookUserProfilesService,
    private facebookTagsService: FacebookTagsService, 
    private partnerService: PartnerService,
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.loadDataFromApi(this.customerId);
    
    this.getPartnersList();

    this.searchNamePhoneRefUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.getPartnersList();
      });
  }
  
  loadDataFromApi(id) {
    this.loading = true;
    this.facebookUserProfilesService.get(id).subscribe(res => {
      this.dataCustomer = res;
      this.selectedTags = this.dataCustomer.tags;
      this.selectedPartner = this.dataCustomer.partnerId;
      this.loading = false;
      console.log(this.dataCustomer);
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }

  save_selectedTags(event) {
    this.selectedTags = event;
  }

  onSave() {
    var val = {
      partnerId: this.selectedPartner,
      tagIds: []
    }
    for (let i = 0; i < this.selectedTags.length; i++) {
      val.tagIds.push(this.selectedTags[i].id);
    }
    console.log(this.selectedTags);
    console.log(val);
    this.facebookUserProfilesService.setData(this.customerId, val).subscribe(res => {
      this.notificationService.show({
        content: 'Lưu thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.activeModal.close(true);
    }, err => {
      console.log(err);
      this.activeModal.close(true);
    })
  }

  getPartnersList() {
    var val = new PartnerPaged();
    val.limit = 10;
    val.offset = 0;
    val.search = this.searchNamePhoneRef || '';
    val.customer = true;

    this.partnerService.getPaged(val).pipe(
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
    }, err => {
      console.log(err);
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

  handleFilter(event) {
    this.searchNamePhoneRef = event;
  }

}
