import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SmsAccountService, SmsAccountPaged } from '../sms-account.service';
import { SmsCampaignService } from '../sms-campaign.service';
import { SmsComfirmDialogComponent } from '../sms-comfirm-dialog/sms-comfirm-dialog.component';
import { SmsComposerService } from '../sms-composer.service';
import { SmsMessageService } from '../sms-message.service';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService, SmsTemplateFilter } from '../sms-template.service';

@Component({
  selector: 'app-sms-manual-dialog',
  templateUrl: './sms-manual-dialog.component.html',
  styleUrls: ['./sms-manual-dialog.component.css']
})
export class SmsManualDialogComponent implements OnInit {
  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent
  @ViewChild("smsAccountCbx", { static: true }) smsAccountCbx: ComboBoxComponent

  title: string;
  filteredTemplate: any[];
  filteredSmsAccount: any[];
  formGroup: FormGroup;
  smsMessageDetail: any
  submitted = false;

  campaign: any;
  resIds: string[] = [];
  resModel: string;

  isTemplateCopy = false;
  templateTypeTab: string;
  template: any = {
    templateType: 'text',
    text: ''
  }
  textareaLimit: number = 459;
  get f() { return this.formGroup.controls; }

  constructor(
    public activeModal: NgbActiveModal,
    private smsTemplateService: SmsTemplateService,
    private smsMessageService: SmsMessageService,
    private modalService: NgbModal,
    private fb: FormBuilder,
    private smsAccountService: SmsAccountService,
    private notificationService: NotificationService,
    private intlService: IntlService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      template: null,
      smsAccount: [null, Validators.required],
      name: ['', Validators.required],
      templateName: ''
    })

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

  checkedTemplateCopy(event) {
    var check = event.target.checked
    if (check) {
      this.isTemplateCopy = true;
    } else {
      this.isTemplateCopy = false;
    }

  }


  searchSmsTemplate(q?: string) {
    var filter = new SmsTemplateFilter();
    filter.search = q || "";
    filter.type = this.templateTypeTab;
    return this.smsTemplateService.getAutoComplete(filter);
  }

  addTemplate() {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo mẫu tin';
    modalRef.componentInstance.templateTypeTab = this.templateTypeTab;
    modalRef.result.then((val) => {
      this.loadSmsTemplate();
    })
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

  getLimitText() {
    var text = this.formGroup.get('body').value;
    if (text) {
      return this.textareaLimit - text.length;
    } else {
      return this.textareaLimit;
    }
  }

  loadAccount() {
    this.searchAccount().subscribe(
      (result: any) => {
        if (result && result.items) {
          this.filteredSmsAccount = result.items
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

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) return false;
    if (!this.template.text) return;
    var val = this.formGroup.value;
    if (this.isTemplateCopy && val.templateName == '') {
      return false
    };
    val.smsAccountId = val.smsAccount ? val.smsAccount.id : null;
    val.smsTemplateId = val.template ? val.template.id : null;
    val.smsCampaignId = this.campaign ? this.campaign.id : null;
    val.resModel = this.resModel;
    val.date = this.intlService.formatDate(new Date(), "yyyy-MM-ddTHH:mm");
    val.resIds = this.resIds;
    // val.body = JSON.stringify(this.template);
    val.body = this.template ? this.template.text : '';
    const modalRef = this.modalService.open(SmsComfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = "Xác nhận gửi tin nhắn";
    modalRef.componentInstance.campaign = this.campaign ? this.campaign : null;
    modalRef.componentInstance.brandName = val.smsAccount.brandName;
    modalRef.componentInstance.timeSendSms = "Gửi ngay";
    modalRef.componentInstance.body = this.template ? this.template.text : '';
    modalRef.componentInstance.numberSms = this.resIds ? this.resIds.length : 0;
    modalRef.result.then(() => {
      this.smsMessageService.create(val).subscribe(
        (res: any) => {
          this.smsMessageService.actionSendSms(res.id).subscribe(
            () => {
              this.notify("Gửi tin nhắn thành công", true);
              this.activeModal.close(res);
            }
          )
          if (this.isTemplateCopy && val.templateName != '') {
            var valueTemplate = {
              name: val.templateName,
              body: val.body,
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
    });
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
  onCancel() {
    this.activeModal.dismiss();
  }
}
