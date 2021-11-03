import { Component, OnInit, Input, ViewChild, Inject } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { CommissionSettlementReportDetailOutput, CommissionSettlementsService, CommissionSettlementReportOutput, CommissionSettlementDetailReportPar, CommissionSettlementFilterReport } from '../commission-settlements.service';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import * as _ from 'lodash';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-commission-settlement-report-detail',
  templateUrl: './commission-settlement-report-detail.component.html',
  styleUrls: ['./commission-settlement-report-detail.component.css']
})
export class CommissionSettlementReportDetailComponent implements OnInit {
  gridData: GridDataResult;
  loading = false;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  search: string = '';
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;
  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  reportDetailResults: GridDataResult;
  listCommissionType: any[];
  employeeId: string = '';
  filteredEmployees: EmployeeSimple[] = [];
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  commissionType: string = '';
  filteredCommissionType: any[] = [
    { name: 'Bác sĩ', value: 'doctor' },
    { name: 'Phụ tá', value: 'assistant' },
    { name: 'Tư vấn', value: 'counselor' }
  ]
  constructor(
    private commissionSettlementsService: CommissionSettlementsService,
    private employeeService: EmployeeService,
    private intl: IntlService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();
    this.loadEmployees();

    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.employeeCbx.loading = true)),
      switchMap(value => this.searchEmployees(value))
    ).subscribe(result => {
      this.filteredEmployees = result;
      this.employeeCbx.loading = false;
    });

    this.searchChange();
  }

  searchChange() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new CommissionSettlementFilterReport();
    val.offset = this.skip;
    val.limit = this.limit;
    val.search = this.search ? this.search : '';
    val.employeeId = this.employeeId ? this.employeeId : '';
    val.commissionType = this.commissionType ? this.commissionType : '';
    val.dateFrom = this.dateFrom ? this.intl.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = this.dateFrom ? this.intl.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.groupBy = 'employee';
    this.commissionSettlementsService.getReportDetail(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res: any) => {
      this.reportDetailResults = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }

  loadEmployees() {
    this.searchEmployees().subscribe(result => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, result, 'id');
    });
  }

  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter || '';
    return this.employeeService.getEmployeeSimpleList(val);
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  onDateSearchChange(value) {
    if (value) {
      this.dateFrom = value.dateFrom;
      this.dateTo = value.dateTo;
    }
    else {
      this.dateFrom = null;
      this.dateTo = null;
    }
    this.skip = 0;
    this.loadDataFromApi();
  }

  valueEmployeeChange(value) {
    value ? this.employeeId = value.id : this.employeeId = '';
    this.skip = 0;
    this.loadDataFromApi();
  }

  valueCommissionTypeChange(value) {
    value ? this.commissionType = value.value : this.commissionType = '';
    this.skip = 0;
    this.loadDataFromApi();
  }

  exportCommissionExcelFile() { 
    var val = new CommissionSettlementFilterReport();
    val.dateFrom = this.dateFrom ? this.intl.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.dateTo = this.dateTo ? this.intl.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.employeeId = this.employeeId ? this.employeeId : '';
    val.commissionType = this.commissionType ? this.commissionType : '';
    val.groupBy = 'employee';
    this.commissionSettlementsService.excelCommissionDetailExport(val).subscribe((res: any) => {
      let filename = "Chi tiết hoa hồng nhân viên";
      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }
}
