import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IrRulesRoutingModule } from './ir-rules-routing.module';
import { IRRuleService } from './ir-rule.service';
import { IrRuleListComponent } from './ir-rule-list/ir-rule-list.component';
import { IrRuleCuDialogComponent } from './ir-rule-cu-dialog/ir-rule-cu-dialog.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [IrRuleListComponent, IrRuleCuDialogComponent],
  imports: [
    CommonModule,
    IrRulesRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [IRRuleService],
  entryComponents: [
    IrRuleCuDialogComponent
  ]
})
export class IrRulesModule { }
