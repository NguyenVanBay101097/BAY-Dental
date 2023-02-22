import { Component, OnInit, ViewChild, Output, EventEmitter, Input } from '@angular/core';
import { ProductService, ProductBasic2, ProductPaged } from '../product.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { GridDataResult, PageChangeEvent, GridComponent, RowClassArgs } from '@progress/kendo-angular-grid';

@Component({
  selector: 'app-product-search-list',
  templateUrl: './product-search-list.component.html',
  styleUrls: ['./product-search-list.component.css'],
})
export class ProductSearchListComponent implements OnInit {

  search: string;
  searchUpdate = new Subject<string>();
  loading = false;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  indexProduct = 0;
  @Input() paged: ProductPaged;
  @Output() selected = new EventEmitter<ProductBasic2>();

  @ViewChild('grid', { static: true }) grid: GridComponent;

  constructor(private productService: ProductService) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  onSelectionChange(event: any) {
    if (event.selectedRows.length) {
      this.selected.emit(event.selectedRows[0].dataItem);
    }
  }

  onCellClick(event: any) {
    if (event.dataItem) {
      this.selected.emit(event.dataItem);
    }
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  rowCallback = (context: RowClassArgs) => {
    const active = context.index === this.indexProduct;
    return {
      active: active,
    };
  }

  onKeydown(e) {
    if (e.keyCode == 40) {
      this.indexProduct += 1;
      if (this.indexProduct > this.gridData.data.length - 1) {
        this.indexProduct = 0;
      }
    } else if (e.keyCode == 38) {
      this.indexProduct -= 1;
      if (this.indexProduct < 0) {
        this.indexProduct = this.gridData.data.length - 1;
      }
    } else if (e.keyCode == 13) {
      this.selected.emit(this.gridData.data[this.indexProduct]);
    } else {
      this.indexProduct = 0;
    }
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.paged.limit;
    val.offset = this.skip;
    val.type = this.paged.type;
    val.search = this.search || '';

    this.productService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }
}
