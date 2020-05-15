import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UomCategoryRoutingModule } from './uom-category-routing.module';
import { UomCategoryCrUpComponent } from './uom-category-cr-up/uom-category-cr-up.component';
import { UomCategoryListComponent } from './uom-category-list/uom-category-list.component';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { UomCategoryService } from './uom-category.service';

@NgModule({
  declarations: [UomCategoryCrUpComponent, UomCategoryListComponent],
  imports: [
    CommonModule,
    UomCategoryRoutingModule,
    NgbModalModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule
  ],
  entryComponents: [UomCategoryCrUpComponent],
})
export class UomCategoryModule { }
