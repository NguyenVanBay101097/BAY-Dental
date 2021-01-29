import { Component, OnInit } from '@angular/core';
import { ProductService, ProductImportExcelViewModel, ProductImportExcelBaseViewModel, ProductPaged } from '../product.service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-product-import-excel-dialog',
  templateUrl: './product-import-excel-dialog.component.html',
  styleUrls: ['./product-import-excel-dialog.component.css']
})
export class ProductImportExcelDialogComponent implements OnInit {
  fileBase64 = '';
  type: string;
  errors: string[];
  title: string;
  update: string;
  isUpdate: boolean;
  constructor(private productService: ProductService, public activeModal: NgbActiveModal, private notificationService: NotificationService,
    private errorService: AppSharedShowErrorService) { }

  ngOnInit() {
  }

  onFileChange(value) {
    this.fileBase64 = value;
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

  onSave() {
    if (!this.fileBase64 || this.fileBase64 === '') {
      this.notify('error','Vui lòng chọn file để import');
      return;
    }
    var val = new ProductImportExcelBaseViewModel();
    val.fileBase64 = this.fileBase64;
    this.actionImport(val).subscribe((result: any) => {
      if (result.success) {
        this.notify('success', 'Import dữ liệu thành công');
        this.activeModal.close(true);
      } else {
        this.errors = result.errors;
      }
    }, err => {
    });
  }

  actionImport(val: any) {
    if (this.type == 'service') {
      return this.productService.importService(val);
    } else if (this.type == 'medicine') {
      return this.productService.importMedicine(val);
    } else if (this.type == 'product') {
      return this.productService.importProduct(val);
    } else if(this.type == 'labo') {
      return this.productService.importLabo(val);
    } if(this.type == 'labo_attach') {
      return this.productService.importLaboAttach(val);
    }
     else {

    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  updateFileExcel(){
    if (!this.fileBase64 || this.fileBase64 === '') {
      this.notify('error','Vui lòng chọn file để cập nhật');
      return;
    }
    var val = new ProductImportExcelBaseViewModel();
    val.fileBase64 = this.fileBase64;
    if(this.type == 'service'){
      this.productService.updateServiceFromExcel(val).subscribe((result: any) => {
        if (result.success) {
          this.notify('success', 'Cập nhật dữ liệu thành công');
          this.activeModal.close(true);
        } else {
          this.errors = result.errors;
          this.notify('error', 'Cập nhật dữ liệu không thành công');
        }
      }, err => {
      });
    }

    if(this.type == 'product'){
      this.productService.updateProductFromExcel(val).subscribe((result: any) => {
        if (result.success) {
          this.notify('success', 'Cập nhật dữ liệu thành công');
          this.activeModal.close(true);
        } else {
          this.errors = result.errors;
          this.notify('error', 'Cập nhật dữ liệu không thành công');
        }
      }, err => {
      });
    }

    if(this.type == 'medicine'){
      this.productService.updateMedicineFromExcel(val).subscribe((result: any) => {
        if (result.success) {
          this.notify('success', 'Cập nhật dữ liệu thành công');
          this.activeModal.close(true);
        } else {
          this.errors = result.errors;
          this.notify('error', 'Cập nhật dữ liệu không thành công');
        }
      }, err => {
      });
    }
  }

  loadExcelUpdateSeviceFile(){
    var paged = new ProductPaged();

    // paged.search = this.searchService || "";
    // paged.categId = this.cateId || "";
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

  loadExcelUpdateProductFile(){
    var paged = new ProductPaged();

    //paged.search = this.searchProduct || "";
    //paged.categId = this.cateId || "";
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

  loadExcelUpdateMedicineFile(){
    var paged = new ProductPaged();

    //paged.search = this.searchProduct || "";
    //paged.categId = this.cateId || "";
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
}
