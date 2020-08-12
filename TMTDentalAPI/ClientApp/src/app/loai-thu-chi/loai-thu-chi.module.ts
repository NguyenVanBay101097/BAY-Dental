import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LoaiThuChiRoutingModule } from './loai-thu-chi-routing.module';
import { LoaiThuChiListComponent } from './loai-thu-chi-list/loai-thu-chi-list.component';
import { LoaiThuChiService } from './loai-thu-chi.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [LoaiThuChiListComponent],
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
