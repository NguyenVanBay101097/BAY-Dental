import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { AccountCommonPartnerReportOverviewFilter, AccountCommonPartnerReportService } from '../account-common-partner-report.service';
import { PartnerReportSourceComponent } from '../partner-report-source/partner-report-source.component';

@Component({
  selector: 'app-partner-report-overview',
  templateUrl: './partner-report-overview.component.html',
  styleUrls: ['./partner-report-overview.component.css']
})
export class PartnerReportOverviewComponent implements OnInit, AfterViewInit {
  @ViewChild('servicesComp', { static: false }) reportSourceComp: PartnerReportSourceComponent;
  @ViewChild("companyCbx", { static: true }) companyCbx: ComboBoxComponent;

  revenueExpect: { text: string, value: boolean }[] = [
    { text: 'Có dự kiến thu', value: true },
    { text: 'Không có dự kiến thu', value: false }
  ];

  totalDebits: { text: string, value: boolean }[] = [
    { text: 'Có công nợ', value: true },
    { text: 'Không có công nợ', value: false }
  ];

  orderStates: { text: string, value: string }[] = [
    { text: 'Chưa phát sinh', value: 'draft' },
    { text: 'Đang điều trị', value: 'sale' },
    { text: 'Hoàn thành', value: 'done' }
  ];

  reportSumary: any;
  reportSource: any;
  companies: CompanySimple[] = [];
  filter = new AccountCommonPartnerReportOverviewFilter();
  constructor(
    private companyService: CompanyService,
    private accountCommonPartnerReportService: AccountCommonPartnerReportService
  ) { }

  ngOnInit() {
    this.loadAllData();
    this.loadCompanies();

  }

  ngAfterViewInit(): void {
    this.filterCompanyCbx();
  }

  filterCompanyCbx() {
    this.companyCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.companyCbx.loading = true)),
        switchMap((value) => this.searchCompany$(value)
        )
      )
      .subscribe((x) => {
        this.companies = x.items;
        this.companyCbx.loading = false;
      });
  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  onSelectCompany(e) {
    this.filter.companyId = e ? e.id : null;
    this.loadAllData();
  }

  loadReportSumary() {
    let val = Object.assign({}, this.filter) as AccountCommonPartnerReportOverviewFilter;
    this.accountCommonPartnerReportService.getPartnerReportSumaryOverview(val).subscribe((res: any) => {
      this.reportSumary = res;
    }, error => console.log(error))
  }

  loadAllData() {
    setTimeout(() => {
      this.loadReportSumary();
      this.reportSourceComp?.loadReportSource();
    }, 0);
  }

  onSelectOrderStates(e) {
    this.filter.orderState = e ? e.value : null;
    this.loadAllData();
  }

  onSelectTotalDebits(e) {
    this.filter.isDebt = e ? e.value : null;
    this.loadAllData();
  }

  onSelectOrderResiduals(e) {
    this.filter.isRevenueExpect = e ? e.value : null;
    this.loadAllData();
  }

}
