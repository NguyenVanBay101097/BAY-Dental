import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ResConfigSettingsService } from '../res-config-settings.service';

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
  constructor(private fb: FormBuilder, private configSettingsService: ResConfigSettingsService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      groupDiscountPerSOLine: false,
      groupLoyaltyCard: false,
      groupSaleCouponPromotion: false,
      loyaltyPointExchangeRate: 0,
    });

    this.configSettingsService.defaultGet().subscribe(result => {
      this.formGroup.patchValue(result);
    });
  }

  onSave() {
    var val = this.formGroup.value;
    this.configSettingsService.create(val).subscribe(result => {
      this.configSettingsService.excute(result.id).subscribe(() => {
        localStorage.removeItem('groups');
        window.location.reload();
      });
    })
  }

  get groupLoyaltyCard() {
    return this.formGroup.get('groupLoyaltyCard').value;
  }
}
