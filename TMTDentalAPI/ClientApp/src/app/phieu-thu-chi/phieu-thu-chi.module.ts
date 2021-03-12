import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PhieuThuChiRoutingModule } from './phieu-thu-chi-routing.module';
import { PhieuThuChiListComponent } from './phieu-thu-chi-list/phieu-thu-chi-list.component';
import { PhieuThuChiFormComponent } from './phieu-thu-chi-form/phieu-thu-chi-form.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [PhieuThuChiListComponent, PhieuThuChiFormComponent],
  imports: [
    CommonModule,
    PhieuThuChiRoutingModule, 
    MyCustomKendoModule, 
    FormsModule, 
    ReactiveFormsModule,
    NgbModule
  ], 
  providers: [
  ],
  entryComponents: [
    PhieuThuChiFormComponent,
  ],
})
export class PhieuThuChiModule { }
