import { GridModule } from '@progress/kendo-angular-grid';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { NgModule } from '@angular/core';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { NotificationModule } from '@progress/kendo-angular-notification';

const kendo = [
    GridModule,
    ButtonsModule,
    InputsModule,
    DropDownsModule,
    DateInputsModule,
    NotificationModule
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