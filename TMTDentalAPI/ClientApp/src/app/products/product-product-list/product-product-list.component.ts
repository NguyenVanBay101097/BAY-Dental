import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ProductService, ProductPaged } from '../product.service';
import { ProductDialogComponent } from '../product-dialog/product-dialog.component';
import { Product } from '../product';
import { IntlService } from '@progress/kendo-angular-intl';
import { map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { ProductCategoryBasic, ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductImportExcelDialogComponent } from '../product-import-excel-dialog/product-import-excel-dialog.component';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductServiceCuDialogComponent } from '../product-service-cu-dialog/product-service-cu-dialog.component';
import { ProductServiceImportDialogComponent } from '../product-service-import-dialog/product-service-import-dialog.component';
import { ProductProductCuDialogComponent } from '../product-product-cu-dialog/product-product-cu-dialog.component';
import { HasGroupsDirective } from 'src/app/shared/has-groups-directive';
import { PermissionService } from 'src/app/shared/permission.service';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-product-product-list',
  templateUrl: './product-product-list.component.html',
  styleUrls: ['./product-product-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ProductProductListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  title = 'Vật tư';

  search: string;
  searchCateg: ProductCategoryBasic;
  filteredCategs: ProductCategoryBasic[];
  searchUpdate = new Subject<string>();
  hasDefined: boolean;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  @ViewChild(HasGroupsDirective, { static: false }) hasGroup: HasGroupsDirective;

  constructor(
    private productService: ProductService,
    public intl: IntlService,
    private productCategoryService: ProductCategoryService,
    private route: ActivatedRoute,
    private modalService: NgbModal,
    private permissionService: PermissionService,
    private authService: AuthService
  ) { }

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

    setTimeout(() => {
      this.hasDefined = this.permissionService.hasOneDefined(['product.group_uom']);
    }, 100);

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
    val.type2 = 'product';

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

  onCategChange(value) {
    this.searchCateg = value;
    this.loadDataFromApi();
  }

  searchCategories(search?: string) {
    var val = new ProductCategoryPaged();
    val.search = search;
    val.type = 'product';
    return this.productCategoryService.autocomplete(val);
  }

  importFromExcel() {
    let modalRef = this.modalService.open(ProductImportExcelDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Import excel';
    modalRef.componentInstance.type = 'product';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  createItem() {
    let modalRef = this.modalService.open(ProductProductCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: ' + this.title;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: Product) {
    let modalRef = this.modalService.open(ProductProductCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: ' + this.title;
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;

    modalRef.result.then(() => {
      this.productService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
}


