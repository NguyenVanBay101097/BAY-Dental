import { Component, OnInit } from '@angular/core';
import { ProductService, ProductImportExcelViewModel, ProductImportExcelBaseViewModel, ProductPaged } from '../product.service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { of } from 'rxjs';

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
  isUpdate: boolean = false;
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
      if (this.isUpdate) {
        this.notify('error', 'Vui lòng chọn file để cập nhật');
      }
      else {
        this.notify('error', 'Vui lòng chọn file để import');
      }
      return;
    }

    if (this.errors.length > 0)
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
