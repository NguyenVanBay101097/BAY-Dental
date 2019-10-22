import { BrowserModule } from '@angular/platform-browser';
import { NgModule, LOCALE_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
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
    FetchDataComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
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
    SharedModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: HttpHandleErrorInterceptor, multi: true },
    { provide: LOCALE_ID, useValue: 'vi-VN' },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
