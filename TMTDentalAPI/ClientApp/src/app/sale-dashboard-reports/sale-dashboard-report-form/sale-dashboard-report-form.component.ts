import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { values } from 'lodash';
import { forkJoin, observable, Observable, Subject } from 'rxjs';
import { debounceTime, map, switchMap, tap, groupBy } from 'rxjs/operators';
import { AccountCommonPartnerReportSearch, AccountCommonPartnerReportSearchV2, AccountCommonPartnerReportService, ReportPartnerDebitReq } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { AccountFinancialReportBasic, AccountFinancialReportService } from 'src/app/account-financial-report/account-financial-report.service';
import { AccoutingReport, ReportFinancialService } from 'src/app/account-financial-report/report-financial.service';
import { AccountReportGeneralLedgerService, ReportCashBankGeneralLedger } from 'src/app/account-report-general-ledgers/account-report-general-ledger.service';
import { AuthService } from 'src/app/auth/auth.service';
import { CashBookService } from 'src/app/cash-book/cash-book.service';
import { CommissionSettlementFilterReport, CommissionSettlementReportOutput, CommissionSettlementsService } from 'src/app/commission-settlements/commission-settlements.service';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { GetRevenueSumTotalReq, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerCustomerReportInput, PartnerCustomerReportOutput, PartnerService } from 'src/app/partners/partner.service';
import { PhieuThuChiSearch, PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { RevenueReportResultDetails, RevenueReportSearch, RevenueReportService } from 'src/app/revenue-report/revenue-report.service';
import { PartnerOldNewReport, PartnerOldNewReportSearch, PartnerOldNewReportService } from 'src/app/sale-report/partner-old-new-report.service';
import { FinancialRevenueReportComponent } from '../financial-revenue-report/financial-revenue-report.component';
import { SaleDashboardReportChartFlowMonthComponent } from '../sale-dashboard-report-chart-flow-month/sale-dashboard-report-chart-flow-month.component';
import { SaleDashboardReportChartFlowYearComponent } from '../sale-dashboard-report-chart-flow-year/sale-dashboard-report-chart-flow-year.component';

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
  @ViewChild('monthCbx', { static: true }) monthCbx: ComboBoxComponent;
  @ViewChild(SaleDashboardReportChartFlowYearComponent, { static: true }) yearReport: SaleDashboardReportChartFlowYearComponent;
  @ViewChild(FinancialRevenueReportComponent, { static: true }) revenueReport: FinancialRevenueReportComponent;
  @ViewChild(SaleDashboardReportChartFlowMonthComponent, { static: true }) monthReport: SaleDashboardReportChartFlowMonthComponent;
  formGroup: FormGroup;
  public aggregates: any[] = [
    { field: 'debit', aggregate: 'sum' }, { field: 'credit', aggregate: 'sum' } , { field: 'end', aggregate: 'sum' }
  ];

  public aggregatesThuChi: any[] = [
    { field: 'amount', aggregate: 'sum' }
  ];

  public aggregatesCommissionSettlement: any[] = [
    { field: 'amount', aggregate: 'sum' }
  ];

  dateFrom: Date;
  dateTo: Date;
  totalAmountNCC: number;
  totalDebitNCC: number;
  totalDebitNCCByMonth: number;
  totalCreditNCCByMonth: number;
  accountFinancialReportBasic: AccountFinancialReportBasic = new AccountFinancialReportBasic();
  totalDebitCustomer: number;
  totalDoanhThu: number;
  totalChiPhi: number;
  totalThu: number;
  totalChi: number;
  partnerOldNewReport: PartnerOldNewReport;
  filteredCompanies: CompanyBasic[] = [];
  totalHoaHong: number;
  reportLedgerBank: any;
  moneyCash: number;
  moneyBank: number;
  public reportCurrentYears: any[];
  public reportOldYears: any[];
  companyId: string;
  filterMonthDate: Date = new Date();
  sumRevenue: any;
  groupBy: string = 'groupby:day';

  currentYear = new Date().getFullYear();
  oldYear = this.currentYear - 1;
  companyChangeSubject = new Subject();

  filterGroupby : any[] = [
    { value: 'groupby:day', text: 'Ngày' },
    { value: 'groupby:month', text: 'Tháng' },
  ];

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor(
    private partnerService: PartnerService,
    private fb: FormBuilder,
    private reportService: AccountCommonPartnerReportService,
    private companyService: CompanyService,
    private intlService: IntlService,
    private saleOrderService: SaleOrderService,
    private reportGeneralLedgerService: AccountReportGeneralLedgerService,
    private accountFinancialReportService: AccountFinancialReportService,
    private reportFinancialService: ReportFinancialService,
    private PhieuThuChiService: PhieuThuChiService,
    private commissionSettlementReportsService: CommissionSettlementsService,
    private router: Router,
    private partnerOldNewReportService: PartnerOldNewReportService,
    private cashBookService: CashBookService,
  ) { }

  ngOnInit() {
    var dateFrom = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), 1) : null;
    var dateTo = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth() + 1, 0).getDate()) : null;
    this.dateFrom = dateFrom;
    this.dateTo = dateTo;

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
    this.loadDebitCustomer();
    this.loadFinacialReport();
    this.loadDebitNCCByMonth();
    this.loadPhieuThuChiReport();
    this.getRevenueSumTotal();
    this.loadCommissionSettlementReport();
  }

  changeCompany(e) {
    setTimeout(() => {
      this.loadAllData();
      this.yearReport.loadData();
      this.revenueReport.loadReport();
      this.monthReport.loadData();
    });
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

  // filter by company

  loadDebitNCC() {
    var val = new AccountCommonPartnerReportSearchV2();
    val.resultSelection = "supplier";
    val.companyId = this.companyId || '';
    this.reportService.getSummaryPartner(val).subscribe(res => {
      if (res) {
        this.totalDebitNCC = (res.debit - res.credit) * -1;
      }
    }, err => {
      console.log(err);
    })
  }

  loadDebitCustomer() {
    var val = new ReportPartnerDebitReq();
    val.companyId = this.companyId;
    this.reportService.reportPartnerDebitSummary(val).subscribe((res: any) => {
      this.totalDebitCustomer = res.balance;
    }, err => {
      console.log(err);
    })
  }

  getRevenueSumTotal() {
    var val = new GetRevenueSumTotalReq();
    val.companyId = this.companyId || '';
    this.saleOrderService.getRevenueSumTotal(val).subscribe((res:any) => {
      this.sumRevenue = res;
    }
    );
  }


  loadDataMoney() {
    var companyId = this.companyId ? this.companyId : null;
    let cash = this.cashBookService.getTotal({ resultSelection: "cash", companyId: companyId });
    let bank = this.cashBookService.getTotal({ resultSelection: "bank", companyId: companyId });
    forkJoin([cash, bank]).subscribe(results => {
      this.moneyCash = results[0];
      this.moneyBank = results[1];
    });
  }

  // filter by month and company

  changeMonth() {
    var dateFrom = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), 1) : null;
    var dateTo = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth() + 1, 0).getDate()) : null;
    this.dateFrom = dateFrom;
    this.dateTo = dateTo;
    this.loadByMonthAndCompany();
    setTimeout(() => {
      this.monthReport.loadData(); 
    });
  }

  onChangeType(value) {
    this.groupBy = value;
  }

  onSearchDateChange(e) {
    this.dateFrom = e.dateFrom;
    this.dateTo = e.dateTo;
    // this.loadAllData();
  }

  setGroupbyFilter(time: any) {
    this.groupBy = time.value;
  }


  loadByMonthAndCompany() {
    this.loadFinacialReport();
    this.loadDebitNCCByMonth();
    this.loadCommissionSettlementReport();
    this.loadPhieuThuChiReport();
  }

  loadFinacialReport() {
    this.accountFinancialReportService.getProfitAndLossReport().subscribe(
      result => {
        this.accountFinancialReportBasic = result;
        var val = new AccoutingReport();
        val.companyId = this.companyId;

        var dateFrom = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), 1) : null;
        var dateTo = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth() + 1, 0).getDate()) : null;
        val.dateFrom = dateFrom ? this.intlService.formatDate(dateFrom, 'yyyy-MM-dd') : null;
        val.dateTo = dateTo ? this.intlService.formatDate(dateTo, 'yyyy-MM-dd') : null;
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
    // var dateFrom = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), 1) : null;
    // var dateTo = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth() + 1, 0).getDate()) : null;
    // val.fromDate = dateFrom ? this.intlService.formatDate(dateFrom, 'yyyy-MM-dd') : null;
    // val.toDate = dateTo ? this.intlService.formatDate(dateTo, 'yyyy-MM-dd') : null;
    val.companyId = this.companyId;
    this.reportService.getSummary(val).subscribe(res => {
      var total = aggregateBy(res, this.aggregates);
      if (total) {    
        this.totalDebitNCCByMonth = total['debit'] ? total['debit'].sum : 0;
        this.totalCreditNCCByMonth = total['credit'] ? total['credit'].sum : 0;
        this.totalAmountNCC = total['end'] ? total['end'].sum : 0;
      }
    }, err => {
      console.log(err);
    })
  }

  loadPhieuThuChiReport() {
    var val = new PhieuThuChiSearch()
    val.companyId = this.companyId || '';
    var dateFrom = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), 1) : null;
    var dateTo = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth() + 1, 0).getDate()) : null;
    val.dateFrom = dateFrom ? this.intlService.formatDate(dateFrom, 'yyyy-MM-dd') : '';
    val.dateTo = dateTo ? this.intlService.formatDate(dateTo, 'yyyy-MM-dd') : '';
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
    var val = new CommissionSettlementFilterReport();
    val.companyId = this.companyId;
    var dateFrom = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), 1) : null;
    var dateTo = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth() + 1, 0).getDate()) : null;
    val.dateFrom = dateFrom ? this.intlService.formatDate(dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = dateTo ? this.intlService.formatDate(dateTo, 'yyyy-MM-dd') : null;
    this.commissionSettlementReportsService.getSumReport(val).subscribe((result: any) => {
      this.totalHoaHong = result || 0;
    }, err => {
      console.log(err);
    });
  }

  redirectTo(value) {
    switch (value) {
      case "ncc-debit-report":
        this.router.navigateByUrl("report-account-common/partner?result_selection=supplier");
        break;
      case "customer-debit-report":
        this.router.navigateByUrl("report-account-common/partner?result_selection=customer");
        break;
      case "money-report":
        this.router.navigateByUrl("report-general-ledgers/cash-bank");
        break;
      case "new-old-customer-report":
        this.router.navigateByUrl("sale-report/partner");
        break;
      case "financial-report":
        this.router.navigateByUrl("financial-report");
        break;
      case "chi-report":
        this.router.navigateByUrl("phieu-thu-chi?type=chi");
        break;
      case "thu-report":
        this.router.navigateByUrl("phieu-thu-chi?type=thu");
        break;
      case "hoa-hong-report":
        this.router.navigateByUrl("commission-settlements/report");
        break;
      default:
        break;
    }
  }
}
