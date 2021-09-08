import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';
import { AppHomeComponent } from './app-home/app-home.component';
import { AuthGuard } from './auth/auth.guard';
import { PartnerGeneralSettingsComponent } from './partner-general-settings/partner-general-settings.component';

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
        path: 'catalog',
        loadChildren: () => import('./catalog/catalog.module').then(m => m.CatalogModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'setting',
        loadChildren: () => import('./setting/setting.module').then(m => m.SettingModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'report',
        loadChildren: () => import('./report/report.module').then(m => m.ReportModule),
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
