import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IrModelsRoutingModule } from './ir-models-routing.module';
import { IRModelService } from './ir-model.service';
import { IrModelListComponent } from './ir-model-list/ir-model-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { IrModelCuDialogComponent } from './ir-model-cu-dialog/ir-model-cu-dialog.component';

@NgModule({
  declarations: [IrModelListComponent, IrModelCuDialogComponent],
  imports: [
    CommonModule,
    IrModelsRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [IRModelService],
  entryComponents: [
    IrModelCuDialogComponent
  ]
})
export class IrModelsModule { }
