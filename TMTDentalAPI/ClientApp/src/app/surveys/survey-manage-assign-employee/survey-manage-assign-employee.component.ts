import { Route } from '@angular/compiler/src/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Validators } from '@angular/forms';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { forkJoin, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerFilter, PartnerService } from 'src/app/partners/partner.service';
import { SurveyManageAssignEmployeeCreateDialogComponent } from '../survey-manage-assign-employee-create-dialog/survey-manage-assign-employee-create-dialog.component';
import { SurveyAssignmentGetCountVM, SurveyAssignmentPaged, SurveyService } from '../survey.service';

@Component({
  selector: 'app-survey-manage-assign-employee',
  templateUrl: './survey-manage-assign-employee.component.html',
  styleUrls: ['./survey-manage-assign-employee.component.css']
})
export class SurveyManageAssignEmployeeComponent implements OnInit {
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;

  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  filteredEmployees: EmployeeSimple[];
  search: string;
  limit = 10;
  offset = 0;
  edit = false;
  dateFrom: Date;
  dateTo: Date;
  numberDone: number = 0;
  numberContact: number = 0;
  numberDraft: number = 0;
  status: string = '';
  loading = false;
  formGroup: FormGroup;
  employeeId: string;
  private editedRowIndex: number;
  statusCount: any = {};
  statuses = [
    { value: "draft", name: "Chưa gọi" },
    { value: "done", name: "Hoàn thành" },
    { value: "contact", name: "Đang liên hệ" },
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
    private partnerService: PartnerService,
    private employeeService: EmployeeService,
    private notificationService: NotificationService,
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
      console.log(res);
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  loadStatusCount() {
    forkJoin(this.statuses.map(x => {
      var val = new SurveyAssignmentGetCountVM();
      val.status = x.value;
      val.employeeId = this.employeeId ? this.employeeId : null;
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

  loadEmployees() {
    return this.searchEmployees().subscribe(result => this.filteredEmployees = result);
  }

  searchEmployees(search?: string) {
    var val = new EmployeePaged();
    val.search = search;
    val.isAllowSurvey = true;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  onSearchDateChange(event) {
    this.dateTo = event.dateTo;
    this.dateFrom = event.dateFrom;
    this.loadDataFromApi();
    this.loadStatusCount();
  }

  createEmpAssign() {
    let modalRef = this.modalService.open(SurveyManageAssignEmployeeCreateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Tạo phân việc';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  public onStatusChange(state) {
    this.status = state ? state.value : '';
    this.loadDataFromApi();
  }

  // click in cell to edit
  editHandler({ sender, rowIndex, dataItem }) {
    this.closeEditor(sender);
    this.formGroup = this.fb.group({
      id: dataItem.id,
      employee: dataItem.employee
    });

    this.editedRowIndex = rowIndex;

    sender.editRow(rowIndex, this.formGroup);
  }

  private closeEditor(grid, rowIndex = this.editedRowIndex) {
    grid.closeRow(rowIndex);
    this.editedRowIndex = undefined;
    this.formGroup = undefined;
  }

  public cancelHandler({ sender, rowIndex }) {
    this.closeEditor(sender, rowIndex);
  }

  public saveHandler({ sender, rowIndex, formGroup, isNew }): void {
    const survey = formGroup.value;
    if (!survey.employee) {
      this.closeEditor(sender, rowIndex);
      return;
    }
    var surveyOld = this.gridData.data.find(x => x.id === survey.id)
    surveyOld.employee = survey.employee;
    surveyOld.employeeId = survey.employee.id;
    this.surveyService.updateAssignment(survey.id, surveyOld).subscribe(
      () => {
        this.notificationService.show({
          content: 'Cập nhật thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }
    )
    sender.closeRow(rowIndex);
  }

  clickItem(event) {
    if (event.dataItem)
      this.router.navigateByUrl('/surveys/form-manage/' + event.dataItem.id);
  }

  onChaneEmp(emp) {
    this.employeeId = emp ? emp.id : null;
    this.loadDataFromApi();
    this.loadStatusCount();

  }

  employee(id) {
    var emp = this.filteredEmployees.find(x => x.id == id);
    return emp;
  }

  public createFormGroup(dataItem: any): FormGroup {
    return this.fb.group({
      id: dataItem.id,
      employee: dataItem.employee
    });
  }


}
