import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WorkEntryTypeListComponent } from './work-entry-type-list/work-entry-type-list.component';
import { WorkEntryTypeCreateUpdateDialogComponent } from './work-entry-type-create-update-dialog/work-entry-type-create-update-dialog.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from "../shared/shared.module";
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { WorkEntryTypesRoutingModule } from './work-entry-types-routing.module';
import { WorkEntryTypeService } from './work-entry-type.service';

@NgModule({
  declarations: [WorkEntryTypeListComponent, WorkEntryTypeCreateUpdateDialogComponent],
  imports: [
    CommonModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MyCustomKendoModule,
    WorkEntryTypesRoutingModule
  ], 
  providers: [
    WorkEntryTypeService
  ],
  entryComponents: [
    WorkEntryTypeCreateUpdateDialogComponent   
  ]
})
export class WorkEntryTypesModule { }
