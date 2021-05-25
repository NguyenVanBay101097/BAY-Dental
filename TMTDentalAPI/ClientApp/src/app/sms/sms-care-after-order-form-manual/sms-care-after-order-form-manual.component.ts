import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NotificationService } from '@progress/kendo-angular-notification';
import { fromEvent, Subject } from 'rxjs';
import { distinctUntilChanged, filter } from 'rxjs/operators';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductsOdataService } from 'src/app/shared/services/ProductsOdata.service';

@Component({
  selector: 'app-sms-care-after-order-form-manual',
  templateUrl: './sms-care-after-order-form-manual.component.html',
  styleUrls: ['./sms-care-after-order-form-manual.component.css']
})
export class SmsCareAfterOrderFormManualComponent implements OnInit {

  @ViewChild("searchInput", { static: true }) searchInput: ElementRef;
  listProducts: any[] = [];
  totalListProducts: any[] = [];
  searchUpdate = new Subject<string>();

  constructor(
    private notificationService: NotificationService,
    private productOdataService: ProductsOdataService,
  ) { }

  ngOnInit() {
  }

  searchInit() {
    fromEvent(document, 'keyup').pipe(
      filter((s: any) => s.keyCode === 113)
    ).subscribe(s => {     
      this.searchInput.nativeElement.focus();
    });

    this.searchUpdate
      .pipe(distinctUntilChanged())
      .subscribe((value) => {
        this.onSearch(value);
      });
  }

  onKeyUp(s) {
    if (s.key === 'Enter' || s.keyCode === 13) {
      if (this.listProducts) { this.selectProduct(this.listProducts[0]); }
    }
  }

  loadDataDefault() {
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
    val = val.trim().toLowerCase();
    if (val === '') {
      this.listProducts = this.totalListProducts;
      return;
    }
    this.listProducts = this.totalListProducts.filter(x => x.searchString.includes(val));
    return;
  }

  selectProduct(item) {

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
