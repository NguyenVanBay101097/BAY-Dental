import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ConfigPrintFormComponent } from './config-print-form/config-print-form.component';
import { ConfigPrintManagementComponent } from './config-print-management/config-print-management.component';
import { PrintPaperSizeListComponent } from './print-paper-size-list/print-paper-size-list.component';


const routes: Routes = [
    {
        path: 'config-print-managerment',
        component: ConfigPrintManagementComponent,
        children: [
          { path: '', redirectTo: 'config-print-form', pathMatch: 'full' },
          { path: 'config-print-form', component: ConfigPrintFormComponent },
          { path: 'print-paper-sizes', component: PrintPaperSizeListComponent },
        ]
      }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConfigPrintsRoutingModule { }
