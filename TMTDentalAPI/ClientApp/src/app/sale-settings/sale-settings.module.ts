import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaleSettingsRoutingModule } from './sale-settings-routing.module';
import { SaleSettingsOverviewComponent } from './sale-settings-overview/sale-settings-overview.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SaleSettingsService } from './sale-settings.service';

@NgModule({
  declarations: [SaleSettingsOverviewComponent],
  imports: [
    CommonModule,
    SaleSettingsRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
  ],
  providers: [
    SaleSettingsService
  ]
})
export class SaleSettingsModule { }
