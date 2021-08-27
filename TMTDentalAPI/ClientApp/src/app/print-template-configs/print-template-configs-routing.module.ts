import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PrintTemplateConfigCuComponent } from './print-template-config-cu/print-template-config-cu.component';
import { PrintTemplateConfigListComponent } from './print-template-config-list/print-template-config-list.component';

const routes: Routes = [
  // { path: 'list', component: PrintTemplateConfigListComponent },
  { path: '', component: PrintTemplateConfigCuComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PrintTemplatesRoutingModule { }
