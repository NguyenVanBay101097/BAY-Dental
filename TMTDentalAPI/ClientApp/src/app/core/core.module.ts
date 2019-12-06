import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WebFormat } from './services/web-format.service';
import { WebUtils } from './services/web-utils.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ],
  exports: [
  ],
  providers: [
    WebFormat,
    WebUtils
  ]
})
export class CoreModule { }
