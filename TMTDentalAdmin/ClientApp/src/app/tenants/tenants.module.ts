import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TenantsRoutingModule } from './tenants-routing.module';
import { TenantRegisterComponent } from './tenant-register/tenant-register.component';
import { TenantService } from './tenant.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TenantListComponent } from './tenant-list/tenant-list.component';
import { MyCustomKendoModule } from '../my-custom-kendo.module';
import { TenantUpdateExpiredDialogComponent } from './tenant-update-expired-dialog/tenant-update-expired-dialog.component';

@NgModule({
  imports: [
    CommonModule,
    TenantsRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule
  ],
  declarations: [TenantRegisterComponent, TenantListComponent, TenantUpdateExpiredDialogComponent],
  providers: [
    TenantService
  ],
  entryComponents: [
    TenantUpdateExpiredDialogComponent
  ]
})
export class TenantsModule { }
