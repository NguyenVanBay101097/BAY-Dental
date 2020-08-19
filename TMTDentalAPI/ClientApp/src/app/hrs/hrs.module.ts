import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HrsRoutingModule } from './hrs-routing.module';
import { HrPayrollStructureListComponent } from './hr-payroll-structure-list/hr-payroll-structure-list.component';
import { HrPayrollStructureCreateUpdateComponent } from './hr-payroll-structure-create-update/hr-payroll-structure-create-update.component';
import { HrPayrollStructureService } from './hr-PayrollStructure.service';
import { SharedModule, ColumnResizingService } from '@progress/kendo-angular-grid';
import { HrSalaryRuleListComponent } from './hr-salary-rule-list/hr-salary-rule-list.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { HrSalaryRuleCrudDialogComponent } from './hr-salary-rule-crud-dialog/hr-salary-rule-crud-dialog.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { HrPayrollStructureTypeListComponent } from './hr-payroll-structure-type-list/hr-payroll-structure-type-list.component';
import { HrPayrollStructureTypeCreateComponent } from './hr-payroll-structure-type-create/hr-payroll-structure-type-create.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';

@NgModule({
  declarations: [HrPayrollStructureTypeListComponent, HrPayrollStructureTypeCreateComponent, HrPayrollStructureListComponent, HrPayrollStructureCreateUpdateComponent, HrSalaryRuleListComponent, HrSalaryRuleCrudDialogComponent],
  imports: [
    CommonModule,
    HrsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    LayoutModule,
    DropDownsModule,
    NgbModule
  ],
  providers: [
    HrPayrollStructureService,
    ColumnResizingService
  ],
  entryComponents: [
    HrSalaryRuleCrudDialogComponent, HrPayrollStructureTypeCreateComponent
  ]
})
export class HrsModule { }
