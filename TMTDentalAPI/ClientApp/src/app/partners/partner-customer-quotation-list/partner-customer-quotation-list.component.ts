import { Component, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-partner-customer-quotation-list',
  templateUrl: './partner-customer-quotation-list.component.html',
  styleUrls: ['./partner-customer-quotation-list.component.css']
})
export class PartnerCustomerQuotationListComponent implements OnInit {
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
  constructor() { }

  ngOnInit() {
    // this.loading = true;
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
  }

  createItem() {

  }

  editItem() {

  }

  deleteItem() {

  }
  pageChange() {

  }
  onSearchDateChange(data){
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
  }
}
