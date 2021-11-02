import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SmsAccountPaged, SmsAccountService } from '../sms-account.service';
import { SmsBirthdayAutomationConfigService } from '../sms-birthday-automation-config.service';
import { SmsCampaignService } from '../sms-campaign.service';
import { SmsTemplateFilter, SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-birthday-form-automatic',
  templateUrl: './sms-birthday-form-automatic.component.html',
  styleUrls: ['./sms-birthday-form-automatic.component.css']
})
export class SmsBirthdayFormAutomaticComponent implements OnInit {

  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent
  @ViewChild("smsAccountCbx", { static: true }) smsAccountCbx: ComboBoxComponent
  @ViewChild('textarea') textarea: ElementRef;

  formGroup: FormGroup;
  filteredConfigSMS: any[];
  filteredSmsAccount: any[];
  skip: number = 0;
  id: string;
  limit: number = 20;
  type: string;
  filteredTemplate: any[];
  companyId: string;
  textareaLimit: number = 200;
  campaign: any;
  isTemplateCopy = false;
  template: any = {
    text: '',
    templateType: 'text'
  };
  submitted: boolean = false;
  public today: Date = new Date;
  public timeReminder: Date = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDay(), 0, 30, 0);
  public timeRunJob: Date = new Date();
  get f() { return this.formGroup.controls; }
  get textValue() { return this.formGroup.get('body').value; }

  constructor(
    private fb: FormBuilder,
    private smsTemplateService: SmsTemplateService,
    private smsConfigService: SmsBirthdayAutomationConfigService,
    private intlService: IntlService,
    private smsAccountService: SmsAccountService,
    private notificationService: NotificationService,
    private smsCampaignService: SmsCampaignService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      template: [null, Validators.required],
      smsAccount: [null, Validators.required],
      active: false,
      scheduleTimeObj: new Date(),
      dayBeforeSend: 0,
      templateName: '',
      body: ''
    })
    var user_change_company_vm = localStorage.getItem('user_change_company_vm');
    if (user_change_company_vm) {
      var companyInfo = JSON.parse(user_change_company_vm);
      this.companyId = companyInfo.currentCompany.id;
    }
    this.loadDataFormApi();
    this.loadDefaultCampaignBirthday();
    this.loadSmsTemplate();
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

  loadDefaultCampaignBirthday() {
    this.smsCampaignService.getDefaultCampaignBirthday().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      })
  }

  loadDataFormApi() {
    this.smsConfigService.getByCompany().subscribe(
      (res: any) => {
        if (res) {
          this.id = res.id;
          this.formGroup.patchValue(res);
          this.formGroup.get('scheduleTimeObj').patchValue(new Date(res.scheduleTime));
          if (res.body) {
            this.template = {
              text: res.body,
              templateType: 'text'
            }
          }
          if (res.dateSend) {
            this.formGroup.get('dateTimeSend').patchValue(new Date(res.dateSend))
          }
        } else {
          this.id = null;
        }
      }
    )
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

  get templateValue() {
    return this.formGroup.get('template').value;
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
        if (result) {
          this.formGroup.get('template').patchValue(result[0]);
          this.onChangeTemplate(result[0])
        }
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
    this.f.body.setValue(this.template.text);
  }

  searchSmsTemplate(q?: string) {
    var filter = new SmsTemplateFilter();
    filter.search = q || "";
    filter.type = "partner";
    return this.smsTemplateService.getAutoComplete(filter);
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) return;
    var val = this.formGroup.value;
    val.smsAccountId = val.smsAccount ? val.smsAccount.id : null;
    val.scheduleTime = this.intlService.formatDate(val.scheduleTimeObj, "yyyy-MM-ddTHH:mm");
    val.timeBeforSend = Number.parseInt(val.timeBeforSend);
    val.companyId = this.companyId;
    val.templateId = val.template ? val.template.id : null;
    val.smsCampaignId = this.campaign ? this.campaign.id : null;
    this.smsConfigService.saveConfig(val).subscribe(
      res => {
        this.notify("Lưu thành công", true);
        this.loadDataFormApi();
      }
    )

    if (this.isTemplateCopy && val.templateName != '') {
      var template = {
        text: val.body,
        templateType: 'text'
      }
      var valueTemplate = {
        name: val.templateName,
        body: JSON.stringify(template),
        type: "partner"
      }
      this.smsTemplateService.create(valueTemplate).subscribe(
        () => {
          this.loadSmsTemplate();
        }
      )
    }
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
