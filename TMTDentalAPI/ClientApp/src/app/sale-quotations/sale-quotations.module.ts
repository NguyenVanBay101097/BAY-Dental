import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaleQuotationsRoutingModule } from './sale-quotations-routing.module';
import { SaleQuotationListComponent } from './sale-quotation-list/sale-quotation-list.component';
import { SaleQuotationCreateUpdateComponent } from './sale-quotation-create-update/sale-quotation-create-update.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SaleQuotationCreateUpdateDialogComponent } from './sale-quotation-create-update-dialog/sale-quotation-create-update-dialog.component';

@NgModule({
  declarations: [
    SaleQuotationListComponent,
    SaleQuotationCreateUpdateComponent,
    SaleQuotationCreateUpdateDialogComponent
  ],
  imports: [
    CommonModule,
    SaleQuotationsRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    NgbModule,
  ]
})
export class SaleQuotationsModule { }
