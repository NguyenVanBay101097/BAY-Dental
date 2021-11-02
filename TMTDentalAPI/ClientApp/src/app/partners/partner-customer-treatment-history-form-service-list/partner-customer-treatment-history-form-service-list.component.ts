import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductBasic2, ProductPaged, ProductService } from 'src/app/products/product.service';
import {
  ToothDisplay
} from 'src/app/teeth/tooth.service';

@Component({
  selector: 'app-partner-customer-treatment-history-form-service-list',
  templateUrl: './partner-customer-treatment-history-form-service-list.component.html',
  styleUrls: ['./partner-customer-treatment-history-form-service-list.component.css']
})
export class PartnerCustomerTreatmentHistoryFormServiceListComponent implements OnInit {
  @Output() newEventEmiter = new EventEmitter<any>()
  limit: number = 20;
  skip: number = 0;
  toothDisplays: ToothDisplay[] = []
  search: string;
  partnerId: string;
  listProductServices: ProductBasic2[] = [];
  searchUpdate = new Subject<string>();

  thTable_services = [
    { name: 'Tên dịch vụ' },
    { name: 'Đơn giá' }
  ]

  constructor(
    private productService: ProductService,
    private fb: FormBuilder
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
    var value = {
      amountPaid: 0,
      amountResidual: 0,
      diagnostic: '',
      note: '',
      discount: 0,
      discountFixed: 0,
      discountType: 'percentage',
      employee: null,
      employeeId: '',
      name: item.name,
      priceSubTotal: 0,
      priceUnit: item.listPrice,
      productId: item.id,
      product: {
        id: item.id,
        name: item.name
      },
      productUOMQty: 1,
      state: 'draft',
      teeth: this.fb.array([]),
      toothCategory: null,
      toothCategoryId: '',

    };
    this.newEventEmiter.emit(value)
  }
}