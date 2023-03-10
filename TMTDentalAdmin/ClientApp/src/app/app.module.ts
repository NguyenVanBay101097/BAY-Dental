import { BrowserModule } from '@angular/platform-browser';
import { NgModule, LOCALE_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
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
import { AppRoutingModule } from './app-routing.module';
import { CoreModule } from '@app/core.module';
import { AuthLayoutComponent } from './layout/auth-layout/auth-layout.component';
import { ContentLayoutComponent } from './layout/content-layout/content-layout.component';
import { HttpErrorInterceptor } from '@app/interceptors/http-error.interceptor';
import { SidebarComponent } from './layout/sidebar/sidebar.component';
import { HeaderComponent } from './layout/header/header.component';
import { EmployeeAdminModule } from './employee-admins/employee-admins.module';
import { SidebarNavComponent } from './layout/sidebar/sidebar-nav/sidebar-nav.component';
import { SidebarNavItemComponent } from './layout/sidebar/sidebar-nav-item/sidebar-nav-item.component';
import { AsideToggleDirective, HtmlAttributesDirective, SidebarToggleDirective, SidebarMinimizeDirective, MobileSidebarToggleDirective, SidebarOffCanvasCloseDirective, BrandMinimizeDirective } from './layout/shared/layout.directive';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

export function tokenGetter() {
  return localStorage.getItem("access_token");
}

registerLocaleData(localeVi, 'vi');

@NgModule({
  declarations: [
    AuthLayoutComponent,
    ContentLayoutComponent,
    AppComponent,
    NavMenuComponent,
    SidebarComponent,
    HeaderComponent,
    SidebarNavComponent,
    SidebarNavItemComponent,
    SidebarToggleDirective,
    HtmlAttributesDirective,
    AsideToggleDirective,
    SidebarMinimizeDirective,
    MobileSidebarToggleDirective,
    SidebarOffCanvasCloseDirective,
    BrandMinimizeDirective
  ],
  imports: [
    // BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule,
    TenantsModule,
    MyCustomKendoModule,
    AuthModule,
    CoreModule,
    NgbModule,
    SharedModule,
    EmployeeAdminModule,
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
    { provide: HTTP_INTERCEPTORS, useClass: HttpErrorInterceptor, multi: true },
    { provide: LOCALE_ID, useValue: 'vi-VN' },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
