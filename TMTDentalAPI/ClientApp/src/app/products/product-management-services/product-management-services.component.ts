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
  //category
  type: string = 'service';
  searchCate: string;
  categories: any[];
  sourceCategories: any[];
  searchCateUpdate = new Subject<string>();
  //service
  loading = false;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  searchService: string;
  cateId: string;
  searchServiceUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute,
    private router: Router,
    private productCategoryService: ProductCategoryService,
    private productService: ProductService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    // this.route.paramMap.subscribe(params => {
    // this.type = params.get('type');
    // this.loadCategories();
    // });

    this.searchCateUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.searchCategories(value);
      });
    this.loadCategories();
    this.searchServiceUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.loadServices();
      });
    this.loadServices();
  }

  searchCategories(val) {
    val = val.trim().toLowerCase();
    if (val === '') {
      this.categories = this.sourceCategories;
      return;
    }
    this.categories = this.sourceCategories.filter(x => x.name.toLowerCase().includes(val));
  }

  loadCategories() {
    var val = new ProductCategoryPaged();
    val.limit = 0;
    val.offset = 0;
    val.search = this.searchCate || '';
    val.type = this.type;

    this.productCategoryService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.sourceCategories = res.data;
      this.categories = this.sourceCategories;
    }, err => {
      console.log(err);
    })
  }

  loadServices() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.searchService || "";
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

  reLoadService(cate: any) {
    if (this.cateId === cate.id) {
      return;
    }
    this.cateId = cate.id;
    this.loadServices();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadServices();
  }

  createCate() {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: nhóm dịch vụ';
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(result => {
      this.loadCategories();
    }, () => {
    });
  }

  editCate(item: ProductCategory) {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: nhóm dịch vụ';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(() => {
      this.loadCategories();
    }, () => {
    });
  }

  deleteCate(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: nhóm dịch vụ';
    modalRef.result.then(() => {
      this.productCategoryService.delete(item.id).subscribe(() => {
        this.loadCategories();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
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
      let filename = "ExportedExcelFile";
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
