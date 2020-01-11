import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleCouponProgramListComponent } from './sale-coupon-program-list/sale-coupon-program-list.component';
import { SaleCouponProgramCreateUpdateComponent } from './sale-coupon-program-create-update/sale-coupon-program-create-update.component';
import { SaleCouponListComponent } from './sale-coupon-list/sale-coupon-list.component';
import { SalePromotionProgramListComponent } from './sale-promotion-program-list/sale-promotion-program-list.component';
import { SalePromotionProgramCreateUpdateComponent } from './sale-promotion-program-create-update/sale-promotion-program-create-update.component';

const routes: Routes = [
  {
    path: 'coupon-programs',
    component: SaleCouponProgramListComponent
  },
  {
    path: 'coupon-programs/form',
    component: SaleCouponProgramCreateUpdateComponent
  },
  {
    path: 'coupons',
    component: SaleCouponListComponent
  },
  {
    path: 'promotion-programs',
    component: SalePromotionProgramListComponent
  },
  {
    path: 'promotion-programs/form',
    component: SalePromotionProgramCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleCouponPromotionRoutingModule { }
