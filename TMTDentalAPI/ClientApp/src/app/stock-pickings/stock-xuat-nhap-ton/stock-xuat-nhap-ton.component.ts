import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/internal/operators/debounceTime';
import { distinctUntilChanged } from 'rxjs/operators';
import { ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { StockReportService, StockReportXuatNhapTonItem, StockReportXuatNhapTonSearch } from 'src/app/stock-reports/stock-report.service';
import { StockXuatNhapTonDetailDialogComponent } from '../stock-xuat-nhap-ton-detail-dialog/stock-xuat-nhap-ton-detail-dialog.component';

@Component({
  selector: 'app-stock-xuat-nhap-ton',
  templateUrl: './stock-xuat-nhap-ton.component.html',
  styleUrls: ['./stock-xuat-nhap-ton.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class StockXuatNhapTonComponent implements OnInit {

  loading = false;
  items: StockReportXuatNhapTonItem[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  searchProduct: ProductSimple;
  searchCateg: ProductCategoryBasic;

  search: string;
  searchUpdate = new Subject<string>();

  filteredProducts: ProductSimple[];
  filteredCategs: ProductCategoryBasic[];

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  sumBegin: number = 0;
  sumEnd: number = 0;
  sumImport: number = 0;
  sumExport: number = 0;
  constructor(
    private reportService: StockReportService,
    private intlService: IntlService,
    private modalService: NgbModal,
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  onSearchChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new StockReportXuatNhapTonSearch();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.productId = this.searchProduct ? this.searchProduct.id : null;
    val.productCategId = this.searchCateg ? this.searchCateg.id : null;
    val.search = this.search ? this.search : null;

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

    this.sumBegin = this.gridData.data.map(val => val.begin).reduce((accumulator, currentValue) => {
      return accumulator + currentValue;
    }, 0);

    this.sumEnd = this.gridData.data.map(val => val.end).reduce((accumulator, currentValue) => {
      return accumulator + currentValue;
    }, 0);

    this.sumImport = this.gridData.data.map(val => val.import).reduce((accumulator, currentValue) => {
      return accumulator + currentValue;
    }, 0);

    this.sumExport = this.gridData.data.map(val => val.export).reduce((accumulator, currentValue) => {
      return accumulator + currentValue;
    }, 0);
  }

  cellClick(item: any) {
    const modalRef = this.modalService.open(StockXuatNhapTonDetailDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Lịch sử nhập xuất ' + item.productName;
    modalRef.componentInstance.productId = item.productId;
    modalRef.componentInstance.dateFrom = item.dateFrom;
    modalRef.componentInstance.dateTo = item.dateTo;
    modalRef.result.then((res) => {

    })
  }

  exportExcelFile() {
    var val = new StockReportXuatNhapTonSearch();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.productId = this.searchProduct ? this.searchProduct.id : null;
    val.productCategId = this.searchCateg ? this.searchCateg.id : null;
    val.search = this.search ? this.search : null;
    this.reportService.exportExcel(val).subscribe(
      rs => {
        let filename = 'NhapXuatTon';
        let newBlob = new Blob([rs], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        console.log(rs);

        let data = window.URL.createObjectURL(newBlob);
        let link = document.createElement('a');
        link.href = data;
        link.download = filename;
        link.click();
        setTimeout(() => {
          // For Firefox it is necessary to delay revoking the ObjectURL
          window.URL.revokeObjectURL(data);
        }, 100);
      }
    );
  }
}
