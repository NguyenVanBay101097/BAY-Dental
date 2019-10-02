import { Component, ViewChild, OnInit } from '@angular/core';
import { ProductCategoryService, ProductCategoryPaged, ProductCategoryDisplay } from '../product-category.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { ProductCategoryDialogComponent } from '../product-category-dialog/product-category-dialog.component';
import { ProductCategory } from '../product-category';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-category-list',
  templateUrl: './product-category-list.component.html',
  providers: [ProductCategoryService],

  styleUrls: ['./product-category-list.component.css']
})
export class ProductCategoryListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private productCategoryService: ProductCategoryService,
    private route: ActivatedRoute,
    private windowService: WindowService, private dialogService: DialogService) {
  }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ProductCategoryPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.productCategoryService.getPaged(val).pipe(
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

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    const windowRef = this.windowService.open({
      title: "Thêm nhóm sản phẩm",
      content: ProductCategoryDialogComponent,
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

  editItem(item: ProductCategory) {
    const windowRef = this.windowService.open({
      title: "Sửa nhóm sản phẩm",
      content: ProductCategoryDialogComponent,
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

  deleteItem(item) {
    const dialog: DialogRef = this.dialogService.open({
      title: "Xóa nhóm sản phẩm",
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
          this.productCategoryService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }
}
