import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { debounceTime, switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'app-sms-birthday-form-automatic',
  templateUrl: './sms-birthday-form-automatic.component.html',
  styleUrls: ['./sms-birthday-form-automatic.component.css']
})
export class SmsBirthdayFormAutomaticComponent implements OnInit {

  @ViewChild("configSMSCbx", { static: true }) configSMSCbx: ComboBoxComponent
  formGroup: FormGroup;
  filteredConfigSMS: any[];
  skip: number = 0;
  id: string;
  limit: number = 20;
  type: string;
  public today: Date = new Date;
  public timeReminder: Date = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDay(), 0, 30, 0);
  public timeRunJob: Date = new Date();
  constructor(
    // private tSMSRmConfigService: TSMSReminderConfigService,
    // private smsService: SmsBrandNameService,
    private fb: FormBuilder,
    private intlService: IntlService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      timeReminder: [this.timeReminder, Validators.required],
      timeRunJob: [this.timeRunJob, Validators.required],
      content: ['', Validators.required],
      configSMS: [null, Validators.required]
    })

    // this.configSMSCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.configSMSCbx.loading = true)),
    //   switchMap(value => this.searchConfigSMS(value))
    // ).subscribe(result => {
    //   this.filteredConfigSMS = result;
    //   this.configSMSCbx.loading = false;
    // });

    this.loadConfigSMS();
    this.loadDataFromApi();
  }

  loadConfigSMS() {
    // this.searchConfigSMS().subscribe(
    //   (res: any) => {
    //     this.filteredConfigSMS = res;
    //     if (!this.formGroup.get('configSMS')) {
    //       this.formGroup.get('configSMS').patchValue(res[0] ? res[0] : null);
    //     }
    //   }
    // )
  }

  searchConfigSMS(q?: string) {
    // var val = {
    //   limit: this.limit,
    //   offset: this.skip,
    //   search: q || ''
    // }
    // return this.smsService.getAutoComplete(val);
  }

  loadDataFromApi() {
    // var type = 'birthday';
    // this.tSMSRmConfigService.get(type).subscribe(
    //   (res: any) => {
    //     if (res && res.id) {
    //       this.id = res.id;
    //       this.formGroup.patchValue(res);
    //       this.timeReminder = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDay(), res.hourReminder, res.minuteReminder, 0);
    //       this.formGroup.get('timeReminder').patchValue(this.timeReminder);
    //     }
    //   }
    // )
  }

  onSave() {
    // if (this.formGroup.invalid) return;
    // var val = this.formGroup.value;
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


}
