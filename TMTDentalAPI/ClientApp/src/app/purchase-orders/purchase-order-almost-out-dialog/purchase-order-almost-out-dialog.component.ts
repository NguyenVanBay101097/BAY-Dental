import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ProductService } from 'src/app/products/product.service';

@Component({
  selector: 'app-purchase-order-almost-out-dialog',
  templateUrl: './purchase-order-almost-out-dialog.component.html',
  styleUrls: ['./purchase-order-almost-out-dialog.component.css']
})
export class PurchaseOrderAlmostOutDialogComponent implements OnInit {
  @Input() title: string;
  listProductFilter: any[] = []
  selectedIds: any[] = [];
  productType: string = 'product';
  listProduct: any[] = [];

  constructor(
    public activeModal: NgbActiveModal,
    private productService: ProductService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var search = "";
    this.productService.getProductsComingEnd(search).subscribe((res: any) => {
      this.listProduct = res;
      this.loadListProduct();
    }, (err) => {
      console.log(err);
    })
  }

  loadListProduct() {
    this.listProductFilter = this.listProduct.filter(x => x.type2.includes(this.productType))
  }

  onSave() {
    let products = [];
    this.selectedIds.forEach((val) => {
      const data = this.listProduct.find(x => x.id.includes(val));
      products.push(data);
    });
    products.map(product => {
      return product.qty = product.minInventory - product.inventory;
    })
    this.activeModal.close(products);
  }

  onChangeType(value) {
    this.productType = value;
    this.loadListProduct();
  }

  onChangeSearch(value) {
    if (value == '' || !value) {
      this.loadListProduct();
    } else {
      this.listProductFilter = this.listProductFilter.filter(x => this.RemoveVietnamese(x.name).includes(value));
    }
    return this.listProductFilter;
  }

  RemoveVietnamese(text) {
    text = text.toLowerCase().trim();
    text = text.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    text = text.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    text = text.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    text = text.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    text = text.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    text = text.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    text = text.replace(/đ/g, "d");
    // Some system encode vietnamese combining accent as individual utf-8 characters
    text = text.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // Huyền sắc hỏi ngã nặng 
    text = text.replace(/\u02C6|\u0306|\u031B/g, ""); // Â, Ê, Ă, Ơ, Ư
    return text;
  }

  onCancel() {
    this.activeModal.dismiss()
  }

}
