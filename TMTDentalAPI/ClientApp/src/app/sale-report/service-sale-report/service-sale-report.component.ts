import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { Subject } from 'rxjs/internal/Subject';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { SaleOrderLinePaged } from 'src/app/partners/partner.service';
import { ToothBasic } from 'src/app/teeth/tooth.service';
import { SaleReportService, ServiceReportReq } from '../sale-report.service';


@Component({
  selector: 'app-service-sale-report',
  templateUrl: './service-sale-report.component.html',
  styleUrls: ['./service-sale-report.component.css']
})
export class ServiceSaleReportComponent implements OnInit {
  filter = new SaleOrderLinePaged();
  companies: CompanySimple[] = [];
  employees: EmployeeSimple[] = [];
  gridData: GridDataResult;
  loading = false;
  searchUpdate = new Subject<string>();
  filterMonth: any = "";
  stateDisplay= {
    sale:"Đang điều trị",
    done: "Hoàn thành"
  }
  
  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild("empCbx", { static: true }) empVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;

  constructor(
    private saleOrderLineService: SaleOrderLineService,
    private companyService: CompanyService,
    private employeeService: EmployeeService,
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadCompanies();
    this.loadEmployees();
    this.FilterCombobox();
    this.loadAllData();
  }

  
  loadAllData() {
    var val = Object.assign({}, this.filter) as SaleOrderLinePaged;
    val.companyId = val.companyId || '';
    val.employeeId = val.employeeId || '';
    val.dateOrderFrom = val.dateOrderFrom ? moment(val.dateOrderFrom).format('YYYY/MM/DD') : '';
    val.dateOrderTo = val.dateOrderTo ? moment(val.dateOrderTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.saleOrderLineService.getPaged(val).subscribe(res => {
      this.gridData = <GridDataResult>{
        data: res.items,
        total: res.totalItems
      }
      this.loading = false;
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
      this.filter.offset = 0;
      this.loadAllData();
    })

    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter.dateOrderFrom = this.filter.dateOrderFrom || new Date(y, m, 1);
    this.filter.dateOrderTo = this.filter.dateOrderTo || new Date(y, m + 1, 0);
    this.filter.limit = 20;
    this.filter.offset = 0;
    this.filter.state = 'sale';
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

  onSearchDateChange(e) {
    this.filter.dateOrderFrom = e.dateFrom;
    this.filter.dateOrderTo = e.dateTo;
    this.filter.offset = 0;
    this.loadAllData();
  }

  onSelectCompany(e) {
    this.filter.companyId = e ? e.id : null;
    this.filter.offset = 0;
    this.loadAllData();
  }

  onSelectEmployee(e) {
    this.filter.employeeId = e ? e.id : null;
    this.filter.offset = 0;
    this.loadAllData();
  }
  
  pageChange(e) {
    this.filter.offset = e.skip;
    this.loadAllData();
  }

  onChangeFilterMonth() {
    (this.filter.monthFrom as any) = '';
    (this.filter.monthTo as any) = '';
    this.filter.offset = 0;

    if(this.filterMonth) {
     this.filter.monthFrom = this.filterMonth.from || '';
     this.filter.monthTo = this.filterMonth.to || '';
    }
    this.loadAllData();
  }

  getTeethDisplay(teeth: ToothBasic[]){
    return teeth.map(x=> x.name).join(' ')
  }

}
