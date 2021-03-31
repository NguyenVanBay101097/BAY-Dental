import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { QuotationService, QuotationsPaged } from '../../quotation.service';
// import { QuotationService, QuotationsPaged } from './quotation.service';

@Component({
  selector: 'app-partner-customer-quotation-list',
  templateUrl: './partner-customer-quotation-list.component.html',
  styleUrls: ['./partner-customer-quotation-list.component.css']
})
export class PartnerCustomerQuotationListComponent implements OnInit {
  partnerId: string;
  gridData: GridDataResult;
  dateFrom: Date;
  dateTo: Date;
  limit: number = 20;
  skip: number = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  selectedIds: any;
  constructor(
    private intlService: IntlService,
    private quotationService: QuotationService,
    private router: Router,
  ) { }

  ngOnInit() {
    this.loading = true;
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.getPaged();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.skip = 0;
        this.getPaged();
      });
  }

  getPaged() {
    this.loading = true;
    var val = new QuotationsPaged();
    val.limt = this.limit;
    val.offset = this.skip;
    val.search = this.search ? this.search : '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59");
    this.quotationService.getPaged(val).pipe(
      map(res => (<GridDataResult>{
        data: res.items,
        total: res.totalItems
      }))
    ).subscribe(
      result => {
        this.gridData = result;
        console.log(this.gridData);
        this.loading = false
      }
    )
  }

  createQuotation() {
  }

  editQuotation() {

  }

  deleteQuotation() {

  }

  pageChange(event) {
    this.skip = event.skip;
    this.getPaged();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.getPaged();
  }
}
