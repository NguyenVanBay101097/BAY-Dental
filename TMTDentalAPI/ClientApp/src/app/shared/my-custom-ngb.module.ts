import { NgModule } from '@angular/core';
import { NgbDatepickerModule, NgbDropdownModule, NgbModalModule, NgbNavModule, NgbPopoverModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';


const ngb = [
    NgbDatepickerModule,
    NgbDropdownModule,
    NgbModalModule,
    NgbNavModule,
    NgbPopoverModule,
    NgbPaginationModule
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