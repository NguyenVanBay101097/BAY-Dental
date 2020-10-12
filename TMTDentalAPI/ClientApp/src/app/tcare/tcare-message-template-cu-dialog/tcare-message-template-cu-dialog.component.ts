import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { validate } from 'fast-json-patch';
import { stringify } from 'querystring';
import { TCareMessageTemplatePaged, TCareMessageTemplateService } from '../tcare-message-template.service';

@Component({
  selector: 'app-tcare-message-template-cu-dialog',
  templateUrl: './tcare-message-template-cu-dialog.component.html',
  styleUrls: ['./tcare-message-template-cu-dialog.component.css']
})
export class TcareMessageTemplateCuDialogComponent implements OnInit {
  id: string;
  title: string;
  formGroup: FormGroup;
  templates: any[] = [
    {
      text: null,
      templateType: 'text'
    }
  ];
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private templateService: TCareMessageTemplateService,
    private notificationService: NotificationService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      content: [this.templates, Validators.required]
    });

    setTimeout(() => {
      if (this.id) {
        this.loadDataFromApi();
      }
    });

  }

  get contentControl() { return this.formGroup.get('content'); }

  loadDataFromApi() {
    this.templateService.get(this.id).subscribe((res: any) => {
      this.formGroup.patchValue(res);
      this.templates = JSON.parse(res.content);
      this.contentControl.setValue(this.templates);
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

  onSave() {
    if (this.formGroup.invalid) { return false; }
    const val = this.formGroup.value;
    val.content = JSON.stringify(this.templates);

    if (this.id) {
      this.templateService.update(this.id, val).subscribe(
        (res) => {
          this.notify('thành công', true);
          this.activeModal.close(val);
        }
      );
    } else {
      this.templateService.create(val).subscribe((res) => {
        this.notify('thành công', true);
        this.activeModal.close(val);
      });
    }
  }

  TemplateValueChange(e) {
    this.templates[e.index] = e.template;
  }

}
