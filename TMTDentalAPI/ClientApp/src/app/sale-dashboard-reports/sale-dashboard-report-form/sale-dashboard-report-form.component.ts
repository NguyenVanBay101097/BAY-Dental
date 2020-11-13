import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountCommonPartnerReportSearch, AccountCommonPartnerReportService } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { AccountReportGeneralLedgerService, ReportCashBankGeneralLedger } from 'src/app/account-report-general-ledgers/account-report-general-ledger.service';
import { AuthService } from 'src/app/auth/auth.service';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerCustomerReportInput, PartnerCustomerReportOutput, PartnerService } from 'src/app/partners/partner.service';

@Component({
  selector: 'app-sale-dashboard-report-form',
  templateUrl: './sale-dashboard-report-form.component.html',
  styleUrls: ['./sale-dashboard-report-form.component.css'],
  host: {
    'class': 'w-100 h-100'
  }
})
export class SaleDashboardReportFormComponent implements OnInit {
  @ViewChild('companyCbx', { static: true }) companyCbx: ComboBoxComponent;
  formGroup: FormGroup
  public aggregates: any[] = [
    { field: 'end', aggregate: 'sum' },
  ];
  totalDebitNCC: number;
  customerReport: PartnerCustomerReportOutput;
  filteredCompanies: CompanyBasic[] = [];
  reportLedgerBank: any;
  moneyledger: number;
  moneyBank: number;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public series: any[] = [{
    name: '2019',
    data: [0.010, 0.375, 1.161, 0.684, 3.7, 3.269, 1.083, 5.127, 3.690, 2.995, 3.690, 2.995]
  }, {
    name: '2020',
    data: [1.988, 2.733, 3.994, 3.464, 4.001, 3.939, 1.333, 2.245, 4.339, 2.727, 10.690, 15.995]
  }];
  public categories: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

  constructor(
    private authService: AuthService,
    private partnerService: PartnerService,
    private fb: FormBuilder,
    private reportService: AccountCommonPartnerReportService,
    private companyService: CompanyService,
    private intlService: IntlService,
    private reportGeneralLedgerService: AccountReportGeneralLedgerService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      company: null,
      dataTo: new Date,
      dateForm: null,
    });

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
    this.loadAllData();
  }

  loadAllData() {
    this.loadDebitNCC();
    this.loadDataMoney();
    this.loadPartnerCustomerReport();
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
    return this.companyService.getPaged(params);
  }

  loadDebitNCC() {
    var val = new AccountCommonPartnerReportSearch();
    val.resultSelection = "supplier";
    val.companyId = this.formGroup.get('company').value ? this.formGroup.get('company').value.id : null;

    this.reportService.getSummary(val).subscribe(res => {

      var total = aggregateBy(res, this.aggregates);
      if (total) {
        this.totalDebitNCC = total['end'].sum
      }
    }, err => {
      console.log(err);
    })
  }

  loadDataMoney() {
    var val = new ReportCashBankGeneralLedger();
    val.companyId = this.authService.userInfo.companyId;
    this.reportGeneralLedgerService.getCashBankReport(val).subscribe(result => {
      this.reportLedgerBank = result;
      if (this.reportLedgerBank && this.reportLedgerBank.accounts && this.reportLedgerBank.accounts.length > 0) {
        this.moneyledger = this.reportLedgerBank.accounts[0] ? this.reportLedgerBank.accounts[0].balance : 0;
        this.moneyBank =this.reportLedgerBank.accounts[1] ? this.reportLedgerBank.accounts[1].balance : 0;
      }
    }, err => {
      console.log(err);
    });
  }

  loadPartnerCustomerReport() {
    var val = new PartnerCustomerReportInput();
    this.partnerService.getPartnerCustomerReport(val).subscribe(
      result => {
        this.customerReport = result;
      },
      error => {

      }
    );
  }

}
