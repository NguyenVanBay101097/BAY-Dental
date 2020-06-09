import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { FacebookPageService } from '../facebook-page.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { FacebookUserProfilesService } from '../facebook-user-profiles.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { FacebookPageMarketingCustomerDialogComponent } from '../facebook-page-marketing-customer-dialog/facebook-page-marketing-customer-dialog.component';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-facebook-page-marketing-customer-list',
  templateUrl: './facebook-page-marketing-customer-list.component.html',
  styleUrls: ['./facebook-page-marketing-customer-list.component.css']
})
export class FacebookPageMarketingCustomerListComponent implements OnInit {

  constructor(private modalService: NgbModal,
    private facebookPageService: FacebookPageService,
    private facebookUserProfilesService: FacebookUserProfilesService,
    private notificationService: NotificationService, private route: ActivatedRoute) { }

  dataSendMessage: any[] = [];
  gridData: GridDataResult;
  limit = 10;
  skip = 0;
  search: string;
  loading = false;
  searchUpdate = new Subject<string>();

  pageId: string;

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

    this.route.parent.paramMap.subscribe((param: ParamMap) => {
      this.pageId = param.get('id');
      this.loadDataFromApi();
    });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = {
      limit: this.limit,
      offset: this.skip,
      search: this.search || '',
      fbPageId: this.pageId
    };

    this.facebookUserProfilesService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
      console.log(res);
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }

  syncUsers() {
    if (this.pageId) {
      this.facebookPageService.syncUsers([this.pageId]).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }
  }

  editItem(item: any) {
    let modalRef = this.modalService.open(FacebookPageMarketingCustomerDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.customerId = item.id;

    modalRef.result.then((result) => {
      this.loadDataFromApi();
    }, (reason) => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  removePartner(dataItem: any, rowIndex) {
    var val = [
      dataItem.id
    ]
    this.facebookUserProfilesService.removePartner(val).subscribe(res => {
      this.loading = false;
      console.log(res);
      //this.loadDataFromApi();
      this.gridData.data[rowIndex].partnerId = null;
      this.notificationService.show({
        content: 'Hủy kết nối thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }
}
