import { Component, OnInit, ViewChild } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { SaleReportItem, SaleReportService, SaleReportSearch, SaleReportPartnerSearch, SaleReportPartnerItem } from '../sale-report.service';
import * as _ from 'lodash';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-sale-report-partner',
  templateUrl: './sale-report-partner.component.html',
  styleUrls: ['./sale-report-partner.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})

export class SaleReportPartnerComponent implements OnInit {
  loading = false;
  items: SaleReportPartnerItem[] = [];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  partnerDisplay = "all";
  search: string;
  searchUpdate = new Subject<string>();
  monthsFrom: number;
  monthsTo: number;

  groups: { text: string, value: string }[] = [
    { text: 'Tất cả', value: 'all' },
    { text: 'Khách mới', value: 'new' },
    { text: 'Khách cũ', value: 'old' },
  ];

  public total: any;
  public aggregates: any[] = [
    { field: 'orderCount', aggregate: 'sum' },
  ];


  constructor(private intlService: IntlService, private saleReportService: SaleReportService) {
  }

  ngOnInit() {
    // this.dateFrom = new Date(this.monthStart);
    // this.dateTo = new Date(this.monthEnd);
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    var val = new SaleReportPartnerSearch();
    val.state = "sale,done";
    val.partnerDisplay = this.partnerDisplay;
    val.monthsFrom = this.monthsFrom;
    val.monthsTo = this.monthsTo;
    val.search = this.search || '';
    this.loading = true;
    this.saleReportService.getReportPartner(val).subscribe(result => {
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
    this.monthsFrom = data.monthsFrom;
    this.monthsTo = data.monthsTo;
    this.loadDataFromApi();
  }

  setGroupBy(groupBy) {
    this.partnerDisplay = groupBy;
    this.loadDataFromApi();
  }
}

