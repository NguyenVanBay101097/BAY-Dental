import { Component, OnInit, ViewChild } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { SaleReportItem, SaleReportService, SaleReportSearch, SaleReportPartnerSearch, SaleReportPartnerItem, SaleReportPartnerItemV3, SaleReportPartnerSearchV3 } from '../sale-report.service';
import * as _ from 'lodash';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { PartnerOldNewReport, PartnerOldNewReportSearch, PartnerOldNewReportService } from '../partner-old-new-report.service';

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
  items: PartnerOldNewReport[] = [];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  dateFrom: Date;
  dateTo: Date;
  listCompanies: CompanyBasic[] = [];
  companyFilter: CompanyBasic
  public total: any;
  public aggregates: any[] = [
    { field: 'orderCount', aggregate: 'sum' },
  ];


  constructor(
    private intlService: IntlService,
    private partnerOldNewReportService: PartnerOldNewReportService,
    private companyService: CompanyService
  ) {
  }

  ngOnInit() {
    this.dateFrom = new Date(this.monthStart);
    this.dateTo = new Date(this.monthEnd);
    this.loadDataFromApi();
    this.loadCompanies();
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
    var val = new PartnerOldNewReportSearch();
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59");
    val.companyId = this.companyFilter ? this.companyFilter.id : null;
    this.loading = true;
    this.partnerOldNewReportService.getPartnerOldNewReport(val).subscribe(result => {
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

}

