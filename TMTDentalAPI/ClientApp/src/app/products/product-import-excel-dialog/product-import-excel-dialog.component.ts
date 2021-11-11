import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ProductImportExcelBaseViewModel, ProductService } from '../product.service';

@Component({
  selector: 'app-product-import-excel-dialog',
  templateUrl: './product-import-excel-dialog.component.html',
  styleUrls: ['./product-import-excel-dialog.component.css']
})
export class ProductImportExcelDialogComponent implements OnInit {
  fileBase64 = '';
  type: string;
  errors: string[] = [];
  title: string;
  isUpdate: boolean = false;
  correctFormat = true;
  constructor(private productService: ProductService, 
    public activeModal: NgbActiveModal, 
    private notificationService: NotificationService,
    ) { }

  ngOnInit() {
  }

  onFileChange(value) {
    this.fileBase64 = value;
    this.errors = [];
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
    if (!this.correctFormat){
      this.notify('error','File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng');
      return;
    }
    if (!this.fileBase64 || this.fileBase64 === '') {
      if (this.isUpdate) {
        this.notify('error', 'Vui lòng chọn file để cập nhật');
      }
      else {
        this.notify('error', 'Vui lòng chọn file để import');
      }
      return;
    }

    if (this.errors && this.errors.length > 0)
      return;

    var val = new ProductImportExcelBaseViewModel();
    val.fileBase64 = this.fileBase64;

    if (!this.isUpdate) {
      this.actionImport(val).subscribe((result: any) => {
        if (result.success) {
          this.notify('success', 'Import dữ liệu thành công');
          this.activeModal.close(true);
        } else {
          this.errors = result.errors;
        }
      }, err => {
      });
    } else {
      this.actionUpdateExcel(val).subscribe((result: any) => {
        if (result.success) {
          this.notify('success', 'Cập nhật dữ liệu thành công');
          this.activeModal.close(true);
        } else {
          this.errors = result.errors;
        }
      });
    }
  }

  actionImport(val: any) {
    if (this.type == 'service') {
      return this.productService.importService(val);
    } else if (this.type == 'medicine') {
      return this.productService.importMedicine(val);
    } else if (this.type == 'product') {
      return this.productService.importProduct(val);
    } else if (this.type == 'labo') {
      return this.productService.importLabo(val);
    } if (this.type == 'labo_attach') {
      return this.productService.importLaboAttach(val);
    }
  }

  actionUpdateExcel(val: any) {
    if (this.type == 'service') {
      return this.productService.updateServiceFromExcel(val);
    } else if (this.type == 'medicine') {
      return this.productService.updateMedicineFromExcel(val);
    } else if (this.type == 'product') {
      return this.productService.updateProductFromExcel(val);
    } else {
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  notifyError(value) {
    this.errors = value;
  }
}
