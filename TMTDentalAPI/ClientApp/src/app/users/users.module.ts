import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersRoutingModule } from './users-routing.module';
import { UserService } from './user.service';
import { UserListComponent } from './user-list/user-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule } from '@angular/forms';
import { UserCuDialogComponent } from './user-cu-dialog/user-cu-dialog.component';

@NgModule({
  declarations: [UserListComponent, UserCuDialogComponent],
  imports: [
    CommonModule,
    UsersRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule
  ],
  providers: [
    UserService
  ],
  entryComponents: [
    UserCuDialogComponent
  ]
})
export class UsersModule { }
