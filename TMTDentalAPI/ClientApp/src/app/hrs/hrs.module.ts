import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { AccountCommonPartnerReportsModule } from '../account-common-partner-reports/account-common-partner-reports.module';
import { EmployeeService } from '../employees/employee.service';
import { HrJobsModule } from '../hr-jobs/hr-jobs.module';
import { SalaryPaymentModule } from '../salary-payment/salary-payment.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SelectEmployeeDialogComponent } from '../shared/select-employee-dialog/select-employee-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { TimeKeepingModule } from '../time-keeping/time-keeping.module';
import { HrPayrollStructureCreateUpdateComponent } from './hr-payroll-structure-create-update/hr-payroll-structure-create-update.component';
import { HrPayrollStructureListComponent } from './hr-payroll-structure-list/hr-payroll-structure-list.component';
import { HrPayrollStructureTypeCreateComponent } from './hr-payroll-structure-type-create/hr-payroll-structure-type-create.component';
import { HrPayrollStructureTypeListComponent } from './hr-payroll-structure-type-list/hr-payroll-structure-type-list.component';
import { HrPayrollStructureService } from './hr-payroll-structure.service';
import { HrPayslipDateFilterComponent } from './hr-payslip-date-filter/hr-payslip-date-filter.component';
import { HrPayslipRunConfirmDialogComponent } from './hr-payslip-run-confirm-dialog/hr-payslip-run-confirm-dialog.component';
import { HrPayslipRunFormComponent } from './hr-payslip-run-form/hr-payslip-run-form.component';
import { HrPayslipRunListComponent } from './hr-payslip-run-list/hr-payslip-run-list.component';
import { HrPayslipToPayCreateUpdateComponent } from './hr-payslip-to-pay-create-update/hr-payslip-to-pay-create-update.component';
import { HrPayslipToPayListComponent } from './hr-payslip-to-pay-list/hr-payslip-to-pay-list.component';
import { HrPayslipService } from './hr-payslip.service';
import { HrPaysliprunService } from './hr-paysliprun.service';
import { HrSalaryConfigCreateUpdateComponent } from './hr-salary-config-create-update/hr-salary-config-create-update.component';
import { HrSalaryConfigService } from './hr-salary-config.service';
import { HrSalaryPaymentComponent } from './hr-salary-payment/hr-salary-payment.component';
import { HrSalaryReportDetailComponent } from './hr-salary-report-detail/hr-salary-report-detail.component';
import { HrSalaryReportListComponent } from './hr-salary-report-list/hr-salary-report-list.component';
import { HrSalaryRuleCrudDialogComponent } from './hr-salary-rule-crud-dialog/hr-salary-rule-crud-dialog.component';
import { HrsRoutingModule } from './hrs-routing.module';


@NgModule({
  declarations: [
    HrPayrollStructureTypeListComponent,
    HrPayrollStructureTypeCreateComponent,
    HrPayrollStructureListComponent,
    HrPayrollStructureCreateUpdateComponent,
    HrSalaryRuleCrudDialogComponent,
    HrPayslipToPayCreateUpdateComponent,
    HrPayslipToPayListComponent,
    HrPayslipRunListComponent,
    HrPayslipRunFormComponent,
    HrPayslipRunConfirmDialogComponent,
    SelectEmployeeDialogComponent,
    HrPayslipDateFilterComponent,
    HrSalaryConfigCreateUpdateComponent,
    HrSalaryReportListComponent,
    HrSalaryReportDetailComponent,
    HrSalaryPaymentComponent,
  ],
  imports: [
    CommonModule,
    HrsRoutingModule,
    MyCustomKendoModule,
    AccountCommonPartnerReportsModule,
    ReactiveFormsModule,
    FormsModule,
    LayoutModule,
    DropDownsModule,
    NgbModule,
    SharedModule,
    TimeKeepingModule,
    SalaryPaymentModule,
    HrJobsModule
  ],
  providers: [
    HrPayrollStructureService,
    HrPayslipService,
    HrPaysliprunService,
    EmployeeService,
    HrSalaryConfigService
  ],
  entryComponents: [
    HrSalaryRuleCrudDialogComponent, HrPayrollStructureTypeCreateComponent,
    HrPayslipRunConfirmDialogComponent, SelectEmployeeDialogComponent, HrSalaryPaymentComponent
  ]
})
export class HrsModule { }
