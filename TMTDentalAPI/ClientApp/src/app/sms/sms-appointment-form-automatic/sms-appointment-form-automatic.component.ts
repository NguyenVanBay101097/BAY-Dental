import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SmsConfigService } from '../sms-config.service';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-appointment-form-automatic',
  templateUrl: './sms-appointment-form-automatic.component.html',
  styleUrls: ['./sms-appointment-form-automatic.component.css']
})
export class SmsAppointmentFormAutomaticComponent implements OnInit {
  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent
  formGroup: FormGroup;
  filteredConfigSMS: any[];
  skip: number = 0;
  id: string;
  limit: number = 20;
  type: string;
  filteredTemplate: any[];
  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    private smsTemplateService: SmsTemplateService,
    private smsConfigService: SmsConfigService,
    private notificationService: NotificationService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      appointmentTemplate: [null, Validators.required],
      isAppointmentAutomation: false,
      isBirthdayAutomation: false,
      birthdayTemplateId: null
    })

    this.loadDataFormApi();
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

  loadDataFormApi() {
    this.smsConfigService.getConfigByCompany().subscribe(
      (res: any) => {
        if (res) {
          this.id = res.id;
          this.formGroup.patchValue(res);
        } else {
          this.id = null;
        }
      }
    )
  }

  loadSmsTemplate() {
    this.searchSmsTemplate().subscribe(
      (res: any) => {
        this.filteredTemplate = res;
      }
    )
  }

  searchSmsTemplate(q?: string) {
    return this.smsTemplateService.getAutoComplete(q);
  }

  onSave() {
    if (this.formGroup.invalid) return;
    var val = this.formGroup.value;
    val.appointmentTemplateId = val.appointmentTemplate.id;
    if (this.id) {
      this.smsConfigService.update(this.id, val).subscribe(
        res => {
          this.notify('cập nhật thành công', true);
          console.log(res);
        }
      )
    } else {
      this.smsConfigService.create(val).subscribe(
        res => {
          this.notify("thêm mới thành công", true);
          console.log(res);
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
