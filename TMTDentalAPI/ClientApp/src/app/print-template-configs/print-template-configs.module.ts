import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PrintTemplatesRoutingModule } from './print-template-configs-routing.module';
import { CKEditorModule } from 'ckeditor4-angular';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { PrintTemplateConfigListComponent } from './print-template-config-list/print-template-config-list.component';
import { PrintTemplateConfigCuComponent } from './print-template-config-cu/print-template-config-cu.component';
import { PrintTemplateConfigService, SafeHtmlPipe } from './print-template-config.service';

@NgModule({
  declarations: [PrintTemplateConfigListComponent, PrintTemplateConfigCuComponent, SafeHtmlPipe],
  imports: [
    CommonModule,
    PrintTemplatesRoutingModule,
    CKEditorModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    SharedModule,
  ],
  providers: [
    PrintTemplateConfigService,
  ],
})
export class PrintTemplateConfigsModule { }
