import { BrowserModule } from "@angular/platform-browser";
import { NgModule, LOCALE_ID } from "@angular/core";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { HeroesModule } from "./heroes/heroes.module";
import { ProductCategoriesModule } from "./product-categories/product-categories.module";
import { CoreModule } from "./core/core.module";
import { SharedModule } from "./shared/shared.module";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { MatNativeDateModule } from "@angular/material/core";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { AuthModule } from "./auth/auth.module";
import { ProductsModule } from "./products/products.module";
import { AuthInterceptor } from "./auth/auth-interceptor";

import { IntlModule } from "@progress/kendo-angular-intl";
// Load all required data for the bg locale
import "@progress/kendo-angular-intl/locales/vi/all";
import { SaleOrdersModule } from "./sale-orders/sale-orders.module";
import { UsersModule } from "./users/users.module";
import { PartnersModule } from "./partners/partners.module";
import { AccountInvoicesModule } from "./account-invoices/account-invoices.module";
import { AccountPaymentsModule } from "./account-payments/account-payments.module";
import { HttpHandleErrorInterceptor } from "./http-handle-error-interceptor";
import { AccountJournalsModule } from "./account-journals/account-journals.module";
import { ToothCategoriesModule } from "./tooth-categories/tooth-categories.module";
import { TeethModule } from "./teeth/teeth.module";

import { registerLocaleData } from "@angular/common";
import localeVi from "@angular/common/locales/vi";
import { DotKhamsModule } from "./dot-khams/dot-khams.module";
import { ToaThuocsModule } from "./toa-thuocs/toa-thuocs.module";
import { AppointmentModule } from "./appointment/appointment.module";

import { JwtModule, JwtInterceptor } from "@auth0/angular-jwt";
import { HomeModule } from "./home/home.module";
import { RoutingsModule } from "./routings/routings.module";
import { StockPickingsModule } from "./stock-pickings/stock-pickings.module";
import { StockPickingTypesModule } from "./stock-picking-types/stock-picking-types.module";
import { LaboOrderLinesModule } from "./labo-order-lines/labo-order-lines.module";
import { StockMovesModule } from "./stock-moves/stock-moves.module";
import { PrintLayoutComponent } from "./print-layout/print-layout.component";
import { AccountCommonPartnerReportsModule } from "./account-common-partner-reports/account-common-partner-reports.module";
import { StockReportsModule } from "./stock-reports/stock-reports.module";
import { AccountInvoiceReportsModule } from "./account-invoice-reports/account-invoice-reports.module";
import { RolesModule } from "./roles/roles.module";
import { SchedulerModule } from "@progress/kendo-angular-scheduler";
import { CustomComponentModule } from "./common/common.module";
import { CompaniesModule } from "./companies/companies.module";
import { DotKhamLinesModule } from "./dot-kham-lines/dot-kham-lines.module";
import { PartnerCategoriesModule } from "./partner-categories/partner-categories.module";
import { ChartsModule } from "@progress/kendo-angular-charts";
import "hammerjs";
import { UploadModule } from "@progress/kendo-angular-upload";
import { LayoutModule } from "@progress/kendo-angular-layout";
import { EmployeesModule } from "./employees/employees.module";
import { EmployeeCategoriesModule } from "./employee-categories/employee-categories.module";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { MatStepperModule } from "@angular/material/stepper";
import { HistoryModule } from "./history/history.module";
import { NgbModule, NgbDateParserFormatter } from "@ng-bootstrap/ng-bootstrap";
import { NgbDateCustomParserFormatter } from "./core/ngb-date-custom-parser-formatter";
import { ResGroupsModule } from "./res-groups/res-groups.module";
import { IrModelsModule } from "./ir-models/ir-models.module";
import { IrRulesModule } from "./ir-rules/ir-rules.module";
import { PriceListModule } from "./price-list/price-list.module";
import { LaboOrdersModule } from "./labo-orders/labo-orders.module";
import { PurchaseOrdersModule } from "./purchase-orders/purchase-orders.module";

import { RefreshTokenInterceptor } from './auth/refresh-token-interceptor';
import { RealRevenueReportModule } from './real-revenue-report/real-revenue-report.module';
import { SaleReportModule } from './sale-report/sale-report.module';
import { MomentModule } from 'ngx-moment';
import { MailMessagesModule } from './mail-messages/mail-messages.module';
import 'moment/locale/vi';
import { PartnerReportModule } from './partner-report/partner-report.module';
import { ResBanksModule } from './res-banks/res-banks.module';
import { ResPartnerBanksModule } from './res-partner-banks/res-partner-banks.module';
import { CardTypesModule } from './card-types/card-types.module';
import { CardCardsModule } from './card-cards/card-cards.module';
import { SaleSettingsModule } from './sale-settings/sale-settings.module';
import { SaleCouponPromotionModule } from './sale-coupon-promotion/sale-coupon-promotion.module';
import { PromotionProgramsModule } from './promotion-programs/promotion-programs.module';
import { ResConfigSettingsModule } from './res-config-settings/res-config-settings.module';
import { JournalReportsModule } from './journal-reports/journal-reports.module';
import { ZaloOaConfigModule } from './zalo-oa-config/zalo-oa-config.module';
import { RevenueReportModule } from './revenue-report/revenue-report.module';
import { MarketingCampaignsModule } from './marketing-campaigns/marketing-campaigns.module';
import { FacebookConfigModule } from './facebook-config/facebook-config.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FacebookModule } from 'ngx-facebook';
import { SocialsChannelModule } from './socials-channel/socials-channel.module';
import { ServiceCardTypesModule } from './service-card-types/service-card-types.module';
import { ServiceCardOrdersModule } from './service-card-orders/service-card-orders.module';
import { ServiceCardCardsModule } from './service-card-cards/service-card-cards.module';
import { PickerModule } from '@ctrl/ngx-emoji-mart';
import { EmojiModule } from '@ctrl/ngx-emoji-mart/ngx-emoji';
import { UomCategoryModule } from './uom-categories/uom-category.module';
import { UomModule } from './uoms/uom.module';
import { TcareModule } from './tcare/tcare.module';
import { PartnerCustomerDetailComponent } from './partners/partner-customer-detail/partner-customer-detail.component';
import { PartnerSourcesModule } from "./partner-sources/partner-sources.module";
import { SamplePrescriptionsModule } from './sample-prescriptions/sample-prescriptions.module';
import { ReportPartnerSourcesModule } from './report-partner-sources/report-partner-sources.module';
import { LoaiThuChiModule } from './loai-thu-chi/loai-thu-chi.module';
import { PhieuThuChiModule } from './phieu-thu-chi/phieu-thu-chi.module';
import { AccountFinancialReportModule } from './account-financial-report/account-financial-report.module';
import { AccountReportGeneralLedgersModule } from './account-report-general-ledgers/account-report-general-ledgers.module';

export function tokenGetter() {
  return localStorage.getItem("access_token");
}

registerLocaleData(localeVi, "vi");

@NgModule({
  declarations: [
    AppComponent,
    PrintLayoutComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MatNativeDateModule,
    AppRoutingModule,
    HeroesModule,
    ProductCategoriesModule,
    AppointmentModule,
    CoreModule,
    IntlModule,
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ["http://localhost:50396"],
        blacklistedRoutes: [],
      },
    }),
    FlexLayoutModule,
    SharedModule,
    AuthModule,
    ProductsModule,
    SaleOrdersModule,
    UsersModule,
    PartnersModule,
    AccountInvoicesModule,
    AccountPaymentsModule,
    AccountJournalsModule,
    ToothCategoriesModule,
    TeethModule,
    DotKhamsModule,
    ToaThuocsModule,
    HomeModule,
    EmployeesModule,
    EmployeeCategoriesModule,
    RoutingsModule,
    HttpClientModule,
    StockPickingsModule,
    StockPickingTypesModule,
    LaboOrderLinesModule,
    StockMovesModule,
    AccountCommonPartnerReportsModule,
    StockReportsModule,
    AccountInvoiceReportsModule,
    RolesModule,
    SchedulerModule,
    CustomComponentModule,
    CompaniesModule,
    DotKhamLinesModule,
    PartnerCategoriesModule,
    ChartsModule,
    UploadModule,
    LayoutModule,
    InputsModule,
    MatStepperModule,
    NgbModule,
    HistoryModule,
    ResGroupsModule,
    IrModelsModule,
    IrRulesModule,
    PriceListModule,
    LaboOrdersModule,
    PurchaseOrdersModule,
    RealRevenueReportModule,
    SaleReportModule,
    MomentModule.forRoot({
      relativeTimeThresholdOptions: {
        m: 59,
      },
    }),
    MailMessagesModule,
    PartnerReportModule,
    PartnerSourcesModule,
    ReportPartnerSourcesModule,
    ResBanksModule,
    ResPartnerBanksModule,
    JournalReportsModule,
    CardTypesModule,
    CardCardsModule,
    SaleSettingsModule,
    SaleCouponPromotionModule,
    PromotionProgramsModule,
    ResConfigSettingsModule,
    ZaloOaConfigModule,
    RevenueReportModule,
    MarketingCampaignsModule,
    FacebookConfigModule,
    FacebookModule.forRoot(),
    SocialsChannelModule,
    ServiceCardTypesModule,
    ServiceCardOrdersModule,
    ServiceCardCardsModule,
    PickerModule,
    EmojiModule,
    // thắng import
    UomCategoryModule,
    UomModule,
    TcareModule,
    // end
    SamplePrescriptionsModule, 
    LoaiThuChiModule, 
    PhieuThuChiModule,
    AccountFinancialReportModule,
    AccountReportGeneralLedgersModule,
  ],
  providers: [
    JwtInterceptor, // Providing JwtInterceptor allow to inject JwtInterceptor manually into RefreshTokenInterceptor
    {
      provide: HTTP_INTERCEPTORS,
      useExisting: JwtInterceptor,
      multi: true,
    },
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpHandleErrorInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: RefreshTokenInterceptor,
      multi: true,
    },
    { provide: LOCALE_ID, useValue: "vi-VN" },
    { provide: NgbDateParserFormatter, useClass: NgbDateCustomParserFormatter },
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
