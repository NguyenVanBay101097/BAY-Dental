import { Component, OnInit } from '@angular/core';
import { FacebookTagsService, FacebookTagsPaged } from '../facebook-tags.service';
import { FacebookMassMessagingService, ActionStatisticsPaged, TagStatisticsPaged } from '../facebook-mass-messaging.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-facebook-mass-messaging-create-update-dialog',
  templateUrl: './facebook-mass-messaging-create-update-dialog.component.html',
  styleUrls: ['./facebook-mass-messaging-create-update-dialog.component.css']
})
export class FacebookMassMessagingCreateUpdateDialogComponent implements OnInit {
  loading: boolean = true;
  massMessagingId: string;
  massMessagingType: string;
  listCustomers: any[] = [];
  offsetCustomers: number = 0;
  limitCustomers: number = 10;
  showAddTag: number = 1;
  listTags: any[] = [];
  selectedTag: any;
  searchTag: string;
  pages: number[] = [];
  selectedPage: number = 1;
  
  constructor( public activeModal: NgbActiveModal,
    public facebookTagsService: FacebookTagsService, public facebookMassMessagingService: FacebookMassMessagingService
  ) { }

  ngOnInit() {
    setTimeout(() => {
      this.getCustomers();
    })
  }
  getCustomers() {
    this.loading = true;
    var val = new ActionStatisticsPaged();
    val.id = this.massMessagingId;
    val.offset = this.offsetCustomers;
    val.limit = this.limitCustomers;
    val.type = this.massMessagingType;
    this.facebookMassMessagingService.getActionStatistics(this.massMessagingId, val).subscribe(res => {
      this.listCustomers = res['items'];
      this.pages = [];
      var temp = res['totalItems']/this.limitCustomers + 1;
      for (let i = 1; i <= temp; i++) {
        this.pages.push(i);
      }
      this.loading = false;
    }, err=> {
      this.loading = false;
      console.log(err);
    })
  }
  changeTag(event) {
    var val = new TagStatisticsPaged();
    val.id = this.massMessagingId;
    val.type = this.massMessagingType;
    val.tagIds = [event.id];
    this.facebookMassMessagingService.setTagStatistics(val).subscribe(res => {
      this.showAddTag = 3;
    }, err => {
      console.log(err);
    })
  }
  onClickAddTag() {
    var val = new FacebookTagsPaged();
    val.offset = 0;
    val.limit = 10;
    val.search = this.searchTag || '';
    this.facebookTagsService.getTags(val).subscribe(res => {
      this.listTags = res['items'];
      this.showAddTag = 2;
      console.log(this.listTags);
    }, err => {
      console.log(err);
    })
  }
  onClickPage(i) {
    this.selectedPage = i;
    this.offsetCustomers = (i-1)*this.limitCustomers;
    this.getCustomers();
  }
}
