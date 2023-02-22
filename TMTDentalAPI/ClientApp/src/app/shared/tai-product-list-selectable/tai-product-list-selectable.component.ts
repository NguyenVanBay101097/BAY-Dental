import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ProductBasic2 } from 'src/app/products/product.service';

@Component({
  selector: 'app-tai-product-list-selectable',
  templateUrl: './tai-product-list-selectable.component.html',
  styleUrls: ['./tai-product-list-selectable.component.css']
})
export class TaiProductListSelectableComponent implements OnInit {
  @Input() productList: ProductBasic2[] = [];
  productSelectedIndex = 0;
  @Output() changeProduct = new EventEmitter<ProductBasic2>();
  constructor() { }

  ngOnInit() {
  }

  selectProduct(product: ProductBasic2) {
    this.changeProduct.emit(product);
  }

  moveUp() {
    this.productSelectedIndex += 1;
    if (this.productSelectedIndex > this.productList.length - 1) {
      this.productSelectedIndex = 0;
    };
  }

  moveDown() {
    this.productSelectedIndex -= 1;
    if (this.productSelectedIndex < 0) {
      this.productSelectedIndex = this.productList.length - 1;
    }
  }

  selectCurrent() {
    this.selectProduct(this.productList[this.productSelectedIndex]);
  }

  resetIndex() {
    this.productSelectedIndex = 0;
  }
}
