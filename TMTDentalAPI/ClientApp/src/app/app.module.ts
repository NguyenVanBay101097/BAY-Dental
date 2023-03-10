import { NgModule, LOCALE_ID, ErrorHandler } from "@angular/core";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
// import { SharedModule } from "./shared/shared.module";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { AuthInterceptor } from "./auth/auth-interceptor";

// Load all required data for the bg locale
import "@progress/kendo-angular-intl/locales/vi/all";
import { HttpHandleErrorInterceptor } from "./http-handle-error-interceptor";
import { registerLocaleData, CommonModule } from "@angular/common";
import localeVi from "@angular/common/locales/vi";
import { JwtModule, JwtInterceptor } from "@auth0/angular-jwt";
import { RoutingsModule } from "./routings/routings.module";
import "hammerjs";
import { NgbDateParserFormatter } from "@ng-bootstrap/ng-bootstrap";
import { NgbDateCustomParserFormatter } from "./core/ngb-date-custom-parser-formatter";
import { RefreshTokenInterceptor } from './auth/refresh-token-interceptor';
import { MomentModule } from 'ngx-moment';
import 'moment/locale/vi';
import { MyErrorHandler } from './my-error-handler';
import { FacebookModule } from 'ngx-facebook';
import { NoCacheInterceptor } from './http-interceptors/no-cache.interceptor';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { PartnerGeneralSettingsComponent } from './partner-general-settings/partner-general-settings.component';
import { AppHomeComponent } from './app-home/app-home.component';
import { AppSidebarModule } from "./layout/sidebar/app-sidebar.module";
import { AppHeaderModule } from "./layout/header/app-header.module";
import { MyCustomNgbModule } from "./shared/my-custom-ngb.module";
// import { ThemeService } from "ng2-charts";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { BrowserModule } from "@angular/platform-browser";
import { SearchAllComponent } from "./search-all/search-all.component";
import { NgSelectModule } from "@ng-select/ng-select";
import { FormsModule } from "@angular/forms";
import { LoadingComponent } from "./shared/loading/loading.component";
import { MyCustomKendoModule } from "./shared/my-customer-kendo.module";
import { GridModule } from '@progress/kendo-angular-grid';
import { TmtAutonumericModule } from 'tmt-autonumeric';
import { NgxEchartsModule } from 'ngx-echarts';
import * as echarts from 'echarts';
export function tokenGetter() {
  return localStorage.getItem("access_token");
}


registerLocaleData(localeVi, "vi");

@NgModule({
  declarations: [
    AppComponent,
    PartnerGeneralSettingsComponent,
    AppHomeComponent,
    SearchAllComponent,
    LoadingComponent,
  ],
  imports: [
    BrowserModule,
    CommonModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ["http://localhost:50396"],
        blacklistedRoutes: [],
      },
    }),
    AppSidebarModule,
    AppHeaderModule,
    MyCustomNgbModule,
    NgSelectModule,
    MyCustomKendoModule,
    FormsModule,
    RoutingsModule,
    MomentModule.forRoot({
      relativeTimeThresholdOptions: {
        m: 59,
      },
    }),
    TmtAutonumericModule.forRoot({
      emptyInputBehavior: 'null'
    }),
    FacebookModule.forRoot(),
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production }),
    GridModule,
    NgxEchartsModule.forRoot({
      echarts,
    }),
  ],
  providers: [
    // ThemeService,
    JwtInterceptor, // Providing JwtInterceptor allow to inject JwtInterceptor manually into RefreshTokenInterceptor
    {
      provide: HTTP_INTERCEPTORS,
      useExisting: JwtInterceptor,
      multi: true,
    },
    { provide: HTTP_INTERCEPTORS, useClass: NoCacheInterceptor, multi: true },
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
    { provide: ErrorHandler, useClass: MyErrorHandler },
    // ThemeService
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
