import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TenantSaleOrderProcessUpdatesRoutingModule } from './tenant-sale-order-process-updates-routing.module';
import { TenantSaleOrderProcessUpdateListComponent } from './tenant-sale-order-process-update-list/tenant-sale-order-process-update-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from 'app/my-custom-kendo.module';

@NgModule({
  declarations: [TenantSaleOrderProcessUpdateListComponent],
  imports: [
    CommonModule,
    TenantSaleOrderProcessUpdatesRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule
  ]
})
export class TenantSaleOrderProcessUpdatesModule { }
