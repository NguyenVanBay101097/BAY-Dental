import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ResConfigSettingsService } from 'src/app/res-config-settings/res-config-settings.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { Product } from '../product';
import { ProductImportExcelDialogComponent } from '../product-import-excel-dialog/product-import-excel-dialog.component';
import { ProductMedicineCuDialogComponent } from '../product-medicine-cu-dialog/product-medicine-cu-dialog.component';
import { ProductPaged, ProductService } from '../product.service';

@Component({
  selector: 'app-product-management-medicines',
  templateUrl: './product-management-medicines.component.html',
  styleUrls: ['./product-management-medicines.component.css']
})
export class ProductManagementMedicinesComponent implements OnInit {
  //product
  type = 'medicine';
  loading = false;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  searchMedicine: string;
  cateId: string;
  selectedCateg: any;
  searchMedicineUpdate = new Subject<string>();
  configsettings: any;
  categories: any[] = [];
  canAdd = false;
  canEdit = false;
  canDelete = false;
  active: boolean = true;

  constructor(
    private productCategoryService: ProductCategoryService,
    private configSettingsService: ResConfigSettingsService,
    private productService: ProductService,
    private modalService: NgbModal,
    private checkPermissionService: CheckPermissionService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.searchMedicineUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.skip = 0;
        this.loadMedicines();
      });
    this.loadMedicines();
    this.loadCategories();
    this.checkPermission();
  }



  loadMedicines() {
    this.loading = true;
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.searchMedicine || "";
    val.categId = this.selectedCateg ? this.selectedCateg.id : '';
    val.type2 = this.type;
    val.active = this.active;
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
          // console.log(res);
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
    this.loadMedicines();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadMedicines();
  }

  onDeletecate(index) {
    this.categories.splice(index, 1);
    this.loadMedicines();
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

  createMedicine() {
    let modalRef = this.modalService.open(ProductMedicineCuDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Th??m: thu???c";
    modalRef.result.then(
      () => {
        this.loadMedicines();
      },
      () => { }
    );
  }

  editMedicine(item: Product) {
    let modalRef = this.modalService.open(ProductMedicineCuDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "S???a: thu???c";
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(
      () => {
        this.loadMedicines();
      },
      () => { }
    );
  }

  deleteMedicine(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "X??a: thu???c";

    modalRef.result.then(
      () => {
        this.productService.delete(item.id).subscribe(
          () => {
            this.loadMedicines();
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
    modalRef.componentInstance.type = "medicine";
    modalRef.result.then(
      () => {
        this.loadMedicines();
      },
      () => { }
    );
  }

  exportExcelFile() {
    var paged = new ProductPaged();

    paged.search = this.searchMedicine || "";
    paged.categId = this.cateId || "";
    this.productService.excelMedicineExport(paged).subscribe((rs) => {
      let filename = "danh_sach_thuoc";
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

  updateMedicineFromExcel() {
    let modalRef = this.modalService.open(ProductImportExcelDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
      scrollable: true
    });
    modalRef.componentInstance.title = "C???p nh???t Excel";
    modalRef.componentInstance.type = "medicine";
    modalRef.componentInstance.isUpdate = true;

    modalRef.result.then(
      () => {
        this.loadMedicines();
      },
      () => { }
    );
  }

  checkPermission() {
    this.canAdd = this.checkPermissionService.check(['Catalog.Products.Create']);
    this.canEdit = this.checkPermissionService.check(['Catalog.Products.Update']);
    this.canDelete = this.checkPermissionService.check(['Catalog.Products.Delete']);
  }

  onActionUnArchive(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });

    modalRef.componentInstance.title = "Ng???ng s??? d???ng thu???c";
    modalRef.componentInstance.body = `B???n c?? ch???c mu???n ng???ng s??? d???ng thu???c ${item.name}?`;

    modalRef.result.then(() => {
      this.productService.actionUnArchive([item.id]).subscribe((res: any) => {
        this.loadMedicines();
      }, error => console.log(error));
    }, () => { });
  }

  onActionArchive(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });

    modalRef.componentInstance.title = "S??? d???ng l???i thu???c";
    modalRef.componentInstance.body = `B???n c?? ch???c mu???n s??? d???ng l???i thu???c ${item.name}?`;

    modalRef.result.then(() => {
      this.productService.actionArchive([item.id]).subscribe((res: any) => {
        this.loadMedicines();
      }, error => console.log(error));
    }, () => { });
  }
  
  onStateSelect(event) {
    this.active = event.target.value ? event.target.value : true;
    this.loadMedicines();
  }
}
