import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { QuotationsRoutingModule } from './quotations-routing.module';
import { QuotationCreateUpdateFormComponent } from './quotation-create-update-form/quotation-create-update-form.component';
import { SaleOrdersModule } from '../sale-orders/sale-orders.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { QuotationService } from './quotation.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { QuotationLineCuComponent } from './quotation-line-cu/quotation-line-cu.component';
import { QuotationLinePromotionDialogComponent } from './quotation-line-promotion-dialog/quotation-line-promotion-dialog.component';
import { PromotionDiscountComponent } from './promotion-discount/promotion-discount.component';
import { QuotationLineApplyCouponComponent } from './quotation-line-apply-coupon/quotation-line-apply-coupon.component';
import { QuotationPromotionDialogComponent } from './quotation-promotion-dialog/quotation-promotion-dialog.component';
import { QuotationApplyCouponComponent } from './quotation-apply-coupon/quotation-apply-coupon.component';

@NgModule({
  declarations: [QuotationCreateUpdateFormComponent, QuotationLineCuComponent, QuotationLinePromotionDialogComponent, PromotionDiscountComponent, QuotationLineApplyCouponComponent, QuotationPromotionDialogComponent, QuotationApplyCouponComponent],
  imports: [
    CommonModule,
    QuotationsRoutingModule,
    SaleOrdersModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    SharedModule,
    NgbModule,
  ],
  providers: [QuotationService],
  entryComponents:[QuotationLinePromotionDialogComponent,QuotationPromotionDialogComponent]
})
export class QuotationsModule { }
