import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ResConfigSettingsService } from '../res-config-settings.service';
import { AuthService } from 'src/app/auth/auth.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { IntlService } from '@progress/kendo-angular-intl';

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
    private authService: AuthService, private notificationService: NotificationService, private intlService: IntlService) {
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
      groupUoM: false,
      groupServiceCard: false,
      productListpriceRestrictCompany: false,
      groupTCare: false,
      tCareRunAtObj: new Date(2000, 2, 10, 0, 0, 0),
      groupMedicine: false,
      groupSurvey: false,
    });

    this.configSettingsService.defaultGet().subscribe((result: any) => {
      this.formGroup.patchValue(result);
      if (result.tCareRunAt) {
        var tCareRunAt = new Date(result.tCareRunAt);
        this.formGroup.get('tCareRunAtObj').patchValue(tCareRunAt);
      }
    });
  }

  onChangeCompanyShareProduct() {
    var companyShareProduct = this.companyShareProductValue;
    if (companyShareProduct == false) {
      this.formGroup.get('productListpriceRestrictCompany').setValue(companyShareProduct);
    }
  }

  onSave() {
    var val = this.formGroup.value;
    val.tCareRunAt = val.tCareRunAtObj ? this.intlService.formatDate(val.tCareRunAtObj, 'yyyy-MM-ddTHH:mm:ss') : null;
    if (val.groupServiceCard) {
      this.configSettingsService.insertServiceCardData().subscribe(() => {
        this.configSettingsService.create(val).subscribe(result => {
          this.configSettingsService.excute(result.id).subscribe(() => {
            this.authService.getGroups().subscribe((result: any) => {
              window.location.reload();
            });
          });
        })
      });
    } else {
      this.configSettingsService.create(val).subscribe(result => {
        this.configSettingsService.excute(result.id).subscribe(() => {
          this.authService.getGroups().subscribe((result: any) => {
            window.location.reload();
          });
        });
      })
    }
  }

  get groupLoyaltyCard() {
    return this.formGroup.get('groupLoyaltyCard').value;
  }

  get groupMultiCompany() {
    return this.formGroup.get('groupMultiCompany').value;
  }

  get companyShareProductValue() {
    return this.formGroup.get('companyShareProduct').value;
  }

  get groupTCareValue() {
    return this.formGroup.get('groupTCare').value;
  }
}
