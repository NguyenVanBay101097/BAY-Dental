import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LaboFinishLineListComponent } from './labo-finish-line-list/labo-finish-line-list.component';
import { LaboFinishLineCuDialogComponent } from './labo-finish-line-cu-dialog/labo-finish-line-cu-dialog.component';
import { LaboFinishLinesRoutingModule } from './labo-finish-lines-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { LaboFinishLineService } from './labo-finish-line.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { LaboFinnishLineImportComponent } from './labo-finnish-line-import/labo-finnish-line-import.component';

@NgModule({
  declarations: [LaboFinishLineListComponent, LaboFinishLineCuDialogComponent, LaboFinnishLineImportComponent],
  imports: [
    CommonModule,
    LaboFinishLinesRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    DragDropModule,
    SharedModule,
    NgbModule,
    MyCustomKendoModule
  ],
  schemas: [NO_ERRORS_SCHEMA],
  providers: [LaboFinishLineService],
  entryComponents: [LaboFinishLineCuDialogComponent]
})
export class LaboFinishLinesModule { }
