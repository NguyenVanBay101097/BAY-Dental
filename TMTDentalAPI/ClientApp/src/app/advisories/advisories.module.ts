import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdvisoriesRoutingModule } from './advisories-routing.module';
import { AdvisoryService } from './advisory.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    AdvisoriesRoutingModule
  ],
  providers: [AdvisoryService]
})
export class AdvisoriesModule { }
