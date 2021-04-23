import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
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
    private smsConfigService: SmsConfigService
  ) { }

  ngOnInit() {
    this.loadSmsTemplate();

    this.formGroup = this.fb.group({
      appointmentTemplate: [null, Validators.required],
      isAppointmentAutomation: false
    })

    this.smsTemplateCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsTemplateCbx.loading = true)),
      switchMap(value => this.searchSmsTemplate(value))
    ).subscribe((result: any) => {
      this.filteredTemplate = result;
      this.smsTemplateCbx.loading = false;
    });
  }

  loadSmsTemplate() {
    this.searchSmsTemplate().subscribe(
      (res: any) => {
        this.filteredTemplate = res;
        this.formGroup.get('appointmentTemplate').patchValue(res[0] ? res[0] : null)
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
    this.smsConfigService.create(val).subscribe(
      res => {
        console.log(res);
      }
    )
  }

  addTemplate() {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo mẫu tin';
    modalRef.result.then((val) => {
      this.loadSmsTemplate();
    })
  }

}
