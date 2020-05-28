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

@NgModule({
  declarations: [
    TcareCampaignCreateUpdateComponent,
    TcareCampaignDialogSequencesComponent,
    TcareCampaignCreateDialogComponent,
    TcareCampaignListComponent,
    TcareCampaignDialogRuleComponent
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
    TcareCampaignCreateDialogComponent
  ]
})
export class TcareModule { }
