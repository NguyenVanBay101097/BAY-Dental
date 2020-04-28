import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ServiceCardCardsRoutingModule } from './service-card-cards-routing.module';
import { ServiceCardCardListComponent } from './service-card-card-list/service-card-card-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ServiceCardCardHistoriesComponent } from './service-card-card-histories/service-card-card-histories.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [ServiceCardCardListComponent, ServiceCardCardHistoriesComponent],
  imports: [
    CommonModule,
    ServiceCardCardsRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    NgbModule
  ]
})
export class ServiceCardCardsModule { }
