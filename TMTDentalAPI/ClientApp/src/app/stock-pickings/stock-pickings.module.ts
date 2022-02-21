import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { StockPickingsRoutingModule } from './stock-pickings-routing.module';
import { StockPickingListComponent } from './stock-picking-list/stock-picking-list.component';
import { StockPickingService } from './stock-picking.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { StockPickingCreateUpdateComponent } from './stock-picking-create-update/stock-picking-create-update.component';
import { StockPickingMlDialogComponent } from './stock-picking-ml-dialog/stock-picking-ml-dialog.component';
import { StockPickingOutgoingListComponent } from './stock-picking-outgoing-list/stock-picking-outgoing-list.component';
import { StockPickingOutgoingCreateUpdateComponent } from './stock-picking-outgoing-create-update/stock-picking-outgoing-create-update.component';
import { SharedModule } from '../shared/shared.module';
import { StockPickingIncomingListComponent } from './stock-picking-incoming-list/stock-picking-incoming-list.component';
import { StockPickingIncomingCreateUpdateComponent } from './stock-picking-incoming-create-update/stock-picking-incoming-create-update.component';
import { StockPickingManagementComponent } from './stock-picking-management/stock-picking-management.component';
import { StockReportService } from '../stock-reports/stock-report.service';
import { StockXuatNhapTonComponent } from './stock-xuat-nhap-ton/stock-xuat-nhap-ton.component';
import { StockPickingIncomingDetailComponent } from './stock-picking-incoming-detail/stock-picking-incoming-detail.component';
import { StockInventoriesModule } from '../stock-inventories/stock-inventories.module';
import { StockPickingRequestProductComponent } from './stock-picking-request-product/stock-picking-request-product.component';
import { StockPickingRequestProductDialogComponent } from './stock-picking-request-product-dialog/stock-picking-request-product-dialog.component';
import { StockXuatNhapTonDetailDialogComponent } from './stock-xuat-nhap-ton-detail-dialog/stock-xuat-nhap-ton-detail-dialog.component';
import { PurchaseOrdersModule } from '../purchase-orders/purchase-orders.module';
import { RoleService } from '../roles/role.service';

@NgModule({
  declarations: [
    StockPickingListComponent,
    StockPickingCreateUpdateComponent,
    StockPickingMlDialogComponent,
    StockPickingOutgoingListComponent,
    StockPickingOutgoingCreateUpdateComponent,
    StockPickingIncomingListComponent,
    StockPickingIncomingCreateUpdateComponent,
    StockPickingManagementComponent,
    StockXuatNhapTonComponent,
    StockPickingIncomingDetailComponent,
    StockPickingRequestProductComponent,
    StockXuatNhapTonDetailDialogComponent,
    StockPickingRequestProductDialogComponent
  ],
  imports: [
    CommonModule,
    StockPickingsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    StockInventoriesModule,
    FormsModule,
    SharedModule,
    PurchaseOrdersModule,
    NgbModule
  ],
  providers: [
    StockPickingService, StockReportService, RoleService
  ],
  entryComponents: [
    StockPickingMlDialogComponent,
    StockPickingRequestProductDialogComponent,
    StockXuatNhapTonDetailDialogComponent
  ]
})
export class StockPickingsModule { }
