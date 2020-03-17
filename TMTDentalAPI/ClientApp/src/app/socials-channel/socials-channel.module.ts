import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SocialsChannelRoutingModule } from './socials-channel-routing.module';
import { FacebookComponent } from './facebook/facebook.component';
import { FacebookDialogComponent } from './facebook-dialog/facebook-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PartnerCreateUpdateComponent } from '../partners/partner-create-update/partner-create-update.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FacebookDashboardComponent } from './facebook-dashboard/facebook-dashboard.component';
import { FacebookPageManagementComponent } from './facebook-page-management/facebook-page-management.component';
import { FacebookPageMarketingCampaignsComponent } from './facebook-page-marketing-campaigns/facebook-page-marketing-campaigns.component';
import { FacebookPageMarketingCampaignCreateUpdateComponent } from './facebook-page-marketing-campaign-create-update/facebook-page-marketing-campaign-create-update.component';
import { FacebookPageMarketingCampaignListComponent } from './facebook-page-marketing-campaign-list/facebook-page-marketing-campaign-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FacebookPageMarketingCampaignActivityDetailComponent } from './facebook-page-marketing-campaign-activity-detail/facebook-page-marketing-campaign-activity-detail.component';

@NgModule({
  declarations: [FacebookComponent, FacebookDialogComponent, FacebookDashboardComponent, FacebookPageManagementComponent, FacebookPageMarketingCampaignsComponent, FacebookPageMarketingCampaignCreateUpdateComponent, FacebookPageMarketingCampaignListComponent, FacebookPageMarketingCampaignActivityDetailComponent],
  imports: [
    CommonModule,
    SocialsChannelRoutingModule,
    FormsModule,
    NgbModule,
    DropDownsModule,
    FlexLayoutModule,
    MyCustomKendoModule,
    ReactiveFormsModule
  ],
  exports: [
  ],
  entryComponents: [
    FacebookDialogComponent,
    PartnerCreateUpdateComponent
  ]
})
export class SocialsChannelModule { }
