import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { CustomerReceiptReportService, CustomerReceiptTimeDetailFilter } from '../customer-receipt-report.service';

@Component({
  selector: 'app-customer-receipt-report-for-time-detail',
  templateUrl: './customer-receipt-report-for-time-detail.component.html',
  styleUrls: ['./customer-receipt-report-for-time-detail.component.css']
})
export class CustomerReceiptReportForTimeDetailComponent implements OnInit {
  @Input() dateFrom: Date;
  @Input() dateTo: Date;
  @Input() companyId: string;
  @Input() itemTime: any;
  loading = false;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  total: number;
  gridData: GridDataResult;
  customerReceiptTimes: any[] = [];
  searchUpdate = new Subject<string>();
  search: string;
  state: string;
  isExamination: string;
  isNotTreatment: string;

  constructor(
    private customerReceiptReportService: CustomerReceiptReportService,
    private intlService: IntlService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataApi();
      });

    this.loadDataApi();

  }

  loadDataApi() {
    this.loading = true;
    var val = new CustomerReceiptTimeDetailFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.companyId = this.companyId || '';
    val.time = this.itemTime.time;
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    this.customerReceiptReportService.getCustomerReceiptForTimeDetailPaged(val).pipe(
      map((response) => <GridDataResult>{
        data: response.items,
        total: response.totalItems,
      }
      )
    ).subscribe(
      (res) => {
        this.customerReceiptTimes = res.data;
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

  getExamination(value) {
    switch (value) {
      case true:
        return 'T??i kh??m';
      case false:
        return 'Kh??m m???i';
    }
  }

  getMinute(value) {
    if(value == null || value == undefined){
      return '';
    }
    return `${value} ph??t`;
  }

  getState(value) {
    switch (value) {
      case 'examination':
        return '??ang kh??m';
      case 'done':
        return 'Ho??n th??nh';
      default:
        return 'Ch??? kh??m';
    }
  }

  getTreatment(value) {
    switch (value) {
      case true:
        return 'Kh??ng ??i???u tr???';
      case false:
        return 'C?? ??i???u tr???';
    }
  }
}
