import { Component, OnInit } from '@angular/core';
import { ProductService } from 'src/app/products/product.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-select-uom-product-dialog',
  templateUrl: './select-uom-product-dialog.component.html',
  styleUrls: ['./select-uom-product-dialog.component.css']
})
export class SelectUomProductDialogComponent implements OnInit {
  gridData: any[];
  productId: string;
  constructor(private productService: ProductService, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    setTimeout(() => {
      this.loadDataFromApi();
    });
  }

  selectUOM(value: any) {
    if (value.selectedRows.length) {
      this.activeModal.close(value.selectedRows[0].dataItem);
    }
  }

  loadDataFromApi() {
    this.productService.getUOMs(this.productId).subscribe((result: any) => {
      this.gridData = result;
    });
  }
}
