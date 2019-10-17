import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IrModelsRoutingModule } from './ir-models-routing.module';
import { IRModelService } from './ir-model.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    IrModelsRoutingModule
  ],
  providers: [IRModelService]
})
export class IrModelsModule { }
