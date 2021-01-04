import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PermissionService } from 'src/app/shared/permission.service';
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
  searchProductUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute,
    private router: Router,
    private productCategoryService: ProductCategoryService,
    private productService: ProductService,
    public permissionService: PermissionService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.searchProductUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.loadProducts();
      });
    this.loadProducts();
  }



  loadProducts() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.searchProduct || "";
    val.categId = this.cateId || "";
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

  onSelectedCate(cate: any) {
    if (this.cateId === cate.id) {
      return;
    }
    this.cateId = cate.id;
    this.loadProducts();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadProducts();
  }

  onDeletecate(e) {
    this.cateId = null;
    this.loadProducts();
  }

  createProduct() {
    let modalRef = this.modalService.open(ProductProductCuDialogComponent, {
      size: "lg",
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
      size: "lg",
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
      size: "lg",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
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

}
