import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CommissionsRoutingModule } from './commissions-routing.module';
import { CommissionListComponent } from './commission-list/commission-list.component';
import { CommissionCuDialogComponent } from './commission-cu-dialog/commission-cu-dialog.component';
import { CommissionService } from './commission.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { CommissionCreateUpdateComponent } from './commission-create-update/commission-create-update.component';

@NgModule({
  declarations: [CommissionListComponent, CommissionCuDialogComponent, CommissionCreateUpdateComponent],
  imports: [
    CommonModule,
    CommissionsRoutingModule, 
    FormsModule, 
    MyCustomKendoModule, 
    ReactiveFormsModule
  ],
  providers: [
    CommissionService
  ],
  entryComponents: [
    CommissionCuDialogComponent
  ]
})
export class CommissionsModule { }
