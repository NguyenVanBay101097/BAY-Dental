import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountInvoiceReportRevenueEmployeeComponent } from './account-invoice-report-revenue-employee/account-invoice-report-revenue-employee.component';
import { AccountInvoiceReportRevenueManageComponent } from './account-invoice-report-revenue-manage/account-invoice-report-revenue-manage.component';
import { AccountInvoiceReportRevenuePartnerComponent } from './account-invoice-report-revenue-partner/account-invoice-report-revenue-partner.component';
import { AccountInvoiceReportRevenueServiceComponent } from './account-invoice-report-revenue-service/account-invoice-report-revenue-service.component';
import { AccountInvoiceReportRevenueComponent } from './account-invoice-report-revenue/account-invoice-report-revenue.component';
import { SaleOrderReportRevenueComponent } from './sale-order-report-revenue/sale-order-report-revenue.component';

const routes: Routes = [
  { path: '',component: AccountInvoiceReportRevenueManageComponent,
  children: [
  { path: 'revenue-time', component:  AccountInvoiceReportRevenueComponent},
  { path: 'revenue-service', component:  AccountInvoiceReportRevenueServiceComponent},
  { path: 'revenue-employee', component:  AccountInvoiceReportRevenueEmployeeComponent},
  { path: 'revenue-partner', component:  AccountInvoiceReportRevenuePartnerComponent},
  { path: 'revenue-expecting', component:  SaleOrderReportRevenueComponent},
  {path: '', redirectTo: 'revenue-time', pathMatch: 'full'}
  ]
 },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountInvoiceReportsRoutingModule { }
