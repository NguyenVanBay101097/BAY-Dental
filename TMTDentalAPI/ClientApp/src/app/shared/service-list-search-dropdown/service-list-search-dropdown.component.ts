import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { categories } from '@ctrl/ngx-emoji-mart/ngx-emoji';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { identity } from 'lodash';
import { combineLatest, forkJoin, fromEvent, merge, Observable, of, OperatorFunction, Subject, throwError } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, filter, map, switchMap, tap, withLatestFrom } from 'rxjs/operators';
import { ProductServiceCuDialogComponent } from 'src/app/products/product-service-cu-dialog/product-service-cu-dialog.component';
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
  @Input() placeHolder: string;
  focus$ = new Subject<any>();

  constructor(
    private productService: ProductService,
    private modalService: NgbModal
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
      return this.productService.autocomplete2(val).pipe(map((res: any) => {

        if (res.length > 0) {
          this.listProducts = res;
          return res;
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
    var emitValue = (value)=> {
      this.onSelectService.emit({
        id: value.id,
        defaultCode: value.defaultCode,
        name: value.name,
        listPrice: value.listPrice,
        categName: value.categ.name,
        categId: value.categ.id
      });
      this.model = "";
      input.value = '';
    }
    input.blur();
    $event.preventDefault();
    var item = $event.item;
    if (item.error){
      let modalRef = this.modalService.open(ProductServiceCuDialogComponent, {
        size: 'xl',
        windowClass: "o_technical_modal",
        keyboard: false,
        backdrop: "static",
      });
      modalRef.componentInstance.title = "Thêm: dịch vụ";
      modalRef.componentInstance.name = input.value;
      modalRef.result.then(
        (res) => {
      emitValue(res);
        },
        () => { }
      );
    } else {
      emitValue(item);
    }
   
  }

}
