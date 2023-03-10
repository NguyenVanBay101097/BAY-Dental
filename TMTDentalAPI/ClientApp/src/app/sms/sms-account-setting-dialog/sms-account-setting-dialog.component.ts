import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SmsAccountService, SmsAccountBasic } from '../sms-account.service';

@Component({
  selector: 'app-sms-account-setting-dialog',
  templateUrl: './sms-account-setting-dialog.component.html',
  styleUrls: ['./sms-account-setting-dialog.component.css']
})
export class SmsAccountSettingDialogComponent implements OnInit {

  formGroup: FormGroup;
  switchBrand: string = "fpt";
  id: string;
  submitted: boolean = false;
  title: string;
  constructor(
    private fb: FormBuilder,
    private smsAccountService: SmsAccountService,
    public activeModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      provider: [this.switchBrand, Validators.required],
      brandName: ['', Validators.required],
      clientId: ['', Validators.required],
      clientSecret: ['', Validators.required],
      userName: '',
      password: '',
      apiKey: '',
      secretkey: '',
    });

    if (this.id) {
      this.loadDataFromApi();
    }
  }

  get f() {
    return this.formGroup.controls
  }

  loadDataFromApi() {
    this.smsAccountService.getDisplay(this.id).subscribe(
      (result: any) => {
        var obj = new SmsAccountBasic();
        this.formGroup.patchValue(obj);
        if (result) {
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
    if (this.switchBrand == 'fpt' && (this.f.clientId.value == '' || this.f.clientSecret.value == ''))
      return;
    else if (this.switchBrand == 'esms' && (this.f.apiKey.value == '' || this.f.secretkey.value == ''))
      return;
    else {
      value = this.formGroup.value;
      value.name = this.switchBrand == "fpt" ? "FPT" : "E-SMS";
    }
    return value;
  }

  onSave() {
    this.submitted = true;

    if (this.formGroup.invalid)
      return false;

    var val = this.getFormGroup();
    if (!val)
      return;

    if (this.id) {
      this.smsAccountService.update(this.id, val).subscribe(
        () => {
          this.loadDataFromApi();
          this.activeModal.close();
        }
      )
    } else {
      this.smsAccountService.create(val).subscribe(
        () => {
          this.loadDataFromApi();
          this.activeModal.close();
        }
      )
    }
  }

}
