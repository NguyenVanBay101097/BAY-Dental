import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HistoriesListComponent } from '../history/histories-list/histories-list.component';
import { PartnerCategoryListComponent } from '../partner-categories/partner-category-list/partner-category-list.component';
import { PartnerSourceListComponent } from '../partner-sources/partner-source-list/partner-source-list.component';
import { PartnerTitleListComponent } from '../partner-titles/partner-title-list/partner-title-list.component';
import { LaboManagementComponent } from './labo-management/labo-management.component';
import { PartnerInfoCustomerManagementComponent } from './partner-info-customer-management/partner-info-customer-management.component';
import { PartnerSupplierListComponent } from './partner-supplier-list/partner-supplier-list.component';

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
    path: 'member-level', 
    loadChildren: () => import('../member-level/member-level.module').then(m => m.MemberLevelModule)
  },
  {
    path: 'suppliers',
    component: PartnerSupplierListComponent
  },
  {
    path: 'products',
    loadChildren: () => import('../products/products.module').then(m => m.ProductsModule)
  },
  {
    path: 'sample-prescriptions',
    loadChildren: () => import('../sample-prescriptions/sample-prescriptions.module').then(m => m.SamplePrescriptionsModule),
  },
  {
    path: 'uoms',
    loadChildren: () => import('../uoms/uom.module').then(m => m.UomModule),
  },
  {
    path: 'uom-categories',
    loadChildren: () => import('../uom-categories/uom-category.module').then(m => m.UomCategoryModule),
  },
  {
    path: 'commissions',
    loadChildren: () => import('../commissions/commissions.module').then(m => m.CommissionsModule),
  },
  {
    path: 'employees',
    loadChildren: () => import('../employees/employees.module').then(m => m.EmployeesModule),
  },
  {
    path: 'tooth-diagnosis',
    loadChildren: () => import('../tooth-diagnosis/tooth-diagnosis.module').then(m => m.ToothDiagnosisModule),
  },
  {
    path: 'loai-thu-chi',
    loadChildren: () => import('../loai-thu-chi/loai-thu-chi.module').then(m => m.LoaiThuChiModule),
  },
  {
    path: 'stock',
    loadChildren: () => import('../stock-inventories/stock-inventories.module').then(m => m.StockInventoriesModule),
  },
  {
    path: 'labo-managerment',
    component: LaboManagementComponent,
    children: [
      { path: '', redirectTo: 'labo-bite-joints', pathMatch: 'full' },
      {
        path: 'labo-bite-joints',
        loadChildren: () => import('../labo-bite-joints/labo-bite-joints.module').then(m => m.LaboBiteJointsModule),
      },
      {
        path: 'labo-bridges',
        loadChildren: () => import('../labo-bridges/labo-bridges.module').then(m => m.LaboBridgesModule),
      },
      {
        path: 'labo-finish-lines',
        loadChildren: () => import('../labo-finish-lines/labo-finish-lines.module').then(m => m.LaboFinishLinesModule),
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CatalogRoutingModule { }
