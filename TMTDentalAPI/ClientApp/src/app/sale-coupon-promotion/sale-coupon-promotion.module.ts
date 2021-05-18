import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaleCouponPromotionRoutingModule } from './sale-coupon-promotion-routing.module';
import { SaleCouponProgramListComponent } from './sale-coupon-program-list/sale-coupon-program-list.component';
import { SaleCouponProgramService } from './sale-coupon-program.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SaleCouponProgramCreateUpdateComponent } from './sale-coupon-program-create-update/sale-coupon-program-create-update.component';
import { SaleCouponProgramGenerateCouponsDialogComponent } from './sale-coupon-program-generate-coupons-dialog/sale-coupon-program-generate-coupons-dialog.component';
import { SaleCouponListComponent } from './sale-coupon-list/sale-coupon-list.component';
import { SaleCouponService } from './sale-coupon.service';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SalePromotionProgramListComponent } from './sale-promotion-program-list/sale-promotion-program-list.component';
import { SalePromotionProgramCreateUpdateComponent } from './sale-promotion-program-create-update/sale-promotion-program-create-update.component';
import { SaleCouponProgramFilterActiveComponent } from './sale-coupon-program-filter-active/sale-coupon-program-filter-active.component';
import { SaleCouponListDialogComponent } from './sale-coupon-list-dialog/sale-coupon-list-dialog.component';
import { DiscountPricePopoverComponent } from './sale-promotion-program-create-update/discount-price-popover/discount-price-popover.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [SaleCouponProgramListComponent, SaleCouponProgramCreateUpdateComponent, SaleCouponProgramGenerateCouponsDialogComponent, SaleCouponListComponent, SalePromotionProgramListComponent, SalePromotionProgramCreateUpdateComponent, SaleCouponProgramFilterActiveComponent, SaleCouponListDialogComponent, DiscountPricePopoverComponent],
  imports: [
    CommonModule,
    SaleCouponPromotionRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomKendoModule,
    NgbModule,
    SharedModule
  ],
  providers: [
    SaleCouponProgramService,
    SaleCouponService
  ],
  entryComponents: [
    SaleCouponProgramGenerateCouponsDialogComponent,
    SaleCouponListDialogComponent,
    DiscountPricePopoverComponent
  ]
})
export class SaleCouponPromotionModule { }
