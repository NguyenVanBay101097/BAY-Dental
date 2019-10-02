import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RolesRoutingModule } from './roles-routing.module';
import { RoleListComponent } from './role-list/role-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RoleService } from './role.service';
import { RoleCreateUpdateComponent } from './role-create-update/role-create-update.component';

@NgModule({
  declarations: [RoleListComponent, RoleCreateUpdateComponent],
  imports: [
    CommonModule,
    RolesRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [
    RoleService
  ]
})
export class RolesModule { }
