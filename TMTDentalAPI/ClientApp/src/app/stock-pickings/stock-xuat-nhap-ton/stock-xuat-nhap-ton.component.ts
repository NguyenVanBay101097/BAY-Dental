import { Component, Inject, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import { ExcelExportData, Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { saveAs } from '@progress/kendo-file-saver';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/internal/operators/debounceTime';
import { distinctUntilChanged } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
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
  excelDataExport: ExcelExportData;
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

  showOrigin = false;
  showExpiry = false;
  showAverageExport = false;
  showMinInventory = false;
  isExpand = false;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  sumBegin: number = 0;
  sumEnd: number = 0;
  inventoryValue: string = '';
  excelItems: any[]=[];
  sumImport: number = 0;
  sumExport: number = 0;

  filteredInventory: { text: string, value: string }[] = [
    { text: 'Trên mức tối thiểu', value: 'above_minInventory' },
    { text: 'Dưới mức tối thiểu', value: 'below_minInventory' }
  ];
  minInventoryFilter: string;

  clickedRowItem: any;

  constructor(
    private reportService: StockReportService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private authService: AuthService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { 
    this.pagerSettings = config.pagerSettings 
    // this.excelData = this.excelData.bind(this);
    this.allData = this.allData.bind(this);
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

    this.getLocalStorge();
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
    val.companyId = this.authService.userInfo.companyId;
    this.reportService.getXuatNhapTonSummary(val).subscribe(res => {
      this.items = res;
      this.excelItems = res;
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
    this.limit = event.take;
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
    this.excelItems = items;
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

  getLocalStorge(){
    let columns = localStorage.getItem('stock_report_columns');
    if (columns){
      this.showAverageExport = columns.indexOf('averageExport') != -1;
      this.showOrigin = columns.indexOf('origin') != -1;
      this.showMinInventory = columns.indexOf('minInventory') != -1;
      this.showExpiry = columns.indexOf('expiry') != -1;
    }
  }

  setLocalStorge(columns: any[]){
    if (this.showAverageExport) 
      columns.push("averageExport");
    if (this.showExpiry) 
      columns.push("expiry");  
    if (this.showMinInventory) 
      columns.push("minInventory");
    if (this.showOrigin) 
      columns.push("origin");
    localStorage.setItem('stock_report_columns',columns.toString());
  }

  onCellClick(e) {
    this.clickedRowItem = e.dataItem;
  }

  onDblClick() {
    const product = this.clickedRowItem;
    const modalRef = this.modalService.open(StockXuatNhapTonDetailDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Lịch sử nhập - xuất';
    modalRef.componentInstance.item = product;
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

  getDataAfterInventoryChange(value){
    var items = [];
    if (value == 'above_minInventory') {
      items = this.items.filter(x => x.end >= x.minInventory);
    } else if (value == 'below_minInventory') {
      items = this.items.filter(x => x.end < x.minInventory);
    } else {
      items = this.items.slice();
    }

    return items;
  }

  inventoryChange(value) {
    this.skip = 0;
    this.inventoryValue = value;
    var items = this.getDataAfterInventoryChange(value);
    this.loadItems2(items);
  }

  public exportExcelFile(grid: GridComponent) {
    grid.saveAsExcel();
  }

  public allData(): ExcelExportData {
    const result: ExcelExportData = {
      data: this.excelItems
    }
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

  apply(form: NgForm){
    this.isExpand = false;
    this.showAverageExport = form.controls["averageExport"].value;
    this.showExpiry = form.controls["expiry"].value;
    this.showMinInventory = form.controls["minInventory"].value;
    this.showOrigin = form.controls["origin"].value;
    let columns = [];
    this.setLocalStorge(columns);
  }
}
