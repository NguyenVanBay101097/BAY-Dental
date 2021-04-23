import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SmsTemplateCrUpComponent } from '../sms-template-cr-up/sms-template-cr-up.component';
import { SmsTemplateService } from '../sms-template.service';

@Component({
  selector: 'app-sms-birthday-form-automatic',
  templateUrl: './sms-birthday-form-automatic.component.html',
  styleUrls: ['./sms-birthday-form-automatic.component.css']
})
export class SmsBirthdayFormAutomaticComponent implements OnInit {

  @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent

  formGroup: FormGroup;
  filteredConfigSMS: any[];
  skip: number = 0;
  id: string;
  limit: number = 20;
  type: string;
  filteredTemplate: any[];
  public today: Date = new Date;
  public timeReminder: Date = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDay(), 0, 30, 0);
  public timeRunJob: Date = new Date();
  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    private smsTemplateService: SmsTemplateService,
  ) { }

  ngOnInit() {
    this.loadSmsTemplate();

    this.formGroup = this.fb.group({
      content: [null, Validators.required],
      isAuto:false
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
        this.formGroup.get('content').patchValue(res[0] ? res[0] : null)
      }
    )
  }

  searchSmsTemplate(q?: string) {
    return this.smsTemplateService.getAutoComplete(q);
  }

  onSave() {
    if (this.formGroup.invalid) return;
    var val = this.formGroup.value;
    console.log(val);
    
    // val.configSMSId = val.configSMS ? val.configSMS.id : null;
    // val.timeReminder = this.intlService.formatDate(val.timeReminder, "yyyy-dd-MMTHH:mm")
    // if (this.id) {
    //   this.tSMSRmConfigService.update(this.id, val).subscribe(
    //     () => {
    //       this.loadDataFromApi();
    //     }
    //   )
    // } else {
    //   this.tSMSRmConfigService.create(val).subscribe(
    //     () => {
    //       this.loadDataFromApi();
    //     }
    //   )
    // }
  }

  addTemplate() {
    const modalRef = this.modalService.open(SmsTemplateCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo mẫu tin';
    modalRef.result.then((val) => {
      this.loadSmsTemplate();
    })
  }

}
