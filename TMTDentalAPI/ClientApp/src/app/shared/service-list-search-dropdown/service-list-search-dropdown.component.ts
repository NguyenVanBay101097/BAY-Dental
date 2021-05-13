import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { identity } from 'lodash';
import { combineLatest, forkJoin, fromEvent, merge, Observable, of, OperatorFunction, Subject, throwError } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, filter, map, switchMap, tap, withLatestFrom } from 'rxjs/operators';
import { ProductPaged, ProductService } from 'src/app/products/product.service';

@Component({
  selector: 'app-service-list-search-dropdown',
  templateUrl: './service-list-search-dropdown.component.html',
  styleUrls: ['./service-list-search-dropdown.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class ServiceListSearchDropdownComponent implements OnInit {

  @ViewChild("searchInput", { static: true }) searchInput: ElementRef;
  listProducts = [];
  model: any;
  searching = false;
  searchFailed = false;

  @Output() onSelectService = new EventEmitter<any>()
  focus$ = new Subject<any>();

  constructor(
    private productService: ProductService
  ) { }

  ngOnInit() {
    fromEvent(document, 'keyup').pipe(
      filter((s: any) => s.keyCode === 113)
    ).subscribe(s => {
      this.searchInput.nativeElement.focus();
    });

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
          if(term.value && term.value.trim() != '') 
         return of(this.listProducts);
         else {
           term = term.value;
         }
        }
        return this.seachService$(term).pipe(
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

  formatter = (x: any) => x.name;

  seachService$(search) {
    var val = new ProductPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search ? search : '';
    val.type2 = 'service';
    if (val.search) {
      return this.productService.getPaged(val).pipe(map((res: any) => {

        if (res.items.length > 0) {
          this.listProducts = res.items;
          return res.items;
        } else {
          this.listProducts = [{ error: true }];
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
      listPrice: item.listPrice
    });

    this.model = "";
    input.value = '';
  }

}
