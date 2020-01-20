import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ZaloOaConfigRoutingModule } from './zalo-oa-config-routing.module';
import { ZaloOaConfigEstablishComponent } from './zalo-oa-config-establish/zalo-oa-config-establish.component';
import { ZaloOAConfigService } from './zalo-oa-config.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [ZaloOaConfigEstablishComponent],
  imports: [
    CommonModule,
    ZaloOaConfigRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    ZaloOAConfigService
  ]
})
export class ZaloOaConfigModule { }
