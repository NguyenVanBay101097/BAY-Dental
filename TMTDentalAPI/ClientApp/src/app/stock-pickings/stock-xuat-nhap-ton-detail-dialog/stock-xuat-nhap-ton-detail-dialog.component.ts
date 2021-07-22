import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GridComponent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { map } from 'rxjs/operators';
import { GetStockHistoryReq, StockReportService } from 'src/app/stock-reports/stock-report.service';

@Component({
  selector: 'app-stock-xuat-nhap-ton-detail-dialog',
  templateUrl: './stock-xuat-nhap-ton-detail-dialog.component.html',
  styleUrls: ['./stock-xuat-nhap-ton-detail-dialog.component.css']
})
export class StockXuatNhapTonDetailDialogComponent implements OnInit {
  title: string;
  gridData: GridDataResult;
  loading: boolean = false;
  limit: number = 5;
  skip: number = 0;
  item: any;
  allGridData: GridDataResult;
  fileExcelName: string;
  constructor(
    public activeModal: NgbActiveModal,
    private stockReportService: StockReportService,
    private intlService: IntlService,

  ) {
    this.allData = this.allData.bind(this);
  }

  ngOnInit() {
    this.loadDataFromApi();
    this.fileExcelName = 'Lich Su Nhap Xuat ' + this.item.productName;
  }

  loadDataFromApi() {
    var val = new GetStockHistoryReq();
    val.dateFrom = this.item.dateFrom || '';
    val.dateTo = this.item.dateTo || '';
    val.productId = this.item.productId || '';
    val.companyId = this.item.companyId || '';
    val.limit = this.limit;
    val.offset = this.skip;

    this.stockReportService.getStockHistory(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items.map(x => {
          return {
            ...x,
            date: new Date(x.date)
          }
        }),
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

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  public allData = (): any => {
    var val = new GetStockHistoryReq();
    val.dateFrom = this.item.dateFrom || '';
    val.dateTo = this.item.dateTo || '';
    val.productId = this.item.productId || '';
    val.companyId = this.item.companyId || '';
    val.limit = 0;
    val.offset = 0;

    var observable = this.stockReportService.getStockHistory(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items.map(x => {
          return {
            ...x,
            date: new Date(x.date)
          }
        }),
        total: response.totalItems
      }))
    );

    return observable;
  }

  exportExcelFile(grid: GridComponent) {
    grid.saveAsExcel();
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}
