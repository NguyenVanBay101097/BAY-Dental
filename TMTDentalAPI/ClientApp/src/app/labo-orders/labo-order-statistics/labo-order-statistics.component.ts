import { Component, ViewChild, OnInit, Output, EventEmitter, ElementRef } from "@angular/core";
import { GridDataResult, PageChangeEvent } from "@progress/kendo-angular-grid";
import {
  map,
  debounceTime,
  distinctUntilChanged,
  tap,
  switchMap,
} from "rxjs/operators";
import { Subject } from "rxjs";
import { IntlService } from "@progress/kendo-angular-intl";
import { NgbModal, NgbPopover } from "@ng-bootstrap/ng-bootstrap";
import {
  LaboOrderService,
  LaboOrderStatisticsPaged,
  LaboOrderBasic,
} from "../labo-order.service";
import { ComboBoxComponent, PopupSettings } from "@progress/kendo-angular-dropdowns";
import { PartnerService } from "src/app/partners/partner.service";
import { PartnerPaged } from "src/app/partners/partner-simple";
import { ProductService, ProductPaged } from "src/app/products/product.service";
import * as _ from "lodash";
import { LaboOrderLineService } from "../labo-order-line.service";
import { LaboOrderStatisticUpdateDialogComponent } from './labo-order-statistic-update-dialog/labo-order-statistic-update-dialog.component';

@Component({
  selector: "app-labo-order-statistics",
  templateUrl: "./labo-order-statistics.component.html",
  styleUrls: ["./labo-order-statistics.component.css"],
  host: {
    class: "o_action o_view_controller",
  },
})
export class LaboOrderStatisticsComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  search: string;
  searchCustomer: string;
  searchSupplier: string;
  searchProduct: string;
  sentDateFrom: Date;
  sentDateTo: Date;
  receivedDateFrom: Date;
  receivedDateTo: Date;
  searchUpdate = new Subject<string>();
  id: string;
  @ViewChild("popOver", { static: true }) public popover: NgbPopover;
  paged: LaboOrderStatisticsPaged;
  public value: Date = new Date();
  filteredSuppliers: any;
  @ViewChild("supplierCbx", { static: true }) supplierCbx: ComboBoxComponent;

  filteredProducts: any;
  @ViewChild("productCbx", { static: true }) productCbx: ComboBoxComponent;

  constructor(
    private laboOrderService: LaboOrderService,
    private partnerService: PartnerService,
    private laboOrderLineService: LaboOrderLineService,
    private productService: ProductService,
    private intlService: IntlService,
    private modalService: NgbModal
  ) {}

  ngOnInit() {
    this.paged = new LaboOrderStatisticsPaged();

    this.loadDataFromApi();

    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.loadDataFromApi();
      });

    this.loadFilteredPartners();
    this.loadFilteredProducts();

    this.productCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.productCbx.loading = true)),
        switchMap((value) => this.searchProducts(value))
      )
      .subscribe((result) => {
        this.filteredProducts = result;
        this.productCbx.loading = false;
      });

    this.supplierCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.supplierCbx.loading = true)),
        switchMap((value) => this.searchSuppliers(value))
      )
      .subscribe((result) => {
        this.filteredSuppliers = result;
        this.supplierCbx.loading = false;
      });
  }

  loadFilteredProducts() {
    this.searchProducts().subscribe(
      (result) => (this.filteredProducts = result)
    );
  }

  searchProducts(search?: string) {
    var val = new ProductPaged();
    val.isLabo = true;
    val.search = search || "";
    return this.productService.autocomplete2(val);
  }

  onChangeDate(value: Date) {
    setTimeout(() => {
      this.loadDataFromApi();
    }, 200);
  }

  loadFilteredPartners() {
    this.searchSuppliers().subscribe((result) => {
      this.filteredSuppliers = _.unionBy(this.filteredSuppliers, result, "id");
    });
  }

  searchSuppliers(filter?: string) {
    var val = new PartnerPaged();
    val.supplier = true;
    val.search = filter || "";
    return this.partnerService.getAutocompleteSimple(val);
  }

  onChangeSupplier(data) {
    this.loadDataFromApi();
  }

  stateGet(state) {
    switch (state) {
      case "purchase":
        return "Đơn hàng";
      case "done":
        return "Đã khóa";
      case "cancel":
        return "Đã hủy";
      default:
        return "Nháp";
    }
  }

  onDateOrderSearchChange(data) {
    this.paged.dateOrderFrom = data.dateFrom
      ? this.intlService.formatDate(data.dateFrom, "yyyy-MM-dd")
      : null;
    this.paged.dateOrderTo = data.dateTo
      ? this.intlService.formatDate(data.dateTo, "yyyy-MM-dd")
      : null;
    this.loadDataFromApi();
  }

  onDatePlannedSearchChange(data) {
    this.paged.datePlannedFrom = data.dateFrom
      ? this.intlService.formatDate(data.dateFrom, "yyyy-MM-dd")
      : null;
    this.paged.datePlannedTo = data.dateTo
      ? this.intlService.formatDate(data.dateTo, "yyyy-MM-dd")
      : null;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.paged.limit = this.limit;
    this.paged.offset = this.skip;
    console.log(this.paged);

    this.laboOrderService
      .getPaged(this.paged)
      .pipe(
        map(
          (response: any) =>
            <GridDataResult>{
              data: response.items,
              total: response.totalItems,
            }
        )
      )
      .subscribe(
        (res) => {
          this.gridData = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  onAdvanceSearchChange(data) {
    this.sentDateFrom = data.sentDateFrom;
    this.sentDateTo = data.sentDateTo;
    this.receivedDateFrom = data.receivedDateFrom;
    this.receivedDateTo = data.receivedDateTo;

    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  public onChange(value: Date): void {
    this.value = value;  
  }

 
  toggleWithGreeting(popover,dataItem) {         
    if (popover.isOpen()) {       
      popover.close();
    } else { 
      this.value = new Date(dataItem.warrantyPeriod ? dataItem.warrantyPeriod : Date());    
      popover.open();
    }
  }

  editItem(item) {
    const modalRef = this.modalService.open(LaboOrderStatisticUpdateDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Cập nhật chi tiết phiếu labo';
    modalRef.componentInstance.line = Object.assign({}, item);
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, er => { });
  }

}
