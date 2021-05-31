import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { QuotationCreateUpdateFormComponent } from './quotation-create-update-form/quotation-create-update-form.component';

const routes: Routes = [
  {
    path: 'form',
    component: QuotationCreateUpdateFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QuotationsRoutingModule { }
