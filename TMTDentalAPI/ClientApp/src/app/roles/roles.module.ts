import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RolesRoutingModule } from './roles-routing.module';
import { RoleListComponent } from './role-list/role-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RoleService } from './role.service';
import { MyCustomNgbModule } from '../shared/my-custom-ngb.module';
import { SharedModule } from '../shared/shared.module';
import { RoleFormComponent } from './role-form/role-form.component';

@NgModule({
  declarations: [RoleListComponent, RoleFormComponent],
  imports: [
    CommonModule,
    RolesRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomNgbModule,
    SharedModule
  ],
  providers: [
    RoleService
  ]
})
export class RolesModule { }
