import { NgModule } from '@angular/core';
import { NgbDatepickerModule, NgbDropdownModule, NgbModalModule, NgbNavModule, NgbPopoverModule } from '@ng-bootstrap/ng-bootstrap';


const ngb = [
    NgbDatepickerModule,
    NgbDropdownModule,
    NgbModalModule,
    NgbNavModule,
    NgbPopoverModule,
];

@NgModule({
    exports: [
        ...ngb
    ],
    imports: [
        ...ngb
    ]
})

export class MyCustomNgbModule { }