import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ResConfigSettingsRoutingModule } from './res-config-settings-routing.module';
import { ResConfigSettingsFormComponent } from './res-config-settings-form/res-config-settings-form.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ResConfigSettingsService } from './res-config-settings.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';

@NgModule({
  declarations: [ResConfigSettingsFormComponent],
  imports: [
    CommonModule,
    ResConfigSettingsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule
  ],
  providers: [
    ResConfigSettingsService
  ]
})
export class ResConfigSettingsModule { }
