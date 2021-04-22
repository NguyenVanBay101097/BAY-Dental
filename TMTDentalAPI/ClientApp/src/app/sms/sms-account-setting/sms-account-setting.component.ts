import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';
import SmsAccountService, { SmsAccountBasic } from '../sms-account.service';

@Component({
  selector: 'app-sms-account-setting',
  templateUrl: './sms-account-setting.component.html',
  styleUrls: ['./sms-account-setting.component.css']
})
export class SmsAccountSettingComponent implements OnInit {
  formGroup: FormGroup;
  switchBrand: string = "esms";
  id: string;
  constructor(
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private smsAccountService: SmsAccountService
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

  onItemChange(event) {
    this.switchBrand = event.target.value;
    this.loadDataFromApi();
  }

  onSave() {
    if (this.formGroup.invalid) return false;
    var val = this.formGroup.value;
    if (this.id) {
      this.smsAccountService.update(this.id, val).subscribe(
        () => {
          this.loadDataFromApi();
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
