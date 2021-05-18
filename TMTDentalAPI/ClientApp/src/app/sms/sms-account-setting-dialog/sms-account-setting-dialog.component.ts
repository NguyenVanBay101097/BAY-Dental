import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import  { SmsAccountBasic, SmsAccountService } from '../sms-account.service';

@Component({
  selector: 'app-sms-account-setting-dialog',
  templateUrl: './sms-account-setting-dialog.component.html',
  styleUrls: ['./sms-account-setting-dialog.component.css']
})
export class SmsAccountSettingDialogComponent implements OnInit {

  formGroup: FormGroup;
  switchBrand: string = "esms";
  id: string;
  constructor(
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private smsAccountService: SmsAccountService,
    private activeModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      provider: [this.switchBrand, Validators.required],
      brandName: ['', Validators.required],
      clientId: '',
      clientSecret: '',
      userName: '',
      password: '',
      apiKey: '',
      secretkey: '',
    });
    this.loadDataFromApi();
  }

  get f() {
    return this.formGroup.controls
  }

  loadDataFromApi() {
    this.smsAccountService.get(this.switchBrand).subscribe(
      (result: any) => {
        var obj = new SmsAccountBasic();
        this.formGroup.patchValue(obj);
        if (result) {
          this.id = result.id
          this.formGroup.patchValue(result);
          this.formGroup.get('provider').patchValue(result.provider);
        } else {
          this.id = null;
          this.formGroup.get('brandName').patchValue('');
          this.formGroup.get('provider').patchValue(this.switchBrand);
        }
      }
    )
  }

  onItemChange(type) {
    this.switchBrand = type;
    this.loadDataFromApi();
  }

  getFormGroup() {
    var value: any;
    if (this.switchBrand == 'fpt' && (this.f.clientId.value == '' || this.f.clientSecret.value == '')) return;
    else if (this.switchBrand == 'esms' && (this.f.apiKey.value == '' || this.f.secretkey.value == '')) return;
    else {
      value = this.formGroup.value;
      value.name = this.switchBrand =="fpt" ? "FPT" : "E-SMS";
    }
    return value;
  }

  onSave() {
    if (this.formGroup.invalid) return false;
    var val = this.getFormGroup();
    if (!val) return;
    if (this.id) {
      this.smsAccountService.update(this.id, val).subscribe(
        () => {
          this.loadDataFromApi();
          this.activeModal.close();
          this.notificationService.show({
            content: 'Cập nhật cấu hình thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        }
      )
    } else {
      this.smsAccountService.create(val).subscribe(
        () => {
          this.loadDataFromApi();
          this.activeModal.close();
          this.notificationService.show({
            content: 'Cấu hình thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        }
      )
    }
  }

}
