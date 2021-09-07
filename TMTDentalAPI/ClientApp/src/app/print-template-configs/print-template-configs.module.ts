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
import { PrintTemplateService } from './print-template.service';
import { PrintPaperSizeCreateUpdateDialogComponent } from '../config-prints/print-paper-size-create-update-dialog/print-paper-size-create-update-dialog.component';

@NgModule({
  declarations: [PrintTemplateConfigListComponent, PrintTemplateConfigCuComponent, SafeHtmlPipe, PrintPaperSizeCreateUpdateDialogComponent],
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
    PrintTemplateService,
  ],
  entryComponents: [
    PrintPaperSizeCreateUpdateDialogComponent
  ]
})
export class PrintTemplateConfigsModule { }
