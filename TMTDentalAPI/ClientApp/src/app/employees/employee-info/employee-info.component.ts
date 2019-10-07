import { Component, OnInit } from '@angular/core';
import { EmployeeService } from '../employee.service';
import { EmployeeDisplay } from '../employee';

@Component({
  selector: 'app-employee-info',
  templateUrl: './employee-info.component.html',
  styleUrls: ['./employee-info.component.css']
})
export class EmployeeInfoComponent implements OnInit {

  constructor(private service: EmployeeService) { }

  id: string;//id nhân viên
  employee = new EmployeeDisplay;

  ngOnInit() {
    this.getEmployeeInfo()
  }

  getEmployeeInfo() {
    this.service.getEmployee(this.id).subscribe(
      rs => {
        this.employee = rs;
        console.log(this.employee);
      }
    )
  }
}
