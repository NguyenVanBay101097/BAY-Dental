import { Component, OnInit, ViewChild } from "@angular/core";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { GridDataResult } from "@progress/kendo-angular-grid";
import { IntlService } from "@progress/kendo-angular-intl";
import { NotificationService } from "@progress/kendo-angular-notification";
import { Subject } from "rxjs";
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from "rxjs/operators";
import { SaleOrderService, SaleOrderToSurveyFilter } from "src/app/core/services/sale-order.service";
import { EmployeePaged, EmployeeSimple } from "src/app/employees/employee";
import { EmployeeService } from "src/app/employees/employee.service";
import { SurveyAssignmentDefaultGetPar, SurveyAssignmentPaged, SurveyAssignmentService } from "../survey.service";
@Component({
  selector: 'app-survey-manage-assign-employee',
  templateUrl: './survey-manage-assign-employee.component.html',
  styleUrls: ['./survey-manage-assign-employee.component.css']
})
export class SurveyManageAssignEmployeeComponent implements OnInit {

  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;

  dateFrom: Date;
  dateTo: Date;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  gridData: GridDataResult;
  limit = 10;
  offset = 0;
  filteredEmployees: EmployeeSimple[];

  constructor(
    private employeeService: EmployeeService,
    private intlService: IntlService,
    private surveyAssignmentService: SurveyAssignmentService,
    private notificationService: NotificationService,
    private saleOrderService: SaleOrderService
  ) { }

  ngOnInit(): void {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.offset = 0;
        this.search = value || '';
        this.offset = 0;
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
    this.loadEmployees();
  }

  loadEmployees() {
    return this.employeeService.getAllowSurveyList().subscribe((result: any) => {
      this.filteredEmployees = result;
    });
  }

  loadDataFromApi(paged = null) {
    this.loading = true;
    var filter = new SaleOrderToSurveyFilter();
    filter.limit = this.limit;
    filter.offset = this.offset;
    filter.search = this.search ? this.search : '';
    filter.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    filter.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;

    this.saleOrderService.getToSurveyPaged(filter).pipe(
      map((response: any) => (<GridDataResult>{
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

  pageChange(e) {
    this.offset = e.skip;
    this.loadDataFromApi();
  }

  onSearchDateChange(event) {
    this.dateTo = event.dateTo;
    this.dateFrom = event.dateFrom;
    this.offset = 0;
    this.loadDataFromApi();
  }

  onAutoAssign() {
    if (this.gridData.data.length == 0) {
      this.notify('error', 'Không có khảo sát để phân việc!');
      return;
    }

    if (this.filteredEmployees.length == 0) {
      this.notify('error', 'Không có nhân viên nào để phân việc');
      return;
    }

    this.gridData.data.forEach((ass, index) => {
      var emp = this.filteredEmployees[index % this.filteredEmployees.length];
      ass.employee = emp;
    });

    // var val = {
    //   limit: 0,
    //   offset: 0,
    // }

    // this.employeeService.GetEmployeeSurveyCount(val).subscribe((res) => {
    //   let emps = res.items;
    //   this.gridData.data.forEach(ass => {
    //     let minEmp = emps.reduce((pre, cur) => {
    //       return pre.totalAssignment < cur.totalAssignment ? pre : cur;
    //     });

    //     ass.employee = { id: minEmp.id, name: minEmp.name };
    //     ass.employeeId = minEmp.id;

    //     minEmp.totalAssignment++;
    //   });
    // });

    //  var paged =  new SurveyAssignmentDefaultGetPar();
    //  paged.limit = this.limit;
    //  paged.offset = this.offset;
    //  paged.search = this.search ? this.search : '';
    //  paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    //  paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:50");
    //  paged.isRandomAssign = true;
    //  this.loadDataFromApi(paged);
  }

  onEmployeeChange(val, index) {
    if (val) {
      this.gridData.data[index].employee = val;
      this.gridData.data[index].employeeId = val.id;
    } else {
      this.gridData.data[index].employee = null;
      this.gridData.data[index].employeeId = null;
    }
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }


  onSave() {
    debugger;
    var data = this.gridData.data.filter(x => x.employee != null)
      .map(x => {
        return { employeeId: x.employee.id, saleOrderId: x.id };
      });

    if (data.length == 0) {
      this.notify('error', 'Bạn chưa chọn nhân viên để phân việc');
      return;
    }

    this.surveyAssignmentService.createListAssign(data).subscribe(
      () => {
        this.notify('success', 'Lưu thành công');
        this.loadDataFromApi();
      }
    )
  }
}
