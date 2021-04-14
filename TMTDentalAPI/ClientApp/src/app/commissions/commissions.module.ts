import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CommissionsRoutingModule } from './commissions-routing.module';
import { CommissionListComponent } from './commission-list/commission-list.component';
import { CommissionService } from './commission.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { CommissionCreateUpdateComponent } from './commission-create-update/commission-create-update.component';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { CommissionCreateUpdateDialogComponent } from './commission-create-update-dialog/commission-create-update-dialog.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PopupModule } from '@progress/kendo-angular-popup';
import { CommissionListV2Component } from './commission-list-v2/commission-list-v2.component';
import { CommissionProductRuleService } from './commission-product-rule.service';
import { CommissionProductRuleListComponent } from './commission-product-rule-list/commission-product-rule-list.component';
import { CommissionDialogComponent } from './commission-dialog/commission-dialog.component';
import { CommissionProductRuleDialogComponent } from './commission-product-rule-dialog/commission-product-rule-dialog.component';

@NgModule({
  declarations: [
    CommissionListComponent, 
    CommissionCreateUpdateComponent, 
    CommissionCreateUpdateDialogComponent, 
    CommissionListV2Component, 
    CommissionProductRuleListComponent, 
    CommissionDialogComponent, 
    CommissionProductRuleDialogComponent
  ],
  imports: [
    CommonModule,
    CommissionsRoutingModule, 
    FormsModule, 
    MyCustomKendoModule, 
    ReactiveFormsModule, 
    InputsModule, 
    NgbModule, 
    PopupModule
  ],
  providers: [
    CommissionService,
    CommissionProductRuleService
  ],
  entryComponents: [
    CommissionCreateUpdateDialogComponent,
    CommissionDialogComponent, 
    CommissionProductRuleDialogComponent
  ]
})
export class CommissionsModule { }
