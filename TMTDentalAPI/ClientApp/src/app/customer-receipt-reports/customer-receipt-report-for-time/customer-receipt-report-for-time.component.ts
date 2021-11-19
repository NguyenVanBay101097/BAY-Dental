import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { ChartDataset, ChartOptions } from 'chart.js';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { CustomerReceiptReportFilter, CustomerReceiptReportService } from '../customer-receipt-report.service';

@Component({
  selector: 'app-customer-receipt-report-for-time',
  templateUrl: './customer-receipt-report-for-time.component.html',
  styleUrls: ['./customer-receipt-report-for-time.component.css']
})
export class CustomerReceiptReportForTimeComponent implements OnInit {
  loading = false;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  total: number;
  gridData: GridDataResult;
  customerReceiptTimes: any[] = [];
  listCompany: CompanySimple[] = [];
  searchUpdate = new Subject<string>();
  search: string;
  dateFrom: Date;
  dateTo: Date;
  public today: Date = new Date(new Date().toDateString());
  companyId: string;
  barChartLabels: string[] = [];
  public barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      title: {
        text: 'BÁO CÁO THEO GIỜ TIẾP NHẬN',
        display: true,
        font: {
          size: 16
        }
      },
      legend: {
        display: false
      },
      tooltip: {
        mode: 'index'
      }
    },
  };

  public barChartData: ChartDataset[] = [
    {
      label: 'Số lượng',
      data: [],
      backgroundColor: 'rgba(35, 149, 255, 1)',
      hoverBackgroundColor: 'rgba(35, 149, 255, 0.8)',
      hoverBorderColor: 'rgba(35, 149, 255, 1)'
    },
  ];
  @ViewChild("companyCbx", { static: true }) companyCbx: ComboBoxComponent;

  constructor(
    private customerReceiptReportService: CustomerReceiptReportService,
    private intlService: IntlService,
    private companyService: CompanyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.dateFrom = this.today;
    this.dateTo = this.today;

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataApi();
      });

    this.loadCompanies();

    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.companyCbx.loading = true)),
      switchMap((value) => this.searchCompany(value)
      )
    )
      .subscribe((x) => {
        this.listCompany = x.items;
        this.companyCbx.loading = false;
      });

    this.loadDataApi();
  }

  getDataApiParam() {
    var val = new CustomerReceiptReportFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.companyId = this.companyId || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    return val;
  }

  loadDataApi() {
    this.loading = true;
    var val = this.getDataApiParam();
    this.customerReceiptReportService.getCustomerReceiptTimePaged(val).pipe(
      map((response) => <GridDataResult>{
        data: response.items,
        total: response.totalItems,
      }
      )
    ).subscribe(
      (res) => {
        this.customerReceiptTimes = res.data;
        this.barChartLabels = res.data.map(x => x.timeRange);
        this.barChartData[0].data = res.data.map(x => x.timeRangeCount);
        this.gridData = res;
        this.total = res.total;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataApi();
  }

  onSelectCompany(e) {
    this.companyId = e ? e.id : null;
    this.skip = 0;
    this.loadDataApi();
  }



  searchCompany(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  loadCompanies() {
    this.searchCompany().subscribe(res => {
      this.listCompany = res.items;
    });
  }


  getExamination(value) {
    switch (value) {
      case true:
        return 'Tái khám';
      case false:
        return 'Khám mới';
    }
  }


  onExcelExport() {
    var val = new CustomerReceiptReportFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.companyId = this.companyId || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    this.customerReceiptReportService.exportExcelReportForTime(val).subscribe((res: any) => {
      let filename = "BaoCaoTiepNhan_TheoGioTiepNhan";
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

  onExportPDF() {
    var val = this.getDataApiParam();
    this.loading = true;
    this.customerReceiptReportService.reportPagedForTimePdf(val).subscribe(res => {
      this.loading = false;
      let filename = "TiepNhanTongQuan";

      let newBlob = new Blob([res], {
        type:
          "application/pdf",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

}
