import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { Product } from '../product';

@Component({
  selector: 'app-product-service-form',
  templateUrl: './product-service-form.component.html',
  styleUrls: ['./product-service-form.component.css']
})
export class ProductServiceFormComponent implements OnInit {
  @Input() formGroup: FormGroup;
  filteredCategories: ProductCategoryBasic[] = [];
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  @Input() product: Product;
  @Output() validate = new EventEmitter<boolean>();
  constructor(private productCategoryService: ProductCategoryService) { }

  ngOnInit() {
    this.categCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.categCbx.loading = true)),
      switchMap(value => this.searchCategories(value))
    ).subscribe(result => {
      this.filteredCategories = result;
      this.categCbx.loading = false;
    });

    this.loadFilteredCategories();
  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q;
    val.limit = 20;
    return this.productCategoryService.autocomplete(val);
  }

  loadFilteredCategories() {
    this.searchCategories().subscribe(result => this.filteredCategories = result);
  }
}
