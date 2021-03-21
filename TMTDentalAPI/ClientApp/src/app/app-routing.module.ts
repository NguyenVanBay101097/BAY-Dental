import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';

const routes: Routes = [
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
    path: 'sale-dashboard-reports',
    loadChildren: () => import('./sale-dashboard-reports/sale-dashboard-reports.module').then(m => m.SaleDashboardReportsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'work-entry-types',
    loadChildren: () => import('./work-entry-types/work-entry-types.module').then(m => m.WorkEntryTypesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'employee-categories',
    loadChildren: () => import('./employee-categories/employee-categories.module').then(m => m.EmployeeCategoriesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'employees',
    loadChildren: () => import('./employees/employees.module').then(m => m.EmployeesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'hr',
    loadChildren: () => import('./hrs/hrs.module').then(m => m.HrsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'resource-calendars',
    loadChildren: () => import('./resource-calendars/resource-calendars.module').then(m => m.ResourceCalendarsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'salary-payment',
    loadChildren: () => import('./salary-payment/salary-payment.module').then(m => m.SalaryPaymentModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'time-keepings',
    loadChildren: () => import('./time-keeping/time-keeping.module').then(m => m.TimeKeepingModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'histories',
    loadChildren: () => import('./history/history.module').then(m => m.HistoryModule),
    canActivate: [AuthGuard]
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
    path: 'partner-categories',
    loadChildren: () => import('./partner-categories/partner-categories.module').then(m => m.PartnerCategoriesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'partner-sources',
    loadChildren: () => import('./partner-sources/partner-sources.module').then(m => m.PartnerSourcesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'products',
    loadChildren: () => import('./products/products.module').then(m => m.ProductsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'product-categories',
    loadChildren: () => import('./product-categories/product-categories.module').then(m => m.ProductCategoriesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'sample-prescriptions',
    loadChildren: () => import('./sample-prescriptions/sample-prescriptions.module').then(m => m.SamplePrescriptionsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'companies',
    loadChildren: () => import('./companies/companies.module').then(m => m.CompaniesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'users',
    loadChildren: () => import('./users/users.module').then(m => m.UsersModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'res-groups',
    loadChildren: () => import('./res-groups/res-groups.module').then(m => m.ResGroupsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'config-settings',
    loadChildren: () => import('./res-config-settings/res-config-settings.module').then(m => m.ResConfigSettingsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'phieu-thu-chi',
    loadChildren: () => import('./phieu-thu-chi/phieu-thu-chi.module').then(m => m.PhieuThuChiModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'loai-thu-chi',
    loadChildren: () => import('./loai-thu-chi/loai-thu-chi.module').then(m => m.LoaiThuChiModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'financial-report',
    loadChildren: () => import('./account-financial-report/account-financial-report.module').then(m => m.AccountFinancialReportModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'report-general-ledgers',
    loadChildren: () => import('./account-report-general-ledgers/account-report-general-ledgers.module').then(m => m.AccountReportGeneralLedgersModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'revenue-report',
    loadChildren: () => import('./revenue-report/revenue-report.module').then(m => m.RevenueReportModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'report-account-common',
    loadChildren: () => import('./account-common-partner-reports/account-common-partner-reports.module').then(m => m.AccountCommonPartnerReportsModule)
  },
  {
    path: 'stock-report-xuat-nhap-ton',
    loadChildren: () => import('./stock-reports/stock-reports.module').then(m => m.StockReportsModule)
  },
  {
    path: 'real-revenue-report',
    loadChildren: () => import('./real-revenue-report/real-revenue-report.module').then(m => m.RealRevenueReportModule)
  },
  {
    path: 'sale-report',
    loadChildren: () => import('./sale-report/sale-report.module').then(m => m.SaleReportModule)
  },
  {
    path: 'dot-kham-report',
    loadChildren: () => import('./dot-kham-steps/dot-kham-steps.module').then(m => m.DotKhamStepsModule)
  },
  {
    path: 'partner-report-location',
    loadChildren: () => import('./partner-report/partner-report.module').then(m => m.PartnerReportModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'report-partner-sources',
    loadChildren: () => import('./report-partner-sources/report-partner-sources.module').then(m => m.ReportPartnerSourcesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'card-cards',
    loadChildren: () => import('./card-cards/card-cards.module').then(m => m.CardCardsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'card-types',
    loadChildren: () => import('./card-types/card-types.module').then(m => m.CardTypesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'programs',
    loadChildren: () => import('./sale-coupon-promotion/sale-coupon-promotion.module').then(m => m.SaleCouponPromotionModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'service-card-orders',
    loadChildren: () => import('./service-card-orders/service-card-orders.module').then(m => m.ServiceCardOrdersModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'service-card-types',
    loadChildren: () => import('./service-card-types/service-card-types.module').then(m => m.ServiceCardTypesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'service-cards',
    loadChildren: () => import('./service-card-cards/service-card-cards.module').then(m => m.ServiceCardCardsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'uoms',
    loadChildren: () => import('./uoms/uom.module').then(m => m.UomModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'uom-categories',
    loadChildren: () => import('./uom-categories/uom-category.module').then(m => m.UomCategoryModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'commissions',
    loadChildren: () => import('./commissions/commissions.module').then(m => m.CommissionsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'employees',
    loadChildren: () => import('./employees/employees.module').then(m => m.EmployeesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'labo-bite-joints',
    loadChildren: () => import('./labo-bite-joints/labo-bite-joints.module').then(m => m.LaboBiteJointsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'labo-bridges',
    loadChildren: () => import('./labo-bridges/labo-bridges.module').then(m => m.LaboBridgesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'labo-finish-lines',
    loadChildren: () => import('./labo-finish-lines/labo-finish-lines.module').then(m => m.LaboFinishLinesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'commission-report',
    loadChildren: () => import('./commission-reports/commission-reports.module').then(m => m.CommissionReportsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'commission-settlements',
    loadChildren: () => import('./commission-settlements/commission-settlements.module').then(m => m.CommissionSettlementsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'partner-titles',
    loadChildren: () => import('./partner-titles/partner-titles.module').then(m => m.PartnerTitlesModule)
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
    path: 'roles',
    loadChildren: () => import('./roles/roles.module').then(m => m.RolesModule),
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
  }
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
