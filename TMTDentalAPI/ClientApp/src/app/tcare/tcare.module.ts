import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TcareRoutingModule } from './tcare-routing.module';
import { TcareCampaignCreateUpdateComponent } from './tcare-campaign-create-update/tcare-campaign-create-update.component';
import { TcareService } from './tcare.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
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
    ClickOutsideDirective
  ],
  imports: [
    CommonModule,
    NgbModalModule,
    MyCustomKendoModule,
    TcareRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [TcareService],
  entryComponents: [
    TcareCampaignDialogRuleComponent,
    TcareCampaignDialogSequencesComponent,
    TcareCampaignCreateDialogComponent,
    TcareCampaignDialogMessageComponent,
    AudienceFilterBirthdayComponent,
    AudienceFilterLastTreatmentDayComponent,
    AudienceFilterPartnerCategoryComponent,
    AudienceFilterServiceComponent,
    AudienceFilterServiceCategoryComponent
  ]
})
export class TcareModule { }
