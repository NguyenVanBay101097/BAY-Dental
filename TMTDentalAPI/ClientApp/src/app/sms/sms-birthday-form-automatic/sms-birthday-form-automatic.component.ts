import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { validator } from 'fast-json-patch';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import SmsAccountService, { SmsAccountPaged } from '../sms-account.service';
import { SmsConfigService } from '../sms-config.service';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-birthday-form-automatic',
  templateUrl: './sms-birthday-form-automatic.component.html',
  styleUrls: ['./sms-birthday-form-automatic.component.css']
})
export class SmsBirthdayFormAutomaticComponent implements OnInit {

  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent
  @ViewChild("smsAccountCbx", { static: true }) smsAccountCbx: ComboBoxComponent

  formGroup: FormGroup;
  filteredConfigSMS: any[];
  filteredSmsAccount: any[];
  skip: number = 0;
  id: string;
  limit: number = 20;
  type: string;
  filteredTemplate: any[];
  textareaLimit: number = 200;
  template: any =
    {
      text: '',
      templateType: 'text'
    };
  public today: Date = new Date;
  public timeReminder: Date = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDay(), 0, 30, 0);
  public timeRunJob: Date = new Date();
  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    private smsTemplateService: SmsTemplateService,
    private smsConfigService: SmsConfigService,
    private intlService: IntlService,
    private smsAccountService: SmsAccountService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      template: [null, Validators.required],
      smsAccount: [null, Validators.required],
      isBirthdayAutomation: false,
      thoiDiem: new Date(),
      thoiGian: 0,
    })
    this.loadDataFormApi();
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
      this.filteredSmsAccount = result;
      this.smsAccountCbx.loading = false;
    });
  }

  loadDataFormApi() {
    this.smsConfigService.getConfigByCompany().subscribe(
      (res: any) => {
        if (res) {
          this.id = res.id;
          this.formGroup.patchValue(res);
          if (res.template) {
            this.template = JSON.parse(res.template.body);
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
          this.filteredSmsAccount = result.items
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

  onSave() {
    if (this.formGroup.invalid) return;
    var val = this.formGroup.value;
    val.smsAccountId = val.smsAccount ? val.smsAccount.id : null;
    val.thoiDiem = this.intlService.formatDate(val.thoiDiem, "yyyy-MM-ddTHH:mm");
    val.thoiGian = Number.parseInt(val.thoiGian);
    val.templateId = val.template ? val.template.id : null;
    val.body = this.template ? JSON.stringify(this.template) : '';
    console.log(val);
    return
    if (this.id) {
      this.smsConfigService.update(this.id, val).subscribe(
        res => {
          // console.log(res);
          this.notify("cập nhật thiết lập thành công", true);
        }
      )
    } else {
      this.smsConfigService.create(val).subscribe(
        res => {
          // console.log(res);
          this.notify("thiết lập thành công", true);
        }
      )
    }

  }

  addTemplate() {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo mẫu tin';
    modalRef.result.then((val) => {
      this.loadSmsTemplate();
    })
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
