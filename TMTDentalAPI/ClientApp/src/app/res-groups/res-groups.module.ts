import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { ResGroupCreateUpdateComponent } from './res-group-create-update/res-group-create-update.component';
import { ResGroupAccessCuDialogComponent } from './res-group-access-cu-dialog/res-group-access-cu-dialog.component';
import { ResGroupListComponent } from './res-group-list/res-group-list.component';
import { ResGroupsRoutingModule } from './res-groups-routing.module';
import { ResGroupService } from './res-group.service';
import { IRModelService } from '../ir-models/ir-model.service';

@NgModule({
  declarations: [ResGroupListComponent, ResGroupCreateUpdateComponent, ResGroupAccessCuDialogComponent],
  imports: [
    CommonModule,
    ResGroupsRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    NgbModule,
  ],
  providers: [
    ResGroupService,
    IRModelService
  ],
  entryComponents: [ResGroupAccessCuDialogComponent]
})
export class ResGroupsModule { }
