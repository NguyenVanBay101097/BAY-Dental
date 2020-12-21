import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LaboBiteJointsRoutingModule } from './labo-bite-joints-routing.module';
import { LaboBiteJointListComponent } from './labo-bite-joint-list/labo-bite-joint-list.component';
import { LaboBiteJointCuDialogComponent } from './labo-bite-joint-cu-dialog/labo-bite-joint-cu-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { LaboBiteJointService } from './labo-bite-joint.service';

@NgModule({
  declarations: [LaboBiteJointListComponent, LaboBiteJointCuDialogComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    DragDropModule,
    SharedModule,
    NgbModule,
    LaboBiteJointsRoutingModule,
    MyCustomKendoModule
  ],
  providers: [LaboBiteJointService],
  entryComponents: [LaboBiteJointCuDialogComponent]
})
export class LaboBiteJointsModule { }
