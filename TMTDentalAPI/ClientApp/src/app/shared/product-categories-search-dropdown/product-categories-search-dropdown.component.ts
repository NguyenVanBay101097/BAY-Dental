import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { merge, Observable, of, OperatorFunction, Subject, throwError } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';

@Component({
  selector: 'app-product-categories-search-dropdown',
  templateUrl: './product-categories-search-dropdown.component.html',
  styleUrls: ['./product-categories-search-dropdown.component.css']
})
export class ProductCategoriesSearchDropdownComponent implements OnInit {
  focus$ = new Subject<any>();
  @ViewChild("searchInput", { static: true }) searchInput: ElementRef;
  listProductCategories = [];
  model: any;
  searching = false;
  searchFailed = false;

  @Output() onSelectService = new EventEmitter<any>()
  @Output() searchOutput = new EventEmitter<any>()
  @Input() searchResult$: Observable<any>;
  @Input() searchResult = [];
  
  constructor(
    private productCategoryService: ProductCategoryService
  ) { }

  ngOnInit(): void {
  }

  forCusEmit(e) {
    this.focus$.next({ type: 'forcus', value: e.target.value });
  }

  search: OperatorFunction<any, any[]> = (text$: Observable<any>) => {
    var debouncedText$ = text$.pipe(
      debounceTime(300),
      distinctUntilChanged()
    )

    var inputForcus$ = this.focus$;

    return merge(debouncedText$, inputForcus$).pipe(
      tap((res) => { this.searching = true }),
      switchMap((term) => {
        if(term.type && term.type == 'forcus') { 
          if(!term.value || term.value.trim() != '') 
         return of(this.listProductCategories);
         else {
           term = term.value;
         }
        }
        this.searchOutput.emit(term);
        return this.searchResult$.pipe(
          tap(res=> this.listProductCategories = res),
          tap(() => this.searchFailed = false),
          catchError(() => {
            this.searchFailed = true;
            return of([]);
          }))
      }

      ),
      tap(() => this.searching = false)
    );
  }

  seachProductCategories$(search) {
    var val = new ProductCategoryPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search ? search : '';
    if (val.search) {
      return this.productCategoryService.autocomplete(val).pipe(map((res: any) => {
        if (res.length > 0) {
          this.listProductCategories = res;
          return res;
        } else {
          this.listProductCategories = [{ error: true }];
          return [{ error: true }];
        }
      }));
    } else {
      return throwError({})
    }
  }

  onSelectValue($event, input) {
    $event.preventDefault();
    var item = $event.item;
    if (item.error) return;
    this.onSelectService.emit({
      id: item.id,
      name: item.name,
    });
    this.model = "";
    input.value = '';
  }

  formatter = (x: any) => x.name;


}
