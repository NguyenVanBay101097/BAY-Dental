import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IrRuleListComponent } from './ir-rule-list/ir-rule-list.component';

const routes: Routes = [
  {
    path: 'ir-rules',
    component: IrRuleListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IrRulesRoutingModule { }
