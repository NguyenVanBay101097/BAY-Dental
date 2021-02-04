import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { SurveyAssignmentGetCountVM, SurveyAssignmentPaged, SurveyService } from '../survey.service';

@Component({
  selector: 'app-survey-assignment-list',
  templateUrl: './survey-assignment-list.component.html',
  styleUrls: ['./survey-assignment-list.component.css']
})
export class SurveyAssignmentListComponent implements OnInit {
  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;

  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  filteredEmployees: EmployeeSimple[];
  employees: EmployeeSimple[] = [];
  search: string;
  limit = 20;
  offset = 0;
  edit = false;
  dateFrom: Date;
  dateTo: Date;
  numberDone: number = 0;
  numberContact: number = 0;
  numberDraft: number = 0;
  status: string = '';
  employeeId: string;
  loading = false;

  statusCount: any = {};
  statuses = [
    { value: "done", name: "Hoàn thành" },
    { value: "contact", name: "Đang liên hệ" },
    { value: "draft", name: "Chưa gọi" },
    { value: "", name: "Tổng khảo sát" }
  ];

  filteredStatus = [
    { value: "done", name: "Hoàn thành" },
    { value: "contact", name: "Đang liên hệ" },
    { value: "draft", name: "Chưa gọi" },
  ];


  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private intlService: IntlService,
    private modalService: NgbModal,
    private router: Router,
    private employeeService: EmployeeService,
    private surveyService: SurveyService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.loadDataFromApi();
      });

    this.empCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.empCbx.loading = true)),
      switchMap(value => this.searchEmployees(value))
    ).subscribe(result => {
      this.filteredEmployees = result;
      this.empCbx.loading = false;
    });

    this.loadDataFromApi();
    this.loadStatusCount();
    this.loadEmployees();
  }

  loadDataFromApi() {
    this.loading = true;
    var paged = new SurveyAssignmentPaged();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.employeeId = this.employeeId ? this.employeeId : '';
    paged.search = this.search ? this.search : '';
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:50");
    paged.status = this.status;
    this.surveyService.getPaged(paged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      this.loading = false;
    })
  }

  loadEmployees() {
    return this.searchEmployees().subscribe(result => {
      this.filteredEmployees = result;
      this.employees = result;
    });
  }

  searchEmployees(search?: string) {
    var val = new EmployeePaged();
    val.search = search;
    val.isAllowSurvey = true;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  onChaneEmp(emp) {
    this.employeeId = emp ? emp.id : null;
    this.loadDataFromApi();
    this.loadStatusCount();

  }

  employee(id) {
    var emp = this.employees.find(x => x.id == id);
    return emp;
  }

  clickItem(item) {
    var id = item.dataItem.id;
    this.router.navigate(['/surveys/form'], { queryParams: { id: id } });
  }

  public pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.loadDataFromApi();
  }

  loadStatusCount() {
    forkJoin(this.statuses.map(x => {
      var val = new SurveyAssignmentGetCountVM();
      val.status = x.value;
      val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
      val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
      return this.surveyService.getSumary(val).pipe(
        switchMap(count => of({ status: x.value, count: count }))
      );
    })).subscribe((result) => {
      result.forEach(item => {
        this.statusCount[item.status] = item.count;
      });
    });
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
    this.loadStatusCount();
  }

  statusChange(item) {
    this.status = item ? item.value : '';
    this.loadDataFromApi();
  }

  GetStatus(status) {
    switch (status) {
      case 'contact':
        return 'Đang liên hệ';
      case 'done':
        return 'Hoàn thành';
      case 'draft':
        return 'Chưa gọi';
    }
  }

}
