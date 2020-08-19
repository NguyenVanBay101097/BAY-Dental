import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HrsRoutingModule } from './hrs-routing.module';
import { HrPayrollStructureTypeListComponent } from './hr-payroll-structure-type-list/hr-payroll-structure-type-list.component';
import { HrPayrollStructureTypeCreateComponent } from './hr-payroll-structure-type-create/hr-payroll-structure-type-create.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';

@NgModule({
  declarations: [HrPayrollStructureTypeListComponent, HrPayrollStructureTypeCreateComponent],
  imports: [
    CommonModule,
    HrsRoutingModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule
  ],
  entryComponents: [HrPayrollStructureTypeCreateComponent]
})
export class HrsModule { }
