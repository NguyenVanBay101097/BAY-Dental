import { GridModule } from '@progress/kendo-angular-grid';
import { NgModule } from '@angular/core';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { NumericTextBoxModule } from '@progress/kendo-angular-inputs';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { DateTimePickerModule } from '@progress/kendo-angular-dateinputs';
import { TimePickerModule } from '@progress/kendo-angular-dateinputs';
import { ComboBoxModule, DropDownListModule, DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { MultiSelectModule } from '@progress/kendo-angular-dropdowns';
import { ChartModule } from '@progress/kendo-angular-charts';
import { TreeViewModule } from '@progress/kendo-angular-treeview';
import { PDFModule, ExcelModule } from '@progress/kendo-angular-grid';
import { PAGER_CONFIG, PAGER_GRID_CONFIG } from './pager-grid-kendo.config';

const kendo = [
    GridModule,
    NumericTextBoxModule,
    DatePickerModule,
    DateTimePickerModule,
    TimePickerModule,
    ComboBoxModule,
    MultiSelectModule,
    NotificationModule,
    ChartModule,
    TreeViewModule,
    ExcelModule,
    PDFModule,
    DropDownListModule,
    DropDownsModule
];

@NgModule({
    exports: [
        ...kendo
    ],
    imports: [
        ...kendo
    ],
    providers: [{ provide: PAGER_GRID_CONFIG, useValue: PAGER_CONFIG }],
})
export class MyCustomKendoModule { }