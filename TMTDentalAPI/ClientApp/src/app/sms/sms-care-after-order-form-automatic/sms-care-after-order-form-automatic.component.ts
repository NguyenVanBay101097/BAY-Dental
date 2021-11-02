import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { SmsCampaignService } from '../sms-campaign.service';
import { SmsCareAfterOrderAutomationConfigService } from '../sms-care-after-order-automation-config.service';
import { SmsCareAfterOrderFormAutomaticDialogComponent } from '../sms-care-after-order-form-automatic-dialog/sms-care-after-order-form-automatic-dialog.component';

@Component({
  selector: 'app-sms-care-after-order-form-automatic',
  templateUrl: './sms-care-after-order-form-automatic.component.html',
  styleUrls: ['./sms-care-after-order-form-automatic.component.css']
})
export class SmsCareAfterOrderFormAutomaticComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  offset = 0;
  pagerSettings: any;
  campaign: any;
  loading = false;
  searchUpdate = new Subject<string>();
  search: string;
  states: string = "false,true";
  filterStatus = [
    { name: "Kích hoạt", value: "active" },
    { name: "Không kích hoạt", value: "inactive" }
  ];

  constructor(
    private modalService: NgbModal,
    private smsConfigService: SmsCareAfterOrderAutomationConfigService,
    private smsCampaignService: SmsCampaignService,
    private notificationService: NotificationService,
    private authService: AuthService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();
    this.loadDefaultCampaignCareAfterOrder();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.offset = 0;
        this.loadDataFromApi();
      });
  }

  loadDefaultCampaignCareAfterOrder() {
    this.smsCampaignService.getDefaultCareAfterOrder().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      })
  }

  onStatusChange(event) {
    if (event && event.value === "active") {
      this.states = "true";
    }
    else if (event && event.value === "inactive") {
      this.states = "false";
    }
    else {
      this.states = "false,true";
    }
    this.offset = 0;
    this.loadDataFromApi();
  }

  editItem(item) {
    var modalRef = this.modalService.open(SmsCareAfterOrderFormAutomaticDialogComponent, { size: "md", windowClass: "o_technical_modal", keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Thiết lập tin nhắn gửi tự động";
    // modalRef.componentInstance.templateTypeTab = "care_after_order";
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(
      result => {
        this.loadDataFromApi();
      }
    )
  }

  deleteItem(item) {
    if (item.active) {
      this.notify("Không thể xóa thiết lập đang kích hoạt", false);
      return;
    }
    var modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Xóa thiết lập gửi tự động";
    modalRef.componentInstance.body = "Bạn có muốn xóa thiết lập gửi tin tự động không?";
    modalRef.result.then(
      result => {
        this.smsConfigService.delete(item.id).subscribe(
          () => {
            this.loadDataFromApi();
          }
        )
      }
    )
  }

  loadDataFromApi() {
    this.loading = true;
    var val = {
      limit: this.limit,
      offset: this.offset,
      search: this.search || '',
      states: this.states,
      companyId: this.authService.userInfo.companyId
    }
    this.smsConfigService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  setupAutomaic() {
    var modalRef = this.modalService.open(SmsCareAfterOrderFormAutomaticDialogComponent, { size: "md", windowClass: "o_technical_modal", keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Thiết lập tin nhắn gửi tự động";
    modalRef.componentInstance.campaign = this.campaign;
    // modalRef.componentInstance.templateTypeTab = "care_after_order";
    modalRef.result.then(
      result => {
        this.loadDataFromApi();
      }
    )
  }

  pageChange(event): void {
    this.offset = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  notify(title, isSuccess = true) {
    this.notificationService.show({
      content: title,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: isSuccess ? 'success' : 'error', icon: true },
    });
  }
}
