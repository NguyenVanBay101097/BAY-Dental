import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TeethRoutingModule } from './teeth-routing.module';
import { ToothSelectDialogComponent } from './tooth-select-dialog/tooth-select-dialog.component';
import { ToothService } from './tooth.service';

@NgModule({
  declarations: [ToothSelectDialogComponent],
  imports: [
    CommonModule,
    TeethRoutingModule
  ],
  providers: [
    ToothService
  ]
})
export class TeethModule { }
