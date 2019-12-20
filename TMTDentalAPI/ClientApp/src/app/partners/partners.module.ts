import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PartnersRoutingModule } from './partners-routing.module';
import { PartnerService } from './partner.service';
import { PartnerListComponent } from './partner-list/partner-list.component';
import { PartnerCreateUpdateComponent } from './partner-create-update/partner-create-update.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { PartnerInfoComponent } from './partner-info/partner-info.component';
import { PartnerCustomerListComponent } from './partner-customer-list/partner-customer-list.component';
import { PartnerCustomerListDetailComponent } from './partner-customer-list-detail/partner-customer-list-detail.component';
import { PartnerCustomerInfoComponent } from './partner-customer-info/partner-customer-info.component';
import { PartnerCustomerInvoicesComponent } from './partner-customer-invoices/partner-customer-invoices.component';
import { PartnerHistoryComponent } from './partner-history/partner-history.component';
import { DotKhamCreateUpdateComponent } from '../dot-khams/dot-kham-create-update/dot-kham-create-update.component';
import { PartnerDetailListComponent } from './partner-detail-list/partner-detail-list.component';
import { PartnerCustomerCuDialogComponent } from './partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnerSupplierCuDialogComponent } from './partner-supplier-cu-dialog/partner-supplier-cu-dialog.component';
import { PartnerSupplierListComponent } from './partner-supplier-list/partner-supplier-list.component';
import { MatStepperModule } from '@angular/material/stepper';
import { PartnerInvoiceLinesComponent } from './partner-invoice-lines/partner-invoice-lines.component';
import { PartnerImportComponent } from './partner-import/partner-import.component';
import { PartnerPaymentsComponent } from './partner-payments/partner-payments.component';
import { PurchaseOrderRefundComponent } from './purchase-order-refund/purchase-order-refund.component';
import { PartnerCardsTabPaneComponent } from './partner-cards-tab-pane/partner-cards-tab-pane.component';
import { CardCardsModule } from '../card-cards/card-cards.module';
import { SharedModule } from '../shared/shared.module';
import { PartnerTabSaleOrderListComponent } from './partner-tab-sale-order-list/partner-tab-sale-order-list.component';
import { PartnerSearchDialogComponent } from './partner-search-dialog/partner-search-dialog.component';

@NgModule({
  declarations: [PartnerListComponent, PartnerCreateUpdateComponent, PartnerInfoComponent,
    PartnerCustomerListComponent, PartnerCustomerListDetailComponent, PartnerCustomerInfoComponent,
    PartnerCustomerInvoicesComponent, PartnerHistoryComponent, PartnerDetailListComponent, PartnerCustomerCuDialogComponent,
    PartnerSupplierCuDialogComponent, PartnerSupplierListComponent, PartnerInvoiceLinesComponent, PartnerImportComponent, PartnerPaymentsComponent, PurchaseOrderRefundComponent, PartnerCardsTabPaneComponent, PartnerTabSaleOrderListComponent, PartnerSearchDialogComponent],
  imports: [
    CommonModule,
    PartnersRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    MatStepperModule,
    SharedModule
  ],
  entryComponents: [
    PartnerCreateUpdateComponent,
    DotKhamCreateUpdateComponent,
    PartnerImportComponent,
    PartnerCustomerCuDialogComponent,
    PartnerSearchDialogComponent
  ],
  providers: [
    PartnerService
  ]
})
export class PartnersModule { }
