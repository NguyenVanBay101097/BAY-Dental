import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ResConfigSettingsService } from 'src/app/res-config-settings/res-config-settings.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
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
searchMedicine: string;
cateId: string;
selectedCateg: any;
searchMedicineUpdate = new Subject<string>();
configsettings:any;
categories: any[] = [];

constructor(private route: ActivatedRoute,
  private router: Router,
  private productCategoryService: ProductCategoryService,
  private configSettingsService: ResConfigSettingsService,
  private productService: ProductService,
  private modalService: NgbModal
) { }

ngOnInit() {
  this.searchMedicineUpdate
    .pipe(debounceTime(400), distinctUntilChanged())
    .subscribe((value) => {
      this.loadMedicines();
    });
  this.loadMedicines();
  this.loadConfigsettings();
  this.loadCategories();
}



loadMedicines() {
  this.loading = true;
  var val = new ProductPaged();
  val.limit = this.limit;
  val.offset = this.skip;
  val.search = this.searchMedicine || "";
  val.categId =  this.selectedCateg ? this.selectedCateg.id : '';
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
        console.log(res);
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
  this.loadMedicines();
}

loadConfigsettings(){
  this.configSettingsService.defaultGet().subscribe(
    (result:any) => {
      this.configsettings = result;
    }
  );
}

pageChange(event: PageChangeEvent): void {
  this.skip = event.skip;
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
    size: "lg",
    windowClass: "o_technical_modal",
    keyboard: false,
    backdrop: "static",
  });
  modalRef.componentInstance.title = "Thêm: thuốc";
  modalRef.result.then(
    () => {
      this.loadMedicines();
    },
    () => { }
  );
}

editMedicine(item: Product) {
  let modalRef = this.modalService.open(ProductMedicineCuDialogComponent, {
    size: "lg",
    windowClass: "o_technical_modal",
    keyboard: false,
    backdrop: "static",
  });
  modalRef.componentInstance.title = "Sửa: thuốc";
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
  modalRef.componentInstance.title = "Xóa: thuốc";

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
    size: "lg",
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

updateMedicineFromExcel(){
  let modalRef = this.modalService.open(ProductImportExcelDialogComponent, {
    size: "lg",
    windowClass: "o_technical_modal",
    keyboard: false,
    backdrop: "static",
    scrollable: true
  });
  modalRef.componentInstance.title = "Cập nhật Excel";
  modalRef.componentInstance.type = "medicine";
  modalRef.componentInstance.isUpdate = true;
  
  modalRef.result.then(
    () => {
      this.loadMedicines();
    },
    () => { }
  );
}

}
