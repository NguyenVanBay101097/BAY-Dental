import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleCouponProgramListComponent } from './sale-coupon-program-list/sale-coupon-program-list.component';
import { SaleCouponProgramCreateUpdateComponent } from './sale-coupon-program-create-update/sale-coupon-program-create-update.component';
import { SaleCouponListComponent } from './sale-coupon-list/sale-coupon-list.component';

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
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleCouponPromotionRoutingModule { }
