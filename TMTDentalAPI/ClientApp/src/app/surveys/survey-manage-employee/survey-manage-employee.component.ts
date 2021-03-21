import { Component, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, takeUntil } from 'rxjs/operators';
import { EmployeeService } from 'src/app/employees/employee.service';
import { SurveyAssignmentService } from '../survey.service';

@Component({
  selector: 'app-survey-manage-employee',
  templateUrl: './survey-manage-employee.component.html',
  styleUrls: ['./survey-manage-employee.component.css']
})
export class SurveyManageEmployeeComponent implements OnInit {
  gridData: GridDataResult;
  limit = 10;
  skip = 0;
  loading = false;
  search = '';
  searchSB = new Subject<string>();

  dateFrom: Date;
  dateTo: Date;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());


  constructor(
    private employeeService: EmployeeService,
    private intlService: IntlService,
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.searchSB.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe((val) => {
      this.search = val || '';
      this.skip = 0;
      this.loadDataFromApi();
    });

    this.loadDataFromApi()
  }

  onSearchDateChange(event) {
    this.skip = 0;
    this.dateTo = event.dateTo;
    this.dateFrom = event.dateFrom;
    this.loadDataFromApi()
  }

  loadDataFromApi() {
    this.loading = true;
    var val = {
      limit: this.limit,
      offset: this.skip,
      search: this.search,
      dateFrom: this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd"),
      dateTo: this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:50")
    }
    this.employeeService.GetEmployeeSurveyCount(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
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
    this.skip = e.skip;
    this.loadDataFromApi();
  }

}
