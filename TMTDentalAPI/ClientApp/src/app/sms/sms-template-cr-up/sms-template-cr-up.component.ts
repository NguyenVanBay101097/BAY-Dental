import { Component, OnInit, ViewChild } from '@angular/core';
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
  title: string;
  formGroup: FormGroup;
  filteredConfigSMS: any[];
  limit: number = 20;
  skip: number = 0;
  textareaLimit: number = 500;
  id: string;
  templates: any[] = [
    {
      text: null,
      templateType: 'text'
    }
  ];
  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private smsTemplateService: SmsTemplateService,
    private notificationService: NotificationService,
    
  ) { }

  ngOnInit() {

    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      body: [this.templates, Validators.required],
    })

    setTimeout(() => {
      if (this.id) {
        this.loadDataFromApi();
      }
    });

  }

  get f() { return this.formGroup.controls; }

  get bodyControl() { return this.formGroup.get('body'); }

  loadDataFromApi() {
    this.smsTemplateService.get(this.id).subscribe((res: any) => {
      this.formGroup.patchValue(res);
      this.templates = JSON.parse(res.body);
      this.bodyControl.setValue(this.templates);
    });
  }

  onSave() {
    if (this.formGroup.invalid) { return false; }
    var formValue = this.formGroup.value;
    formValue.body = JSON.stringify(this.templates);
    
    if (this.id) {
      this.smsTemplateService.update(this.id, formValue).subscribe(
        (res) => {
          this.notify('thành công', true);
          this.activeModal.close(formValue);
        }
      );
    }
    else {
      this.smsTemplateService.create(formValue).subscribe(result => {
        this.notify("thành công", true)
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

}
