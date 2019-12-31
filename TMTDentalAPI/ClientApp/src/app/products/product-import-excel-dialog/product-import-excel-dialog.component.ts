import { Component, OnInit } from '@angular/core';
import { ProductService, ProductImportExcelViewModel } from '../product.service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-product-import-excel-dialog',
  templateUrl: './product-import-excel-dialog.component.html',
  styleUrls: ['./product-import-excel-dialog.component.css']
})
export class ProductImportExcelDialogComponent implements OnInit {
  fileBase64 = '';
  type: string;
  type2: string;
  errors: string[];
  title: string;
  constructor(private productService: ProductService, public activeModal: NgbActiveModal) { }

  ngOnInit() {
  }

  onFileChange(value) {
    this.fileBase64 = value;
  }

  onSave() {
    if (!this.fileBase64 || this.fileBase64 === '') {
      return;
    }
    var val = new ProductImportExcelViewModel();
    val.fileBase64 = this.fileBase64;
    val.type = this.type;
    val.type2 = this.type2;
    this.productService.importExcel(val).subscribe((result: any) => {
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
