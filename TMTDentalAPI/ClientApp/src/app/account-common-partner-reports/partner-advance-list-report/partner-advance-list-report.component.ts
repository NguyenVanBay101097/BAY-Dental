import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-partner-advance-list-report',
  templateUrl: './partner-advance-list-report.component.html',
  styleUrls: ['./partner-advance-list-report.component.css']
})
export class PartnerAdvanceListReportComponent implements OnInit {
  loading = false;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  dateFrom: Date;
  dateTo: Date;
  companies: CompanySimple[] = [];
  companyId: string;

  search: string = '';
  searchUpdate = new Subject<string>();


  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;

  constructor(
    private intlService: IntlService,
    private companyService: CompanyService,

    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }


  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe(() => {
        // this.loadDataFromApi();
      });

    // this.loadDataFromApi();
    // this.loadCompanies();
    this.companyVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.companyVC.loading = true)),
        switchMap((value) => this.searchCompany$(value))
      )
      .subscribe((x) => {
        this.companies = x.items;
        this.companyVC.loading = false;
      });
    this.loadCompanies();

  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }

  onSelectCompany(e) {
    this.companyId = e ? e.id : null;
    // this.loadDataFromApi();
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  onExcelExport(args: any) {

  }

  exportExcel(){
    
  }

  printReport() {

  }

  searchChangeDate(value: any) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    // this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    // this.loadItems();
  }
}
