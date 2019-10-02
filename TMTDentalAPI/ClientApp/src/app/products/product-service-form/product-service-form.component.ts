import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, NgForm } from '@angular/forms';
import { Product } from '../product';
import { ProductCategoryBasic, ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';

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
  constructor(private fb: FormBuilder, private productCategoryService: ProductCategoryService) { }

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
