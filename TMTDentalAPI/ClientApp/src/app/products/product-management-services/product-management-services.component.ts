import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationStart, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map } from 'rxjs/operators';
import { ProductCategory } from 'src/app/product-categories/product-category';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductCategoryDialogComponent } from 'src/app/shared/product-category-dialog/product-category-dialog.component';
import { Product } from '../product';
import { ProductImportExcelDialogComponent } from '../product-import-excel-dialog/product-import-excel-dialog.component';
import { ProductServiceCuDialogComponent } from '../product-service-cu-dialog/product-service-cu-dialog.component';
import { ProductPaged, ProductService } from '../product.service';

@Component({
  selector: 'app-product-management-services',
  templateUrl: './product-management-services.component.html',
  styleUrls: ['./product-management-services.component.css']
})
export class ProductManagementServicesComponent implements OnInit {
  //service
  type = 'service';
  loading = false;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  searchService: string;
  cateId: string;
  selectedCateg: any;
  searchServiceUpdate = new Subject<string>();
  categories: any[] = [];

  constructor(private route: ActivatedRoute,
    private router: Router,
    private productCategoryService: ProductCategoryService,
    private productService: ProductService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.searchServiceUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.loadServices();
      });
    this.loadServices();
    this.loadCategories();
  }

  

  loadServices() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.searchService || "";
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

  onSelectedCate(cate: any) {
    this.selectedCateg = cate;
    this.loadServices();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadServices();
  }

  onDeleteCate(index) {
    this.categories.splice(index, 1);
    this.loadServices();
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

  createService() {
    let modalRef = this.modalService.open(ProductServiceCuDialogComponent, {
      size: "lg",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm: dịch vụ";
    modalRef.result.then(
      () => {
        this.loadServices();
      },
      () => { }
    );
  }

  editService(item: Product) {
    let modalRef = this.modalService.open(ProductServiceCuDialogComponent, {
      size: "lg",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Sửa: dịch vụ";
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(
      () => {
        this.loadServices();
      },
      () => { }
    );
  }

  deleteService(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Xóa: dịch vụ";

    modalRef.result.then(
      () => {
        this.productService.delete(item.id).subscribe(
          () => {
            this.loadServices();
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
      scrollable: true
    });
    modalRef.componentInstance.title = "Import excel";
    modalRef.componentInstance.type = "service";
    modalRef.result.then(
      () => {
        this.loadServices();
      },
      () => { }
    );
  }

  exportExcelFile() {
    var paged = new ProductPaged();

    paged.search = this.searchService || "";
    paged.categId = this.cateId || "";
    this.productService.excelServiceExport(paged).subscribe((rs) => {
      let filename = "danh_sach_dich_vu";
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
