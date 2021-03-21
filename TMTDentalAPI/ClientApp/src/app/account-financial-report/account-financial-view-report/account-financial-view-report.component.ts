import { Component, OnInit, ViewChild } from '@angular/core';
import { AccountFinancialReportService, AccountFinancialReportBasic } from '../account-financial-report.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { CompanyService, CompanyBasic, CompanyPaged } from 'src/app/companies/company.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';
import { AccountLineItem, AccoutingReport, ReportFinancialService } from '../report-financial.service';

@Component({
  selector: 'app-account-financial-view-report',
  templateUrl: './account-financial-view-report.component.html',
  styleUrls: ['./account-financial-view-report.component.css']
})
export class AccountFinancialViewReportComponent implements OnInit {
  @ViewChild('companyCbx', { static: true }) companyCbx: ComboBoxComponent;
  accountLines: AccountLineItem[] = []
  formGroup: FormGroup;
  accountFinancialReportBasic: AccountFinancialReportBasic = new AccountFinancialReportBasic();
  filteredCompanies: CompanyBasic[] = [];
  companyId: string;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private reportFinancialService: ReportFinancialService,
    private accountFinancialReportService: AccountFinancialReportService,
    private fb: FormBuilder,
    private companyService: CompanyService,
    private intl: IntlService
  ) { }
  ngOnInit() {
    this.formGroup = this.fb.group({
      dateTo: this.monthEnd,
      dateFrom: this.monthStart,
      debitCredit: true,
      company: null
    })

    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.companyCbx.loading = true),
      switchMap(val => this.searchCompany(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.filteredCompanies = rs.items;
        this.companyCbx.loading = false;
      });

    this.loadCompany();
    this.getDefault();
  }

  getDefault() {
    this.accountFinancialReportService.getProfitAndLossReport().subscribe(
      result => {
        this.accountFinancialReportBasic = result;
        this.reportFinancial();
      }
    )
  }

  loadCompany() {
    this.searchCompany().subscribe(
      result => {
        this.filteredCompanies = result.items;
      }
    )
  }

  searchCompany(search?: string) {
    var params = new CompanyPaged();
    params.limit = 20;
    params.offset = 0;
    params.search = search || '';
    params.active = true;
    return this.companyService.getPaged(params);
  }

  reportFinancial() {
    if (this.formGroup.invalid) {
      return false;
    }
    var val = new AccoutingReport();
    val.companyId = this.formGroup.get('company') && this.formGroup.get('company').value ? this.formGroup.get('company').value.id : null;
    val.dateFrom = this.intl.formatDate(this.formGroup.get('dateFrom') ? this.formGroup.get('dateFrom').value : null, "yyyy-MM-dd");
    val.dateTo = this.intl.formatDate(this.formGroup.get('dateTo') ? this.formGroup.get('dateTo').value : null, "yyyy-MM-dd");
    val.debitCredit = this.formGroup.get('debitCredit') ? this.formGroup.get('debitCredit').value : false;
    if (this.accountFinancialReportBasic)
      val.accountReportId = this.accountFinancialReportBasic.id;
    this.reportFinancialService.getAccountLinesItem(val).subscribe(
      result => {
        this.accountLines = result;
      }
    )
  }
}
