import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountCommonPartnerReportSearch, AccountCommonPartnerReportService } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { AccountFinancialReportBasic, AccountFinancialReportService } from 'src/app/account-financial-report/account-financial-report.service';
import { AccoutingReport, ReportFinancialService } from 'src/app/account-financial-report/report-financial.service';
import { AccountReportGeneralLedgerService, ReportCashBankGeneralLedger } from 'src/app/account-report-general-ledgers/account-report-general-ledger.service';
import { AuthService } from 'src/app/auth/auth.service';
import { CommissionSettlementReport, CommissionSettlementReportOutput, CommissionSettlementsService } from 'src/app/commission-settlements/commission-settlements.service';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerCustomerReportInput, PartnerCustomerReportOutput, PartnerService } from 'src/app/partners/partner.service';
import { PhieuThuChiSearch, PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { RevenueReportSearch, RevenueReportService } from 'src/app/revenue-report/revenue-report.service';

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
    { field: 'end', aggregate: 'sum' }, { field: 'credit', aggregate: 'sum' }
  ];

  public aggregatesThuChi: any[] = [
    { field: 'amount', aggregate: 'sum' }
  ];

  public aggregatesCommissionSettlement: any[] = [
    { field: 'amount', aggregate: 'sum' }
  ];

  totalDebitNCC: number;
  totalDebitNCCByMonth: number;
  totalCreditNCCByMonth: number;
  accountFinancialReportBasic: AccountFinancialReportBasic = new AccountFinancialReportBasic();
  totalDebitCustomer: number;
  totalDoanhThu: number;
  totalChiPhi: number;
  totalThu: number;
  totalChi: number;
  customerReport: PartnerCustomerReportOutput;
  filteredCompanies: CompanyBasic[] = [];
  totalHoaHong: number;
  reportLedgerBank: any;
  moneyledger: number;
  moneyBank: number;
  public reportCurrentYear: any[] = [];
  public reportOldYear: any[] = [];
  currentYear = new Date().getFullYear();
  oldYear = this.currentYear - 1;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor(
    private authService: AuthService,
    private partnerService: PartnerService,
    private fb: FormBuilder,
    private reportService: AccountCommonPartnerReportService,
    private companyService: CompanyService,
    private intlService: IntlService,
    private reportGeneralLedgerService: AccountReportGeneralLedgerService,
    private revenueReportService: RevenueReportService,
    private accountFinancialReportService: AccountFinancialReportService,
    private reportFinancialService: ReportFinancialService,
    private PhieuThuChiService: PhieuThuChiService,
    private commissionSettlementReportsService: CommissionSettlementsService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      company: null,
      dataTo: this.monthEnd,
      dateForm: this.monthStart,
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
    this.loadReportRevenueCurrentYear();
    this.loadReportRevenueOldYear();
    this.loadDebitCustomer();
    this.loadFinacialReport();
    this.loadDebitNCCByMonth();
    this.loadPhieuThuChiReport();
    this.loadCommissionSettlementReport();
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

  loadDebitCustomer() {
    var val = new AccountCommonPartnerReportSearch();
    val.resultSelection = "customer";
    val.companyId = this.formGroup.get('company').value ? this.formGroup.get('company').value.id : null;

    this.reportService.getSummary(val).subscribe(res => {

      var total = aggregateBy(res, this.aggregates);
      if (total) {
        this.totalDebitCustomer = total['end'].sum
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
        this.moneyBank = this.reportLedgerBank.accounts[1] ? this.reportLedgerBank.accounts[1].balance : 0;
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

  loadReportRevenueCurrentYear() {
    var val = new RevenueReportSearch();
    val.companyId = this.formGroup.get('company').value ? this.formGroup.get('company').value.id : '';
    val.groupBy = "date:month-current-year";
    this.revenueReportService.getReportFlowYear(val).subscribe(result => {
      if (result && result.details) {
        this.defindMonthOfCurrentYear(result.details)
      }
    }, error => {
      console.log(error);
    });
  }

  loadReportRevenueOldYear() {
    var val = new RevenueReportSearch();
    val.companyId = this.formGroup.get('company').value ? this.formGroup.get('company').value.id : '';
    val.groupBy = "date:month-old-year";
    this.revenueReportService.getReportFlowYear(val).subscribe(result => {
      if (result && result.details) {
        this.defindMonthOfOldYear(result.details)
      }
    }, error => {
      console.log(error);
    });
  }

  defindMonthOfCurrentYear(details) {
    for (let index = 1; index <= new Date().getMonth() + 1; index++) {
      var value = {
        month: index,
        data: 0,
        year: new Date().getFullYear()
      }
      var model = details.find(x => x.month == index);
      if (model) {
        value.data = model.balance;
        this.reportCurrentYear.push(value);
      } else {
        this.reportCurrentYear.push(value);
      }
    }
  }

  defindMonthOfOldYear(details) {
    for (let index = 1; index <= new Date().getMonth() + 1; index++) {
      var value = {
        year: new Date().getFullYear() - 1,
        month: index,
        data: 0
      }
      var model = details.find(x => x.month == index);
      if (model) {
        value.data = model.balance;
        this.reportOldYear.push(value);
      } else {
        this.reportOldYear.push(value);
      }
    }
  }

  loadFinacialReport() {
    this.accountFinancialReportService.getProfitAndLossReport().subscribe(
      result => {
        this.accountFinancialReportBasic = result;
        if (this.formGroup.invalid) {
          return false;
        }
        var val = new AccoutingReport();
        val.companyId = this.formGroup.get('company') && this.formGroup.get('company').value ? this.formGroup.get('company').value.id : null;
        val.dateFrom = this.intlService.formatDate(this.formGroup.get('dateFrom') ? this.formGroup.get('dateFrom').value : null, "yyyy-MM-dd");
        val.dateTo = this.intlService.formatDate(this.formGroup.get('dateTo') ? this.formGroup.get('dateTo').value : null, "yyyy-MM-dd");
        val.debitCredit = this.formGroup.get('debitCredit') ? this.formGroup.get('debitCredit').value : false;
        if (this.accountFinancialReportBasic)
          val.accountReportId = this.accountFinancialReportBasic.id;
        this.reportFinancialService.getAccountLinesItem(val).subscribe(
          result => {
            var dt = result.find(x => x.name === "Doanh thu" && x.level == 1);
            if (dt) {
              this.totalDoanhThu = dt.balance;
            }
            var cp = result.find(x => x.name === "Chi phí" && x.level == 1);
            if (cp) {
              this.totalChiPhi = cp.balance;
            }
          }
        )
      }
    )
  }

  loadDebitNCCByMonth() {
    var val = new AccountCommonPartnerReportSearch();
    val.resultSelection = "supplier";
    val.fromDate = this.intlService.formatDate(this.formGroup.get('dateFrom') ? this.formGroup.get('dateFrom').value : null, "yyyy-MM-dd");
    val.toDate = this.intlService.formatDate(this.formGroup.get('dateTo') ? this.formGroup.get('dateTo').value : null, "yyyy-MM-dd");
    val.companyId = this.formGroup.get('company').value ? this.formGroup.get('company').value.id : null;

    this.reportService.getSummary(val).subscribe(res => {
      var total = aggregateBy(res, this.aggregates);
      if (total) {
        this.totalDebitNCCByMonth = total['end'] ? total['end'].sum : 0;
        this.totalCreditNCCByMonth = total['credit'] ? total['credit'].sum : 0;
      }
    }, err => {
      console.log(err);
    })
  }

  loadPhieuThuChiReport() {
    var val = new PhieuThuChiSearch()
    val.companyId = this.formGroup.get('company').value ? this.formGroup.get('company').value.id : '';
    val.dateFrom = this.intlService.formatDate(this.formGroup.get('dateFrom') ? this.formGroup.get('dateFrom').value : null, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.formGroup.get('dateTo') ? this.formGroup.get('dateTo').value : null, "yyyy-MM-dd");
    this.PhieuThuChiService.reportPhieuThuChi(val).subscribe(
      result => {
        if (result) {
          var thu = aggregateBy(result.filter(x => x.type === "thu"), this.aggregatesThuChi);
          this.totalThu = thu && thu['amount'] ? thu['amount'].sum : 0;
          var chi = aggregateBy(result.filter(x => x.type === "chi"), this.aggregatesThuChi);
          this.totalChi = chi && chi['amount'] ? chi['amount'].sum : 0;
        }
      }
    )
  }

  loadCommissionSettlementReport() {
    var val = new CommissionSettlementReport();
    val.companyId = this.formGroup.get('company').value ? this.formGroup.get('company').value.id : '';
    val.dateFrom = this.intlService.formatDate(this.formGroup.get('dateFrom') ? this.formGroup.get('dateFrom').value : null, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.formGroup.get('dateTo') ? this.formGroup.get('dateTo').value : null, "yyyy-MM-dd");
    this.commissionSettlementReportsService.getReport(val).subscribe(result => {
      if (result) {
        var total = aggregateBy(result, this.aggregatesCommissionSettlement);
        this.totalHoaHong = total && total['amount'] ? total['amount'].sum : 0;
      }

    }, err => {
      console.log(err);
    });

  }
}
