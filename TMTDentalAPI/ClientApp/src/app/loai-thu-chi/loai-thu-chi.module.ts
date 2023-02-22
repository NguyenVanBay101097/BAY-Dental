import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { LoaiThuChiListComponent } from './loai-thu-chi-list/loai-thu-chi-list.component';
import { LoaiThuChiManagementComponent } from './loai-thu-chi-management/loai-thu-chi-management.component';
import { LoaiThuChiRoutingModule } from './loai-thu-chi-routing.module';


@NgModule({
  declarations: [LoaiThuChiListComponent, LoaiThuChiManagementComponent],
  imports: [
    CommonModule,
    LoaiThuChiRoutingModule, 
    MyCustomKendoModule, 
    FormsModule, 
    ReactiveFormsModule,
    NgbModule
  ], 
  providers: [],
  entryComponents: [
  ],
})
export class LoaiThuChiModule { }
