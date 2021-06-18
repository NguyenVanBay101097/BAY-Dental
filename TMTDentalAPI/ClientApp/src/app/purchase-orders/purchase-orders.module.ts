import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PurchaseOrdersRoutingModule } from './purchase-orders-routing.module';
import { PurchaseOrderCreateUpdateComponent } from './purchase-order-create-update/purchase-order-create-update.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { PurchaseOrderService } from './purchase-order.service';
import { PurchaseOrderLineService } from './purchase-order-line.service';
import { PurchaseOrderListComponent } from './purchase-order-list/purchase-order-list.component';
import { PurchaseOrderListProductTabpanelComponent } from './purchase-order-list-product-tabpanel/purchase-order-list-product-tabpanel.component';
import { NgbModule, NgbNavModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [PurchaseOrderCreateUpdateComponent, PurchaseOrderListComponent, PurchaseOrderListProductTabpanelComponent],
  imports: [
    CommonModule,
    PurchaseOrdersRoutingModule,
    FormsModule,
    NgbModule,
    NgbNavModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    SharedModule,
  ],
  providers: [
    PurchaseOrderService,
    PurchaseOrderLineService
  ]
})
export class PurchaseOrdersModule { }
