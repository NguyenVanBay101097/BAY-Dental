import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import * as moment from 'moment';
import { of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { SaleReportService, ServiceReportReq } from '../sale-report.service';
import { ServiceReportManageService } from '../service-report-management/service-report-manage';

@Component({
  selector: 'app-service-report-time',
  templateUrl: './service-report-time.component.html',
  styleUrls: ['./service-report-time.component.css']
})
export class ServiceReportTimeComponent implements OnInit {
  filter = new ServiceReportReq();
  companies: CompanySimple[] = [];
  employees: EmployeeSimple[] = [];
  allDataGrid: any;
  allDataExport: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  searchUpdate = new Subject<string>();
  // filterState = "";
  search: string;
  dateFrom: Date;
  dateTo: Date;
  state: string;
  companyId: string;
  employeeId: string;
  
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
    var val = new ServiceReportReq();
    val.search = this.search || '';
    val.companyId = this.companyId || '';
    val.employeeId = this.employeeId || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    val.state = this.state || '';
    // var val = Object.assign({}, this.filter) as ServiceReportReq;

    // val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    // val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.filter = val;
    this.saleReportService.getServiceReportByTime(val).subscribe(res => {
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
    ).subscribe(r=> {
      this.skip = 0;
      this.loadAllData();
    })

    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.dateFrom = this.filter.dateFrom || new Date(y, m, 1);
    this.dateTo = this.filter.dateTo || new Date(y, m + 1, 0);
    this.state = 'sale,done,cancel';
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
    this.dateFrom = e.dateFrom;
    this.dateTo = e.dateTo;
    this.skip = 0;
    this.loadAllData();
  }

  onSelectCompany(e) {
    this.companyId = e ? e.id : null;
    this.skip = 0;
    this.loadAllData();
  }

  onSelectEmployee(e) {
    this.employeeId = e ? e.id : null;
    this.skip = 0;
    this.loadAllData();
  }

  pageChange(e) {
    this.skip = e.skip;
    this.loadReport();
  }

  onChangeFilterState(state) {
    this.state = state.value;
    this.skip = 0;
    this.loadAllData();
  }

  public allData = (): any => {
    var newData = [];
    this.allDataGrid.forEach(acc => {
      var s = Object.assign({}, acc);
      newData.push(s);
    });
    newData.forEach(acc => {
      acc.date2 = acc.date;
      acc.date = acc.date ? moment(acc.date).format('DD/MM/YYYY') : '';
      return acc;
    });
    const observable = of(newData).pipe(
      map(res => {
        return {
          data: res,
          total: res.length
        }
      })
    );

    observable.pipe(
    ).subscribe((result) => {
      this.allDataExport = result;
    });

    return observable;

  }
  exportExcel(grid: GridComponent) {
    grid.saveAsExcel();
  }

  public onExcelExport(args: any): void {
    args.preventDefault();
    const data = this.allDataExport.data;
    this.serviceReportManageService.emitChange({
       data : data,
       args : args,
       filter : this.filter,
       title: 'BaoCaoDichVu_TheoTG',
       header:'BÁO CÁO DỊCH VỤ THEO THỜI GIAN'
    })
  }

  onExportPDF() {
    var val = Object.assign({}, this.filter) as ServiceReportReq;
    
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.saleReportService.getServiceReportByTimePdf(val).subscribe(res => {
      this.loading = false;
      let filename ="BaoCaoDichVuTheoThoiGian";

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


  onPrint(){
    var val = Object.assign({}, this.filter) as ServiceReportReq;
    
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
      this.saleReportService.serviceReportByTimePrint(val).subscribe((result: any) => {
        this.loading = false;
        this.printService.printHtml(result);
      });
  }
}
