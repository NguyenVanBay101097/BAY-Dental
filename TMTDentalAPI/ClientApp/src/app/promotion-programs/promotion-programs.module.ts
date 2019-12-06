import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PromotionProgramsRoutingModule } from './promotion-programs-routing.module';
import { PromotionProgramListComponent } from './promotion-program-list/promotion-program-list.component';
import { PromotionProgramService } from './promotion-program.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { PromotionProgramCreateUpdateComponent } from './promotion-program-create-update/promotion-program-create-update.component';
import { PromotionProgramRuleCuDialogComponent } from './promotion-program-rule-cu-dialog/promotion-program-rule-cu-dialog.component';

@NgModule({
  declarations: [PromotionProgramListComponent, PromotionProgramCreateUpdateComponent, PromotionProgramRuleCuDialogComponent],
  imports: [
    CommonModule,
    PromotionProgramsRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomKendoModule,
  ],
  providers: [
    PromotionProgramService
  ],
  entryComponents: [
    PromotionProgramRuleCuDialogComponent
  ]
})
export class PromotionProgramsModule { }
