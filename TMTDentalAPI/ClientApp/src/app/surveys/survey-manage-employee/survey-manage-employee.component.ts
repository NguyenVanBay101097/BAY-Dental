import { Component, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, takeUntil } from 'rxjs/operators';
import { EmployeeService } from 'src/app/employees/employee.service';
import { SurveyService } from '../survey.service';

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

  constructor(
    private employeeService: EmployeeService
  ) { }

  ngOnInit() {

    this.searchSB.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe((val) => { this.search = val || ''; this.loadDataFromApi(); });

    this.loadDataFromApi()
  }
  loadDataFromApi() {
    this.loading = true;
    var val = {
      limit: this.limit,
      offset: this.skip,
      search: this.search
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
