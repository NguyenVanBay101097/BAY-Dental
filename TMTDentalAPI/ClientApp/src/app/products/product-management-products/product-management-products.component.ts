import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PermissionService } from 'src/app/shared/permission.service';
import { ToothDiagnosisSave } from 'src/app/tooth-diagnosis/tooth-diagnosis.service';
import { Product } from '../product';
import { ProductImportExcelDialogComponent } from '../product-import-excel-dialog/product-import-excel-dialog.component';
import { ProductProductCuDialogComponent } from '../product-product-cu-dialog/product-product-cu-dialog.component';
import { ProductPaged, ProductService } from '../product.service';

@Component({
  selector: 'app-product-management-products',
  templateUrl: './product-management-products.component.html',
  styleUrls: ['./product-management-products.component.css']
})
export class ProductManagementProductsComponent implements OnInit {
  //product
  type = 'product';
  loading = false;
  opened = false;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  searchProduct: string;
  cateId: string;
  selectedCateg: any;
  searchProductUpdate = new Subject<string>();
  categories: any[] = [];
  canAdd = false;
  canEdit = false;
  canDelete =false;
  constructor(private route: ActivatedRoute,
    private router: Router,
    private productCategoryService: ProductCategoryService,
    private productService: ProductService,
    public permissionService: PermissionService,
    private modalService: NgbModal,
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.searchProductUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.skip = 0;
        this.loadProducts();
      });
    this.loadProducts();
    this.loadCategories();
    this.checkPermission();
  }



  loadProducts() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.searchProduct || "";
    val.categId = this.selectedCateg ? this.selectedCateg.id : '';
    val.type2 = this.type;

    this.productService
      .getPaged(val)
      .pipe(
        map(
          (response) =>
            <GridDataResult>{
              data: response.items,
              total: response.totalItems,
            }
        )
      )
      .subscribe(
        (res) => {
          this.gridData = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  onCreateBtnEvent(categ) {
    this.categories.unshift(categ);
  }

  onUpdateBtnEvent(categId) {
    this.productCategoryService.get(categId).subscribe((categ: any) => {
      var index = this.categories.findIndex(x => x.id == categId);
      this.categories[index] = categ;
    });
  }

  onSelectedCate(cate: any) {
    this.selectedCateg = cate;
    this.loadProducts();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadProducts();
  }

  onDeletecate(index) {
    this.categories.splice(index, 1);
    this.loadProducts();
  }

  loadCategories() {
    var val = new ProductCategoryPaged();
    val.limit = 0;
    val.offset = 0;
    val.search = '';
    val.type = this.type;

    this.productCategoryService.getPaged(val).subscribe(res => {
      this.categories = res.items;
    }, err => {
      console.log(err);
    })
  }

  createProduct() {
    let modalRef = this.modalService.open(ProductProductCuDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm: vật tư";
    modalRef.result.then(
      () => {
        this.loadProducts();
      },
      () => { }
    );
  }

  editProduct(item: Product) {
    let modalRef = this.modalService.open(ProductProductCuDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Sửa: vật tư";
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(
      () => {
        this.loadProducts();
      },
      () => { }
    );
  }

  deleteProduct(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Xóa: vật tư";

    modalRef.result.then(
      () => {
        this.productService.delete(item.id).subscribe(
          () => {
            this.loadProducts();
          },
          (err) => {
            console.log(err);
          }
        );
      },
      () => { }
    );
  }

  importFromExcel() {
    let modalRef = this.modalService.open(ProductImportExcelDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
      scrollable: true
    });
    modalRef.componentInstance.title = "Import excel";
    modalRef.componentInstance.type = "product";
    modalRef.result.then(
      () => {
        this.loadProducts();
      },
      () => { }
    );
  }

  exportExcelFile() {
    var paged = new ProductPaged();

    paged.search = this.searchProduct || "";
    paged.categId = this.cateId || "";
    this.productService.excelProductExport(paged).subscribe((rs) => {
      let filename = "danh_sach_vat_tu";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      console.log(rs);

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  updateServiceFromExcel(){
    let modalRef = this.modalService.open(ProductImportExcelDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
      scrollable: true
    });
    modalRef.componentInstance.title = "Cập nhật Excel";
    modalRef.componentInstance.type = "product";
    modalRef.componentInstance.isUpdate = true;
    modalRef.result.then(
      () => {
        this.loadProducts();
      },
      () => { }
    );
  }

  checkPermission(){
    this.canAdd = this.checkPermissionService.check(['Catalog.Products.Create']);
    this.canEdit = this.checkPermissionService.check(['Catalog.Products.Update']);
    this.canDelete = this.checkPermissionService.check(['Catalog.Products.Delete']);
  }

}
