import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// import { HrPayrollStructureListComponent } from './hr-payroll-structure-list/hr-payroll-structure-list.component';
import { HrPayrollStructureCreateUpdateComponent } from './hr-payroll-structure-create-update/hr-payroll-structure-create-update.component';
// import { HrPayrollStructureTypeListComponent } from './hr-payroll-structure-type-list/hr-payroll-structure-type-list.component';
// import { HrPayslipToPayListComponent } from './hr-payslip-to-pay-list/hr-payslip-to-pay-list.component';
import { HrPayslipToPayCreateUpdateComponent } from './hr-payslip-to-pay-create-update/hr-payslip-to-pay-create-update.component';
// import { HrPayslipRunListComponent } from './hr-payslip-run-list/hr-payslip-run-list.component';
import { HrPayslipRunFormComponent } from './hr-payslip-run-form/hr-payslip-run-form.component';
// import { HrSalaryConfigCreateUpdateComponent } from './hr-salary-config-create-update/hr-salary-config-create-update.component';
import { HrSalaryReportListComponent } from './hr-salary-report-list/hr-salary-report-list.component';
import { TimeKeepingViewCalendarComponent } from '../time-keeping/time-keeping-view-calendar/time-keeping-view-calendar.component';
import { SalaryPaymentListComponent } from '../salary-payment/salary-payment-list/salary-payment-list.component';
import { SalaryPaymentFormComponent } from '../salary-payment/salary-payment-form/salary-payment-form.component';
import { HrJobListComponent } from '../hr-jobs/hr-job-list/hr-job-list.component';

const routes: Routes = [
  // {
  //   path: 'payroll-structures',
  //   component: HrPayrollStructureListComponent
  // },
  {
    path: 'payroll-structures/create',
    component: HrPayrollStructureCreateUpdateComponent
  },
  {
    path: 'payroll-structures/edit/:id',
    component: HrPayrollStructureCreateUpdateComponent
  },
  // {
  //   path: 'payroll-structure-types', component: HrPayrollStructureTypeListComponent
  // },
  // {
  //   path: 'payslips', component: HrPayslipToPayListComponent
  // },
  {
    path: 'payslips/create', component: HrPayslipToPayCreateUpdateComponent
  },
  {
    path: 'payslips/edit/:id', component: HrPayslipToPayCreateUpdateComponent
  },
  // {
  //   path: 'payslip-runs', component: HrPayslipRunListComponent
  // },
  {
    path: 'payslip-run/form', component: HrPayslipRunFormComponent
  },
  // {
  //   path: 'salary-configs', component: HrSalaryConfigCreateUpdateComponent
  // },
  {
    path: 'salary-reports', component: HrSalaryReportListComponent
  },
  {
    path: 'time-keepings',
    component: TimeKeepingViewCalendarComponent
  },
  {
    path: 'salary-payment',
    component: SalaryPaymentListComponent
  },
  {
    path: 'salary-payment/form',
    component: SalaryPaymentFormComponent
  },
  {
    path: 'jobs',
    component: HrJobListComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HrsRoutingModule { }
