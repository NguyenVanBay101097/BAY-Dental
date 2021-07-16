import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
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
  limit: number = 20;
  skip: number = 0;
  dateFrom: string;
  dateTo: string;
  productId: string;
  productName: string;
  constructor(
    public activeModal: NgbActiveModal,
    private stockReportService: StockReportService,
    private intl: IntlService,

  ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new GetStockHistoryReq();
    val.dateFrom = '';
    val.dateTo =  '';
    val.productId = this.productId ? this.productId : '';
    val.limit = this.limit;
    val.offset = this.skip;

    this.stockReportService.getStockHistory(val).pipe(
      map((response: any) => (<GridDataResult>{
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

  pageChange(event:PageChangeEvent) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  exportExcelFile() {
    var val = new GetStockHistoryReq();

    val.dateFrom = this.dateFrom ? this.dateFrom : '';
    val.dateTo = this.dateTo ? this.dateTo : '';
    val.productId = this.productId ? this.productId : '';
    val.limit = this.limit;
    val.offset = this.skip;

    this.stockReportService.excelStockHistoryExport(val).subscribe((res: any) => {
      let filename = `Lịch sử nhập xuất ${this.productName}`;
      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}
