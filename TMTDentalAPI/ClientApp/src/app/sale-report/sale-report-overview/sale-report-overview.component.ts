import { Component, OnInit, ViewChild } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { SaleReportItem, SaleReportService, SaleReportSearch } from '../sale-report.service';
import * as _ from 'lodash';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';

@Component({
  selector: 'app-sale-report-overview',
  templateUrl: './sale-report-overview.component.html',
  styleUrls: ['./sale-report-overview.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleReportOverviewComponent implements OnInit {
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  items: SaleReportItem[] = [];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  groupBy = "date";
  groupBy2 = "month";
  search: string;
  searchUpdate = new Subject<string>();
  isQuotation = false;
  viewType = 'list';

  groups: { text: string, value: string }[] = [
    { text: 'Ngày', value: 'date' },
    { text: 'Khách hàng', value: 'customer' },
    { text: 'Nhân viên', value: 'employee' },
    { text: 'Dịch vụ', value: 'product' },
  ];

  group2s: {}[] = [
    { text: 'Ngày', value: 'day' },
    { text: 'Tuần', value: 'week' },
    { text: 'Tháng', value: 'month' },
    { text: 'Quý', value: 'quarter' },
  ];

  states: {}[] = [
    { text: 'Tất cả trạng thái', value: '' },
    { text: 'Đơn hàng', value: 'sale,done' },
    { text: 'Nháp', value: 'draft,cancel' },
  ];

  public total: any;
  public aggregates: any[] = [
    { field: 'productUOMQty', aggregate: 'sum' },
    { field: 'priceTotal', aggregate: 'sum' },
  ];

  listCompanies: CompanyBasic[] = [];
  companyFilter: CompanyBasic;

  constructor(private intlService: IntlService, private saleReportService: SaleReportService, private companyService: CompanyService) {
  }

  ngOnInit() {
    this.dateFrom = new Date(this.monthStart);
    this.dateTo = new Date(this.monthEnd);
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadCompanies();
  }

  getTitle() {
    var item = _.find(this.groups, o => o.value == this.groupBy);
    return item.text;
  }

  setViewType(type) {
    this.viewType = type;
  }

  loadCompanies() {
    var val = new CompanyPaged();
    val.active = true;
    this.companyService.getPaged(val)
      .subscribe(res => {
        this.listCompanies = res.items;
      })
  }

  changeCompany(event) {
    this.companyFilter = event;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new SaleReportSearch();
    val.isQuotation = this.isQuotation;
    val.state = "sale,done";

    if (this.dateFrom) {
      val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    }
    if (this.dateTo) {
      val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    }
    if (this.search) {
      val.search = this.search;
    }
    val.groupBy = this.groupBy;
    if (this.groupBy2 && this.groupBy == 'date') {
      val.groupBy = val.groupBy + ":" + this.groupBy2;
    }

    val.companyId = this.companyFilter ? this.companyFilter.id : null;

    this.loading = true;
    this.saleReportService.getReport(val).subscribe(result => {
      this.items = result;
      this.total = aggregateBy(this.items, this.aggregates);
      this.loadItems();
      this.loading = false;
    }, () => {
      this.loading = false;
    });
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

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
  }

  setGroupBy(groupBy) {
    this.groupBy = groupBy;
    this.loadDataFromApi();
  }

  setGroupBy2(groupBy) {
    this.groupBy2 = groupBy;
    this.loadDataFromApi();
  }
}

