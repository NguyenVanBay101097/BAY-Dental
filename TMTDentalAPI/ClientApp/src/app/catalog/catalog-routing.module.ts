import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HistoriesListComponent } from '../history/histories-list/histories-list.component';
import { PartnerCategoryListComponent } from '../partner-categories/partner-category-list/partner-category-list.component';
import { PartnerSourceListComponent } from '../partner-sources/partner-source-list/partner-source-list.component';
import { PartnerTitleListComponent } from '../partner-titles/partner-title-list/partner-title-list.component';
import { PartnerInfoCustomerManagementComponent } from './partner-info-customer-management/partner-info-customer-management.component';

const routes: Routes = [
  {
    path: 'customer-management',
    component: PartnerInfoCustomerManagementComponent,
    children: [
      { path: '', redirectTo: 'customer-categ', pathMatch: 'full' },
      { path: 'customer-categ', component: PartnerCategoryListComponent },
      { path: 'customer-source', component: PartnerSourceListComponent },
      { path: 'customer-title', component: PartnerTitleListComponent },
      { path: 'customer-history', component: HistoriesListComponent },
    ]
  },
  {
    path: 'surveys',
    loadChildren: () => import('../surveys/surveys.module').then(m => m.SurveysModule),
  },
  {
    path: 'agents',
    loadChildren: () => import('../agents/agents.module').then(m => m.AgentsModule),
  },
  {
    path: 'hr-job',
    loadChildren: () => import('../hr-jobs/hr-jobs.module').then(m => m.HrJobsModule),
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CatalogRoutingModule { }
