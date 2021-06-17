import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SmsAccountPaged, SmsAccountService } from '../sms-account.service';
import { SmsCampaignService } from '../sms-campaign.service';
import { SmsMessageDetailDialogComponent } from '../sms-message-detail-dialog/sms-message-detail-dialog.component';
import { SmsMessagePaged, SmsMessageService } from '../sms-message.service';

@Component({
  selector: 'app-sms-campaign-detail',
  templateUrl: './sms-campaign-detail.component.html',
  styleUrls: ['./sms-campaign-detail.component.css']
})
export class SmsCampaignDetailComponent implements OnInit {

  @ViewChild("smsAccountCbx", { static: true }) smsAccountCbx: ComboBoxComponent;

  gridData: GridDataResult;
  limit = 20;
  offset = 0;
  loading = false;
  searchUpdate = new Subject<string>();
  search: string;
  formGroup: FormGroup
  campaign: any;
  campaignId: string;
  isEdit: boolean = false;
  state = "in_queue"; // waiting: chờ gửi, success: đã gửi
  selectedIds: any = [];
  dateFrom: Date;
  dateTo: Date;
  filteredSmsAccount: any[];
  smsAccountId: string = "";
  submitted = false;
  campaignName: string = '';
  get f() { return this.formGroup.controls; }

  constructor(
    private route: ActivatedRoute,
    private smsMessageService: SmsMessageService,
    private smsCampaignService: SmsCampaignService,
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private smsAccountService: SmsAccountService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      typeDate: "unlimited", // unlimited: vô thời hạn, period: khoảng thời gian
      startDateObj: new Date(),
      endDateObj: new Date(),
      limitMessage: [0, Validators.required],
      stateCheck: true
    });

    this.route.paramMap.subscribe(params => {
      this.campaignId = params.get('id');
      this.loadDataFromApi();
      this.getSmsCampaign();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.offset = 0;
        this.selectedIds = [];
        this.loadDataFromApi();
      });

    this.smsAccountCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsAccountCbx.loading = true)),
      switchMap(value => this.searchAccount(value))
    ).subscribe((result: any) => {
      this.filteredSmsAccount = result ? result.items : [];
      this.smsAccountCbx.loading = false;
    });

    this.loadAccount();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SmsMessagePaged();
    val.limit = this.limit;
    val.offset = this.offset;
    val.search = this.search || "";
    val.campaignId = this.campaignId;
    val.smsAccountId = this.smsAccountId;
    val.state = this.state;
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59");
    this.smsMessageService.getPaged(val).pipe(
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
          console.log(res);
          this.gridData = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  pageChange(event): void {
    this.offset = event.offset;
    this.selectedIds = [];
    this.loadDataFromApi();
  }

  getSmsCampaign() {
    this.smsCampaignService.get(this.campaignId)
      .subscribe(
        (res: any) => {
          if (res) {
            this.campaign = res;
            if (res.state == "running") {
              this.formGroup.get('stateCheck').setValue(true);
            } else {
              this.formGroup.get('stateCheck').setValue(false);
            }

            if (res.typeDate == 'period') {
              res.startDateObj = new Date(res.dateStart);
              res.endDateObj = new Date(res.dateEnd);
            }
            this.formGroup.patchValue(res);
            this.campaignName = res.name;
          }
        },
        (err) => {
          console.log(err);
        }
      );
  }

  changeState() {
    this.selectedIds = [];
    this.offset = 0;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.selectedIds = [];
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.offset = 0;
    this.loadDataFromApi();
  }

  cancelSend() {
    if (this.selectedIds && this.selectedIds.length <= 0) {
      this.notify("Bạn chưa chọn tin nhắn nào để hủy gửi. Vui lòng chọn và thử lại", false);
      return;
    }

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Hủy gửi';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn hủy gửi?';
    modalRef.result.then(() => {
      this.smsMessageService.actionCancelSendSMS(this.selectedIds).subscribe(
        () => {
          this.notify("Hủy thành công", true);
          this.loadDataFromApi();
        }
      )
    });
  }

  onEditCampaign() {
    this.isEdit = true;
  }

  computeTotalMessage() {
    var totalMessageCampaign = 0;
    if (this.campaign) {
      totalMessageCampaign = this.campaign.totalFailedMessages || 0 + this.campaign.totalSuccessfulMessages || 0 + this.campaign.totalWaitedMessages || 0;
    }
    return this.f.limitMessage.value - totalMessageCampaign;
  }

  onSaveCampaign() {
    this.submitted = true;
    if (this.formGroup.invalid) return false;
    var val = this.formGroup.value;
    if (val.typeDate == 'period') {
      val.dateEnd = this.intlService.formatDate(val.endDateObj, "yyyy-MM-ddT23:59");
      val.dateStart = this.intlService.formatDate(val.startDateObj, "yyyy-MM-dd")
    }
    val.state = this.formGroup.get('stateCheck') && this.formGroup.get('stateCheck').value == true ? 'running' : 'shutdown';
    this.smsCampaignService.update(this.campaignId, val).subscribe(
      result => {
        this.notify("Cập nhật chiến dịch thành công");
        this.isEdit = false;
      }
    )
    this.campaignName = val.name;
  }

  getValueFormControl(key) {
    return this.formGroup.get(key).value;
  }

  onClose() {
    this.isEdit = false;
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

  cellClick(item) {
    const modalRef = this.modalService.open(SmsMessageDetailDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.title = 'Tin nhắn đã gửi';
    modalRef.result.then((val) => {
      this.loadDataFromApi();
    })
  }

  onChangeAccount(event) {
    this.smsAccountId = event ? event.id : "";
    this.offset = 0;
    this.loadDataFromApi();
  }

  searchAccount(search?: string) {
    var val = new SmsAccountPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search || '';
    return this.smsAccountService.getPaged(val);
  }

  loadAccount() {
    this.searchAccount().subscribe(
      (result: any) => {
        this.filteredSmsAccount = result ? result.items : [];
      }
    )
  }
}
