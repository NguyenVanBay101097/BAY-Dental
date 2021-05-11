import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { identity } from 'lodash';
import { fromEvent, Observable, of, OperatorFunction } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, filter, map, switchMap, tap } from 'rxjs/operators';
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

  search: OperatorFunction<any, any[]> = (text$: Observable<any>) =>
  text$.pipe(
    debounceTime(300),
    distinctUntilChanged(),
    tap(() => this.searching = true),
    switchMap(term =>
      this.seachService$(term).pipe(
        tap(() => this.searchFailed = false),
        catchError(() => {
          this.searchFailed = true;
          return of([]);
        }))
    ),
    tap(() => this.searching = false)
  )

  formatter = (x: any) => x.name;

seachService$(search) {
  var val = new ProductPaged();
  val.limit = 10;
  val.offset = 0;
  val.search = search ? search : '';
  val.type2 = 'service';
  return this.productService.getPaged(val).pipe(map((res: any) => res.items));
}

onSelectValue($event, input) {
  $event.preventDefault();
  var item = $event.item;
  this.onSelectService.emit({
    id: item.id,
    name: item.name,
    listPrice: item.listPrice
  });

  this.model = "";
  input.value = '';
}

}
