import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ProductService, ProductPaged } from '../product.service';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { ProductDialogComponent } from '../product-dialog/product-dialog.component';
import { Product } from '../product';
import { IntlService } from '@progress/kendo-angular-intl';
import { map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { ProductCategoryBasic, ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductImportExcelDialogComponent } from '../product-import-excel-dialog/product-import-excel-dialog.component';
import { ActivatedRoute } from '@angular/router';
import { ValueAxisLabelsComponent } from '@progress/kendo-angular-charts';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;

  saleOKFilter: boolean;
  purchaseOKFilter: boolean;
  keToaOKFilter: boolean;
  isLaboFilter: boolean;
  productConsuTypeFilter: boolean;
  serviceTypeFilter: boolean;

  search: string;
  searchCateg: ProductCategoryBasic;
  filteredCategs: ProductCategoryBasic[];
  searchUpdate = new Subject<string>();
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;

  constructor(private productService: ProductService, private windowService: WindowService,
    private dialogService: DialogService, public intl: IntlService, private productCategoryService: ProductCategoryService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

    this.categCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.categCbx.loading = true)),
      switchMap(value => this.searchCategories(value))
    ).subscribe(result => {
      this.filteredCategs = result;
      this.categCbx.loading = false;
    });

    this.loadDataFromApi();

    this.loadFilteredCategs();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.categId = this.searchCateg ? this.searchCateg.id : '';
    if (this.saleOKFilter) {
      val.saleOK = this.saleOKFilter;
    }
    if (this.purchaseOKFilter) {
      val.purchaseOK = this.purchaseOKFilter;
    }
    if (this.keToaOKFilter) {
      val.keToaOK = this.keToaOKFilter;
    }
    if (this.isLaboFilter) {
      val.isLabo = this.isLaboFilter;
    }

    var types = [];
    if (this.productConsuTypeFilter) {
      types.push('product,consu');
    }

    if (this.serviceTypeFilter) {
      types.push('service');
    }

    if (types.length) {
      val.type = types.join(',');
    }

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

  loadFilteredCategs() {
    this.searchCategories().subscribe(result => this.filteredCategs = result);
  }

  searchCategories(search?: string) {
    var val = new ProductCategoryPaged();
    val.search = search;
    return this.productCategoryService.autocomplete(val);
  }

  importFromExcel() {
    const windowRef = this.windowService.open({
      title: 'Thêm sản phẩm từ excel',
      content: ProductImportExcelDialogComponent,
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

  createService() {
    const windowRef = this.windowService.open({
      title: 'Thêm dịch vụ',
      content: ProductDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    var productDefaultVal = new Product();
    productDefaultVal.type = 'service';
    productDefaultVal.saleOK = true;
    productDefaultVal.purchaseOK = false;
    instance.productDefaultVal = productDefaultVal;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.loadDataFromApi();
      }
    });
  }

  createMedicine() {
    const windowRef = this.windowService.open({
      title: 'Thêm thuốc',
      content: ProductDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    var productDefaultVal = new Product();
    productDefaultVal.type = 'consu';
    productDefaultVal.saleOK = false;
    productDefaultVal.purchaseOK = false;
    productDefaultVal.keToaOK = true;
    instance.productDefaultVal = productDefaultVal;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.loadDataFromApi();
      }
    });
  }

  createLabo() {
    const windowRef = this.windowService.open({
      title: 'Thêm labo',
      content: ProductDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    var productDefaultVal = new Product();
    productDefaultVal.type = 'consu';
    productDefaultVal.saleOK = false;
    productDefaultVal.purchaseOK = true;
    productDefaultVal.isLabo = true;
    instance.productDefaultVal = productDefaultVal;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.loadDataFromApi();
      }
    });
  }

  createProduct() {
    const windowRef = this.windowService.open({
      title: 'Thêm vật tư',
      content: ProductDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    var productDefaultVal = new Product();
    productDefaultVal.type = 'product';
    productDefaultVal.saleOK = false;
    productDefaultVal.purchaseOK = false;
    productDefaultVal.keToaOK = false;
    instance.productDefaultVal = productDefaultVal;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.loadDataFromApi();
      }
    });
  }

  createItem() {
    const windowRef = this.windowService.open({
      title: 'Thêm sản phẩm',
      content: ProductDialogComponent,
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

  editItem(item: Product) {
    const windowRef = this.windowService.open({
      title: `Sửa sản phẩm`,
      content: ProductDialogComponent,
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