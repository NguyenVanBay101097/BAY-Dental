import { Component, OnInit, ViewChild } from '@angular/core';
import { Validators } from '@angular/forms';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerFilter, PartnerService } from 'src/app/partners/partner.service';
import { SurveyManageAssignEmployeeCreateDialogComponent } from '../survey-manage-assign-employee-create-dialog/survey-manage-assign-employee-create-dialog.component';
import { SurveyAssignmentPaged, SurveyService } from '../survey.service';

@Component({
  selector: 'app-survey-manage-assign-employee',
  templateUrl: './survey-manage-assign-employee.component.html',
  styleUrls: ['./survey-manage-assign-employee.component.css']
})
export class SurveyManageAssignEmployeeComponent implements OnInit {
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;

  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  filteredEmployees: PartnerSimple[];
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

  statuses = [
    { value: "draft", name: "Chưa gọi" },
    { value: "done", name: "Hoàn thành" },
    { value: "contact", name: "Đang liên hệ" }
  ]

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private intlService: IntlService,
    private modalService: NgbModal,
    private partnerService: PartnerService,
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

    // this.employeeCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.employeeCbx.loading = true)),
    //   switchMap(value => this.searchEmployees(value))
    // ).subscribe(result => {
    //   this.filteredEmployees = result;
    //   this.employeeCbx.loading = false;
    // });

    this.loadDataFromApi();
    this.loadSumary();
  }

  loadDataFromApi() {
    this.loading = true;
    var paged = new SurveyAssignmentPaged();
    paged.limit = this.limit;
    paged.offset = this.offset;
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

  loadSumary() {
    var dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    var dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:50");
    var request = this.surveyService.getSumary({ dateFrom: dateFrom, dateTo: dateTo, status: "done" });
    var request1 = this.surveyService.getSumary({ dateFrom: dateFrom, dateTo: dateTo, status: "contact" });
    var request2 = this.surveyService.getSumary({ dateFrom: dateFrom, dateTo: dateTo, status: "draft" });
    forkJoin([request, request1, request2]).subscribe((result) => {
      this.numberDone = result[0];
      this.numberContact = result[1];
      this.numberDraft = result[2];
    })
  }

  loadEmployees() {
    return this.searchEmployees().subscribe(result => this.filteredEmployees = result);
  }

  searchEmployees(search?: string) {
    var val = new PartnerFilter();
    val.search = search;
    val.employee = true;
    return this.partnerService.autocomplete2(val);
  }

  createEmpAssign() {
    let modalRef = this.modalService.open(SurveyManageAssignEmployeeCreateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Tạo phân việc';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  public onStateChange(state) {

  }

  public cellClickHandler({ sender, rowIndex, columnIndex, dataItem, isEdited }) {
    if (!isEdited) {
      sender.editCell(rowIndex, columnIndex, this.createFormGroup(dataItem));
    }
  }

  public cellCloseHandler(args: any) {
    const { formGroup, dataItem } = args;

    if (!formGroup.valid) {
      // prevent closing the edited cell if there are invalid values.
      args.preventDefault();
    } else if (formGroup.dirty) {
      //  do something
    }
  }

  public cancelHandler({ sender, rowIndex }) {
    sender.closeRow(rowIndex);
  }

  public saveHandler({ sender, formGroup, rowIndex }) {
    if (formGroup.valid) {
      // this.editService.create(formGroup.value);
      sender.closeRow(rowIndex);
    }
  }

  public saveChanges(grid: any): void {
    grid.closeCell();
    grid.cancelCell();

    // this.editService.saveChanges();
  }

  public cancelChanges(grid: any): void {
    grid.cancelCell();

    // this.editService.cancelChanges();
  }

  public createFormGroup(dataItem: any): FormGroup {
    return this.fb.group({
      empId: null
    });
  }

}
