import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HrsRoutingModule } from './hrs-routing.module';
import { HrPayrollStructureListComponent } from './hr-payroll-structure-list/hr-payroll-structure-list.component';
import { HrPayrollStructureCreateUpdateComponent } from './hr-payroll-structure-create-update/hr-payroll-structure-create-update.component';
import { HrPayrollStructureService } from './hr-payroll-structure.service';
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
import { HrPayslipService } from './hr-payslip.service';
import { HrPayslipToPayCreateUpdateComponent } from './hr-payslip-to-pay-create-update/hr-payslip-to-pay-create-update.component';
import { HrPayslipToPayListComponent } from './hr-payslip-to-pay-list/hr-payslip-to-pay-list.component';
import { HrPayslipLineListComponent } from './hr-payslip-line-list/hr-payslip-line-list.component';
import { EmployeeCreateUpdateComponent } from '../employees/employee-create-update/employee-create-update.component';
import { HrPaysliprunService } from './hr-paysliprun.service';
import { HrPayslipRunListComponent } from './hr-payslip-run-list/hr-payslip-run-list.component';
import { HrPayslipRunFormComponent } from './hr-payslip-run-form/hr-payslip-run-form.component';
import { HrPayslipRunConfirmDialogComponent } from './hr-payslip-run-confirm-dialog/hr-payslip-run-confirm-dialog.component';
import { SelectEmployeeDialogComponent } from '../shared/select-employee-dialog/select-employee-dialog.component';

@NgModule({
  declarations: [
    HrPayrollStructureTypeListComponent,
    HrPayrollStructureTypeCreateComponent,
    HrPayrollStructureListComponent,
    HrPayrollStructureCreateUpdateComponent,
    HrSalaryRuleListComponent,
    HrSalaryRuleCrudDialogComponent,
    HrPayslipToPayCreateUpdateComponent,
    HrPayslipToPayListComponent,
    HrPayslipLineListComponent,
    EmployeeCreateUpdateComponent,
    HrPayslipRunListComponent,
    HrPayslipRunFormComponent,
    HrPayslipRunConfirmDialogComponent,
    SelectEmployeeDialogComponent
  ],
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
    ColumnResizingService,
    HrPayslipService,
    HrPaysliprunService
  ],
  entryComponents: [
    HrSalaryRuleCrudDialogComponent, HrPayrollStructureTypeCreateComponent,
    EmployeeCreateUpdateComponent,HrPayslipRunConfirmDialogComponent,SelectEmployeeDialogComponent
  ]
})
export class HrsModule { }
