import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { fromEvent, merge, Observable, of, OperatorFunction, Subject, throwError } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, filter, map, switchMap, tap } from 'rxjs/operators';
import { ProductPaged, ProductService } from 'src/app/products/product.service';

@Component({
  selector: 'app-product-list-search-dropdown',
  templateUrl: './product-list-search-dropdown.component.html',
  styleUrls: ['./product-list-search-dropdown.component.css']
})
export class ProductListSearchDropdownComponent implements OnInit {

  @ViewChild("searchInput", { static: true }) searchInput: ElementRef;
  listProducts = [];
  model: any;
  searching = false;
  searchFailed = false;
  type2: string = 'medicine';
  search: string;
  searchUpdate = new Subject<string>();

  @Output() onSelectService = new EventEmitter<any>()
  focus$ = new Subject<any>();

  constructor(
    private productService: ProductService
  ) { }

  ngOnInit() {
    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.LoadDataProduct();
      });

    this.LoadDataProduct();
  }

  forCusEmit(e) {
    this.focus$.next({ type: 'forcus', value: e.target.value });
  }

  formatter = (x: any) => x.name;

  LoadDataProduct() {
    var val = new ProductPaged();
    val.limit = 20;
    val.offset = 0;
    val.purchaseOK = true;
    val.type = 'product';
    val.search = this.search ? this.search : '';
    val.type2 = this.type2;
    this.productService
      .autocomplete2(val).subscribe(
        (res) => {
          console.log(res)
          this.listProducts = res;
        },
        (err) => {
          console.log(err);
        }
      );
  }

  onSelectValue(event) {

    var item = event;
    if (item.error)
      return;

    this.onSelectService.emit({
      id: item.id,
      name: item.name,
      listPrice: item.listPrice
    });
  }

  onChangeSearch(value) {
    this.search = value;
    this.LoadDataProduct();
  }

  onChangeType(value) {
    this.type2 = value;
    this.LoadDataProduct();
  }

}
