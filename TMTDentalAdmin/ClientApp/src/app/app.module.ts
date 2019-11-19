import { BrowserModule } from '@angular/platform-browser';
import { NgModule, LOCALE_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { TenantsModule } from './tenants/tenants.module';
import { MyCustomKendoModule } from './my-custom-kendo.module';
import { AuthModule } from './auth/auth.module';
import { JwtModule } from '@auth0/angular-jwt';
import { AuthInterceptor } from './auth/auth-interceptor';
import { SharedModule } from './shared/shared.module';
import { HttpHandleErrorInterceptor } from './http-handle-error-interceptor';
import { registerLocaleData } from '@angular/common';
import localeVi from '@angular/common/locales/vi';

import { IntlModule } from '@progress/kendo-angular-intl';
// Load all required data for the bg locale
import '@progress/kendo-angular-intl/locales/vi/all';
import { TrialRegistrationComponent } from './trial-registration/trial-registration.component';
import { TrialRegistrationSuccessComponent } from './trial-registration-success/trial-registration-success.component';

export function tokenGetter() {
  return localStorage.getItem("access_token");
}

registerLocaleData(localeVi, 'vi');

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    TrialRegistrationComponent,
    TrialRegistrationSuccessComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'register', component: TrialRegistrationComponent },
      { path: 'register-success', component: TrialRegistrationSuccessComponent },
    ]),
    TenantsModule,
    MyCustomKendoModule,
    AuthModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ["http://localhost:50396"],
        blacklistedRoutes: []
      }
    }),
    SharedModule,
    IntlModule,
    ReactiveFormsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: HttpHandleErrorInterceptor, multi: true },
    { provide: LOCALE_ID, useValue: 'vi-VN' },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
