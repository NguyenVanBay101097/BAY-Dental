import { Component, OnInit, ViewChild } from '@angular/core';
import { JournalReportService } from '../journal-report.service';
import { JournalReportPaged, JournalReport, JournalReportDetailPaged } from '../journal-report';
import { GridDataResult, PageChangeEvent, GridComponent, CellClickEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { SortDescriptor, aggregateBy } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import * as _ from 'lodash';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-journal-reports-view',
  templateUrl: './journal-reports-view.component.html',
  styleUrls: ['./journal-reports-view.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class JournalReportsViewComponent implements OnInit {

  @ViewChild('grid', { static: true }) grid: GridComponent;
  expandKeys: string[] = [];
  loading = false;
  gridView: GridDataResult;
  skip = 0;
  pageSize = 20;
  sort: SortDescriptor[] = [{
    field: 'name',
    dir: 'asc'
  }];

  search: string;
  searchUpdate = new Subject<string>();

  dateFrom: Date;
  dateTo: Date;
  groupBy = "journal";
  groupBy2 = "month";

  groups: { text: string, value: string }[] = [
    { text: 'Thời gian', value: 'date' },
    { text: 'Tài khoản', value: 'journal' },
  ];
  group2s: {}[] = [
    { text: 'Ngày', value: 'day' },
    { text: 'Tuần', value: 'week' },
    { text: 'Tháng', value: 'month' },
    { text: 'Quý', value: 'quarter' },
  ];

  filters: {}[] = [
    { text: 'Tất cả', value: 'all' },
    { text: 'Ngân hàng', value: 'bank' },
    { text: 'Tiền mặt', value: 'cash' },
  ];

  filter = 'all';

  public total: any;
  public aggregates: any[] = [
    { field: 'debitSum', aggregate: 'sum' },
    { field: 'creditSum', aggregate: 'sum' },
    { field: 'balanceSum', aggregate: 'sum' }
  ];

  constructor(private service: JournalReportService, private intlService: IntlService) { }

  ngOnInit() {
    var today = new Date;
    this.dateFrom = new Date(today.getFullYear(), today.getMonth(), 1);
    this.dateTo = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    this.getReports();
    this.searchChange();
  }

  getReports() {
    this.loading = true;
    var paged = new JournalReportPaged;
    if (this.search)
      paged.search = this.search;
    if (this.dateFrom) {
      paged.dateFrom = this.intlService.formatDate(this.dateFrom, 'd', 'en-US');
    }
    if (this.dateTo) {
      paged.dateTo = this.intlService.formatDate(this.dateTo, 'd', 'en-US');
    }
    paged.groupBy = this.groupBy;
    paged.filter = this.filter;
    // if (this.groupBy2 && this.groupBy == 'date') {
    //   paged.groupBy = paged.groupBy + ":" + this.groupBy2;
    // }
    this.service.getReports(paged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1,
        total: rs1.length
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      console.log(rs2);
      this.total = aggregateBy(rs2.data, this.aggregates);
      this.loading = false;

      console.log(rs2);
    }, er => {
      this.loading = true;
      console.log(er);
    }
    )
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.pageSize = event.take;
    this.getReports();
  }

  sortChange(sort: SortDescriptor[]): void {
    this.sort = sort;
    this.getReports();
  }

  cellClick(e: CellClickEvent) {
    this.grid.expandRow(e.rowIndex);
    var index = this.expandKeys.indexOf(e.dataItem.name);
    if (index == -1) {
      this.expandKeys.push(e.dataItem.name)
      this.grid.expandRow(e.rowIndex);
    } else if (index > -1) {
      this.expandKeys.splice(index, 1);
      this.grid.collapseRow(e.rowIndex);
    }
  }

  searchChange() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.getReports();
      });
  }

  // setGroupBy(groupBy) {
  //   this.groupBy = groupBy;
  //   this.getReports();
  // }
  setGroupBy2(groupBy) {
    this.groupBy2 = groupBy;
    this.getReports();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.getReports();
  }

  itemMap(val: JournalReport) {
    var item = new JournalReportDetailPaged;
    item.journalId = val.journalId;
    item.groupBy = "date:" + this.groupBy2;
    item.dateFrom = this.intlService.formatDate(this.dateFrom, 'd', 'en-US');
    item.dateTo = this.intlService.formatDate(this.dateTo, 'd', 'en-US');
    return item;
  }

  changeFilter(val) {
    this.filter = val;
    this.getReports();
  }
}
