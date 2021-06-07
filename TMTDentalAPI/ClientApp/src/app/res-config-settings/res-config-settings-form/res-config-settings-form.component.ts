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
    private smsMessageService: SmsMessageService
    , private notificationService: NotificationService, private intlService: IntlService) {
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
      // conversionRate: [0, Validators.required]
    });


    this.configSettingsService.defaultGet().subscribe((result: any) => {
      this.formGroup.patchValue(result);

      if (result.tCareRunAt) {
        var tCareRunAt = new Date(result.tCareRunAt);
        this.formGroup.get('tCareRunAtObj').patchValue(tCareRunAt);
      }
      if (result.groupSms) {
        this.actionStartJobSmsMessage();
      } else {
        this.actionStopJobSmsMessage();
      }

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

  actionStartJobSmsMessage() {
    this.smsMessageService.actionStartJobAutomatic().subscribe(
      res => { }
    )
  }

  actionStopJobSmsMessage() {
    this.smsMessageService.actionStopJobAutomatic().subscribe(
      res => { }
    )
  }

  onSave() {
    this.submitted = true;
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
