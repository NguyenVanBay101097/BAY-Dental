import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ServiceCardCardsRoutingModule } from './service-card-cards-routing.module';
import { ServiceCardCardListComponent } from './service-card-card-list/service-card-card-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';

@NgModule({
  declarations: [ServiceCardCardListComponent],
  imports: [
    CommonModule,
    ServiceCardCardsRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule
  ]
})
export class ServiceCardCardsModule { }
