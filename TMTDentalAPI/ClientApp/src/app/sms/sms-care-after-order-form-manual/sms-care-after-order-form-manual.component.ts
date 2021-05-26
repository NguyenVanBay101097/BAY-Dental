import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NotificationService } from '@progress/kendo-angular-notification';
import { fromEvent, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductsOdataService } from 'src/app/shared/services/ProductsOdata.service';

@Component({
  selector: 'app-sms-care-after-order-form-manual',
  templateUrl: './sms-care-after-order-form-manual.component.html',
  styleUrls: ['./sms-care-after-order-form-manual.component.css']
})
export class SmsCareAfterOrderFormManualComponent implements OnInit {

  listProducts: any[] = [];
  totalListProducts: any[] = [];
  searchProduct: string;
  searchProductUpdate = new Subject<string>();
  productSelected: any;
  dateFrom: Date;
  dateTo: Date;
  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  search: string = '';
  selectedIds: string[] = [];
  searchUpdate = new Subject<string>();

  constructor(
    private notificationService: NotificationService,
    private productOdataService: ProductsOdataService,
  ) { }

  ngOnInit() {
    this.loadProducts();

    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.searchProductUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.onSearchProduct(value);
      });
  }
  
  loadProducts() {
    const state = {
      filter: {
        logic: 'and',
        filters: [
          { field: 'Type2', operator: 'eq', value: 'service' },
          { field: 'Active', operator: 'eq', value: true }
        ]
      }
    };

    const options = {
      select: 'Id,Name,NameNoSign,DefaultCode,ListPrice',
      orderby: 'DateCreated desc'
    };

    this.productOdataService.getFetch(state, options).subscribe(
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

  onSearchProduct(val) {
    val = val.trim().toLowerCase();
    if (val === '') {
      this.listProducts = this.totalListProducts;
    } else {
      this.listProducts = this.totalListProducts.filter(x => x.searchString.includes(val));
    }
  }

  selectProduct(item) {
    this.productSelected = this.productSelected == item ? null : item;
    
  }

  notify(type, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: type, icon: true }
    });
  }
}
