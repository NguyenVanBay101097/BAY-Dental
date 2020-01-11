import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PromotionProgramListComponent } from './promotion-program-list/promotion-program-list.component';
import { PromotionProgramCreateUpdateComponent } from './promotion-program-create-update/promotion-program-create-update.component';

const routes: Routes = [

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PromotionProgramsRoutingModule { }
