import { Component, OnInit, Output, EventEmitter, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ProductCategoryBasic, ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap, distinctUntilChanged, catchError } from 'rxjs/operators';
import { of, Observable } from 'rxjs';

@Component({
  selector: 'app-product-advance-search',
  templateUrl: './product-advance-search.component.html',
  styleUrls: ['./product-advance-search.component.css']
})
export class ProductAdvanceSearchComponent implements OnInit {
  formGroup: FormGroup;
  @Output() searchChange = new EventEmitter<any>();

  filteredCategs: ProductCategoryBasic[];
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;

  categSearching = false;
  searchFailed = false;
  constructor(private fb: FormBuilder, private productCategoryService: ProductCategoryService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      categ: null,
      saleOK: false,
      keToaOK: false,
      isLabo: false
    });

    // this.categCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.categCbx.loading = true)),
    //   switchMap(value => this.searchCategories(value))
    // ).subscribe(result => {
    //   this.filteredCategs = result;
    //   this.categCbx.loading = false;
    // });

    // this.loadFilteredCategs();
  }

  onSearch() {
    this.searchChange.emit(this.formGroup.value);
  }

  searchCategories(search?: string) {
    var val = new ProductCategoryPaged();
    val.search = search;
    return this.productCategoryService.autocomplete(val);
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(result => this.filteredCategs = result);
  }

  search = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => this.categSearching = true),
      switchMap(term =>
        this.searchCategories(term).pipe(
          tap(() => this.searchFailed = false),
          catchError(() => {
            this.searchFailed = true;
            return of([]);
          }))
      ),
      tap(() => this.categSearching = false)
    )
}

