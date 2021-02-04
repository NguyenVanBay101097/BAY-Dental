import { Component, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { EmployeeService } from 'src/app/employees/employee.service';

@Component({
  selector: 'app-survey-manage-employee',
  templateUrl: './survey-manage-employee.component.html',
  styleUrls: ['./survey-manage-employee.component.css']
})
export class SurveyManageEmployeeComponent implements OnInit {
  gridData: GridDataResult;
  limit = 0;
  skip = 0;
  loading = false;

  constructor(
    private employeeService: EmployeeService
  ) { }

  ngOnInit() {
    this.loadDataFromApi()
  }
  loadDataFromApi() {
    this.loading = true;
    var val = {
      limit: this.limit,
      IsAllowSurvey: true,
      offset: this.skip
    }
    this.employeeService.getEmployeePaged(val).pipe(
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

}
