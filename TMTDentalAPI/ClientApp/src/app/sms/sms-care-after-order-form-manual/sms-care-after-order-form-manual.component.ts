import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { fromEvent, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map } from 'rxjs/operators';
import { SaleOrderLineService, SmsCareAfterOrderPaged } from 'src/app/core/services/sale-order-line.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ProductsOdataService } from 'src/app/shared/services/ProductsOdata.service';
import { SmsManualDialogComponent } from '../sms-manual-dialog/sms-manual-dialog.component';

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
  productId: any;
  dateFrom: Date;
  dateTo: Date;
  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  search: string = '';
  selectedIds: string[] = [];
  searchUpdate = new Subject<string>();
  filterPaged: SmsCareAfterOrderPaged = new SmsCareAfterOrderPaged();
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  constructor(
    private notificationService: NotificationService,
    private productService: ProductService,
    private saleOrderLineService: SaleOrderLineService,
    private modalService: NgbModal,
    private intlService: IntlService,
  ) { }

  ngOnInit() {
    this.loadProducts();

    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.searchProductUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadProducts();
      });
  }

  loadProducts() {
    this.onSearchProduct().subscribe(
      res => {
        if (res && res[0]) {
          this.productId = res[0].id;
        }
        this.listProducts = res;
        this.loadDataFromApi();
      }
    )
  }

  onSearchProduct() {
    var productPaged = new ProductPaged();
    productPaged.limit = 20;
    productPaged.offset = 0;
    productPaged.search = this.searchProduct || '';
    productPaged.type = "service";
    return this.productService.autocomplete2(productPaged);
  }

  searchChangeDate(value) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  selectProduct(item) {
    this.productId = item ? item.id : '';
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    this.filterPaged.limit = this.limit;
    this.filterPaged.offset = this.skip;
    this.filterPaged.search = this.search ? this.search : '';
    this.filterPaged.productId = this.productId || '';
    this.filterPaged.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : '';
    this.filterPaged.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : '';
    this.saleOrderLineService.getSmsCareAfterOrderManual(this.filterPaged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      console.log(res);
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
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

  onSend() {
    if (this.selectedIds.length == 0) {
      this.notify("Bạn phải chọn khách hàng trước khi gửi tin", false);
    }
    else {
      var modalRef = this.modalService.open(SmsManualDialogComponent, { size: "lg", windowClass: "o_technical_modal" });
      modalRef.componentInstance.title = "Tin nhắn chăm sóc sau điều trị";
      modalRef.componentInstance.ids = this.selectedIds ? this.selectedIds : [];
      modalRef.componentInstance.isCareAfterOrder = true;
      modalRef.componentInstance.templateTypeTab = "care_after_order";
      modalRef.result.then(
        result => {
        }
      )
    }
  }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }
}
