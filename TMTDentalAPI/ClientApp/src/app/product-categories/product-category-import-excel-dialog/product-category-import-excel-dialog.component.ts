import { ProductCategoryService, ProductCategoryImportExcelBaseViewModel } from './../product-category.service';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-product-category-import-excel-dialog',
  templateUrl: './product-category-import-excel-dialog.component.html',
  styleUrls: ['./product-category-import-excel-dialog.component.css']
})
export class ProductCategoryImportExcelDialogComponent implements OnInit {
  fileBase64 = '';
  type: string;
  errors: string[];
  title: string;
  constructor(private productCategoryService: ProductCategoryService, public activeModal: NgbActiveModal) { }

  ngOnInit() {
  }
  onFileChange(value) {
    this.fileBase64 = value;
  }

  onSave() {
    if (!this.fileBase64 || this.fileBase64 === '') {
      return;
    }
    var val = new ProductCategoryImportExcelBaseViewModel();
    val.fileBase64 = this.fileBase64;
    val.type = this.type;

    this.productCategoryService.importExcel(val).subscribe((result: any) => {
      if (result.success) {
        this.activeModal.close(true);
      } else {
        this.errors = result.errors;
      }
    });
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}
