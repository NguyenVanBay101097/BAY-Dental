import { Component, OnInit, ViewChild, Output, EventEmitter, Input } from '@angular/core';
import { ProductCategoryBasic, ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';

export class ProductAdvanceFilter {
  categId: string;
}

@Component({
  selector: 'app-product-advance-filter',
  templateUrl: './product-advance-filter.component.html',
  styleUrls: ['./product-advance-filter.component.css']
})
export class ProductAdvanceFilterComponent implements OnInit {
  filteredCategs: ProductCategoryBasic[];
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  @Input() searchCategTitle: string;
  @Input() domain: any;
  @Output() filterChange = new EventEmitter<ProductAdvanceFilter>();
  searchCateg: ProductCategoryBasic;

  constructor(private productCategoryService: ProductCategoryService) { }

  ngOnInit() {
    this.categCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.categCbx.loading = true)),
      switchMap(value => this.searchCategories(value))
    ).subscribe(result => {
      this.filteredCategs = result;
      this.categCbx.loading = false;
    });

    this.loadFilteredCategs();
  }

  searchCategories(search?: string) {
    var val = new ProductCategoryPaged();
    val.search = search;
    if (this.domain) {
      val.medicineCateg = this.domain.medicineCateg || false;
      val.productCateg = this.domain.productCateg || false;
      val.laboCateg = this.domain.laboCateg || false;
      val.serviceCateg = this.domain.serviceCateg || false;
    }
    return this.productCategoryService.autocomplete(val);
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(result => this.filteredCategs = result);
  }

  onFilterChange(event) {
    var filter = this.getFilterData();
    this.filterChange.emit(filter);
  }

  getFilterData() {
    var filter = new ProductAdvanceFilter();
    filter.categId = this.searchCateg ? this.searchCateg.id : null;
    return filter;
  }
}
