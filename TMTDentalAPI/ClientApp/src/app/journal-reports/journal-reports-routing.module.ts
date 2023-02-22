import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { JournalReportsViewComponent } from './journal-reports-view/journal-reports-view.component';

const routes: Routes = [
  {
    path: 'journal-reports',
    component: JournalReportsViewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class JournalReportsRoutingModule { }
