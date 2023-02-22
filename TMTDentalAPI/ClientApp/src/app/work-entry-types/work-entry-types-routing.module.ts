import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { WorkEntryTypeListComponent } from './work-entry-type-list/work-entry-type-list.component';


const routes: Routes = [
  
  { path: '', component: WorkEntryTypeListComponent }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WorkEntryTypesRoutingModule { }