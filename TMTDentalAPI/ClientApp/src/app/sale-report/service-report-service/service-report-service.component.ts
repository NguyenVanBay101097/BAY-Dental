import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import * as moment from 'moment';
import { of } from 'rxjs/internal/observable/of';
import { Subject } from 'rxjs/internal/Subject';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { SaleReportService, ServiceReportReq } from '../sale-report.service';
import { ServiceReportManageService } from '../service-report-management/service-report-manage';

@Component({
  selector: 'app-service-report-service',
  templateUrl: './service-report-service.component.html',
  styleUrls: ['./service-report-service.component.css']
})
export class ServiceReportServiceComponent implements OnInit {
  filter: ServiceReportReq;
  companies: CompanySimple[] = [];
  employees: EmployeeSimple[] = [];
  allDataGrid: any;
  allDataExport: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  searchUpdate = new Subject<string>();

  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild("empCbx", { static: true }) empVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;

  filterState: any[] = [
    { value: 'sale,done,cancel', text: 'Tất cả' },
    { value: 'sale', text: 'Đang điều trị' },
    { value: 'done', text: 'Hoàn thành' },
    { value: 'cancel', text: 'Ngừng điều trị' },
  ];

  constructor(
    private saleReportService: SaleReportService,
    private companyService: CompanyService,
    private employeeService: EmployeeService,
    private intlService: IntlService,
    private serviceReportManageService: ServiceReportManageService,
    private printService: PrintService
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadCompanies();
    this.loadEmployees();
    this.FilterCombobox();
    this.loadAllData();
  }


  loadAllData() {
    var val = Object.assign({}, this.filter) as ServiceReportReq;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.saleReportService.getServiceReportByService(val).subscribe(res => {
      this.allDataGrid = res;
      this.loading = false;
      this.loadReport();
    },
      err => {
        this.loading = false;
      });
  }

  FilterCombobox() {
    this.companyVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.companyVC.loading = true)),
        switchMap((value) => this.searchCompany$(value)
        )
      )
      .subscribe((x: any) => {
        this.companies = x.items;
        this.companyVC.loading = false;
      });

    this.empVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.empVC.loading = true)),
        switchMap((value) => this.searchEmployee$(value)
        )
      )
      .subscribe((x: any) => {
        this.employees = x;
        this.empVC.loading = false;
      });
  }

  initFilterData() {
    this.searchUpdate.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(r => {
      this.skip = 0;
      this.loadAllData();
    })

    var date = new Date(), y = date.getFullYear(), m = date.getMonth();

    this.filter = <ServiceReportReq>{
      state: 'sale,done,cancel',
      dateFrom: new Date(y, m, 1),
      dateTo: new Date(y, m + 1, 0)
    };

    this.skip = 0;
    this.loadAllData();
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }

  searchEmployee$(q?: string) {
    var val = new EmployeePaged();
    val.search = q;
    val.isDoctor = true;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  loadEmployees() {
    this.searchEmployee$().subscribe((res: any) => {
      this.employees = res;
    });
  }

  loadReport() {
    this.gridData = <GridDataResult>{
      total: this.allDataGrid.length,
      data: this.allDataGrid.slice(this.skip, this.skip + this.limit)
    };
  }

  onSearchDateChange(e) {
    this.filter.dateFrom = e.dateFrom;
    this.filter.dateTo = e.dateTo;
    this.skip = 0;
    this.loadAllData();
  }

  onSelectCompany(e) {
    this.filter.companyId = e ? e.id : null;
    this.skip = 0;
    this.loadAllData();
  }

  onSelectEmployee(e) {
    this.filter.employeeId = e ? e.id : null;
    this.skip = 0;
    this.loadAllData();
  }

  pageChange(e) {
    this.skip = e.skip;
    this.loadReport();
  }

  onChangeFilterState(state) {
    this.filter.state = state.value;
    this.skip = 0;
    this.loadAllData();
  }


  public allData = (): any => {
    return {
      data: this.allDataGrid,
      total: this.allDataGrid
    };
  }

  exportExcel() {
    var val = Object.assign({}, this.filter);
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.saleReportService.exportServiceReportByServiceExcel(val).subscribe((rs) => {
      let filename = "BaoCaoTheoDichVu";
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

  public onExcelExport(args: any): void {
    args.preventDefault();
    const data = this.allDataGrid;
    this.serviceReportManageService.emitChange({
      data: data,
      args: args,
      filter: this.filter,
      title: 'BaoCaoTheoDichVu',
      header: 'BÁO CÁO THEO DỊCH VỤ'
    })
  }

  onExportPDF() {
    var val = Object.assign({}, this.filter) as ServiceReportReq;

    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.saleReportService.getServiceReportByServicePdf(val).subscribe(res => {
      this.loading = false;
      let filename = "BaoCaoTheoDichVu";

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
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }


  onPrint() {
    var val = Object.assign({}, this.filter) as ServiceReportReq;

    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.saleReportService.serviceReportByServicePrint(val).subscribe((result: any) => {
      this.loading = false;
      this.printService.printHtml(result);
    });
  }
}