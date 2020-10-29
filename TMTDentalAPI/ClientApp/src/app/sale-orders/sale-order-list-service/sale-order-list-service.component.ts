import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductBasic2, ProductPaged, ProductService } from 'src/app/products/product.service';

@Component({
  selector: 'app-sale-order-list-service',
  templateUrl: './sale-order-list-service.component.html',
  styleUrls: ['./sale-order-list-service.component.css']
})
export class SaleOrderListServiceComponent implements OnInit {

  @Output() newEventEmiter = new EventEmitter<any>()
  limit: number = 20;
  skip: number = 0;
  search: string;
  partnerId: string;
  listProductServices: ProductBasic2[] = [];
  searchUpdate = new Subject<string>();

  thTable_services = [
    { name: 'Dịch vụ' },
    { name: 'Đơn giá' }
  ]

  constructor(
    private productService: ProductService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.loadDataDefault();
      });
    this.loadDataDefault();
  }

  loadDataDefault() {
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || "";
    val.type2 = "service";

    this.productService
      .getPaged(val).subscribe(
        (res) => {
          this.listProductServices = res.items;
        },
        (err) => {
          console.log(err);
        }
      );
  }

  addServiceToSaleOrder(item) {
    item.priceSubTotal = 0;
    item.amountResidual = 0;
    item.priceUnit = item.listPrice;
    item.diagnostic = "";
    item.discountType = "percentage";
    item.amountPaid = 0;
    item.productUOMQty = 1;
    item.discount = 0;
    item.discountFixed = 0;
    this.newEventEmiter.emit(item)
  }

}
