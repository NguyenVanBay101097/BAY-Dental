import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ResConfigSettingsService } from '../res-config-settings.service';
import { AuthService } from 'src/app/auth/auth.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-res-config-settings-form',
  templateUrl: './res-config-settings-form.component.html',
  styleUrls: ['./res-config-settings-form.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ResConfigSettingsFormComponent implements OnInit {
  formGroup: FormGroup;
  constructor(private fb: FormBuilder, private configSettingsService: ResConfigSettingsService,
    private authService: AuthService, private notificationService: NotificationService) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      groupDiscountPerSOLine: false,
      groupLoyaltyCard: false,
      groupSaleCouponPromotion: false,
      loyaltyPointExchangeRate: 0,
      groupMultiCompany: false,
      companySharePartner: false,
      companyShareProduct: false,
      groupServiceCard: false
    });

    this.configSettingsService.defaultGet().subscribe(result => {
      this.formGroup.patchValue(result);
    });
  }

  onSave() {
    var val = this.formGroup.value;
    this.configSettingsService.create(val).subscribe(result => {
      this.configSettingsService.excute(result.id).subscribe(() => {
        this.authService.getGroups().subscribe(result => {
          localStorage.setItem('groups', JSON.stringify(result));
          window.location.reload();
        });
      });
    })
  }

  get groupLoyaltyCard() {
    return this.formGroup.get('groupLoyaltyCard').value;
  }

  get groupMultiCompany() {
    return this.formGroup.get('groupMultiCompany').value;
  }
}
