import { Component, Inject, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PrintService } from 'src/app/shared/services/print.service';
import { SaleReportService, ServiceReportReq } from '../sale-report.service';

@Component({
  selector: 'app-sale-report-overview',
  templateUrl: './sale-report-overview.component.html',
  styleUrls: ['./sale-report-overview.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleReportOverviewComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;

  searchUpdate = new Subject<string>();
  search: string;

  listCompanies: CompanySimple[] = [];
  listEmployees: EmployeeSimple[] = [];
  items: any[] = [];
  filter = new ServiceReportReq();
  sumAmountTotal: number = 0;
  sumAmountPaid: number = 0;
  sumAmountResidual: number = 0;
  constructor(
    private intlService: IntlService,
    private saleReportService: SaleReportService,
    private companyService: CompanyService,
    private employeeService: EmployeeService,
    private printService: PrintService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.initFilterData();
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });

    this.loadCompanies();
    this.loadEmployees();

  }

  initFilterData() {
    const date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter = <ServiceReportReq>{
      state: 'sale,done,cancel',
      dateFrom: new Date(y, m, 1),
      dateTo: new Date(y, m + 1, 0)
    };
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.listCompanies = res.items;
    });
  }

  searchEmployee$(q?: string) {
    let val = new EmployeePaged();
    val.search = q;
    val.isDoctor = true;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  loadEmployees() {
    this.searchEmployee$().subscribe((res: any) => {
      this.listEmployees = res;
    });
  }

  loadDataFromApi() {
    let val = Object.assign({}, this.filter) as ServiceReportReq;
    val.dateFrom = this.filter.dateFrom ? moment(this.filter.dateFrom).format('YYYY-MM-DD') : '';
    val.dateTo = this.filter.dateTo ? moment(this.filter.dateTo).format('YYYY-MM-DD') : '';

    if (this.search) {
      val.search = this.search;
    }

    this.saleReportService.getServiceOverviewReport(val).subscribe((result: any) => {
      console.log(result);
      this.items = result;
      this.loadItems();
    }, (error) => {
      console.log(error);
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };

    const aggregates = aggregateBy(this.items, [
      { aggregate: "sum", field: "amountTotal" },
      { aggregate: "sum", field: "amountPaid" },
      { aggregate: "sum", field: "amountResidual" },
    ]);
    console.log(aggregates);
    this.sumAmountTotal = aggregates.amountTotal ? aggregates.amountTotal.sum : 0;
    this.sumAmountPaid = aggregates.amountPaid ? aggregates.amountPaid.sum : 0;
    this.sumAmountResidual = aggregates.amountResidual ? aggregates.amountResidual.sum : 0;
  }

  onSearchDateChange(data) {
    this.filter.dateFrom = data.dateFrom;
    this.filter.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  onSelectEmployee(event) {
    this.filter.employeeId = event ? event.id : '';
    this.skip = 0;
    this.loadDataFromApi();
  }

  onSelectCompany(event) {
    this.filter.companyId = event ? event.id : '';
    this.skip = 0;
    this.loadDataFromApi();
  }

  exportExcel() {
    var val = Object.assign({}, this.filter) as ServiceReportReq;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.saleReportService.exportServiceOverviewReportExcel(val).subscribe((rs) => {
      let filename = "BaoCaoDichVu_TongQuan";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  onExportPDF() {
    var val = Object.assign({}, this.filter) as ServiceReportReq;

    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.saleReportService.printPdfServiceOverviewReport(val).subscribe(res => {
      let filename = "BaoCaoDichVuTongQuan";

      let newBlob = new Blob([res], {
        type:
          "application/pdf",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {

        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  onPrint() {
    var val = Object.assign({}, this.filter) as ServiceReportReq;

    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.saleReportService.printServiceOverviewReport(val).subscribe((result: any) => {
      this.printService.printHtml(result);
    });
  }
}

