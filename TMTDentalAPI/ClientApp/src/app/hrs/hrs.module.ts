import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HrsRoutingModule } from './hrs-routing.module';
import { HrSalaryRuleComponent } from './hr-salary-rule/hr-salary-rule.component';

@NgModule({
  declarations: [HrSalaryRuleComponent],
  imports: [
    CommonModule,
    HrsRoutingModule
  ]
})
export class HrsModule { }
