import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductPaged, ProductService } from 'src/app/products/product.service';

@Component({
  selector: 'app-purchase-order-list-product-tabpanel',
  templateUrl: './purchase-order-list-product-tabpanel.component.html',
  styleUrls: ['./purchase-order-list-product-tabpanel.component.css']
})
export class PurchaseOrderListProductTabpanelComponent implements OnInit {
  listProducts = [];
  model: any;
  searching = false;
  searchFailed = false;
  type2: string = 'medicine';
  search: string;
  searchUpdate = new Subject<string>();

  @Output() onSelectService = new EventEmitter<any>();
  
  constructor(
    private productService: ProductService
  ) { }

  ngOnInit() {
    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.LoadDataProduct();
      });

    this.LoadDataProduct();
  }

    LoadDataProduct() {
    var val = new ProductPaged();
    val.limit = 20;
    val.offset = 0;
    val.purchaseOK = true;
    val.type = 'product';
    val.search = this.search ? this.search : '';
    val.type2 = this.type2;
    this.productService
      .autocomplete2(val).subscribe(
        (res) => {
          console.log(res)
          this.listProducts = res;
        },
        (err) => {
          console.log(err);
        }
      );
  }

  onSelectValue(event) {
    var item = event;
    if (item.error)
      return;

    this.onSelectService.emit({
      id: item.id,
      name: item.name,
      purchasePrice: item.purchasePrice
    });
  }

  onChangeSearch(value) {
    this.search = value;
    this.LoadDataProduct();
  }

  onChangeType(value) {
    this.type2 = value;
    this.LoadDataProduct();
  }

}
