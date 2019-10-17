import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ResGroupsRoutingModule } from './res-groups-routing.module';
import { ResGroupListComponent } from './res-group-list/res-group-list.component';
import { ResGroupService } from './res-group.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ResGroupCreateUpdateComponent } from './res-group-create-update/res-group-create-update.component';
import { ResGroupAccessCuDialogComponent } from './res-group-access-cu-dialog/res-group-access-cu-dialog.component';

@NgModule({
  declarations: [ResGroupListComponent, ResGroupCreateUpdateComponent, ResGroupAccessCuDialogComponent],
  imports: [
    CommonModule,
    ResGroupsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [
    ResGroupService
  ],
  entryComponents: [ResGroupAccessCuDialogComponent]
})
export class ResGroupsModule { }
