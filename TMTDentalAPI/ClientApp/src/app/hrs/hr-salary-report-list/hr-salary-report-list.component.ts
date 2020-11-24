import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AccountCommonPartnerReportItem, AccountCommonPartnerReportSearch, AccountCommonPartnerReportService } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';

@Component({
  selector: 'app-hr-salary-report-list',
  templateUrl: './hr-salary-report-list.component.html',
  styleUrls: ['./hr-salary-report-list.component.css']
})
export class HrSalaryReportListComponent implements OnInit {
  loading = false;
  items: AccountCommonPartnerReportItem[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  searchPartner: PartnerSimple;
  resultSelection: string;

  search: string;
  searchUpdate = new Subject<string>();

  public total: any;
  public aggregates: any[] = [
    { field: 'end', aggregate: 'sum' },
  ];

  filteredPartners: PartnerSimple[];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;

  constructor(private reportService: AccountCommonPartnerReportService, private intlService: IntlService,
    private partnerService: PartnerService, private route: ActivatedRoute) { }

  ngOnInit() {
    var date = new Date();
    this.dateFrom = new Date(date.getFullYear(), date.getMonth(), 1);
    this.dateTo = new Date(date.getFullYear(), date.getMonth(), date.getDate());

    this.route.queryParamMap.subscribe(params => {
      this.resultSelection = params.get('result_selection');
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    // this.partnerCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.partnerCbx.loading = true)),
    //   switchMap(value => this.searchPartners(value))
    // ).subscribe(result => {
    //   this.filteredPartners = result;
    //   this.partnerCbx.loading = false;
    // });
  }

  onChangeDate(value: any) {
    setTimeout(() => {
      this.loadDataFromApi();
    }, 200);
  }

  searchPartners(search?: string) {
    var val = new PartnerPaged();
    val.customer = true;
    val.search = search;
    return this.partnerService.getAutocompleteSimple(val);
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AccountCommonPartnerReportSearch();
    val.fromDate = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.toDate = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.partnerId = this.searchPartner ? this.searchPartner.id : null;
    if (this.search) {
      val.search = this.search;
    }
    this.reportService.getReportSalaryEmployee(val).subscribe(res => {
      console.log(res);
      this.items = res;
      this.total = aggregateBy(this.items, this.aggregates);
      this.loadItems();
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
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
}
