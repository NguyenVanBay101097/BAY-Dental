import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RoutingsRoutingModule } from './routings-routing.module';
import { RoutingListComponent } from './routing-list/routing-list.component';
import { RoutingService } from './routing.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RoutingCreateUpdateComponent } from './routing-create-update/routing-create-update.component';
import { RoutingLineCuDialogComponent } from './routing-line-cu-dialog/routing-line-cu-dialog.component';

@NgModule({
  declarations: [RoutingListComponent, RoutingCreateUpdateComponent, RoutingLineCuDialogComponent],
  imports: [
    CommonModule,
    RoutingsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [
    RoutingService
  ],
  entryComponents: [
    RoutingLineCuDialogComponent
  ]
})
export class RoutingsModule { }
