import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LoaiThuChiRoutingModule } from './loai-thu-chi-routing.module';
import { LoaiThuChiListComponent } from './loai-thu-chi-list/loai-thu-chi-list.component';
import { LoaiThuChiFormComponent } from './loai-thu-chi-form/loai-thu-chi-form.component';
import { LoaiThuChiService } from './loai-thu-chi.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [LoaiThuChiListComponent, LoaiThuChiFormComponent],
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
    LoaiThuChiFormComponent,
  ],
})
export class LoaiThuChiModule { }
