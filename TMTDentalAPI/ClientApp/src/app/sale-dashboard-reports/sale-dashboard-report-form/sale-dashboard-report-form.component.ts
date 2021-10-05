import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import * as _ from 'lodash';
import { values } from 'lodash';
import { forkJoin, observable, Observable, of, Subject } from 'rxjs';
import { debounceTime, map, switchMap, tap, groupBy } from 'rxjs/operators';
import { AccountCommonPartnerReport, AccountCommonPartnerReportSearch, AccountCommonPartnerReportSearchV2, AccountCommonPartnerReportService, ReportPartnerDebitReq } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { AccountFinancialReportBasic, AccountFinancialReportService } from 'src/app/account-financial-report/account-financial-report.service';
import { AccoutingReport, ReportFinancialService } from 'src/app/account-financial-report/report-financial.service';
import { AccountInvoiceReportService, RevenueReportFilter } from 'src/app/account-invoice-reports/account-invoice-report.service';
import { AccountReportGeneralLedgerService, ReportCashBankGeneralLedger } from 'src/app/account-report-general-ledgers/account-report-general-ledger.service';
import { AuthService } from 'src/app/auth/auth.service';
import { CashBookReportFilter, CashBookService, CashBookSummarySearch, SumaryCashBookFilter } from 'src/app/cash-book/cash-book.service';
import { CommissionSettlementFilterReport, CommissionSettlementReportOutput, CommissionSettlementsService } from 'src/app/commission-settlements/commission-settlements.service';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { DashboardReportService, SumaryRevenueReportFilter } from 'src/app/core/services/dashboard-report.service';
import { GetRevenueSumTotalReq, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { CustomerReceiptReportFilter, CustomerReceiptReportService } from 'src/app/customer-receipt-reports/customer-receipt-report.service';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerCustomerReportInput, PartnerCustomerReportOutput, PartnerService } from 'src/app/partners/partner.service';
import { PhieuThuChiSearch, PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { RevenueReportResultDetails, RevenueReportSearch, RevenueReportService } from 'src/app/revenue-report/revenue-report.service';
import { PartnerOldNewReport, PartnerOldNewReportSearch, PartnerOldNewReportService, PartnerOldNewReportSumReq } from 'src/app/sale-report/partner-old-new-report.service';
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
    { field: 'debit', aggregate: 'sum' }, { field: 'credit', aggregate: 'sum' }, { field: 'end', aggregate: 'sum' }
  ];

  public aggregatesThuChi: any[] = [
    { field: 'amount', aggregate: 'sum' }
  ];

  public aggregatesCommissionSettlement: any[] = [
    { field: 'amount', aggregate: 'sum' }
  ];

  loading = false;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  totalAmountNCC: number = 0;
  totalDebitNCC: number = 0;
  totalDebitNCCByMonth: number;
  totalCreditNCCByMonth: number;
  accountFinancialReportBasic: AccountFinancialReportBasic = new AccountFinancialReportBasic();
  totalDebitCustomer: number = 0;
  totalDoanhThu: number;
  totalChiPhi: number;
  totalThu: number;
  totalChi: number;
  partnerOldNewReport: PartnerOldNewReport;
  filteredCompanies: CompanyBasic[] = [];
  totalHoaHong: number;
  reportLedgerBank: any;
  moneyCash: number = 0;
  moneyBank: number = 0;
  public reportCurrentYears: any[];
  public reportOldYears: any[];
  companyId: string;
  filterMonthDate: Date = new Date();
  sumRevenue: number;
  groupBy: string = 'groupby:day';
  reportAmounTotal: any;
  cashBooks: any[];
  dataCashBooks: any[];
  totalDataCashBook: any;
  revenues: any[];
  dataRevenues: any[];
  dataNoTreatment: any[];
  dataCustomer: any[];
  // startDate: any = {};
  // endDate: any = {};
  currentYear = new Date().getFullYear();
  oldYear = this.currentYear - 1;
  companyChangeSubject = new Subject();
  partnerTypes = [
    { text: 'Khách mới', value: 'new' },
    { text: 'Khách quay lại', value: 'old' }
  ];

  filterGroupby: any[] = [
    { value: 'groupby:day', text: 'Ngày' },
    { value: 'groupby:month', text: 'Tháng' },
  ];

  filterRevenueReport: any[] = [
    { value: 'cash_bank', text: 'TM/CK', code: '131' },
    { value: 'debt', text: 'công nợ khách hàng', code: 'CNKH' },
    { value: 'advance', text: 'khách hàng tạm ứng', code: 'KHTU' },
  ];

  filterSumaryCashbookReport: any[] = [
    { value: 'cash_bank', text: 'TM/CK', code: '131', type: 'customer' },
    { value: 'debt', text: 'công nợ khách hàng', code: 'CNKH', type: 'customer' },
    { value: 'advance', text: 'khách hàng tạm ứng', code: 'KHTU', type: 'customer' },
    { value: 'cash_bank', text: 'Nhà cung cấp ', code: '331', type: 'supplier' },
    { value: 'payroll', text: 'Chi lương và tạm ứng lương nhân viên', code: '334', type: 'customer' },
    { value: 'commission', text: 'Hoa hồng', code: 'HHNGT', type: 'agent' },
    { value: 'all', text: 'Total' },
  ];

  filterTotalAmount: any[] = [
    { value: 'cash', text: 'Quỹ tiền mặt' },
    { value: 'bank', text: 'Quỹ ngân hàng' },
    { value: 'supplierdebit', text: 'Công nợ phải trả' },
    { value: 'customerdebit', text: 'Công nợ phải thu' },
    { value: 'residual', text: 'Dự kiến thu' },
  ];
  yearList: number[] = [];
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  monthFrom = new Date(this.currentYear, 0, 1);
  monthTo = new Date(this.currentYear, 11, 1);

  constructor(
    private partnerService: PartnerService,
    private fb: FormBuilder,
    private revenueReportService: AccountInvoiceReportService,
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
    private partnerOldNewRpService: PartnerOldNewReportService,
    private customerReceiptReportService: CustomerReceiptReportService,
    private cashBookService: CashBookService,
    private dashboardReportService: DashboardReportService
  ) { }

  ngOnInit() {
    var dateFrom = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), 1) : null;
    var dateTo = this.filterMonthDate ? new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth(), new Date(this.filterMonthDate.getFullYear(), this.filterMonthDate.getMonth() + 1, 0).getDate()) : null;
    this.dateFrom = dateFrom;
    this.dateTo = dateTo;
    // this.startDate = new Date(this.dateFrom);
    // this.endDate = new Date(this.dateTo);
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
    this.loadDataRevenueApi();
    this.loadDataCustomerApi();
    this.loadDataNotreatmentApi();
    this.loadDataCashbookApi();
    this.loadTotalDataFromApi();

    this.yearList = _.range(new Date().getFullYear(), 2000, -1);

  }

  loadAllData() {
    this.loadReportAmountTotal();
    this.loadDataRevenueChartApi();
    this.loadDataCashbookChartApi();
  }

  changeCompany(e) {
    setTimeout(() => {
      this.loadAllData();
      this.loadDataRevenueApi()
      this.loadDataCustomerApi();
      this.loadDataNotreatmentApi();
      this.loadDataCashbookApi();
      this.loadTotalDataFromApi();
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

  loadReportAmountTotal() {
    var companyId = this.companyId ? this.companyId : '';
    let res1 = this.cashBookService.getTotal({ resultSelection: "cash", companyId: companyId });
    let res2 = this.cashBookService.getTotal({ resultSelection: "bank", companyId: companyId });
    let res3 = this.reportService.getSummaryPartner({ resultSelection: "supplier", companyId: companyId }).pipe(map(x => x.initialBalance * -1));
    let res4 = this.reportService.reportPartnerDebitSummary(<ReportPartnerDebitReq>{ companyId: companyId });
    let res5 = this.saleOrderService.getRevenueSumTotal({ companyId: companyId }).pipe(map((res: any) => res.residual));
    forkJoin([res1, res2, res3, res4, res5])
      .subscribe(data => {
        this.moneyCash = data[0];
        this.moneyBank = data[1];
        this.totalDebitNCC = data[2];
        this.totalDebitCustomer = data[3].balance;
        this.sumRevenue = data[4];
      });
  }

  loadDataCashbookChartApi() {
    var filter = new CashBookReportFilter();
    filter.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    filter.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    filter.companyId = this.companyId ? this.companyId : '';
    filter.groupBy = this.groupBy;
    this.cashBookService.getChartReport(filter).subscribe((result: any) => {
      this.cashBooks = result;
    });
  }

  loadDataRevenueChartApi() {
    var filter = new RevenueReportFilter();
    filter.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    filter.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    filter.companyId = this.companyId ? this.companyId : '';
    filter.groupBy = this.groupBy;
    // this.revenueReportService.getRevenueReport(filter).subscribe((result: any) => {
    //   this.revenues = result;
    // });   
    this.dashboardReportService.getRevenueChartReport(filter).subscribe((result: any) => {
      this.revenues = result;
    });
  }

  loadDataRevenueApi() {
    forkJoin(this.filterRevenueReport.map(x => {
      var filter = new SumaryRevenueReportFilter();
      filter.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
      filter.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
      filter.companyId = this.companyId ? this.companyId : '';
      filter.resultSelection = x.value;
      filter.accountCode = x.code;
      return this.dashboardReportService.getSumaryRevenueReport(filter).pipe(
        switchMap(total => of({ text: x.value, total: total }))
      );
    })).subscribe((result) => {
      this.dataRevenues = result.map(x => x.total);
    });
  }

  loadDataCashbookApi() {
    forkJoin(this.filterSumaryCashbookReport.map(x => {
      var filter = new SumaryCashBookFilter();
      filter.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
      filter.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
      filter.companyId = this.companyId ? this.companyId : '';
      filter.resultSelection = x.value;
      filter.accountCode = x.code || '';
      filter.partnerType = x.type || '';
      return this.dashboardReportService.getSumaryRevenueReport(filter).pipe(
        switchMap(total => of({ text: x.value, total: total }))
      );
    })).subscribe((result) => {
      this.dataCashBooks = result.map(x => x.total);
    });
  }

  loadTotalDataFromApi() {
    this.loading = true;
    var summarySearch = new CashBookSummarySearch();
    summarySearch.resultSelection = 'cash_bank';
    summarySearch.companyId = this.companyId ? this.companyId : '';
    summarySearch.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    summarySearch.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
    this.cashBookService.getSumary(summarySearch)
      .subscribe(
        (res) => {
          this.totalDataCashBook = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }


  loadDataNotreatmentApi() {
    this.loading = true;
    var val = new CustomerReceiptReportFilter();
    val.limit = 0;
    val.offset = this.skip;
    val.companyId = this.companyId || '';
    val.state = 'done';
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    this.customerReceiptReportService.getCountCustomerReceiptNoTreatment(val).subscribe(
      (res: any[]) => {
        this.dataNoTreatment = res;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadDataCustomerApi() {
    forkJoin(this.partnerTypes.map(x => {
      var val = new PartnerOldNewReportSumReq();
      val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
      val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
      val.companyId = this.companyId || '';
      val.typeReport = x.value;
      return this.partnerOldNewRpService.sumReport(val).pipe(
        switchMap(count => of({ text: x.value, count: count }))
      );
    })).subscribe((result) => {
      this.dataCustomer = result;
    });
  }

  // loadDataSaleOrderApi() {
  //   this.loading = true;
  //   var val = new PartnerOldNewReportSumReq();
  //   val.limit = 0;
  //   val.offset = this.skip;
  //   val.companyId = this.companyId || '';
  //   val.state = 'done';
  //   val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
  //   val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
  //   this.customerReceiptReportService.getCountCustomerReceiptNoTreatment(val).subscribe(
  //     (res: any[]) => {
  //       this.pieDataNoTreatment = res;
  //       this.loading = false;
  //     },
  //     (err) => {
  //       console.log(err);
  //       this.loading = false;
  //     }
  //   );
  // }




  getRevenueSumTotal() {
    var val = new GetRevenueSumTotalReq();
    val.companyId = this.companyId || '';
    this.saleOrderService.getRevenueSumTotal(val).subscribe((res: any) => {
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
    // this.startDate = new Date(this.dateFrom);
    // this.endDate = new Date(this.dateTo);
    this.loadByMonthAndCompany();
    setTimeout(() => {
      this.monthReport.loadData();
    });
  }

  onChangeType(value) {
    this.groupBy = value;
    this.currentYear = new Date().getFullYear();
    if (this.groupBy == 'groupby:month') {
      let dateFrom = new Date(this.currentYear, 0, 1);
      let dateTo = new Date(this.currentYear, 11, 31);
      this.dateFrom = dateFrom;
      this.dateTo = dateTo;
    }
    else {
      this.dateFrom = this.monthStart;
      this.dateTo = this.monthEnd;
    }

    this.loadDataCashbookChartApi();
    this.loadDataRevenueChartApi();
  }

  onSearchDateChange(e) {
    this.dateFrom = e.dateFrom || '';
    this.dateTo = e.dateTo || '';
    // this.startDate = new Date(this.dateFrom);
    // this.endDate = new Date(this.dateTo);
    this.loadDataCashbookChartApi();
    this.loadDataRevenueChartApi();
    this.loadDataRevenueApi();
    this.loadDataCustomerApi();
    this.loadDataNotreatmentApi();
    this.loadDataCashbookApi();
    this.loadTotalDataFromApi();
  }

  setGroupbyFilter(groupby: any) {
    this.groupBy = groupby.value;
    this.loadDataCashbookChartApi();
    this.loadDataRevenueChartApi();
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
      case 'cash-book':
        this.router.navigateByUrl("cash-book/tab-cabo");
        break;
      case 'cash-book-cash':
        this.router.navigateByUrl("cash-book/tab-cabo?result_selection=cash");
        break;
      case 'cash-book-bank':
        this.router.navigateByUrl("cash-book/tab-cabo?result_selection=bank");
        break;
      case 'account-invoice-reports':
        this.router.navigateByUrl("report/account-invoice-reports/revenue-expecting");
        break;
      case "ncc-debit-report":
        this.router.navigateByUrl("report/report-account-common/partner?result_selection=supplier");
        break;
      case "customer-debit-report":
        this.router.navigateByUrl("report/report-account-common/partner-debit");
        break;
      case "money-report":
        this.router.navigateByUrl("report/report-general-ledgers/cash-bank");
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
