import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IrRulesRoutingModule } from './ir-rules-routing.module';
import { IRRuleService } from './ir-rule.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    IrRulesRoutingModule
  ],
  providers: [IRRuleService]
})
export class IrRulesModule { }
