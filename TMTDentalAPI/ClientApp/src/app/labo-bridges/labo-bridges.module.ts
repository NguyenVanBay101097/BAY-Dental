import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LaboBridgesRoutingModule } from './labo-bridges-routing.module';
import { LaboBridgeListComponent } from './labo-bridge-list/labo-bridge-list.component';
import { LaboBridgeCuDialogComponent } from './labo-bridge-cu-dialog/labo-bridge-cu-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { LaboBridgeService } from './labo-bridge.service';
import { LaboFinnishLineImportComponent } from '../labo-finish-lines/labo-finnish-line-import/labo-finnish-line-import.component';

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
