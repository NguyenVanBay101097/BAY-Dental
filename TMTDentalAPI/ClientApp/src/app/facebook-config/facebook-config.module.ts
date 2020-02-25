import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FacebookConfigRoutingModule } from './facebook-config-routing.module';
import { FacebookConfigEstablishComponent } from './facebook-config-establish/facebook-config-establish.component';

@NgModule({
  declarations: [FacebookConfigEstablishComponent],
  imports: [
    CommonModule,
    FacebookConfigRoutingModule
  ]
})
export class FacebookConfigModule { }
