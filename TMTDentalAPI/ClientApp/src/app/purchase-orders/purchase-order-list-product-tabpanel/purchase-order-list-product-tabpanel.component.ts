import { ProductSimple } from './../../products/product-simple';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductPaged, ProductService } from 'src/app/products/product.service';

@Component({
  selector: 'app-purchase-order-list-product-tabpanel',
  templateUrl: './purchase-order-list-product-tabpanel.component.html',
  styleUrls: ['./purchase-order-list-product-tabpanel.component.css']
})
export class PurchaseOrderListProductTabpanelComponent implements OnInit, OnChanges {
  @Input() listProducts: ProductSimple[]= [];
  @Input() isStock: boolean;
  model: any;
  searching = false;
  searchFailed = false;
  type2: string = 'product';
  search: string;
  listProductFilter: ProductSimple[];
  searchUpdate = new Subject<string>();
  @Output() onSelectService = new EventEmitter<any>();

  constructor(
    private productService: ProductService
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadData();
  }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.listProductFilter = this.listProducts.filter(x => x.type2.includes(this.type2));
  }

  onChangeSearch(value) {
    if (value == '' || !value) {
      this.loadData();
    } else {
      this.listProductFilter = this.listProductFilter.filter(x => this.RemoveVietnamese(x.name).includes(value));
    }
    return this.listProductFilter;
  }

  onSelectValue(event) {
    var item = event;
    if (item.error)
      return;

    this.onSelectService.emit({
      id: item.id,
      name: item.name,
      purchasePrice: item.purchasePrice,
      defaultCode: item.defaultCode,
      type2: item.type2
    });
  }

  onChangeType(value) {
    this.type2 = value;
    this.loadData();
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

}
