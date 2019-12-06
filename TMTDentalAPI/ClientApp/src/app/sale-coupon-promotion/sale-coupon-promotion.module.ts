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

@NgModule({
  declarations: [SaleCouponProgramListComponent, SaleCouponProgramCreateUpdateComponent, SaleCouponProgramGenerateCouponsDialogComponent, SaleCouponListComponent],
  imports: [
    CommonModule,
    SaleCouponPromotionRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomKendoModule
  ],
  providers: [
    SaleCouponProgramService,
    SaleCouponService
  ],
  entryComponents: [
    SaleCouponProgramGenerateCouponsDialogComponent
  ]
})
export class SaleCouponPromotionModule { }
