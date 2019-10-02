import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TenantsRoutingModule } from './tenants-routing.module';
import { TenantRegisterComponent } from './tenant-register/tenant-register.component';
import { TenantService } from './tenant.service';
import { ReactiveFormsModule } from '@angular/forms';
import { TenantListComponent } from './tenant-list/tenant-list.component';
import { MyCustomKendoModule } from '../my-custom-kendo.module';

@NgModule({
  imports: [
    CommonModule,
    TenantsRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule
  ],
  declarations: [TenantRegisterComponent, TenantListComponent],
  providers: [
    TenantService
  ]
})
export class TenantsModule { }
