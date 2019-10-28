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
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
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
  type: string;

  showAdvanceFilter = false;

  constructor(private productService: ProductService, private windowService: WindowService,
    private dialogService: DialogService, public intl: IntlService, private productCategoryService: ProductCategoryService,
    private route: ActivatedRoute, private modalService: NgbModal) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

    // this.categCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.categCbx.loading = true)),
    //   switchMap(value => this.searchCategories(value))
    // ).subscribe(result => {
    //   this.filteredCategs = result;
    //   this.categCbx.loading = false;
    // });

    this.route.paramMap.subscribe(params => {
      this.type = params.get('type');
      this.loadDataFromApi();
      this.loadFilteredCategs();
    });
  }

  getTypeTitle() {
    switch (this.type) {
      case 'service':
        return 'Dịch vụ điều trị';
      case 'product':
        return 'Vật tư';
      case 'medicine':
        return 'Thuốc';
      default:
        return 'Sản phẩm';
    }
  }

  getTypeLabel() {
    switch (this.type) {
      case 'service':
        return 'dịch vụ';
      case 'product':
        return 'vật tư';
      case 'medicine':
        return 'thuốc';
      default:
        return 'sản phẩm';
    }
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

    val.type2 = this.type;

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

  onAdvanceSearchChange(filter) {
    this.searchCateg = filter.categ ? filter.categ : null;
    this.saleOKFilter = filter.saleOK;
    this.keToaOKFilter = filter.keToaOK;
    this.isLaboFilter = filter.isLabo;
    this.loadDataFromApi();
  }

  searchCategories(search?: string) {
    var val = new ProductCategoryPaged();
    val.search = search;
    val.type = this.type;
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

  createItem() {
    let modalRef = this.modalService.open(ProductDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: ' + this.getTypeTitle();
    modalRef.componentInstance.type = this.type;
    if (this.type == 'service') {
      var productDefaultVal = new Product();
      productDefaultVal.type = 'service';
      productDefaultVal.saleOK = true;
      productDefaultVal.purchaseOK = false;
      modalRef.componentInstance.productDefaultVal = productDefaultVal;
    } else if (this.type == 'product') {
      var productDefaultVal = new Product();
      productDefaultVal.type = 'product';
      productDefaultVal.saleOK = false;
      productDefaultVal.purchaseOK = false;
      modalRef.componentInstance.productDefaultVal = productDefaultVal;
    } else if (this.type == 'medicine') {
      var productDefaultVal = new Product();
      productDefaultVal.type = 'consu';
      productDefaultVal.saleOK = false;
      productDefaultVal.purchaseOK = false;
      productDefaultVal.keToaOK = true;
      modalRef.componentInstance.productDefaultVal = productDefaultVal;
    } else {
      var productDefaultVal = new Product();
      productDefaultVal.type = 'consu';
      productDefaultVal.saleOK = true;
      productDefaultVal.purchaseOK = true;
      modalRef.componentInstance.productDefaultVal = productDefaultVal;
    }

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  createService() {
    let modalRef = this.modalService.open(ProductDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm dịch vụ';

    var productDefaultVal = new Product();
    productDefaultVal.type = 'service';
    productDefaultVal.saleOK = true;
    productDefaultVal.purchaseOK = false;
    modalRef.componentInstance.productDefaultVal = productDefaultVal;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  createMedicine() {
    let modalRef = this.modalService.open(ProductDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm thuốc';

    var productDefaultVal = new Product();
    productDefaultVal.type = 'consu';
    productDefaultVal.saleOK = false;
    productDefaultVal.purchaseOK = false;
    productDefaultVal.keToaOK = true;
    modalRef.componentInstance.productDefaultVal = productDefaultVal;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  createLabo() {
    let modalRef = this.modalService.open(ProductDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm labo';

    var productDefaultVal = new Product();
    productDefaultVal.type = 'consu';
    productDefaultVal.saleOK = false;
    productDefaultVal.isLabo = true;
    modalRef.componentInstance.productDefaultVal = productDefaultVal;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  createProduct() {
    let modalRef = this.modalService.open(ProductDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm vật tư';

    var productDefaultVal = new Product();
    productDefaultVal.type = 'product';
    productDefaultVal.saleOK = false;
    productDefaultVal.purchaseOK = false;
    modalRef.componentInstance.productDefaultVal = productDefaultVal;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: Product) {
    let modalRef = this.modalService.open(ProductDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: ' + this.getTypeTitle();
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.type = this.type;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa: ' + this.getTypeTitle(),
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