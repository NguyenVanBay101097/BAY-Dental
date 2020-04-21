import { Component, OnInit } from '@angular/core';
import { FacebookUserProfilesService } from '../facebook-user-profiles.service';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookTagsPaged, FacebookTagsService } from '../facebook-tags.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
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
  listTags: any[];
  listAddTags: any[] = [];
  inputSearchTag: string;
  searchTagUpdate = new Subject<string>();
  showButtonCreateTag: boolean = false;

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

    this.searchTagUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadListTags();
      });
    
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
      this.listAddTags = this.dataCustomer.tags;
      this.selectedPartner = this.dataCustomer.partnerId;
      this.loading = false;
      console.log(this.dataCustomer);
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }

  loadListTags() {
    var val = new FacebookTagsPaged();
    val.offset = 0;
    val.limit = 10;
    val.search = this.inputSearchTag || '';
    this.facebookTagsService.getTags(val).subscribe(res => {
      this.listTags = res['items'];
      if (this.listTags.length == 0) {
        this.showButtonCreateTag = true;
      } else {
        this.showButtonCreateTag = false;
      }
      // console.log(this.listTags);
    }, err => {
      console.log(err);
    })
  }

  addTagItem(item) {
    var result = this.listAddTags.find( ({ id }) => id === item.id );
    if (!result) {
      this.listAddTags.push(item);
    }
  }

  deleteTagItem(i) {
    this.listAddTags.splice(i, 1);
  }

  createTag() {
    var val = {
      name: this.inputSearchTag
    };
    this.facebookTagsService.create(val).subscribe(res => {
      this.notificationService.show({
        content: 'Tạo nhãn thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.listAddTags.push(res);
      // console.log(res);
    }, err => {
      console.log(err);
    })
  }

  onSave() {
    var val = {
      partnerId: this.selectedPartner,
      tagIds: []
    }
    for (let i = 0; i < this.listAddTags.length; i++) {
      val.tagIds.push(this.listAddTags[i].id);
    }
    this.facebookUserProfilesService.setData(this.customerId, val).subscribe(res => {
      this.activeModal.close(true);
    }, err => {
      console.log(err);
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
      console.log('getPartnersList', res);
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
