import { ProductPaged, ProductService } from './../../products/product.service';
import { StockInventoryService } from './../stock-inventory.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Subject } from 'rxjs';
import { FormBuilder } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SimpleChange } from '@angular/core';

@Component({
  selector: 'app-stock-inventory-product-list',
  templateUrl: './stock-inventory-product-list.component.html',
  styleUrls: ['./stock-inventory-product-list.component.css']
})
export class StockInventoryProductListComponent implements OnInit {
  @Input() searchText;
  @Output() newEventEmiter = new EventEmitter<any>()
  search: string;
  @Input() inventoryId: string;
  limit = 20;
  skip = 0;
  listFilter: any[] = [];
  listProduct: any[] = [];
  searchUpdate = new Subject<string>();
  constructor(private stockInventoryService: StockInventoryService, private productService: ProductService,
    private fb: FormBuilder,) { }

  ngOnInit() {
    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  ngOnChanges(change: SimpleChange) {
    this.onChangeSearch(this.searchText);
  }

  loadDataFromApi() {
    if (this.inventoryId) {
      var val = new ProductPaged();
      val.limit = 1000;
      val.offset = this.skip;
      val.search = this.search || "";
      val.type = "product";
  
      this.productService
        .getPaged(val).subscribe(
          (res) => {
            this.listFilter = res.items;
            this.listProduct = res.items;
          },
          (err) => {
            console.log(err);
          }
        );
    }
  }

  addProductInventory(item) {   
    this.newEventEmiter.emit(item);
  }

  onChangeSearch(value) {
    if (value == '' || !value) {
      this.listFilter = this.listProduct;
    } else {
      this.listFilter = this.listProduct.filter(x => this.RemoveVietnamese(x.name).includes(this.RemoveVietnamese(value)));
    }
    return this.listFilter;
  }

  RemoveVietnamese(text) {
    text = text.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    text = text.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    text = text.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    text = text.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    text = text.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    text = text.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    text = text.replace(/đ/g, "d");
    text = text.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    text = text.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    text = text.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    text = text.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    text = text.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    text = text.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    text = text.replace(/Đ/g, "D");
    text = text.toLowerCase();
    text = text
      .replace(/[&]/g, "-and-")
      .replace(/[^a-zA-Z0-9._-]/g, "-")
      .replace(/[-]+/g, "-")
      .replace(/-$/, "");
    return text;
  }

}
