import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MarketingCampaignsRoutingModule } from './marketing-campaigns-routing.module';
import { MarketingCampaignListComponent } from './marketing-campaign-list/marketing-campaign-list.component';
import { MarketingCampaignCreateUpdateComponent } from './marketing-campaign-create-update/marketing-campaign-create-update.component';
import { MarketingCampaignLineDialogComponent } from './marketing-campaign-line-dialog/marketing-campaign-line-dialog.component';
import { MarketingCampaignService } from './marketing-campaign.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';

@NgModule({
  declarations: [MarketingCampaignCreateUpdateComponent, MarketingCampaignLineDialogComponent, MarketingCampaignListComponent],
  imports: [
    CommonModule,
    MarketingCampaignsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    NgbModule
  ],
  providers: [
    MarketingCampaignService
  ],
  exports: [
  ],
  entryComponents: [
    MarketingCampaignLineDialogComponent,
  ]
})
export class MarketingCampaignsModule { }
