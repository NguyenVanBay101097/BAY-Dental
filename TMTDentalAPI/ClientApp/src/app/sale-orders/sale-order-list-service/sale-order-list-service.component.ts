import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormArray, FormBuilder } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { fromEvent, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { Product } from 'src/app/products/product';
import { ProductBasic2, ProductPaged, ProductService } from 'src/app/products/product.service';
import { ProductsOdataService } from 'src/app/shared/services/ProductsOdata.service';
import { ToothDisplay } from 'src/app/teeth/tooth.service';

@Component({
  selector: 'app-sale-order-list-service',
  templateUrl: './sale-order-list-service.component.html',
  styleUrls: ['./sale-order-list-service.component.css']
})
export class SaleOrderListServiceComponent implements OnInit {

  @ViewChild("searchInput", {static: true}) searchInput: ElementRef;
  @Output() newEventEmiter = new EventEmitter<any>()
  limit: number = 20;
  skip: number = 0;
  toothDisplays: ToothDisplay[] = []
  search: string;
  partnerId: string;
  totalListProducts: any[] = [];
  listProducts: any[] = [];
  searchUpdate = new Subject<string>();

  thTable_services = [
    { name: 'Dịch vụ' },
    { name: 'Đơn giá' }
  ]

  constructor(
    private productOdataService: ProductsOdataService,
    private modalService: NgbModal,
    private fb: FormBuilder
  ) { }

  ngOnInit() {

    this.searchInit();
    this.loadDataDefault();
  }

  searchInit() {
    fromEvent(document, 'keyup').pipe(
      filter((s: any) => s.keyCode ===	67)
    ).subscribe(s => {
      this.searchInput.nativeElement.focus();
    });

    this.searchUpdate
      .pipe(distinctUntilChanged())
      .subscribe((value) => {
        this.onSearch(value);
      });
  }

  loadDataDefault() {
    // var val = new ProductPaged();
    // val.limit = this.limit;
    // val.offset = this.skip;
    // val.search = this.search || "";
    // val.type2 = "service";

    const state = {
      filter: {
        logic: 'and',
        filters: [
          { field: 'Type2', operator: 'eq', value: 'service' }
        ]
      }
    };

    const options = {
      select: 'Id,Name,NameNoSign,DefaultCode,ListPrice',
      orderby: 'DateCreated desc'
    };

    this.productOdataService
      .getFetch(state, options).subscribe(
        (res: any) => {
          this.listProducts = res.data;
          this.totalListProducts = res.data.map(x => ({
            ...x, searchString: x.Name.toLowerCase() + ' ' + x.NameNoSign.toLowerCase()
              + ' ' + x.DefaultCode.toLowerCase()
          }));

        },
        (err) => {
          console.log(err);
        }
      );
  }

  onSearch(val) {
    val = val.trim();
    if (val === '') {
      this.listProducts = this.totalListProducts;
      return;
    }
    this.listProducts = this.totalListProducts.filter(x => x.searchString.includes(val));
  }

  addServiceToSaleOrder(item) {
    var value = {
      AmountPaid: 0,
      AmountResidual: 0,
      Diagnostic: '',
      Discount: 0,
      DiscountFixed: 0,
      DiscountType: 'percentage',
      Employee: null,
      EmployeeId: '',
      Name: item.Name,
      PriceSubTotal: 0,
      PriceUnit: item.ListPrice,
      ProductId: item.Id,
      Product: {
        Id: item.Id,
        Name: item.Name
      },
      ProductUOMQty: 1,
      State: 'draft',
      Teeth: this.fb.array([]),
      ToothCategory: null,
      ToothCategoryId: '',
    };
    this.newEventEmiter.emit(value);
  }

}
