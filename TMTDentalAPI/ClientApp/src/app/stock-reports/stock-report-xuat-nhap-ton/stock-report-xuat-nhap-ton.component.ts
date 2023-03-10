import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { StockReportService, StockReportXuatNhapTonItem, StockReportXuatNhapTonSearch } from '../stock-report.service';


@Component({
  selector: 'app-stock-report-xuat-nhap-ton',
  templateUrl: './stock-report-xuat-nhap-ton.component.html',
  styleUrls: ['./stock-report-xuat-nhap-ton.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})

export class StockReportXuatNhapTonComponent implements OnInit {

  loading = false;
  items: StockReportXuatNhapTonItem[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  dateFrom: Date;
  dateTo: Date;
  searchProduct: ProductSimple;
  searchCateg: ProductCategoryBasic;

  search: string;
  searchUpdate = new Subject<string>();

  filteredProducts: ProductSimple[];
  filteredCategs: ProductCategoryBasic[];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());

  constructor(private reportService: StockReportService, private intlService: IntlService,
    private productService: ProductService, private productCategService: ProductCategoryService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.dateFrom = new Date(this.monthStart);
    this.dateTo = new Date(this.monthEnd);
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });

    // this.productCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.productCbx.loading = true)),
    //   switchMap(value => this.searchProducts(value))
    // ).subscribe(result => {
    //   this.filteredProducts = result;
    //   this.productCbx.loading = false;
    // });

    // this.categCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.categCbx.loading = true)),
    //   switchMap(value => this.searchCategs(value))
    // ).subscribe(result => {
    //   this.filteredCategs = result;
    //   this.categCbx.loading = false;
    // });
  }

  onSearchChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
  }

  onChangeDate(value: any) {
    setTimeout(() => {
      this.loadDataFromApi();
    }, 200);
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.search = search;
    val.type = "product";
    return this.productService.autocomplete2(val);
  }

  searchCategs(search?: string) {
    var val = new ProductCategoryPaged();
    val.search = search;
    return this.productCategService.autocomplete(val);
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new StockReportXuatNhapTonSearch();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.productId = this.searchProduct ? this.searchProduct.id : null;
    val.productCategId = this.searchCateg ? this.searchCateg.id : null;

    this.reportService.getXuatNhapTonSummary(val).subscribe(res => {
      this.items = res;
      this.loadItems();
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };
  }
}
