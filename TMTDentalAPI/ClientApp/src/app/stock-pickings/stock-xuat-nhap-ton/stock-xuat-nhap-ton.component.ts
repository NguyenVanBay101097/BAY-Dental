import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ExcelExportData, Workbook } from '@progress/kendo-angular-excel-export';
import { saveAs } from '@progress/kendo-file-saver';
import { aggregateBy, process } from '@progress/kendo-data-query';
import { GridComponent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
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
  limit = 10;
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

  filteredInventory: { text: string, value: string }[] = [
    { text: 'Trên mức tối thiểu', value: 'above_minInventory' },
    { text: 'Dưới mức tối thiểu', value: 'below_minInventory' }
  ];
  minInventoryFilter: string;
  constructor(
    private reportService: StockReportService,
    private intlService: IntlService,
    private modalService: NgbModal,
  ) {
    this.excelData = this.excelData.bind(this);
  }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((val) => {
        this.skip = 0;
        var items = this.items.filter(x => x.productCode.toLowerCase().indexOf(val.toLowerCase()) !== -1 ||
         x.productName.toLowerCase().indexOf(val.toLowerCase()) !== -1);
        this.loadItems2(items);
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
    val.minInventoryFilter = this.minInventoryFilter ? this.minInventoryFilter : null;
    this.reportService.getXuatNhapTonSummary(val).subscribe(res => {
      this.items = res;
      this.computeAggregate();
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
    this.items = this.items.filter(x => x.begin != 0 || x.end != 0 || x.import != 0 || x.export != 0)
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };
  }

  loadItems2(items): void {
    this.gridData = {
      data: items.slice(this.skip, this.skip + this.limit),
      total: items.length
    };

    const result = aggregateBy(items, [
      { aggregate: "sum", field: "begin" },
      { aggregate: "sum", field: "import" },
      { aggregate: "sum", field: "export" },
      { aggregate: "sum", field: "end" },
    ]);

    this.sumBegin = result.begin ? result.begin.sum : 0;
    this.sumImport = result.import ? result.import.sum : 0;
    this.sumExport = result.export ? result.export.sum : 0;
    this.sumEnd = result.end ? result.end.sum : 0;
  }

  cellClick(item: any) {
    const product = this.gridData.data[item.path[1].rowIndex];
    const modalRef = this.modalService.open(StockXuatNhapTonDetailDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Lịch sử nhập - xuất';
    modalRef.componentInstance.productId = product.productId;
    modalRef.componentInstance.dateFrom = product.dateFrom;
    modalRef.componentInstance.dateTo = product.dateTo;
    modalRef.componentInstance.productName = product.productName;
    modalRef.result.then((res) => {

    })
  }

  computeAggregate() {
    const result = aggregateBy(this.items, [
      { aggregate: "sum", field: "begin" },
      { aggregate: "sum", field: "import" },
      { aggregate: "sum", field: "export" },
      { aggregate: "sum", field: "end" },
    ]);
    this.sumBegin = result.begin ? result.begin.sum : 0;
    this.sumImport = result.import ? result.import.sum : 0;
    this.sumExport = result.export ? result.export.sum : 0;
    this.sumEnd = result.end ? result.end.sum : 0;
  }

  inventoryChange(value) {
    this.skip = 0;
    if (value == 'above_minInventory') {
      var items = this.items.filter(x => x.end >= x.minInventory);
      this.loadItems2(items);
    } else if (value == 'below_minInventory') {
      var items = this.items.filter(x => x.end < x.minInventory);
      this.loadItems2(items);
    } else {
      var items = this.items.slice();
      this.loadItems2(items);
    }
  }

  public exportExcelFile(grid: GridComponent) {
    grid.saveAsExcel();
  }

  public excelData(): ExcelExportData {
    const result: ExcelExportData = {
      data: process(this.items, {
        sort: [{ field: 'productCode', dir: 'asc' }]
      }).data
    };
    return result;
  }

  onExcelExport(args) {
    args.preventDefault();
    this.loading = true;
    const workbook = args.workbook;
    const index = workbook.sheets[0].rows.findIndex(x => x.type == 'footer');
    workbook.sheets[0].rows.splice(index, 1);
    new Workbook(workbook).toDataURL().then((dataUrl: string) => {
      saveAs(dataUrl, "NhapXuatTon.xlsx");
      this.loading = false;
    });
  }
}
