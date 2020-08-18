import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersRoutingModule } from './users-routing.module';
import { UserService } from './user.service';
import { UserListComponent } from './user-list/user-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule } from '@angular/forms';
import { UserCuDialogComponent } from './user-cu-dialog/user-cu-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { UserListImportComponent } from './user-list-import/user-list-import.component';

@NgModule({
  declarations: [UserListComponent, UserCuDialogComponent, UserListImportComponent],
  imports: [
    CommonModule,
    UsersRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    SharedModule,
  ],
  providers: [
    UserService
  ],
  entryComponents: [
    UserCuDialogComponent,
    UserListImportComponent
  ]
})
export class UsersModule { }
