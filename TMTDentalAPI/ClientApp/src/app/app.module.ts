import { BrowserModule } from '@angular/platform-browser';
import { NgModule, LOCALE_ID } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeroesModule } from './heroes/heroes.module';
import { ProductCategoriesModule } from './product-categories/product-categories.module';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatNativeDateModule } from '@angular/material/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthModule } from './auth/auth.module';
import { ProductsModule } from './products/products.module';
import { AuthInterceptor } from './auth/auth-interceptor';

import { IntlModule } from '@progress/kendo-angular-intl';
// Load all required data for the bg locale
import '@progress/kendo-angular-intl/locales/vi/all';
import { SaleOrdersModule } from './sale-orders/sale-orders.module';
import { UsersModule } from './users/users.module';
import { PartnersModule } from './partners/partners.module';
import { AccountInvoicesModule } from './account-invoices/account-invoices.module';
import { AccountPaymentsModule } from './account-payments/account-payments.module';
import { HttpHandleErrorInterceptor } from './http-handle-error-interceptor';
import { AccountJournalsModule } from './account-journals/account-journals.module';
import { ToothCategoriesModule } from './tooth-categories/tooth-categories.module';
import { TeethModule } from './teeth/teeth.module';

import { registerLocaleData } from '@angular/common';
import localeVi from '@angular/common/locales/vi';
import { DotKhamsModule } from './dot-khams/dot-khams.module';
import { ToaThuocsModule } from './toa-thuocs/toa-thuocs.module';
import { AppointmentModule } from './appointment/appointment.module';

import { JwtModule } from "@auth0/angular-jwt";
import { HomeModule } from './home/home.module';
import { RoutingsModule } from './routings/routings.module';
import { StockPickingsModule } from './stock-pickings/stock-pickings.module';
import { StockPickingTypesModule } from './stock-picking-types/stock-picking-types.module';
import { LaboOrderLinesModule } from './labo-order-lines/labo-order-lines.module';
import { StockMovesModule } from './stock-moves/stock-moves.module';
import { PrintLayoutComponent } from './print-layout/print-layout.component';
import { AccountCommonPartnerReportsModule } from './account-common-partner-reports/account-common-partner-reports.module';
import { StockReportsModule } from './stock-reports/stock-reports.module';
import { AccountInvoiceReportsModule } from './account-invoice-reports/account-invoice-reports.module';
import { RolesModule } from './roles/roles.module';
import { SchedulerModule } from '@progress/kendo-angular-scheduler';
import { CustomComponentModule } from './common/common.module';
import { CompaniesModule } from './companies/companies.module';
import { DotKhamLinesModule } from './dot-kham-lines/dot-kham-lines.module';
import { PartnerCategoriesModule } from './partner-categories/partner-categories.module';
import { ChartsModule } from '@progress/kendo-angular-charts';
import 'hammerjs';
import { UploadModule } from '@progress/kendo-angular-upload';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { EmployeesModule } from './employees/employees.module';
import { EmployeeCategoriesModule } from './employee-categories/employee-categories.module';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { MatStepperModule } from '@angular/material/stepper';
import { HistoryModule } from './history/history.module';
import { NgbModule, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateCustomParserFormatter } from './core/ngb-date-custom-parser-formatter';
import { PriceListModule } from './price-list/price-list.module';


export function tokenGetter() {
  return localStorage.getItem("access_token");
}

registerLocaleData(localeVi, 'vi');

@NgModule({
  declarations: [
    AppComponent,
    PrintLayoutComponent,
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
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ["http://localhost:50396"],
        blacklistedRoutes: []
      }
    }),
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
    PriceListModule

  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: HttpHandleErrorInterceptor, multi: true },
    { provide: LOCALE_ID, useValue: 'vi-VN' },
    { provide: NgbDateParserFormatter, useClass: NgbDateCustomParserFormatter }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
