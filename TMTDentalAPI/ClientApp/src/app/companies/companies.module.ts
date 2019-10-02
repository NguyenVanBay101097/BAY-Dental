import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CompaniesRoutingModule } from './companies-routing.module';
import { CompanyListComponent } from './company-list/company-list.component';
import { CompanyService } from './company.service';
import { CompanyCuDialogComponent } from './company-cu-dialog/company-cu-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule } from '@angular/forms';
import { CompanyCreateUpdateComponent } from './company-create-update/company-create-update.component';

@NgModule({
  declarations: [CompanyListComponent, CompanyCuDialogComponent, CompanyCreateUpdateComponent],
  imports: [
    CommonModule,
    CompaniesRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule
  ],
  providers: [
    CompanyService
  ],
  entryComponents: [
    CompanyCuDialogComponent
  ]
})
export class CompaniesModule { }
