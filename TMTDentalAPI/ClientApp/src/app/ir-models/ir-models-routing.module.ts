import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IrModelListComponent } from './ir-model-list/ir-model-list.component';

const routes: Routes = [
  {
    path: 'ir-models',
    component: IrModelListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IrModelsRoutingModule { }
