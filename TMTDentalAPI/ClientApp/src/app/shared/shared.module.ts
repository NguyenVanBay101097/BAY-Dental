import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MyOwnCustomMaterialModule } from './my-own-custom-material.module';
import { HeaderComponent } from './header/header.component';
import { HeaderMenuComponent } from './header-menu/header-menu.component';
import { HeaderBarComponent } from './header-bar/header-bar.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { TopMenuComponent } from './top-menu/top-menu.component';
import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';
import { RouterModule } from '@angular/router';
import { ChangePasswordDialogComponent } from './change-password-dialog/change-password-dialog.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { LayoutSidebarComponent } from './layout-sidebar/layout-sidebar.component';
import { LayoutHeaderComponent } from './layout-header/layout-header.component';
import { NavSidebarService } from './nav-sidebar.service';
import { FieldBinaryImageSimpleComponent } from './field-binary-image-simple/field-binary-image-simple.component';
import { DateRangeAdvanceSearchComponent } from './date-range-advance-search/date-range-advance-search.component';
import { MyCustomKendoModule } from './my-customer-kendo.module';
import { ImageViewerComponent } from './image-viewer/image-viewer.component';
import { DisableControlDirective } from './disable-control-directive';
import { AppLoadingService } from './app-loading.service';
import { LoadingComponent } from './loading/loading.component';
import { ClickOutsideDirective } from './click-outside-directive';
import { TaiSearchInputComponent } from './tai-search-input/tai-search-input.component';
import { TaiProductListSelectableComponent } from './tai-product-list-selectable/tai-product-list-selectable.component';
import { TaiDateRangeFilterDropdownComponent } from './tai-date-range-filter-dropdown/tai-date-range-filter-dropdown.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TmtOptionSelectDropdownComponent } from './tmt-option-select-dropdown/tmt-option-select-dropdown.component';
import { TaiDateRangeSimpleDropdownComponent } from './tai-date-range-simple-dropdown/tai-date-range-simple-dropdown.component';
import { HeaderNotificationComponent } from './header-notification/header-notification.component';
import { MomentModule } from 'ngx-moment';
import { HeaderAppointmentComponent } from './header-appointment/header-appointment.component';
import { SharedCardCardGridComponent } from './shared-card-card-grid/shared-card-card-grid.component';
import { HasGroupsDirective } from './has-groups-directive';
import { UserProfileEditComponent } from './user-profile-edit/user-profile-edit.component';
import { SharedSaleOrderGridComponent } from './shared-sale-order-grid/shared-sale-order-grid.component';
import { SharedPartnerGridComponent } from './shared-partner-grid/shared-partner-grid.component';
import { SharedErrorDialogComponent } from './shared-error-dialog/shared-error-dialog.component';
import { AppSharedShowErrorService } from './shared-show-error.service';
import { ConfirmDialogV2Component } from './confirm-dialog-v2/confirm-dialog-v2.component';
import { AnchorHostDirective } from './anchor-host.directive';
import { MyAutosizeDirective } from './autosize.directive';
import { CharCountDirective } from './char-count.directive';
import { SelectUomProductDialogComponent } from './select-uom-product-dialog/select-uom-product-dialog.component';
import { RedirectComponentDirective } from './redirect-component.directive';
import { RedirectComponentComponent } from './redirect-component/redirect-component.component';
import { ImageFileUploadComponent } from './image-file-upload/image-file-upload.component';

@NgModule({
  declarations: [
    HeaderComponent,
    HeaderMenuComponent,
    HeaderBarComponent,
    TopMenuComponent,
    ConfirmDialogComponent,
    ChangePasswordDialogComponent,
    LayoutSidebarComponent,
    LayoutHeaderComponent,
    FieldBinaryImageSimpleComponent,
    ImageViewerComponent,
    DateRangeAdvanceSearchComponent,
    DisableControlDirective,
    LoadingComponent,
    ClickOutsideDirective,
    TaiSearchInputComponent,
    TaiProductListSelectableComponent,
    TaiDateRangeFilterDropdownComponent,
    TmtOptionSelectDropdownComponent,
    TaiDateRangeSimpleDropdownComponent,
    HeaderNotificationComponent,
    HeaderAppointmentComponent,
    SharedCardCardGridComponent,
    HasGroupsDirective,
    UserProfileEditComponent,
    SharedSaleOrderGridComponent,
    SharedPartnerGridComponent,
    RedirectComponentComponent,
    RedirectComponentDirective,
    SharedErrorDialogComponent,
    ConfirmDialogV2Component,
    AnchorHostDirective,
    MyAutosizeDirective,
    CharCountDirective,
    SelectUomProductDialogComponent,
    ImageFileUploadComponent
  ],
  exports: [
    HeaderComponent,
    HeaderMenuComponent,
    HeaderBarComponent,
    TopMenuComponent,
    ConfirmDialogComponent,
    LayoutSidebarComponent,
    LayoutHeaderComponent,
    FieldBinaryImageSimpleComponent,
    DateRangeAdvanceSearchComponent,
    DisableControlDirective,
    LoadingComponent,
    ClickOutsideDirective,
    TaiSearchInputComponent,
    TaiProductListSelectableComponent,
    TaiDateRangeFilterDropdownComponent,
    TmtOptionSelectDropdownComponent,
    TaiDateRangeSimpleDropdownComponent,
    HeaderNotificationComponent,
    HeaderAppointmentComponent,
    SharedCardCardGridComponent,
    HasGroupsDirective,
    UserProfileEditComponent,
    SharedSaleOrderGridComponent,
    SharedPartnerGridComponent,
    SharedErrorDialogComponent,
    ConfirmDialogV2Component,
    RedirectComponentComponent,
    AnchorHostDirective,
    MyAutosizeDirective,
    CharCountDirective,
    SelectUomProductDialogComponent,
    RedirectComponentDirective,
    ImageFileUploadComponent
  ],
  imports: [
    CommonModule,
    BrowserAnimationsModule,
    MyOwnCustomMaterialModule,
    FlexLayoutModule,
    RouterModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    NgbModule,
    MomentModule.forRoot({
      relativeTimeThresholdOptions: {
        'm': 59
      }
    })
  ],
  providers: [NavSidebarService, AppLoadingService, AppSharedShowErrorService],
  entryComponents: [
    ConfirmDialogComponent,
    ChangePasswordDialogComponent,
    ImageViewerComponent,
    UserProfileEditComponent,
    SharedErrorDialogComponent,
    ConfirmDialogV2Component,
    SelectUomProductDialogComponent
  ],
})
export class SharedModule { }
