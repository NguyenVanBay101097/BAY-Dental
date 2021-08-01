import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { SmsRoutingModule } from "./sms-routing.module";
import { SmsTemplateListComponent } from "./sms-template-list/sms-template-list.component";
import { SmsTemplateCrUpComponent } from "./sms-template-cr-up/sms-template-cr-up.component";
import { MyCustomKendoModule } from "../shared/my-customer-kendo.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { SharedModule } from "../shared/shared.module";
import { NgbModalModule, NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { SmsTemplateContentComponent } from "./sms-template-content/sms-template-content.component";
import { SmsAccountSettingComponent } from "./sms-account-setting/sms-account-setting.component";
import { SmsBirthdayFormComponent } from "./sms-birthday-form/sms-birthday-form.component";
import { SmsBirthdayFormManualComponent } from "./sms-birthday-form-manual/sms-birthday-form-manual.component";
import { SmsBirthdayFormAutomaticComponent } from "./sms-birthday-form-automatic/sms-birthday-form-automatic.component";
import { SmsAppointmentFormAutomaticComponent } from "./sms-appointment-form-automatic/sms-appointment-form-automatic.component";
import { SmsAppointmentFormManualComponent } from "./sms-appointment-form-manual/sms-appointment-form-manual.component";
import { SmsAppointmentFormComponent } from "./sms-appointment-form/sms-appointment-form.component";
import { SmsStatisticComponent } from "./sms-statistic/sms-statistic.component";
import { SmsManualDialogComponent } from "./sms-manual-dialog/sms-manual-dialog.component";
import { SmsAccountListComponent } from "./sms-account-list/sms-account-list.component";
import { SmsAccountSettingDialogComponent } from "./sms-account-setting-dialog/sms-account-setting-dialog.component";
import { SmsMessageDialogComponent } from "./sms-message-dialog/sms-message-dialog.component";
import { SmsMessageStatisticComponent } from "./sms-message-statistic/sms-message-statistic.component";
import { SmsMessageDetailStatisticComponent } from "./sms-message-detail-statistic/sms-message-detail-statistic.component";
import { SmsCampaignListComponent } from "./sms-campaign-list/sms-campaign-list.component";
import { SmsCampaignCrUpComponent } from "./sms-campaign-cr-up/sms-campaign-cr-up.component";
import { SmsComfirmDialogComponent } from "./sms-comfirm-dialog/sms-comfirm-dialog.component";
import { SmsPartnerListDialogComponent } from "./sms-partner-list-dialog/sms-partner-list-dialog.component";
import { SmsCampaignDetailComponent } from "./sms-campaign-detail/sms-campaign-detail.component";
import { SmsMessageDetailDialogComponent } from "./sms-message-detail-dialog/sms-message-detail-dialog.component";
import { SmsThanksFormComponent } from "./sms-thanks-form/sms-thanks-form.component";
import { SmsThanksFormManualComponent } from "./sms-thanks-form-manual/sms-thanks-form-manual.component";
import { SmsThanksFormAutomaticComponent } from "./sms-thanks-form-automatic/sms-thanks-form-automatic.component";
import { SmsCareAfterOrderFormComponent } from './sms-care-after-order-form/sms-care-after-order-form.component';
import { SmsCareAfterOrderFormAutomaticComponent } from './sms-care-after-order-form-automatic/sms-care-after-order-form-automatic.component';
import { SmsCareAfterOrderFormManualComponent } from './sms-care-after-order-form-manual/sms-care-after-order-form-manual.component';
import { SmsCareAfterOrderFormAutomaticDialogComponent } from './sms-care-after-order-form-automatic-dialog/sms-care-after-order-form-automatic-dialog.component';
import { SmsTooltipsContentComponent } from './sms-tooltips-content/sms-tooltips-content.component';
import { SmsReportComponent } from './sms-report/sms-report.component';
import { SmsCampaignUpdateComponent } from './sms-campaign-update/sms-campaign-update.component';
import { SmsNoteContentComponent } from './sms-note-content/sms-note-content.component';
import { SmsPersonalizedTabsComponent } from './sms-personalized-tabs/sms-personalized-tabs.component';

@NgModule({
  declarations: [
    SmsTemplateListComponent,
    SmsTemplateCrUpComponent,
    SmsTemplateContentComponent,
    SmsAccountSettingComponent,
    SmsBirthdayFormComponent,
    SmsBirthdayFormManualComponent,
    SmsBirthdayFormAutomaticComponent,
    SmsAppointmentFormAutomaticComponent,
    SmsAppointmentFormManualComponent,
    SmsAppointmentFormComponent,
    SmsStatisticComponent,
    SmsManualDialogComponent,
    SmsAccountListComponent,
    SmsAccountSettingDialogComponent,
    SmsMessageDialogComponent,
    SmsMessageStatisticComponent,
    SmsMessageDetailStatisticComponent,
    SmsCampaignListComponent,
    SmsCampaignCrUpComponent,
    SmsComfirmDialogComponent,
    SmsPartnerListDialogComponent,
    SmsCampaignDetailComponent,
    SmsMessageDetailDialogComponent,
    SmsThanksFormComponent,
    SmsThanksFormManualComponent,
    SmsThanksFormAutomaticComponent,
    SmsCareAfterOrderFormComponent,
    SmsCareAfterOrderFormAutomaticComponent,
    SmsCareAfterOrderFormManualComponent,
    SmsTooltipsContentComponent,
    SmsCareAfterOrderFormAutomaticDialogComponent,
    SmsReportComponent,
    SmsCampaignUpdateComponent,
    SmsNoteContentComponent,
    SmsPersonalizedTabsComponent
  ],
  imports: [
    CommonModule,
    SmsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    NgbModalModule,
    NgbModule,
  ],
  entryComponents: [
    SmsTemplateCrUpComponent,
    SmsMessageDialogComponent,
    SmsManualDialogComponent,
    SmsAccountSettingDialogComponent,
    SmsCampaignCrUpComponent,
    SmsComfirmDialogComponent,
    SmsCareAfterOrderFormAutomaticDialogComponent,
    SmsPartnerListDialogComponent,
    SmsMessageDetailDialogComponent,
  ]
})
export class SmsModule { }
