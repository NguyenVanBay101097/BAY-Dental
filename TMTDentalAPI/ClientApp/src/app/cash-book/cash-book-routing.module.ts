import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CashBookTabPageCaBoComponent } from './cash-book-tab-page-ca-bo/cash-book-tab-page-ca-bo.component';
import { CashBookTabPageRePaComponent } from './cash-book-tab-page-re-pa/cash-book-tab-page-re-pa.component';
import { CashBookComponent } from './cash-book/cash-book.component';

const routes: Routes = [
  {
    path: '',
    component: CashBookComponent,
    children: [
      { path: '', redirectTo: 'tab-cabo', pathMatch: 'full' },
      {
        path: 'tab-cabo',
        component: CashBookTabPageCaBoComponent,
      }, 
      {
        path: 'tab-repa',
        component: CashBookTabPageRePaComponent,
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CashBookRoutingModule { }
