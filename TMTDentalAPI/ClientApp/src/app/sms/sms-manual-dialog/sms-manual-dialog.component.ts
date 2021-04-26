import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SmsComposerService } from '../sms-composer.service';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-manual-dialog',
  templateUrl: './sms-manual-dialog.component.html',
  styleUrls: ['./sms-manual-dialog.component.css']
})
export class SmsManualDialogComponent implements OnInit {
  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent

  title: string;
  filteredTemplate: any[];
  formGroup: FormGroup;
  ids: string[] = [];
  submitted = false;
  provider: string;
  template: any = {
    templateType: 'text',
    text: ''
  }
  textareaLimit: number = 600;

  constructor(
    public activeModal: NgbActiveModal,
    private smsTemplateService: SmsTemplateService,
    private smsComposerService: SmsComposerService,
    private modalService: NgbModal,
    private fb: FormBuilder,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      template: null,
      body: [this.template, Validators.required]
    })

    this.loadSmsTemplate();

    this.smsTemplateCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsTemplateCbx.loading = true)),
      switchMap(value => this.searchSmsTemplate(value))
    ).subscribe((result: any) => {
      this.filteredTemplate = result;
      this.smsTemplateCbx.loading = false;
    });
  }

  searchSmsTemplate(q?: string) {
    return this.smsTemplateService.getAutoComplete(q);
  }

  addTemplate() {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo mẫu tin';
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
  get f() { return this.formGroup.controls; }

  valueChange(val) {
    if (val) {
      this.template = JSON.parse(val.body);
    }
    else {
      this.template = {
        templateType: 'text',
        text: ''
      }
    }
    this.formGroup.controls['body'].setValue(this.template);
  }

  getLimitText() {
    var text = this.formGroup.get('body').value;
    if (text) {
      return this.textareaLimit - text.length;
    } else {
      return this.textareaLimit;
    }
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) return false;

    var val = this.formGroup.value;
    val.body = JSON.stringify(val.body);
    val.resIds = this.ids ? this.ids.join(',') : '';
    val.resModel = this.provider;
    if (val.template) {
      val.templateId = val.template.id;
    }
    this.smsComposerService.create(val).subscribe(
      res => {
        this.notify("gửi tin thành công", true);
        this.activeModal.close(res);
      }
    )
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
