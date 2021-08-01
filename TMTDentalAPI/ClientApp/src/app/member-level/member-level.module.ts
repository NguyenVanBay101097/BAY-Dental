import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MemberLevelRoutingModule } from './member-level-routing.module';
import { MemberLevelManagementComponent } from './member-level-management/member-level-management.component';
import { MemberLevelCreateUpdateComponent } from './member-level-create-update/member-level-create-update.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '@progress/kendo-angular-dropdowns';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MemberLevelListComponent } from './member-level-list/member-level-list.component';

@NgModule({
  declarations: [MemberLevelManagementComponent, MemberLevelCreateUpdateComponent, MemberLevelListComponent],
  imports: [
    CommonModule,
    MemberLevelRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    SharedModule,
    NgbModule
  ]
})
export class MemberLevelModule { }
