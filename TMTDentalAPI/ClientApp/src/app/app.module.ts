import { NgModule, LOCALE_ID, ErrorHandler } from "@angular/core";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { SharedModule } from "./shared/shared.module";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
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
import { FlexLayoutModule } from '@angular/flex-layout';
import { MyErrorHandler } from './my-error-handler';
import { FacebookModule } from 'ngx-facebook';
import { NoCacheInterceptor } from './http-interceptors/no-cache.interceptor';


export function tokenGetter() {
  return localStorage.getItem("access_token");
}


registerLocaleData(localeVi, "vi");

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
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
    FlexLayoutModule,
    SharedModule,
    RoutingsModule,
    MomentModule.forRoot({
      relativeTimeThresholdOptions: {
        m: 59,
      },
    }),
    FacebookModule.forRoot()
  ],
  providers: [
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
    { provide: ErrorHandler, useClass: MyErrorHandler }
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
