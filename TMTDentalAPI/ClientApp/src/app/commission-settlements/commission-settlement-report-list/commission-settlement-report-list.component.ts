import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { CommissionSettlementFilterReport, CommissionSettlementsService } from '../commission-settlements.service';

@Component({
  selector: 'app-commission-settlement-report-list',
  templateUrl: './commission-settlement-report-list.component.html',
  styleUrls: ['./commission-settlement-report-list.component.css']
})
export class CommissionSettlementReportListComponent implements OnInit {
  items: any[] = [];
  gridData: GridDataResult;
  search: string = '';
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  dateFrom: Date;
  dateTo: Date;
  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  searchUpdate = new Subject<string>();
  sumReport: number = 0;

  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  filteredEmployees: EmployeeSimple[] = [];
  employeeId: string = '';

  commissionType: string = '';
  filteredCommissionType: any[] = [
    { name: 'Bác sĩ', value: 'doctor' },
    { name: 'Phụ tá', value: 'assistant' },
    { name: 'Tư vấn', value: 'counselor' }
  ]
  constructor(
    private commissionSettlementsService: CommissionSettlementsService,
    private intl: IntlService,
    private employeeService: EmployeeService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadEmployees();

    this.loadDataFromApi();

    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.employeeCbx.loading = true)),
      switchMap(value => this.searchEmployees(value))
    ).subscribe(result => {
      this.filteredEmployees = result;
      this.employeeCbx.loading = false;
    });

  }

  loadDataFromApi() {
    var val = new CommissionSettlementFilterReport();
    val.dateFrom = this.dateFrom ? this.intl.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.dateTo = this.dateTo ? this.intl.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.employeeId = this.employeeId ? this.employeeId : '';
    val.commissionType = this.commissionType ? this.commissionType : '';
    val.groupBy = 'employee';
    this.loading = true;
    this.commissionSettlementsService.getReportPaged(val).subscribe((res: any) => {
      this.items = res;
      this.loadItems();
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    });

    this.commissionSettlementsService.getSumReport(val).subscribe((res: any) => {
      this.sumReport = res;
    }, err => {
      console.log(err);
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

  get amountTotal() {
    var total = 0;
    if(this.items) {
      this.items.forEach(item => {
        total += item.amount;
      });

      return total;
    }

    return 0;
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
  }

  onDateSearchChange(filter) {
    this.dateFrom = filter.dateFrom;
    this.dateTo = filter.dateTo;
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
    this.commissionSettlementsService.excelCommissionExport(val).subscribe((res: any) => {
      let filename = "Tổng quan hoa hồng nhân viên";
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
