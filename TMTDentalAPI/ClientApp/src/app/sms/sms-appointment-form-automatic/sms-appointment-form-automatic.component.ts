import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SmsAccountPaged, SmsAccountService } from '../sms-account.service';
import { SmsAppointmentAutomationConfigService } from '../sms-appointment-automation-config.service';
import { SmsCampaignService } from '../sms-campaign.service';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateFilter, SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-appointment-form-automatic',
  templateUrl: './sms-appointment-form-automatic.component.html',
  styleUrls: ['./sms-appointment-form-automatic.component.css']
})
export class SmsAppointmentFormAutomaticComponent implements OnInit {

  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent
  @ViewChild("smsAccountCbx", { static: true }) smsAccountCbx: ComboBoxComponent
  @ViewChild('textarea') textarea: ElementRef;

  formGroup: FormGroup;
  filteredConfigSMS: any[];
  filteredSmsAccount: any[];
  skip: number = 0;
  id: string;
  campaign: any;
  limit: number = 20;
  type: string;
  filteredTemplate: any[];
  textareaLimit: number = 200;
  template: any = {
    text: '',
    templateType: 'text'
  };
  isTemplateCopy = false;
  submitted: boolean = false;
  public today: Date = new Date;
  public timeReminder: Date = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDay(), 0, 30, 0);
  public timeRunJob: Date = new Date();

  get f() { return this.formGroup.controls; }
  get textValue() { return this.formGroup.get('body').value; }

  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    private smsTemplateService: SmsTemplateService,
    private smsConfigService: SmsAppointmentAutomationConfigService,
    private smsAccountService: SmsAccountService,
    private notificationService: NotificationService,
    private smsCampaignService: SmsCampaignService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      template: [null, Validators.required],
      smsAccount: [null, Validators.required],
      active: false,
      TypeTimeBeforSend: 'hour',
      timeBeforSend: [1, Validators.required],
      templateName: '',
    })

    this.loadDataFormApi();
    this.loadSmsTemplate();
    this.loadDefaultCampaignAppointmentReminder();
    this.loadAccount();
    this.smsTemplateCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsTemplateCbx.loading = true)),
      switchMap(value => this.searchSmsTemplate(value))
    ).subscribe((result: any) => {
      this.filteredTemplate = result;
      this.smsTemplateCbx.loading = false;
    });

    this.smsAccountCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsAccountCbx.loading = true)),
      switchMap(value => this.searchAccount(value))
    ).subscribe((result: any) => {
      this.filteredSmsAccount = result ? result.items : [];
      this.smsAccountCbx.loading = false;
    });
  }

  loadDefaultCampaignAppointmentReminder() {
    this.smsCampaignService.getDefaultCampaignAppointmentReminder().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      })
  }

  checkedTemplateCopy(event) {
    var check = event.target.checked
    if (check) {
      this.isTemplateCopy = true;
      this.f.templateName.setValidators(Validators.required);
      this.f.templateName.updateValueAndValidity();

    } else {
      this.isTemplateCopy = false;
      this.f.templateName.clearValidators();
      this.f.templateName.updateValueAndValidity();
    }

  }

  loadDataFormApi() {
    this.smsConfigService.getByCompany().subscribe(
      (res: any) => {
        if (res) {
          this.id = res.id;
          this.formGroup.patchValue(res);
          if (res.dateSend) {
            this.formGroup.get('dateTimeSend').patchValue(new Date(res.dateSend))
          }
        } else {
          this.id = null;
        }
      }
    )
  }

  loadAccount() {
    this.searchAccount().subscribe(
      (result: any) => {
        if (result && result.items) {
          this.filteredSmsAccount = result.items;
          if (result.items[0]) {
            this.formGroup.get('smsAccount').patchValue(result.items[0]);
          }
        }
      }
    )
  }

  searchAccount(q?: string) {
    var val = new SmsAccountPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = q || '';
    return this.smsAccountService.getPaged(val);
  }

  loadSmsTemplate() {
    this.searchSmsTemplate().subscribe(
      (result: any) => {
        this.filteredTemplate = result;
      }
    )
  }

  searchSmsTemplate(q?: string) {
    var filter = new SmsTemplateFilter();
    filter.search = q || "";
    filter.type = "appointment";
    return this.smsTemplateService.getAutoComplete(filter);
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) return;
    var val = this.formGroup.value;
    val.smsAccountId = val.smsAccount ? val.smsAccount.id : null;
    val.timeBeforSend = Number.parseInt(val.timeBeforSend);
    val.templateId = val.template ? val.template.id : null;
    val.smsCampaignId = this.campaign ? this.campaign.id : null;
    this.smsConfigService.saveConfig(val).subscribe(
      res => {
        this.notify("L??u th??nh c??ng", true);
        this.loadDataFormApi();
      }
    )
  }

  addTemplate() {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'T???o m???u tin';
    modalRef.componentInstance.templateTypeTab = "appointment";
    modalRef.result.then((val) => {
      this.loadSmsTemplate();
    })
  }

  addToContent(tabValue) {
    const selectionStart = this.textarea.nativeElement.selectionStart;
    const selectionEnd = this.textarea.nativeElement.selectionEnd;
    var tabValueNew = tabValue;
    if (this.textValue) {
      tabValueNew = ((selectionStart > 0 && this.textValue[selectionStart - 1] == ' ') ? "" : " ")
        + tabValue
        + (this.textValue[selectionEnd] == ' ' ? "" : " ");
      this.f.body.setValue(
        this.textValue.slice(0, selectionStart)
        + tabValueNew
        + this.textValue.slice(this.textarea.nativeElement.selectionEnd)
      );
    } else {
      this.f.body.setValue(tabValue);
    }

    this.textarea.nativeElement.focus();
    this.textarea.nativeElement.setSelectionRange(selectionStart + tabValueNew.length, selectionStart + tabValueNew.length);
  }

  get templateValue() {
    return this.formGroup.get('template').value;
  }

  changeTimeBeforSend(event) {
    if (+event.target.value <= 0) {
      this.formGroup.get('timeBeforSend').setValue(1);
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

}
