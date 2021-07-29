import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
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
  filterState = "";
  
  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild("empCbx", { static: true }) empVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;

  constructor(
    private saleReportService: SaleReportService,
    private companyService: CompanyService,
    private employeeService: EmployeeService,
    private serviceReportManageService: ServiceReportManageService
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
        this.employees = x.items;
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
    this.filter.dateFrom = this.filter.dateFrom || new Date(y, m, 1);
    this.filter.dateTo = this.filter.dateTo || new Date(y, m + 1, 0);
    this.skip = 0;
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

  onChangeFilterState() {
    this.filter.active = null;
    this.filter.state = '';
    this.skip = 0;

    if(this.filterState) {
      if(this.filterState == "notActive" ){
        this.filter.active = false;
      }else {
        this.filter.state = this.filterState;
      }
    }
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

  // onExportPDF() {
  //   var val = Object.assign({}, this.filter) as SaleOrderLinePaged;
  //   val.companyId = val.companyId || '';
  //   val.employeeId = val.employeeId || '';
  //   val.dateOrderFrom = val.dateOrderFrom ? moment(val.dateOrderFrom).format('YYYY/MM/DD') : '';
  //   val.dateOrderTo = val.dateOrderTo ? moment(val.dateOrderTo).format('YYYY/MM/DD') : '';
  //   this.loading = true;
  //   this.saleOrderLineService.getSaleReportExportPdf(val).subscribe(res => {
  //     this.loading = false;
  //     let filename ="BaoCaoDichVu_DangDieuTri";

  //     let newBlob = new Blob([res], {
  //       type:
  //         "application/pdf",
  //     });

  //     let data = window.URL.createObjectURL(newBlob);
  //     let link = document.createElement("a");
  //     link.href = data;
  //     link.download = filename;
  //     link.click();
  //     setTimeout(() => {
  //       // For Firefox it is necessary to delay revoking the ObjectURL
  //       window.URL.revokeObjectURL(data);
  //     }, 100);
  //   });
  // }


}
