import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PrintTemplatesRoutingModule } from './print-template-configs-routing.module';
import { CKEditorModule } from 'ckeditor4-angular';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { PrintTemplateConfigCuComponent } from './print-template-config-cu/print-template-config-cu.component';
import { PrintTemplateConfigService, SafeHtmlPipe } from './print-template-config.service';
import { PrintTemplateService } from './print-template.service';
import { PrintPaperSizeCreateUpdateDialogComponent } from '../config-prints/print-paper-size-create-update-dialog/print-paper-size-create-update-dialog.component';
import { KeywordListDialogComponent } from './keyword-list-dialog/keyword-list-dialog.component';

@NgModule({
  declarations: [PrintTemplateConfigCuComponent, SafeHtmlPipe, PrintPaperSizeCreateUpdateDialogComponent, KeywordListDialogComponent],
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
    PrintPaperSizeCreateUpdateDialogComponent,
    KeywordListDialogComponent
  ]
})
export class PrintTemplateConfigsModule { }
