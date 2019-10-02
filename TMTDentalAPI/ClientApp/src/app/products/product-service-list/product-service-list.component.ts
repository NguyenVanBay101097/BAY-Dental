import { Component, OnInit } from '@angular/core';
import { ProductService, ProductPaged, ProductBasic2 } from '../product.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { ProductDialogComponent } from '../product-dialog/product-dialog.component';
import { ProductServiceCuDialogComponent } from '../product-service-cu-dialog/product-service-cu-dialog.component';
import { Product } from '../product';
import { ProductAdvanceFilter } from '../product-advance-filter/product-advance-filter.component';

@Component({
  selector: 'app-product-service-list',
  templateUrl: './product-service-list.component.html',
  styleUrls: ['./product-service-list.component.css']
})
export class ProductServiceListComponent implements OnInit {
  constructor(private productService: ProductService, private windowService: WindowService, private dialogService: DialogService) { }
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  search: string;
  searchCategId: string;
  searchUpdate = new Subject<string>();

  opened = false;

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = 'service';
    val.saleOK = true;
    val.categId = this.searchCategId || '';

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

  onPageChange(event: PageChangeEvent) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    const windowRef = this.windowService.open({
      title: 'Thêm dịch vụ',
      content: ProductServiceCuDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.loadDataFromApi();
      }
    });
  }

  editItem(item: ProductBasic2) {
    const windowRef = this.windowService.open({
      title: `Sửa dịch vụ`,
      content: ProductServiceCuDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    instance.id = item.id;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.loadDataFromApi();
      }
    });
  }

  onFilterChange(filter: ProductAdvanceFilter) {
    this.searchCategId = filter.categId;
    this.loadDataFromApi();
  }

  deleteItem(item: ProductBasic2) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa sản phẩm',
      content: 'Bạn có chắc chắn muốn xóa?',
      actions: [
        { text: 'Hủy bỏ', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ],
      width: 450,
      height: 200,
      minWidth: 250
    });

    dialog.result.subscribe((result) => {
      if (result instanceof DialogCloseResult) {
        console.log('close');
      } else {
        console.log('action', result);
        if (result['value']) {
          this.productService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }
}
