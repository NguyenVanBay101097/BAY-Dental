import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { LayoutModule } from '../shared/layout/layout.module';
import { AppHeaderComponent } from './app-header.component';
import { HeaderCountdownComponent } from './header-countdown/header-countdown.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    LayoutModule
  ],
  exports: [
    AppHeaderComponent,
    LayoutModule,
    HeaderCountdownComponent
  ],
  declarations: [
    AppHeaderComponent,
    HeaderCountdownComponent
  ]
})
export class AppHeaderModule {}
