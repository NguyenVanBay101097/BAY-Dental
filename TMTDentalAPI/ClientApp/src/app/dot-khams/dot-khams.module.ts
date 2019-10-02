import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DotKhamsRoutingModule } from './dot-khams-routing.module';
import { DotKhamService } from './dot-kham.service';
import { DotKhamCreateUpdateComponent } from './dot-kham-create-update/dot-kham-create-update.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DotKhamLineService } from './dot-kham-line.service';
import { DotKhamLineOperationService } from './dot-kham-line-operation.service';
import { DotKhamListComponent } from './dot-kham-list/dot-kham-list.component';
import { DragDropModule } from '@angular/cdk/drag-drop';

@NgModule({
  declarations: [DotKhamCreateUpdateComponent, DotKhamListComponent],
  imports: [
    CommonModule,
    DotKhamsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    DragDropModule
  ],
  providers: [
    DotKhamService,
    DotKhamLineService,
    DotKhamLineOperationService
  ],
  entryComponents: [
    DotKhamCreateUpdateComponent
  ]
})
export class DotKhamsModule { }
