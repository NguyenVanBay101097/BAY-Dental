import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CashBookComponent } from './cash-book/cash-book.component';

const routes: Routes = [
  {
    path: '',
    component: CashBookComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CashBookRoutingModule { }
