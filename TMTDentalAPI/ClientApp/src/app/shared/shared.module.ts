import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
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
import { ImageFileUploadComponent } from './image-file-upload/image-file-upload.component';
import { ToaThuocPrintComponent } from './toa-thuoc-print/toa-thuoc-print.component';
import { ImportSampleDataComponent } from './import-sample-data/import-sample-data.component';
import { PartnerProfilePrintComponent } from './partner-profile-print/partner-profile-print.component';
import { AccountPaymentPrintComponent } from './account-payment-print/account-payment-print.component';
import { CheckAddressButtonComponent } from './check-address-button/check-address-button.component';
import { AppointmentCreateUpdateComponent } from './appointment-create-update/appointment-create-update.component';
import { SaleOrderLineDialogComponent } from './sale-order-line-dialog/sale-order-line-dialog.component';
import { ToaThuocCuDialogSaveComponent } from './toa-thuoc-cu-dialog-save/toa-thuoc-cu-dialog-save.component';
import { ToaThuocSamplePrescriptionComponent } from './toa-thuoc-sample-prescription/toa-thuoc-sample-prescription.component';
import { DotKhamCreateUpdateDialogComponent } from './dot-kham-create-update-dialog/dot-kham-create-update-dialog.component';
import { AccountInvoiceRegisterPaymentDialogV2Component } from './account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
import { MyCustomNgbModule } from './my-custom-ngb.module';
import { BinaryFileInputComponent } from './binary-file-input/binary-file-input.component';
import { PartnerSupplierCuDialogComponent } from './partner-supplier-cu-dialog/partner-supplier-cu-dialog.component';
import { ProductCategoryDialogComponent } from './product-category-dialog/product-category-dialog.component';
import { PartnerCustomerCuDialogComponent } from './partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { LoaiThuChiFormComponent } from './loai-thu-chi-form/loai-thu-chi-form.component';
import { PartnerTitleCuDialogComponent } from './partner-title-cu-dialog/partner-title-cu-dialog.component';
import { PartnersBindingDirective } from './directives/partners-binding.directive';
import { PrintSaleOrderComponent } from './print-sale-order/print-sale-order.component';

@NgModule({
  declarations: [
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
    RedirectComponentDirective,
    SharedErrorDialogComponent,
    ConfirmDialogV2Component,
    AnchorHostDirective,
    MyAutosizeDirective,
    CharCountDirective,
    SelectUomProductDialogComponent,
    ImageFileUploadComponent,
    ToaThuocPrintComponent,
    ImportSampleDataComponent, 
    AccountPaymentPrintComponent,
    CheckAddressButtonComponent,
    PartnerProfilePrintComponent,
    AppointmentCreateUpdateComponent,
    SaleOrderLineDialogComponent,
    ToaThuocCuDialogSaveComponent,
    ToaThuocSamplePrescriptionComponent,
    DotKhamCreateUpdateDialogComponent,
    AccountInvoiceRegisterPaymentDialogV2Component,
    BinaryFileInputComponent,
    PartnerSupplierCuDialogComponent,
    ProductCategoryDialogComponent,
    PartnerCustomerCuDialogComponent,
    LoaiThuChiFormComponent,
    PartnerTitleCuDialogComponent,
    PartnersBindingDirective,
    PrintSaleOrderComponent
  ],
  exports: [
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
    AnchorHostDirective,
    MyAutosizeDirective,
    CharCountDirective,
    SelectUomProductDialogComponent,
    RedirectComponentDirective,
    ImageFileUploadComponent,
    ToaThuocPrintComponent,
    AccountPaymentPrintComponent,
    CheckAddressButtonComponent,
    PartnerProfilePrintComponent,
    AppointmentCreateUpdateComponent,
    SaleOrderLineDialogComponent,
    ToaThuocCuDialogSaveComponent,
    DotKhamCreateUpdateDialogComponent,
    AccountInvoiceRegisterPaymentDialogV2Component,
    BinaryFileInputComponent,
    PartnerSupplierCuDialogComponent,
    ProductCategoryDialogComponent,
    PartnerCustomerCuDialogComponent,
    LoaiThuChiFormComponent,
    PartnerTitleCuDialogComponent,
    PartnersBindingDirective,
    PrintSaleOrderComponent
  ],
  imports: [
    CommonModule,
    FlexLayoutModule,
    RouterModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    MyCustomNgbModule,
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
    SelectUomProductDialogComponent,
    ImportSampleDataComponent,
    AppointmentCreateUpdateComponent,
    SaleOrderLineDialogComponent,
    ToaThuocCuDialogSaveComponent,
    DotKhamCreateUpdateDialogComponent,
    AccountInvoiceRegisterPaymentDialogV2Component,
    PartnerSupplierCuDialogComponent,
    ProductCategoryDialogComponent,
    PartnerCustomerCuDialogComponent,
    LoaiThuChiFormComponent,
    PartnerTitleCuDialogComponent
  ],
})
export class SharedModule { }
