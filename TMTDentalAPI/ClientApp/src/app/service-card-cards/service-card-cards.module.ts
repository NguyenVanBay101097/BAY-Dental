import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ServiceCardCardsRoutingModule } from './service-card-cards-routing.module';
import { ServiceCardCardListComponent } from './service-card-card-list/service-card-card-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ServiceCardCardHistoriesComponent } from './service-card-card-histories/service-card-card-histories.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ServiceCardCardsManagementComponent } from './service-card-cards-management/service-card-cards-management.component';
import { ServiceCardCardsPreferentialComponent } from './service-card-cards-preferential/service-card-cards-preferential.component';
import { ServiceCardCardsPreferentialCuDialogComponent } from './service-card-cards-preferential-cu-dialog/service-card-cards-preferential-cu-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { ServiceCardCardsPreferentialImportDialogComponent } from './service-card-cards-preferential-import-dialog/service-card-cards-preferential-import-dialog.component';
import { CardCardsMemberComponent } from './card-cards-member/card-cards-member.component';
import { CardCardsMemberCuDialogComponent } from './card-cards-member-cu-dialog/card-cards-member-cu-dialog.component';
import { ServiceCardTypesModule } from '../service-card-types/service-card-types.module';

@NgModule({
  declarations: [ServiceCardCardListComponent, ServiceCardCardHistoriesComponent, ServiceCardCardsManagementComponent, ServiceCardCardsPreferentialComponent, ServiceCardCardsPreferentialCuDialogComponent, ServiceCardCardsPreferentialImportDialogComponent, CardCardsMemberComponent, CardCardsMemberCuDialogComponent,],
  imports: [
    CommonModule,
    ServiceCardCardsRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    NgbModule,
    ServiceCardTypesModule
  ],
  entryComponents: [
    ServiceCardCardsPreferentialCuDialogComponent,
    ServiceCardCardsPreferentialImportDialogComponent
  ]
})
export class ServiceCardCardsModule { }
