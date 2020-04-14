import { Component, OnInit } from '@angular/core';
import { FacebookUserProfilesService } from '../facebook-user-profiles.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookTagsPaged, FacebookTagsService } from '../facebook-tags.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

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

  constructor(
    public activeModal: NgbActiveModal,
    private facebookUserProfilesService: FacebookUserProfilesService,
    private facebookTagsService: FacebookTagsService, 
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.loadDataFromApi(this.customerId);

    this.searchTagUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadListTags();
      });
  }
  
  loadDataFromApi(id) {
    this.loading = true;
    this.facebookUserProfilesService.get(id).subscribe(res => {
      this.dataCustomer = res;
      this.listAddTags = this.dataCustomer.tags;
      this.loading = false;
      // console.log(this.dataCustomer);
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

      tagIds: []
    }
    for (let i = 0; i < this.listAddTags.length; i++) {
      val.tagIds.push(this.listAddTags[i].id);
    }
    this.facebookUserProfilesService.setTags(this.customerId, val).subscribe(res => {
      this.notificationService.show({
        content: 'Thêm nhãn thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.activeModal.close(true);
    }, err => {
      console.log(err);
    })
  }
}
