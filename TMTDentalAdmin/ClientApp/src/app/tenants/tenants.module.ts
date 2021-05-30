import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TenantsRoutingModule } from './tenants-routing.module';
import { TenantRegisterComponent } from './tenant-register/tenant-register.component';
import { TenantService } from './tenant.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TenantListComponent } from './tenant-list/tenant-list.component';
import { MyCustomKendoModule } from '../my-custom-kendo.module';
import { TenantUpdateExpiredDialogComponent } from './tenant-update-expired-dialog/tenant-update-expired-dialog.component';
import { TrialRegistrationComponent } from './trial-registration/trial-registration.component';
import { TenantUpdateInfoDialogComponent } from './tenant-update-info-dialog/tenant-update-info-dialog.component';
import { TenantExtendHistoryComponent } from './tenant-extend-history/tenant-extend-history.component';
import { EmployeeAdminService } from 'app/employee-admins/employee-admin.service';
import { TenantUpdateExpiredV2DialogComponent } from './tenant-update-expired-v2-dialog/tenant-update-expired-v2-dialog.component';
import { SharedModule } from '@shared/shared.module';
import { TenantsProcessUpdateComponent } from './tenants-process-update/tenants-process-update.component';

@NgModule({
  imports: [
    CommonModule,
    TenantsRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule
  ],
  declarations: [
    TenantRegisterComponent,
    TenantListComponent,
    TenantUpdateExpiredDialogComponent,
    TrialRegistrationComponent,
    TenantUpdateInfoDialogComponent,
    TenantExtendHistoryComponent,
    TenantUpdateExpiredV2DialogComponent,
    TenantsProcessUpdateComponent
  ],
  providers: [
    TenantService,
    EmployeeAdminService
  ],
  entryComponents: [
    TenantUpdateExpiredDialogComponent,
    TenantUpdateInfoDialogComponent,
    TenantUpdateExpiredV2DialogComponent
  ]
})
export class TenantsModule { }
