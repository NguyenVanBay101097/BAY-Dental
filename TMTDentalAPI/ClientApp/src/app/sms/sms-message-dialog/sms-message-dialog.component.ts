import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SmsAccountPaged, SmsAccountService } from '../sms-account.service';
import { SmsCampaignPaged, SmsCampaignService } from '../sms-campaign.service';
import { SmsMessageService } from '../sms-message.service';
import { SmsPartnerListDialogComponent } from '../sms-partner-list-dialog/sms-partner-list-dialog.component';
import { SmsTemplateService } from '../sms-template.service';

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
  textareaLimit: number = 200;

  filteredSmsCampaign: any[];
  filteredSmsAccount: any[];
  filteredTemplate: any[];

  template: any = {
    text: '',
    templateType: 'text'
  };
  isTemplateCopy = false;

  partnerIds: any = [];

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
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      smsCampaign: [null, Validators.required],
      smsAccount: [null, Validators.required],
      template: null,
      typeSend: "manual", // manual: gửi ngay, reminder: đặt lịch
      dateObj: [new Date(), Validators.required]
    })
    this.loadCampaign();
    this.loadAccount();
    this.loadSmsTemplate();

    this.smsCampaignCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsCampaignCbx.loading = true)),
      switchMap(value => this.searchCampaign(value))
    ).subscribe((result: any) => {
      this.filteredSmsCampaign = result;
      this.smsCampaignCbx.loading = false;
    });

    this.smsAccountCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsAccountCbx.loading = true)),
      switchMap(value => this.searchAccount(value))
    ).subscribe((result: any) => {
      this.filteredSmsAccount = result;
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
    return this.smsTemplateService.getAutoComplete(q);
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
    })
  }

  onConfirm() {
    if (this.formGroup.invalid) return;
    var val = this.formGroup.value;
    val.smsCampaignId = val.smsCampaign ? val.smsCampaign.id : null;
    val.smsAccountId = val.smsAccount ? val.smsAccount.id : null;
    val.smsTemplateId = val.template ? val.template.id : null;
    val.body = this.template ? JSON.stringify(this.template) : '';
    val.partnerIds = this.partnerIds;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm");
    console.log(val);

    this.smsMessageService.create(val).subscribe(
      res => {
        this.notify("Thành công", true);
      }
    )
  }
}
