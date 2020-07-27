import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PartnerTitleListComponent } from './partner-title-list/partner-title-list.component';

const routes: Routes = [
  {
    path: 'partner-titles',
    component: PartnerTitleListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnerTitlesRoutingModule { }
