import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ResConfigSettingsService } from '../res-config-settings.service';
import { AuthService } from 'src/app/auth/auth.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { IntlService } from '@progress/kendo-angular-intl';
import { PrintPaperSizeBasic, PrintPaperSizePaged, PrintPaperSizeService } from 'src/app/config-prints/print-paper-size.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { SmsMessageService } from 'src/app/sms/sms-message.service';
import { SmsCampaignService } from 'src/app/sms/sms-campaign.service';
import { forkJoin } from 'rxjs';
import { WebSessionService } from 'src/app/core/services/web-session.service';
import { SessionInfoStorageService } from 'src/app/core/services/session-info-storage.service';
import { mergeMap } from 'rxjs/operators';

@Component({
  selector: 'app-res-config-settings-form',
  templateUrl: './res-config-settings-form.component.html',
  styleUrls: ['./res-config-settings-form.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ResConfigSettingsFormComponent implements OnInit {
  submitted: boolean = false;
  formGroup: FormGroup;
  filterdPaperSizes: PrintPaperSizeBasic[] = [];

  @ViewChild('papersizeCbx', { static: true }) papersizeCbx: ComboBoxComponent;
  constructor(private fb: FormBuilder, private configSettingsService: ResConfigSettingsService, private printPaperSizeService: PrintPaperSizeService,
    private authService: AuthService,
    private webSessionService: WebSessionService,
    private sessionInfoStorageService: SessionInfoStorageService) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      groupDiscountPerSOLine: false,
      groupLoyaltyCard: false,
      groupSaleCouponPromotion: false,
      loyaltyPointExchangeRate: [0, Validators.required],
      groupMultiCompany: false,
      companySharePartner: false,
      companyShareProduct: false,
      groupUoM: false,
      groupSms: false,
      groupServiceCard: false,
      productListpriceRestrictCompany: false,
      groupTCare: false,
      tCareRunAtObj: new Date(2000, 2, 10, 0, 0, 0),
      groupMedicine: false,
      groupSurvey: false,
      groupInsurance: false,
      // conversionRate: [0, Validators.required]
    });


    this.configSettingsService.defaultGet().subscribe((result: any) => {
      this.formGroup.patchValue(result);
    });
  }

  get f() { return this.formGroup.controls; }

  onChangeCompanyShareProduct() {
    var companyShareProduct = this.companyShareProductValue;
    if (companyShareProduct == false) {
      this.formGroup.get('productListpriceRestrictCompany').setValue(companyShareProduct);
    }
  }

  loadPaperSizes() {
    this.searchPaperSizes().subscribe((result) => {
      this.filterdPaperSizes = _.unionBy(this.filterdPaperSizes, result.items, "id");
    });
  }

  searchPaperSizes(q?: string) {
    var val = new PrintPaperSizePaged();
    val.search = q || '';
    return this.printPaperSizeService.getPaged(val);
  }

  onSave() {
    this.submitted = true;
    var val = this.formGroup.value;
    this.configSettingsService.create(val)
    .pipe(
      mergeMap(result => {
        return this.configSettingsService.excute(result.id);
      }),
      mergeMap(() => {
        return this.webSessionService.getSessionInfo();
      })
    )
    .subscribe(sessionInfo => {
      this.sessionInfoStorageService.saveSession(sessionInfo);
      window.location.reload();
    });
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
