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
import { ServiceCardTypeApplyDialogComponent } from './service-card-type-list/service-card-type-apply-dialog/service-card-type-apply-dialog.component';
import { MemberCardCreateUpdateComponent } from './service-card-type-list/member-card-create-update/member-card-create-update.component';
import { CardCardsModule } from '../card-cards/card-cards.module';
import { ServiceCardTypeApplyAllComponent } from './service-card-type-list/service-card-type-apply-all/service-card-type-apply-all.component';


@NgModule({
  declarations: [ServiceCardTypeListComponent, ServiceCardTypeCuDialogComponent, MemberCardListComponent, PreferentialCardListComponent, PreferentialCardCreateUpdateComponent, ServiceCardTypeApplyDialogComponent, MemberCardCreateUpdateComponent, ServiceCardTypeApplyAllComponent],
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
    ServiceCardTypeCuDialogComponent,
    ServiceCardTypeApplyDialogComponent
  ]
})
export class ServiceCardTypesModule { }
