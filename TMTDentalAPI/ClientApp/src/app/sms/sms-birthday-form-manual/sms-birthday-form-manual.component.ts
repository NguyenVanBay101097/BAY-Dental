import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { BirthdayCustomerService, ListPagedBirthdayCustomerRequest } from 'src/app/core/services/birthday-customer.service';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { SmsCampaignService } from '../sms-campaign.service';
import { SmsManualDialogComponent } from '../sms-manual-dialog/sms-manual-dialog.component';
import { SmsTemplateService, SmsTemplateFilter } from '../sms-template.service';

@Component({
  selector: 'app-sms-birthday-form-manual',
  templateUrl: './sms-birthday-form-manual.component.html',
  styleUrls: ['./sms-birthday-form-manual.component.css']
})
export class SmsBirthdayFormManualComponent implements OnInit {

  gridData: any;
  filteredSMSAccount: any[];
  filteredTemplate: any[];
  skip: number = 0;
  limit: number = 20;
  isBirthday: boolean = true;
  isRowSelected: any[];
  search: string = '';
  selectedIds: string[] = [];
  month: any;
  searchUpdate = new Subject<string>();
  campaign: any;
  months = [
    { name: 'Tháng 1', value: 1 },
    { name: 'Tháng 2', value: 2 },
    { name: 'Tháng 3', value: 3 },
    { name: 'Tháng 4', value: 4 },
    { name: 'Tháng 5', value: 5 },
    { name: 'Tháng 6', value: 6 },
    { name: 'Tháng 7', value: 7 },
    { name: 'Tháng 8', value: 8 },
    { name: 'Tháng 9', value: 9 },
    { name: 'Tháng 10', value: 10 },
    { name: 'Tháng 11', value: 11 },
    { name: 'Tháng 12', value: 12 },
  ]
  constructor(
    private partnerService: PartnerService,
    private smsTemplateService: SmsTemplateService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private smsCampaignService: SmsCampaignService,
    private birthCustomerService: BirthdayCustomerService,
    private authService: AuthService
  ) { }

  ngOnInit() {

    this.loadDataFromApi();
    setTimeout(() => {
      this.loadDefaultCampaignBirthday();
    }, 1000);
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    var val = new ListPagedBirthdayCustomerRequest();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.birthCustomerService
      .getListPaged(val)
      .pipe(
        map(
          (response: any) =>
            <GridDataResult>{
              data: response.items,
              total: response.totalItems,
            }
        )
      )
      .subscribe(
        (res) => {
          this.gridData = res;
        },
        (err) => {
          console.log(err);
        }
      );
  }

  searchSmsTemplate(q?: string) {
    var filter = new SmsTemplateFilter();
    filter.search = q || "";
    filter.type = "partner";
    return this.smsTemplateService.getAutoComplete(filter);
  }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDefaultCampaignBirthday() {
    this.smsCampaignService.getDefaultCampaignBirthday().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      })
  }

  onSend() {
    if (this.selectedIds.length == 0) {
      this.notify("Bạn phải chọn khách hàng trước khi gửi tin", false);
    }
    else {
      var modalRef = this.modalService.open(SmsManualDialogComponent, { size: "sm", windowClass: "o_technical_modal" });
      modalRef.componentInstance.title = "Tin nhắn chúc mừng sinh nhật";
      modalRef.componentInstance.resIds = this.selectedIds ? this.selectedIds : [];
      modalRef.componentInstance.resModel = "partner"
      modalRef.componentInstance.templateTypeTab = "partner";
      modalRef.componentInstance.campaign = this.campaign ? this.campaign : null;
      modalRef.result.then(
        () => {
        }
      )
    }

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

  selectMonthChange(event) {
    this.month = event.target.value;
    if (this.month != '0') {
      this.isBirthday = false;
    } else {
      this.isBirthday = true;
      this.month = '';
    }
    this.skip = 0;
    this.loadDataFromApi();
  }
}
