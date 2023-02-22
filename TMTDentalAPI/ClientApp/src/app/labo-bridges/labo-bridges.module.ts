import { DragDropModule } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { LaboBridgeCuDialogComponent } from './labo-bridge-cu-dialog/labo-bridge-cu-dialog.component';
import { LaboBridgeListComponent } from './labo-bridge-list/labo-bridge-list.component';
import { LaboBridgeService } from './labo-bridge.service';
import { LaboBridgesRoutingModule } from './labo-bridges-routing.module';

@NgModule({
  declarations: [LaboBridgeListComponent, LaboBridgeCuDialogComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    DragDropModule,
    SharedModule,
    NgbModule,
    LaboBridgesRoutingModule,
    MyCustomKendoModule
  ],
  providers: [LaboBridgeService],
  entryComponents: [LaboBridgeCuDialogComponent]
})
export class LaboBridgesModule { }
