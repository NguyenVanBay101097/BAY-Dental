import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ToothCategoriesRoutingModule } from './tooth-categories-routing.module';
import { ToothCategoryService } from './tooth-category.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ToothCategoriesRoutingModule
  ],
  providers: [
    ToothCategoryService
  ]
})
export class ToothCategoriesModule { }
