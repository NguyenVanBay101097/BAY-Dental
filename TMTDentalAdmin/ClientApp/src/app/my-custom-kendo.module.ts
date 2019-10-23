import { GridModule } from '@progress/kendo-angular-grid';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { NgModule } from '@angular/core';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { ChartsModule } from '@progress/kendo-angular-charts';
import { SchedulerModule } from '@progress/kendo-angular-scheduler';
import { TreeViewModule } from '@progress/kendo-angular-treeview';
import { UploadModule } from '@progress/kendo-angular-upload';

const kendo = [
    GridModule,
    ButtonsModule,
    DialogsModule,
    InputsModule,
    DropDownsModule,
    DateInputsModule,
    NotificationModule,
    LayoutModule,
    ChartsModule,
    TreeViewModule,
    SchedulerModule,
    UploadModule,
];

@NgModule({
    exports: [
        ...kendo
    ],
    imports: [
        ...kendo
    ]
})
export class MyCustomKendoModule { }