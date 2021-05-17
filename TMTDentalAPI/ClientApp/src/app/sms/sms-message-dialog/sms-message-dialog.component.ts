import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import SmsAccountService, { SmsAccountPaged } from '../sms-account.service';
import { SmsConfigService } from '../sms-config.service';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-message-dialog',
  templateUrl: './sms-message-dialog.component.html',
  styleUrls: ['./sms-message-dialog.component.css']
})
export class SmsMessageDialogComponent implements OnInit {

  @ViewChild("smsAccountCbx", { static: true }) smsAccountCbx: ComboBoxComponent
  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent

  formGroup: FormGroup;
  textareaLimit: number = 200;

  filteredSmsAccount: any[];
  filteredTemplate: any[];

  template: any = {
    text: '',
    templateType: 'text'
  };
  isTemplateCopy = false;

  constructor(
    private fb: FormBuilder, 
    private modalService: NgbModal, 
    private notificationService: NotificationService, 
    private smsAccountService: SmsAccountService, 
    private smsTemplateService: SmsTemplateService, 
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      smsAccount: [null, Validators.required],
      template: [null, Validators.required],
    })
    this.loadAccount();
    this.loadSmsTemplate();

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
}
