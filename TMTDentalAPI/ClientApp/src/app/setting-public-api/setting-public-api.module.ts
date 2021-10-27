import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GenerateTokenComponent } from './generate-token/generate-token.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SettingPublicApiRoutingModule } from './setting-public-api-routing.module';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [GenerateTokenComponent],
  imports: [
    SettingPublicApiRoutingModule,
    CommonModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    NgbModule,
    SharedModule
  ],
})
export class SettingPublicApiModule { }
