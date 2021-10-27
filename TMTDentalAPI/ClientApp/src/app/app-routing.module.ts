import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';
import { AppHomeComponent } from './app-home/app-home.component';
import { AuthGuard } from './auth/auth.guard';
import { PartnerSupplierListComponent } from './catalog/partner-supplier-list/partner-supplier-list.component';

const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: '',
    component: AppHomeComponent,
    children: [
      {
        path: 'agents',
        loadChildren: () => import('./agents/agents.module').then(m => m.AgentsModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'quotations',
        loadChildren: () => import('./quotations/quotations.module').then(m => m.QuotationsModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'stock-inventories',
        loadChildren: () => import('./stock-inventories/stock-inventories.module').then(m => m.StockInventoriesModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'surveys',
        loadChildren: () => import('./surveys/surveys.module').then(m => m.SurveysModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'medicine-orders',
        loadChildren: () => import('./medicine-order/medicine-order.module').then(m => m.MedicineOrderModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'hr',
        loadChildren: () => import('./hrs/hrs.module').then(m => m.HrsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'member-level',
        loadChildren: () => import('./member-level/member-level.module').then(m => m.MemberLevelModule)
      },
      {
        path: 'products',
        loadChildren: () => import('./products/products.module').then(m => m.ProductsModule)
      },
      {
        path: 'customer-management',
        loadChildren: () => import('./customer-management/customer-management.module').then(m => m.CustomerManagementModule)
      },
      {
        path: 'labo-management',
        loadChildren: () => import('./labo-management/labo-management.module').then(m => m.LaboManagementModule)
      },
      {
        path: 'sample-prescriptions',
        loadChildren: () => import('./sample-prescriptions/sample-prescriptions.module').then(m => m.SamplePrescriptionsModule),
      },
      {
        path: 'uoms',
        loadChildren: () => import('./uoms/uom.module').then(m => m.UomModule),
      },
      {
        path: 'uom-categories',
        loadChildren: () => import('./uom-categories/uom-category.module').then(m => m.UomCategoryModule),
      },
      {
        path: 'commissions',
        loadChildren: () => import('./commissions/commissions.module').then(m => m.CommissionsModule),
      },
      {
        path: 'employees',
        loadChildren: () => import('./employees/employees.module').then(m => m.EmployeesModule),
      },
      {
        path: 'tooth-diagnosis',
        loadChildren: () => import('./tooth-diagnosis/tooth-diagnosis.module').then(m => m.ToothDiagnosisModule),
      },
      {
        path: 'loai-thu-chi',
        loadChildren: () => import('./loai-thu-chi/loai-thu-chi.module').then(m => m.LoaiThuChiModule),
      },
      {
        path: 'stock',
        loadChildren: () => import('./stock-inventories/stock-inventories.module').then(m => m.StockInventoriesModule),
      },
      {
        path: 'companies',
        loadChildren: () => import('./companies/companies.module').then(m => m.CompaniesModule),
      },
      {
        path: 'roles',
        loadChildren: () => import('./roles/roles.module').then(m => m.RolesModule),
      },
      {
        path: 'config-settings',
        loadChildren: () => import('./res-config-settings/res-config-settings.module').then(m => m.ResConfigSettingsModule),
      },
      {
        path: 'print-template-config',
        loadChildren: () => import('./print-template-configs/print-template-configs.module').then(m => m.PrintTemplateConfigsModule),
      },
      {
        path: 'sale-dashboard-reports',
        loadChildren: () => import('./sale-dashboard-reports/sale-dashboard-reports.module').then(m => m.SaleDashboardReportsModule),
      },
      {
        path: 'financial-report',
        loadChildren: () => import('./account-financial-report/account-financial-report.module').then(m => m.AccountFinancialReportModule),
      },
      {
        path: 'report-general-ledgers',
        loadChildren: () => import('./account-report-general-ledgers/account-report-general-ledgers.module').then(m => m.AccountReportGeneralLedgersModule),
      },
      {
        path: 'account-invoice-reports',
        loadChildren: () => import('./account-invoice-reports/account-invoice-reports.module').then(m => m.AccountInvoiceReportsModule),
      },
      {
        path: 'report-account-common',
        loadChildren: () => import('./account-common-partner-reports/account-common-partner-reports.module').then(m => m.AccountCommonPartnerReportsModule)
      },
      {
        path: 'sale-dashboard-reports',
        loadChildren: () => import('./sale-dashboard-reports/sale-dashboard-reports.module').then(m => m.SaleDashboardReportsModule),
      },
      {
        path: 'sale-report',
        loadChildren: () => import('./sale-report/sale-report.module').then(m => m.SaleReportModule)
      },
      {
        path: 'customer-receipt-reports',
        loadChildren: () => import('./customer-receipt-reports/customer-receipt-reports.module').then(m => m.CustomerReceiptReportsModule),
      },
      {
        path: 'partner-report-location',
        loadChildren: () => import('./partner-report/partner-report.module').then(m => m.PartnerReportModule),
      },
      {
        path: 'report-partner-sources',
        loadChildren: () => import('./report-partner-sources/report-partner-sources.module').then(m => m.ReportPartnerSourcesModule),
      },
      {
        path: 'sale-orders',
        loadChildren: () => import('./sale-orders/sale-orders.module').then(m => m.SaleOrdersModule),
      },
      {
        path: 'commission-settlements',
        loadChildren: () => import('./commission-settlements/commission-settlements.module').then(m => m.CommissionSettlementsModule),
      },
      {
        path: 'partners',
        loadChildren: () => import('./partners/partners.module').then(m => m.PartnersModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'sale-orders',
        loadChildren: () => import('./sale-orders/sale-orders.module').then(m => m.SaleOrdersModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'sale-quotations',
        loadChildren: () => import('./sale-quotations/sale-quotations.module').then(m => m.SaleQuotationsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'appointments',
        loadChildren: () => import('./appointment/appointment.module').then(m => m.AppointmentModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'labo-orders',
        loadChildren: () => import('./labo-orders/labo-orders.module').then(m => m.LaboOrdersModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'purchase',
        loadChildren: () => import('./purchase-orders/purchase-orders.module').then(m => m.PurchaseOrdersModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'stock',
        loadChildren: () => import('./stock-pickings/stock-pickings.module').then(m => m.StockPickingsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'phieu-thu-chi',
        loadChildren: () => import('./phieu-thu-chi/phieu-thu-chi.module').then(m => m.PhieuThuChiModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'real-revenue-report',
        loadChildren: () => import('./real-revenue-report/real-revenue-report.module').then(m => m.RealRevenueReportModule)
      },
      {
        path: 'dot-kham-report',
        loadChildren: () => import('./dot-kham-steps/dot-kham-steps.module').then(m => m.DotKhamStepsModule)
      },
      {
        path: 'programs',
        loadChildren: () => import('./sale-coupon-promotion/sale-coupon-promotion.module').then(m => m.SaleCouponPromotionModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'commission-report',
        loadChildren: () => import('./commission-reports/commission-reports.module').then(m => m.CommissionReportsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'tcare',
        loadChildren: () => import('./tcare/tcare.module').then(m => m.TcareModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'socials',
        loadChildren: () => import('./socials-channel/socials-channel.module').then(m => m.SocialsChannelModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'zalo-config',
        loadChildren: () => import('./zalo-oa-config/zalo-oa-config.module').then(m => m.ZaloOaConfigModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'dashboard',
        loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'cash-book',
        loadChildren: () => import('./cash-book/cash-book.module').then(m => m.CashBookModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'treatment-lines',
        loadChildren: () => import('./dot-kham-lines/dot-kham-lines.module').then(m => m.DotKhamLinesModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'sms',
        loadChildren: () => import('./sms/sms.module').then(m => m.SmsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'commission-settlements',
        loadChildren: () => import('./commission-settlements/commission-settlements.module').then(m => m.CommissionSettlementsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'day-dashboard-report',
        loadChildren: () => import('./day-dashboard-report/day-dashboard-report.module').then(m => m.DayDashboardReportModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'setting-public-api',
        loadChildren: () => import('./setting-public-api/setting-public-api.module').then(m => m.SettingPublicApiModule),
        canActivate: [AuthGuard]
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
        canActivate: [AuthGuard]
      },
      {
        path: '**',
        redirectTo: 'dashboard',
        pathMatch: 'full',
        canActivate: [AuthGuard]
      },
      {
        path: 'customer-statistics',
        loadChildren: () => import('./customer-statistics/customer-statistics.module').then(m => m.CustomerStatisticsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'users',
        loadChildren: () => import('./users/users.module').then(m => m.UsersModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'labo-order-lines',
        loadChildren: () => import('./labo-order-lines/labo-order-lines.module').then(m => m.LaboOrderLinesModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'emp-categories',
        loadChildren: () => import('./employee-categories/employee-categories.module').then(m => m.EmployeeCategoriesModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'card-cards',
        loadChildren: () => import('./card-cards/card-cards.module').then(m => m.CardCardsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'stock-reports',
        loadChildren: () => import('./stock-reports/stock-reports.module').then(m => m.StockReportsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'partner-categories',
        loadChildren: () => import('./partner-categories/partner-categories.module').then(m => m.PartnerCategoriesModule),
        canActivate: [AuthGuard]
      },
    ]
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(
      routes,
      {
        preloadingStrategy: PreloadAllModules
      }
    )
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
