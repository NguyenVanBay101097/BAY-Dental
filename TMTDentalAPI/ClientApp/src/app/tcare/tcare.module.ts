import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TcareRoutingModule } from './tcare-routing.module';
import { TcareCampaignCreateUpdateComponent } from './tcare-campaign-create-update/tcare-campaign-create-update.component';
import { TcareService } from './tcare.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModalModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { TcareCampaignDialogSequencesComponent } from './tcare-campaign-dialog-sequences/tcare-campaign-dialog-sequences.component';
import { TcareCampaignCreateDialogComponent } from './tcare-campaign-create-dialog/tcare-campaign-create-dialog.component';
import { TcareCampaignListComponent } from './tcare-campaign-list/tcare-campaign-list.component';
import { TcareCampaignDialogRuleComponent } from './tcare-campaign-dialog-rule/tcare-campaign-dialog-rule.component';
import { TcareCampaignDialogMessageComponent } from './tcare-campaign-dialog-message/tcare-campaign-dialog-message.component';
import { AudienceFilterComponent } from './tcare-campaign-dialog-rule/audience-filter/audience-filter.component';
import { AudienceFilterDropdownComponent } from './tcare-campaign-dialog-rule/audience-filter/audience-filter-dropdown/audience-filter-dropdown.component';
import { AudienceFilterBirthdayComponent } from './tcare-campaign-dialog-rule/audience-filter/audience-filter-dropdown/audience-filter-birthday/audience-filter-birthday.component';
import { AudienceFilterLastTreatmentDayComponent } from './tcare-campaign-dialog-rule/audience-filter/audience-filter-dropdown/audience-filter-last-treatment-day/audience-filter-last-treatment-day.component';
import { AudienceFilterServiceComponent } from './tcare-campaign-dialog-rule/audience-filter/audience-filter-dropdown/audience-filter-service/audience-filter-service.component';
import { AudienceFilterServiceCategoryComponent } from './tcare-campaign-dialog-rule/audience-filter/audience-filter-dropdown/audience-filter-service-category/audience-filter-service-category.component';
import { AudienceFilterPartnerCategoryComponent } from './tcare-campaign-dialog-rule/audience-filter/audience-filter-dropdown/audience-filter-partner-category/audience-filter-partner-category.component';
import { MyAutofocusDirective } from './tcare-campaign-dialog-rule/audience-filter/autofocus.directive';
import { ClickOutsideDirective } from './tcare-campaign-dialog-rule/audience-filter/click-outside.directive';
import { TcareCampaignStartDialogComponent } from './tcare-campaign-start-dialog/tcare-campaign-start-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { TcareScenarioListComponent } from './tcare-scenario-list/tcare-scenario-list.component';
import { TcareScenarioCrUpComponent } from './tcare-scenario-cr-up/tcare-scenario-cr-up.component';
import { TcareScenarioCrDialogComponent } from './tcare-scenario-cr-dialog/tcare-scenario-cr-dialog.component';
import { AudienceFilterLastExaminationComponent } from './tcare-campaign-dialog-rule/audience-filter/audience-filter-dropdown/audience-filter-last-examination/audience-filter-last-examination.component';
import { AudienceFilterAppointmentDayComponent } from './tcare-campaign-dialog-rule/audience-filter/audience-filter-dropdown/audience-filter-appointment-day/audience-filter-appointment-day.component';
import { FacebookPluginTextareaComponent } from '../socials-channel/facebook-plugin-textarea/facebook-plugin-textarea.component';
import { PickerModule } from '@ctrl/ngx-emoji-mart';
import { EmojiModule } from '@ctrl/ngx-emoji-mart/ngx-emoji';
import { SocialsChannelModule } from '../socials-channel/socials-channel.module';
import { TcareScenarioMessageTextareaComponent } from './tcare-scenario-message-textarea/tcare-scenario-message-textarea.component';
import { PartnerCategoriesModule } from '../partner-categories/partner-categories.module';
import { TcareMessageTemplateListComponent } from './tcare-message-template-list/tcare-message-template-list.component';
import { TcareMessageTemplateCuDialogComponent } from './tcare-message-template-cu-dialog/tcare-message-template-cu-dialog.component';
import { TcareMessageTemplateContentComponent } from './tcare-message-template-cu-dialog/tcare-message-template-content/tcare-message-template-content.component';
import { SaleCouponProgramService } from '../sale-coupon-promotion/sale-coupon-program.service';
import { TcareMessagingListComponent } from './tcare-messaging-list/tcare-messaging-list.component';
import { TcareQuickreplyDialogComponent } from './tcare-quickreply-dialog/tcare-quickreply-dialog.component';
import { TcareConfigComponent } from './tcare-config/tcare-config.component';

@NgModule({
  declarations: [
    TcareCampaignCreateUpdateComponent,
    TcareCampaignDialogSequencesComponent,
    TcareCampaignCreateDialogComponent,
    TcareCampaignListComponent,
    TcareCampaignDialogRuleComponent,
    TcareCampaignDialogMessageComponent,
    AudienceFilterComponent,
    AudienceFilterDropdownComponent,
    AudienceFilterBirthdayComponent,
    AudienceFilterLastTreatmentDayComponent,
    AudienceFilterServiceComponent,
    AudienceFilterServiceCategoryComponent,
    AudienceFilterPartnerCategoryComponent,
    MyAutofocusDirective,
    ClickOutsideDirective,
    TcareCampaignStartDialogComponent,
    TcareScenarioListComponent,
    TcareScenarioCrUpComponent,
    TcareScenarioCrDialogComponent,
    AudienceFilterLastExaminationComponent,
    AudienceFilterAppointmentDayComponent,
    TcareScenarioMessageTextareaComponent,
    TcareMessageTemplateListComponent,
    TcareMessageTemplateCuDialogComponent,
    TcareMessageTemplateContentComponent,
    TcareMessagingListComponent,
    TcareQuickreplyDialogComponent,
    TcareConfigComponent,
  ],
  imports: [
    CommonModule,
    NgbModalModule,
    MyCustomKendoModule,
    TcareRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    SharedModule, 
    PickerModule,
    EmojiModule,
    PartnerCategoriesModule
  ],

  providers: [TcareService, SaleCouponProgramService],
  entryComponents: [
    TcareCampaignDialogRuleComponent,
    TcareCampaignDialogSequencesComponent,
    TcareCampaignCreateDialogComponent,
    TcareCampaignDialogMessageComponent,
    AudienceFilterBirthdayComponent,
    AudienceFilterLastTreatmentDayComponent,
    AudienceFilterPartnerCategoryComponent,
    AudienceFilterServiceComponent,
    AudienceFilterServiceCategoryComponent,
    AudienceFilterLastExaminationComponent,
    AudienceFilterAppointmentDayComponent,
    TcareCampaignStartDialogComponent,
    TcareScenarioCrDialogComponent,
    TcareMessageTemplateCuDialogComponent
  ]
})
export class TcareModule { }
