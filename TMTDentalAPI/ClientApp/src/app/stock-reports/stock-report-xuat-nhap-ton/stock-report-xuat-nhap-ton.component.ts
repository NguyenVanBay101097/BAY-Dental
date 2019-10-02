import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { StockReportXuatNhapTonItem, StockReportService, StockReportXuatNhapTonSearch } from '../stock-report.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductCategoryBasic, ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';
import { ProductService, ProductPaged, ProductFilter } from 'src/app/products/product.service';


@Component({
  selector: 'app-stock-report-xuat-nhap-ton',
  templateUrl: './stock-report-xuat-nhap-ton.component.html',
  styleUrls: ['./stock-report-xuat-nhap-ton.component.css']
})

export class StockReportXuatNhapTonComponent implements OnInit {

  loading = false;
  items: StockReportXuatNhapTonItem[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  searchProduct: ProductSimple;
  searchCateg: ProductCategoryBasic;

  filteredProducts: ProductSimple[];
  filteredCategs: ProductCategoryBasic[];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;

  constructor(private reportService: StockReportService, private intlService: IntlService,
    private productService: ProductService, private productCategService: ProductCategoryService) { }

  ngOnInit() {
    var date = new Date();
    this.dateFrom = new Date(date.getFullYear(), date.getMonth(), 1);
    this.dateTo = new Date(date.getFullYear(), date.getMonth(), date.getDate());
    this.loadDataFromApi();

    this.productCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productCbx.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.filteredProducts = result;
      this.productCbx.loading = false;
    });

    this.categCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.categCbx.loading = true)),
      switchMap(value => this.searchCategs(value))
    ).subscribe(result => {
      this.filteredCategs = result;
      this.categCbx.loading = false;
    });
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
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'd', 'en-US') : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'd', 'en-US') : null;
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
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };
  }
}
