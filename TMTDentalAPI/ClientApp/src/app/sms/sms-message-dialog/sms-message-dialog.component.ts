import { state } from '@angular/animations';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { SmsAccountService, SmsAccountPaged } from '../sms-account.service';
import { SmsCampaignService, SmsCampaignPaged } from '../sms-campaign.service';
import { SmsComfirmDialogComponent } from '../sms-comfirm-dialog/sms-comfirm-dialog.component';
import { SmsConfigService } from '../sms-config.service';
import { SmsMessageService } from '../sms-message.service';
import { SmsPartnerListDialogComponent } from '../sms-partner-list-dialog/sms-partner-list-dialog.component';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService, SmsTemplateFilter } from '../sms-template.service';

@Component({
  selector: 'app-sms-message-dialog',
  templateUrl: './sms-message-dialog.component.html',
  styleUrls: ['./sms-message-dialog.component.css']
})
export class SmsMessageDialogComponent implements OnInit {

  @ViewChild("smsCampaignCbx", { static: true }) smsCampaignCbx: ComboBoxComponent;
  @ViewChild("smsAccountCbx", { static: true }) smsAccountCbx: ComboBoxComponent;
  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent;

  formGroup: FormGroup;
  title: string;
  filteredSmsCampaign: any[];
  filteredSmsAccount: any[];
  filteredTemplate: any[];

  sendLimit: number = 0;
  limitMessage: number = 0;
  errorSendLimit: boolean = false;
  noLimit: boolean = false;
  textareaLimit: number = 200;
  templateTypeTab;
  template: any = {
    text: '',
    templateType: 'text'
  };
  isTemplateCopy = false;

  partnerIds: any = [];
  submitted = false;

  get f() { return this.formGroup.controls; }

  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    public activeModal: NgbActiveModal,
    private notificationService: NotificationService,
    private smsCampaignService: SmsCampaignService,
    private smsAccountService: SmsAccountService,
    private smsTemplateService: SmsTemplateService,
    private smsMessageService: SmsMessageService,
    private intlService: IntlService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      smsCampaign: [null, Validators.required],
      smsAccount: [null, Validators.required],
      template: null,
      typeSend: "manual", // manual: gửi ngay, automatic: đặt lịch
      scheduleDateObj: new Date(),
      templateName: null
    })
    this.loadCampaign();
    this.loadAccount();
    this.loadSmsTemplate();

    this.smsCampaignCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsCampaignCbx.loading = true)),
      switchMap(value => this.searchCampaign(value))
    ).subscribe((result: any) => {
      this.filteredSmsCampaign = result.items;
      this.smsCampaignCbx.loading = false;
    });

    this.smsAccountCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsAccountCbx.loading = true)),
      switchMap(value => this.searchAccount(value))
    ).subscribe((result: any) => {
      this.filteredSmsAccount = result ? result.items : [];
      this.smsAccountCbx.loading = false;
    });

    this.smsTemplateCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsTemplateCbx.loading = true)),
      switchMap(value => this.searchSmsTemplate(value))
    ).subscribe((result: any) => {
      this.filteredTemplate = result;
      this.smsTemplateCbx.loading = false;
    });
  }

  getValueFormControl(key) {
    return this.formGroup.get(key).value;
  }

  loadCampaign() {
    this.searchCampaign().subscribe(
      (result: any) => {
        if (result && result.items) {
          this.filteredSmsCampaign = result.items
        }
      }
    )
  }



  searchCampaign(search?: string) {
    var val = new SmsCampaignPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search || '';
    val.userCampaign = true;
    val.companyId = this.authService.userInfo.companyId;
    return this.smsCampaignService.getPaged(val);
  }

  loadAccount() {
    this.searchAccount().subscribe(
      (result: any) => {
        if (result && result.items) {
          this.filteredSmsAccount = result.items
        }
      }
    )
  }

  searchAccount(search?: string) {
    var val = new SmsAccountPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search || '';
    return this.smsAccountService.getPaged(val);
  }

  loadSmsTemplate() {
    this.searchSmsTemplate().subscribe(
      (res: any) => {
        this.filteredTemplate = res;
      }
    )
  }

  onChangeTemplate(event) {
    if (event && event.body) {
      this.template = JSON.parse(event.body);
    } else {
      this.template = {
        text: '',
        templateType: 'text'
      }
    }
  }

  searchSmsTemplate(q?: string) {
    var filter = new SmsTemplateFilter();
    filter.search = q || "";
    filter.type = this.templateTypeTab;
    return this.smsTemplateService.getAutoComplete(filter);
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

  checkedTemplateCopy(event) {
    this.isTemplateCopy = event.target.checked;
  }

  onSelectPartners() {
    const modalRef = this.modalService.open(SmsPartnerListDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Danh sách khách hàng';
    modalRef.result.then((res) => {
      this.partnerIds = res;
      if (this.partnerIds && this.limitMessage != 0 && this.partnerIds.length > this.sendLimit) {
        this.partnerIds = [];
        this.errorSendLimit = true;
      }
      else {
        this.errorSendLimit = false;
      }
    })
  }

  GetValueFormGroup() {
    var val = this.formGroup.value;
    val.smsCampaignId = val.smsCampaign ? val.smsCampaign.id : null;
    val.smsAccountId = val.smsAccount ? val.smsAccount.id : null;
    val.smsTemplateId = val.template ? val.template.id : null;
    val.body = this.template ? JSON.stringify(this.template) : '';
    val.resModel = "partner"
    val.resIds = this.partnerIds;
    val.state = val.typeSend == 'automatic' ? 'in_queue' : 'draft';
    if (val.typeSend == "automatic") {
      val.scheduleDate = this.intlService.formatDate(val.scheduleDateObj, "yyyy-MM-ddTHH:mm");
    }
    return val;
  }

  actionSendNow() {
    this.submitted = true;
    if (this.formGroup.invalid) return;
    if (!this.template.text) return;
    if ((this.errorSendLimit && !this.noLimit) || this.partnerIds.length == 0) return;
    var val = this.GetValueFormGroup();
    val.body = this.template ? this.template.text : '';
    const modalRef = this.modalService.open(SmsComfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.campaign = val.smsCampaign;
    modalRef.componentInstance.title = "Xác nhận gửi tin nhắn";
    modalRef.componentInstance.brandName = val.smsAccount.brandName;
    modalRef.componentInstance.timeSendSms = val.typeSend == 'manual' ? "Gửi ngay" : this.intlService.formatDate(val.scheduleDateObj, "dd-MM-yyyy HH:mm");;
    modalRef.componentInstance.body = this.template ? this.template.text : '';
    modalRef.componentInstance.numberSms = this.partnerIds ? this.partnerIds.length : 0;
    modalRef.result.then(() => {
      this.smsMessageService.create(val).subscribe(
        (res: any) => {
          this.smsMessageService.actionSendSms(res.id).subscribe(
            () => {
              this.notify("Gửi tin nhắn thành công", true);
              this.activeModal.close();
            }
          )

          if (this.isTemplateCopy) {
            let template = {
              text: val.body,
              templateType: 'text'
            }
            let valueTemplate = {
              name: val.templateName,
              body: JSON.stringify(template),
              type: this.templateTypeTab
            }
            this.smsTemplateService.create(valueTemplate).subscribe(
              () => {
                this.loadSmsTemplate();
              }
            )
          }
        }
      )
    })
  }

  actionReminder() {
    this.submitted = true;
    if (this.formGroup.invalid) return;
    if (!this.template.text) return;
    if ((this.errorSendLimit && !this.noLimit) || this.partnerIds.length == 0) return;
    var val = this.GetValueFormGroup();
    val.body = this.template ? this.template.text : '';
    const modalRef = this.modalService.open(SmsComfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.campaign = val.smsCampaign;
    modalRef.componentInstance.title = "Xác nhận gửi tin nhắn";
    modalRef.componentInstance.brandName = val.smsAccount.brandName;
    modalRef.componentInstance.timeSendSms = val.typeSend == 'manual' ? "Gửi ngay" : this.intlService.formatDate(val.scheduleDateObj, "dd-MM-yyyy HH:mm");;
    modalRef.componentInstance.body = this.template ? this.template.text : '';
    modalRef.componentInstance.numberSms = this.partnerIds ? this.partnerIds.length : 0;
    modalRef.result.then(() => {
      this.smsMessageService.create(val).subscribe(
        (res: any) => {
          this.smsMessageService.actionSchedule(
            {
              id: res.id,
              scheduleDate: this.intlService.formatDate(val.scheduleDateObj, "yyyy-MM-ddTHH:mm:ss")
            }).subscribe(
            () => {
              this.notify("Gửi tin nhắn thành công", true);
              this.activeModal.close();
            }
          )

          if (this.isTemplateCopy) {
            let template = {
              text: val.body,
              templateType: 'text'
            }
            let valueTemplate = {
              name: val.templateName,
              body: JSON.stringify(template),
              type: this.templateTypeTab
            }
            this.smsTemplateService.create(valueTemplate).subscribe(
              () => {
                this.loadSmsTemplate();
              }
            )
          }
        }
      )
    })


  }

  changeTypeSend(event) {
    // if (event.target.value == "automatic") {
    //   this.formGroup.get("dateObj").setValue(new Date());
    // }
  }

  changeSmsCampaign(event) {
    if (event) {
      if (event.limitMessage == 0) {
        this.noLimit = true;
      }
      if (event.state == 'draft' || event.state == 'shutdown') {
        this.notify('Chiến dịch này chưa được kích hoạt hoặc đã bị dừng. Vui lòng kiểm tra lại chiến dịch', false);
        this.formGroup.get('smsCampaign').setValue(null);
      }
      else if (event.typeDate != 'unlimited' && event.limitMessage != 0 && event.limitMessage <= event.totalMessage) {
        this.notify('Hạn mức gửi tin nhắn đã hết. Vui lòng kiểm tra lại chiến dịch', false);
        this.formGroup.get('smsCampaign').setValue(null);
      }
      this.limitMessage = event.limitMessage;
      this.sendLimit = this.limitMessage - event.totalMessage;
      if (this.sendLimit > 0) {
        this.noLimit = false;
      }
      if (this.partnerIds.length == this.sendLimit) {
        this.errorSendLimit = false;
      }
      if (this.partnerIds.length > this.sendLimit && this.sendLimit > 0) {
        this.partnerIds.length = 0;
        this.errorSendLimit = true;
      }
    }
  }
}

