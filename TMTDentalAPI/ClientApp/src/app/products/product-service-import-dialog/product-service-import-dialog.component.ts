import { Component, OnInit } from '@angular/core';
import { ProductService, ProductImportExcelViewModel, ProductImportExcelBaseViewModel } from '../product.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-product-service-import-dialog',
  templateUrl: './product-service-import-dialog.component.html',
  styleUrls: ['./product-service-import-dialog.component.css']
})
export class ProductServiceImportDialogComponent implements OnInit {
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
    var val = new ProductImportExcelBaseViewModel();
    val.fileBase64 = this.fileBase64;
    this.productService.importService(val).subscribe((result: any) => {
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

