import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { fromEvent, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleOrderLineService, SmsCareAfterOrderPaged } from 'src/app/core/services/sale-order-line.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ProductsOdataService } from 'src/app/shared/services/ProductsOdata.service';
import { SmsCampaignService } from '../sms-campaign.service';
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
  searchPartnerUpdate = new Subject<string>();
  filterPaged: SmsCareAfterOrderPaged = new SmsCareAfterOrderPaged();
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  campaign: any;
  filteredProducts: any[] = [];

  constructor(
    private notificationService: NotificationService,
    private productService: ProductService,
    private saleOrderLineService: SaleOrderLineService,
    private modalService: NgbModal,
    private intlService: IntlService,
    private smsCampaignService: SmsCampaignService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.loadProducts();
    this.loadDataFromApi();
    setTimeout(() => {
      this.loadDefaultCampaignCareAfterOrder();
    }, 1000);
  

    this.searchProductUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.filterProducts(value);
      });

    this.searchPartnerUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.skip = 0;
        this.search = value;
        this.loadDataFromApi();
      });
  }

  loadProducts() {
    this.onSearchProduct().subscribe(
      res => {
        this.listProducts = res;
        this.filteredProducts = this.listProducts.slice();
      }
    )
  }

  filterProducts(val: string) {
    this.filteredProducts = this.listProducts.filter(x => x.name.toLowerCase().includes(val.toLowerCase()) ||
     x.nameNoSign.toLowerCase().includes(val.toLowerCase()));
  }

  onSearchProduct() {
    var search = this.searchProduct || '';
    return this.saleOrderLineService.getProductSmsCareAfterOrder({
      dateFrom: this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : '',
      dateTo: this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : '',
      companyId: this.authService.userInfo.companyId
    });
  }

  searchChangeDate(value) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    this.skip = 0;
    this.productId = null;
    this.loadProducts();
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
    this.filterPaged.companyId = this.authService.userInfo.companyId;
    this.saleOrderLineService.getSmsCareAfterOrderManual(this.filterPaged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  notify(title, isSuccess = true) {
    this.notificationService.show({
      content: title,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: isSuccess ? 'success' : 'error', icon: true },
    });
  }

  loadDefaultCampaignCareAfterOrder() {
    this.smsCampaignService.getDefaultCareAfterOrder().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      })
  }

  onSend() {
    if (this.selectedIds.length == 0) {
      this.notify("Bạn phải chọn khách hàng trước khi gửi tin", false);
    }
    else {
      var modalRef = this.modalService.open(SmsManualDialogComponent, { size: "md", windowClass: "o_technical_modal" });
      modalRef.componentInstance.title = "Tin nhắn chăm sóc sau điều trị";
      modalRef.componentInstance.resIds = this.selectedIds ? this.selectedIds : [];
      modalRef.componentInstance.resModel = 'sale-order-line';
      modalRef.componentInstance.templateTypeTab = "saleOrderLine";
      modalRef.componentInstance.campaign = this.campaign;
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
