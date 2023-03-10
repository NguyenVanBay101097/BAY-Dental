import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ServiceCardTypesRoutingModule } from './service-card-types-routing.module';
import { ServiceCardTypeListComponent } from './service-card-type-list/service-card-type-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ServiceCardTypeCuDialogComponent } from './service-card-type-cu-dialog/service-card-type-cu-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MemberCardListComponent } from './service-card-type-list/member-card-list/member-card-list.component';
import { PreferentialCardListComponent } from './service-card-type-list/preferential-card-list/preferential-card-list.component';
import { PreferentialCardCreateUpdateComponent } from './service-card-type-list/preferential-card-create-update/preferential-card-create-update.component';
import { MemberCardCreateUpdateComponent } from './service-card-type-list/member-card-create-update/member-card-create-update.component';
import { CardCardsModule } from '../card-cards/card-cards.module';
import { ComputePriceInputPopoverComponent } from '../shared/compute-price-input-popover/compute-price-input-popover.component';
import { ServiceCardTypeApplyAllDialogComponent } from './service-card-type-list/card-type-apply-all-dialog/service-card-type-apply-all-dialog.component';
import { ServiceCardTypeApplyCateDialogComponent } from './service-card-type-list/card-type-apply-cate-dialog/service-card-type-apply-cate-dialog.component';
import { MemberCardTypeApplyAllComponent } from './service-card-type-list/service-card-type-apply-all/member-card-type-apply-all.component';
import { MemberCardTypeApplyDialogComponent } from './service-card-type-list/service-card-type-apply-dialog/member-card-type-apply-dialog.component';


@NgModule({
  declarations: [ServiceCardTypeListComponent, ServiceCardTypeCuDialogComponent, MemberCardListComponent, PreferentialCardListComponent, PreferentialCardCreateUpdateComponent, 
    MemberCardTypeApplyDialogComponent, MemberCardCreateUpdateComponent, MemberCardTypeApplyAllComponent, 
    ServiceCardTypeApplyAllDialogComponent, ServiceCardTypeApplyCateDialogComponent],
  imports: [
    ServiceCardTypesRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,      
    FormsModule,
    CommonModule,
    SharedModule,
    MyCustomKendoModule,
    NgbModule,
    CardCardsModule
  
  ],
  entryComponents: [
    ComputePriceInputPopoverComponent
  ]
})
export class ServiceCardTypesModule { }
