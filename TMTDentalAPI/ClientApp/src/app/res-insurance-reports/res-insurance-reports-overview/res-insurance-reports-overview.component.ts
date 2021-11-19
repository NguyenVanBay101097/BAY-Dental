import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-res-insurance-reports-overview',
  templateUrl: './res-insurance-reports-overview.component.html',
  styleUrls: ['./res-insurance-reports-overview.component.css']
})
export class ResInsuranceReportsOverviewComponent implements OnInit, AfterViewInit {
  @ViewChild('companyCbx', { static: false }) companyCbx: ComboBoxComponent;

  gridData: GridDataResult;
  limit: number = 20;
  offset: number = 0;
  pagerSettings: any;
  dateFrom: Date;
  dateTo: Date;
  companyId: string;
  filteredCompanies: any[];
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private companyService: CompanyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit(): void {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.loadCompany();
  }

  ngAfterViewInit(): void {
    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.companyCbx.loading = true),
      switchMap(val => this.searchCompany(val.toString().toLowerCase()))
    ).subscribe(rs => {
      this.filteredCompanies = rs.items;
      this.companyCbx.loading = false;
    });
  }

  searchCompany(search?: string) {
    var params = new CompanyPaged();
    params.limit = 20;
    params.offset = 0;
    params.search = search || '';
    params.active = true;
    return this.companyService.getPaged(params);
  }

  loadCompany(): void {
    this.searchCompany().subscribe(result => {
      this.filteredCompanies = result.items;
    });
  }

  onPageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.limit = event.take;
  }

  changeCompany(e): void {
    console.log(e);
  }

  onSearchDateChange(e): void {
    this.dateFrom = e.dateFrom || '';
    this.dateTo = e.dateTo || '';
  }
}
