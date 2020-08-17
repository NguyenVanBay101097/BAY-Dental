import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CommissionsRoutingModule } from './commissions-routing.module';
import { CommissionListComponent } from './commission-list/commission-list.component';
import { CommissionService } from './commission.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { CommissionCreateUpdateComponent } from './commission-create-update/commission-create-update.component';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { CommissionCreateUpdateDialogComponent } from './commission-create-update-dialog/commission-create-update-dialog.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PopupModule } from '@progress/kendo-angular-popup';

@NgModule({
  declarations: [
    CommissionListComponent, 
    CommissionCreateUpdateComponent, 
    CommissionCreateUpdateDialogComponent
  ],
  imports: [
    CommonModule,
    CommissionsRoutingModule, 
    FormsModule, 
    MyCustomKendoModule, 
    ReactiveFormsModule, 
    InputsModule, 
    NgbModule, 
    PopupModule
  ],
  providers: [
    CommissionService
  ],
  entryComponents: [
    CommissionCreateUpdateDialogComponent
  ]
})
export class CommissionsModule { }
