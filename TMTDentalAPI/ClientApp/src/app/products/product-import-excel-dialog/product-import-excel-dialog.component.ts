import { Component, OnInit } from '@angular/core';
import { ProductService, ProductImportExcelViewModel } from '../product.service';
import { WindowRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'app-product-import-excel-dialog',
  templateUrl: './product-import-excel-dialog.component.html',
  styleUrls: ['./product-import-excel-dialog.component.css']
})
export class ProductImportExcelDialogComponent implements OnInit {
  fileBase64 = '';
  constructor(private productService: ProductService, private window: WindowRef) { }

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
    this.productService.importExcel(val).subscribe(() => {
      this.window.close(true);
    });
  }

  onCancel() {
    this.window.close();
  }
}
