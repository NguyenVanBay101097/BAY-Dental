import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HrsRoutingModule } from './hrs-routing.module';
import { HrPayrollStructureListComponent } from './hr-payroll-structure-list/hr-payroll-structure-list.component';
import { HrPayrollStructureCreateUpdateComponent } from './hr-payroll-structure-create-update/hr-payroll-structure-create-update.component';
import { HrPayrollStructureService } from './hr-payroll-structure.service';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { HrSalaryRuleCrudDialogComponent } from './hr-salary-rule-crud-dialog/hr-salary-rule-crud-dialog.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { HrPayrollStructureTypeListComponent } from './hr-payroll-structure-type-list/hr-payroll-structure-type-list.component';
import { HrPayrollStructureTypeCreateComponent } from './hr-payroll-structure-type-create/hr-payroll-structure-type-create.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { HrPayslipService } from './hr-payslip.service';
import { HrPayslipToPayCreateUpdateComponent } from './hr-payslip-to-pay-create-update/hr-payslip-to-pay-create-update.component';
import { HrPayslipToPayListComponent } from './hr-payslip-to-pay-list/hr-payslip-to-pay-list.component';
import { EmployeeCreateUpdateComponent } from '../employees/employee-create-update/employee-create-update.component';
import { SharedModule } from '../shared/shared.module';
import { EmployeeService } from '../employees/employee.service';
import { HrPayslipDateFilterComponent } from './hr-payslip-date-filter/hr-payslip-date-filter.component';

@NgModule({
  declarations: [
    HrPayrollStructureTypeListComponent,
    HrPayrollStructureTypeCreateComponent,
    HrPayrollStructureListComponent,
    HrPayrollStructureCreateUpdateComponent,
    HrSalaryRuleCrudDialogComponent,
    HrPayslipToPayCreateUpdateComponent,
    HrPayslipToPayListComponent,
    HrPayslipDateFilterComponent,
  ],
  imports: [
    CommonModule,
    HrsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    LayoutModule,
    DropDownsModule,
    NgbModule,
    SharedModule
  ],
  providers: [
    HrPayrollStructureService,
    HrPayslipService
  ],
  entryComponents: [
    HrSalaryRuleCrudDialogComponent, HrPayrollStructureTypeCreateComponent,
  ]
})
export class HrsModule { }
