import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-template-cr-up',
  templateUrl: './sms-template-cr-up.component.html',
  styleUrls: ['./sms-template-cr-up.component.css']
})
export class SmsTemplateCrUpComponent implements OnInit {
  @ViewChild('textarea', { static: false }) textarea: ElementRef;

  title: string;
  formGroup: FormGroup;
  filteredConfigSMS: any[];
  limit: number = 20;
  skip: number = 0;
  textareaLimit: number = 500;
  id: string;
  submitted: boolean = false;
  templateTypeTab: string = '';
  template: any = {
    text: '',
    templateType: 'text'
  };
  listTemplates = [
    { name: 'Chúc mừng sinh nhật', value: 'partner' },
    { name: 'Nhắc lịch hẹn', value: 'appointment' },
    { name: 'Chăm sóc sau điều trị', value: 'saleOrderLine' },
    { name: 'Cảm ơn', value: 'saleOrder' },
    { name: 'Chiến dịch khác', value: 'partnerCampaign' },
  ]
  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private smsTemplateService: SmsTemplateService,
    private notificationService: NotificationService,

  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      body: ['', Validators.required],
      type: [this.listTemplates[0].value, Validators.required]
    })
    this.templateTypeTab = this.listTemplates[0].value;
    setTimeout(() => {
      if (this.id) {
        this.loadDataFromApi();
      }
    });

  }

  get f() { return this.formGroup.controls; }
  get textValue() { return this.formGroup.get('body').value; }

  loadDataFromApi() {
    this.smsTemplateService.get(this.id).subscribe((res: any) => {
      this.formGroup.patchValue(res);
      this.template = JSON.parse(res.body);
      this.f.body.setValue(this.template.text);
    });
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) { return false; }
    var formValue = this.formGroup.value;
    if (this.id) {
      this.smsTemplateService.update(this.id, formValue).subscribe(
        (res) => {
          this.notify('Lưu tin nhắn mẫu thành công', true);
          this.activeModal.close(formValue);
        }
      );
    }
    else {
      this.smsTemplateService.create(formValue).subscribe(result => {
        this.notify("Lưu tin nhắn mẫu thành công", true)
        this.activeModal.close(result);
      }, err => { });
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

  valueChange(value) {
    this.getTextareaLimit(value.type);
  }

  getTextareaLimit(type) {
    if (type == 'e-sms') {
      this.textareaLimit = 650;
    }
    else if (type == 'vietguys') {
      this.textareaLimit = 600;
    }

  }
  selectTemplate(value) {
    this.templateTypeTab = value;
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

  onCancel() {
    this.activeModal.dismiss();
  }
}
