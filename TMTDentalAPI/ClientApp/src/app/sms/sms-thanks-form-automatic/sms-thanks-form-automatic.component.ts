import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SmsAccountPaged, SmsAccountService } from '../sms-account.service';
import { SmsCampaignService } from '../sms-campaign.service';
import { SmsConfigService } from '../sms-config.service';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService, SmsTemplateFilter } from '../sms-template.service';
import { SmsThanksCustomerAutomationConfigService } from '../sms-thanks-customer-automation-config.service';

@Component({
  selector: 'app-sms-thanks-form-automatic',
  templateUrl: './sms-thanks-form-automatic.component.html',
  styleUrls: ['./sms-thanks-form-automatic.component.css']
})
export class SmsThanksFormAutomaticComponent implements OnInit {

  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent
  @ViewChild("smsAccountCbx", { static: true }) smsAccountCbx: ComboBoxComponent
  @ViewChild('textarea', { static: false }) textarea: ElementRef;

  formGroup: FormGroup;
  filteredSmsAccount: any[];
  id: string;
  type: string;
  companyId: string;
  filteredTemplate: any[];
  textareaLimit: number = 200;
  template: any =
    {
      text: '',
      templateType: 'text'
    };
  isTemplateCopy = false;
  today: Date = new Date;
  timeReminder: Date = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDay(), 0, 30, 0);
  timeRunJob: Date = new Date();
  campaign: any;
  submitted: boolean = false;
  get f() { return this.formGroup.controls; }
  get textValue() { return this.formGroup.get('body').value; }

  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    private smsTemplateService: SmsTemplateService,
    private smsConfigService: SmsThanksCustomerAutomationConfigService,
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
      typeTimeBeforSend: 'hour',
      timeBeforSend: [1, Validators.required],
      templateName: '',
    })
  
    this.loadDataFormApi();
    this.loadSmsTemplate();
    this.loadDefaultCampaignThanksCustomer();
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

  loadDefaultCampaignThanksCustomer() {
    this.smsCampaignService.getDefaultThanksCustomer().subscribe(
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
      }
    )
  }

  searchSmsTemplate(q?: string) {
    var filter = new SmsTemplateFilter();
    filter.search = q || "";
    filter.type = "saleOrder";
    return this.smsTemplateService.getAutoComplete(filter);
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) { return false; };
    var val = this.formGroup.value;
    val.smsAccountId = val.smsAccount ? val.smsAccount.id : null;
    val.timeBeforSend = Number.parseInt(val.timeBeforSend);
    val.templateId = val.template ? val.template.id : null;
    val.smsCampaignId = this.campaign ? this.campaign.id : null;
    this.smsConfigService.saveConfig(val).subscribe(
      res => {
        this.notify("Thiết lập thành công", true);
      }
    )
  }

  addTemplate() {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo mẫu tin';
    modalRef.componentInstance.templateTypeTab = "saleOrder";
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
