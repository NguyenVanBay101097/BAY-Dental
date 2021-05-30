import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PartnerAdvanceListComponent } from './partner-advance-list/partner-advance-list.component';

const routes: Routes = [
    {
        path: "", component: PartnerAdvanceListComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PartnerAdvancesRoutingModule { }
