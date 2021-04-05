import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { PartnerAdvanceListComponent } from './partner-advance-list/partner-advance-list.component';
import { PartnerAdvanceCreateUpdateDialogComponent } from './partner-advance-create-update-dialog/partner-advance-create-update-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { PartnerAdvanceService } from './partner-advance.service';
import { PartnerAdvancesRoutingModule } from "./partner-advances-routing.module";

@NgModule({
  declarations: [PartnerAdvanceListComponent, PartnerAdvanceCreateUpdateDialogComponent],
  imports: [
    CommonModule,
    PartnerAdvancesRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    ReactiveFormsModule,
    DragDropModule
  ],providers: 
  [PartnerAdvanceService],
  entryComponents: [
    PartnerAdvanceCreateUpdateDialogComponent
  ]
})
export class PartnerAdvancesModule { }
