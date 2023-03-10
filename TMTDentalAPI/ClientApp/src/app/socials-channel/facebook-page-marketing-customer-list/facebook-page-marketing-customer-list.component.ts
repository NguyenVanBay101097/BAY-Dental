import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { CompositeFilterDescriptor, State } from '@progress/kendo-data-query';
import { Operation } from 'fast-json-patch';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { FacebookUserProfilesODataService } from 'src/app/shared/services/facebook-user-profiles.service';
import { FacebookPageMarketingCustomerDialogComponent } from '../facebook-page-marketing-customer-dialog/facebook-page-marketing-customer-dialog.component';
import { FacebookPageService, MultiUserProfilesVm } from '../facebook-page.service';
import { FacebookUserProfilesService } from '../facebook-user-profiles.service';

@Component({
  selector: 'app-facebook-page-marketing-customer-list',
  templateUrl: './facebook-page-marketing-customer-list.component.html',
  styleUrls: ['./facebook-page-marketing-customer-list.component.css']
})
export class FacebookPageMarketingCustomerListComponent implements OnInit {

  constructor(
    private modalService: NgbModal,
    private route: ActivatedRoute,
    private notificationService: NotificationService,
    private facebookPageService: FacebookPageService,
    private facebookUserProfilesService: FacebookUserProfilesService,
    private facebookUserProfilesODataService: FacebookUserProfilesODataService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettingsPopup }

  dataSendMessage: any[] = [];
  gridData: GridDataResult;
  limit = 10;
  skip = 0;
  pagerSettings: any;
  search: string;
  loading = false;
  searchUpdate = new Subject<string>();
  rowsSelected: any[] = [];
  public mySelection: any[] = [];
  isSelected = false;

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

  onPhonePartnerChange(dataItem, event) {
    dataItem = Object.assign(dataItem, event);
  }

  loadDataFromApi() {
    this.rowsSelected = [];
    this.loading = true;

    var state: State = {
      take: this.limit,
      skip: this.skip,
      filter: this.getFilter()
    };

    this.facebookUserProfilesODataService.getView(state).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      this.loading = false;
    });
  }

  getFilter() {
    var filter: CompositeFilterDescriptor = {
      logic: 'and',
      filters: [
        { field: 'FbPageId', operator: 'eq', value: this.pageId }
      ]
    };

    if (this.search) {
      filter = {
        logic: 'and',
        filters: [
          { field: 'FbPageId', operator: 'eq', value: this.pageId },
          { field: 'Name', operator: 'contains', value: this.search }
        ]
      };
    } else {
      filter = {
        logic: 'and',
        filters: [
          { field: 'FbPageId', operator: 'eq', value: this.pageId }
        ]
      };
    }

    return filter;
  }

  getDisplayName(data) {
    return data ? JSON.parse(data).DisplayName : "";
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

  syncNumberPhoneUsers() {
    // const modalRef = this.modalService.open(TcareQuickreplyDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    // modalRef.componentInstance.title = 'T???o n???i dung g???i';
    // modalRef.result.then((val) => {
    //   if (this.pageId) {
    //     var res = new MultiUserProfilesVm();
    //     res.pageId = this.pageId;
    //     res.userIds = this.rowsSelected;
    //     res.content = val;

    //     this.facebookPageService.syncPartners(res).subscribe(() => {
    //       this.loadDataFromApi();
    //     }, err => {
    //       console.log(err);
    //     });
    //   }
    //   this.loadDataFromApi();
    // });
    if (this.pageId) {
      this.facebookPageService.syncNumberPhoneUsers([this.pageId]).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }
  }

  syncPartners() {
    if (this.pageId) {
      this.facebookPageService.syncPartners([this.pageId]).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }
  }

  syncPartnersMutilUser() {
    if (this.pageId) {
      var res = new MultiUserProfilesVm();
      res.pageId = this.pageId;
      res.userIds = this.mySelection ? this.mySelection : null;
      this.facebookPageService.syncPartnerForMultiUsers(res).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }
  }

  syncPhoneOfMutilUser() {
    if (this.pageId) {
      var res = new MultiUserProfilesVm();
      res.pageId = this.pageId;
      res.userIds = this.mySelection ? this.mySelection : null;
      this.facebookPageService.syncPhoneForMultiUsers(res).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }
  }

  onCreatePartnerClick(dataItem, data) {
    const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, {
      scrollable: true,
      size: "xl",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });

    modalRef.componentInstance.title = "Th??m kh??ch h??ng";
    modalRef.componentInstance.addtionalData = { phone: data.phone };
    modalRef.result.then(
      (result: any) => {
        var patch: Operation[] = [
          { path: 'Phone', op: 'replace', value: result.phone },
          { path: 'PartnerId', op: 'replace', value: result.id }
        ];

        this.facebookUserProfilesODataService.patch(dataItem.Id, patch).subscribe(() => {
          this.facebookUserProfilesODataService.getView({
            filter: {
              logic: 'and',
              filters: [
                { field: 'Id', operator: 'eq', value: dataItem.Id }
              ]
            },
            take: 1
          }).subscribe(result2 => {
            if (result2.data.length) {
              dataItem = Object.assign(dataItem, result2.data[0]);
            }
          });
        });
      },
      (err) => { }
    );
  }

  editItem(item: any) {
    let modalRef = this.modalService.open(FacebookPageMarketingCustomerDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.customerId = item.Id;

    modalRef.result.then((result) => {
      this.loadDataFromApi();
    }, (reason) => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  selectedKeysChange(rows: any[]) {
    this.rowsSelected = rows;
    if (this.rowsSelected.length > 0) {
      this.isSelected = true;
    } else {
      this.isSelected = false;
    }
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
        content: 'H???y k???t n???i th??nh c??ng',
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
