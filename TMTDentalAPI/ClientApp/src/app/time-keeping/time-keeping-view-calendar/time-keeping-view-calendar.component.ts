import { Component, OnInit, NgModuleRef } from '@angular/core';
import { EmployeeService } from 'src/app/employees/employee.service';
import { EmployeeSimple } from 'src/app/employees/employee';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TimeKeepingSetupDialogComponent } from '../time-keeping-setup-dialog/time-keeping-setup-dialog.component';

@Component({
  selector: 'app-time-keeping-view-calendar',
  templateUrl: './time-keeping-view-calendar.component.html',
  styleUrls: ['./time-keeping-view-calendar.component.css']
})
export class TimeKeepingViewCalendarComponent implements OnInit {

  title: string = "Bảng chấm công";
  startDate: Date;
  endDate: Date;
  listEmployeies: EmployeeSimple[] = [];
  dateList: Date[];
  list: any[] = [];
  public today: Date = new Date(new Date().toDateString());
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private employeeService: EmployeeService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    var ob1 = {
      status: 'finish',
      date: this.today
    }
    var ob2 = {
      status: 'chua',
      date: this.monthStart
    }
    this.list.push(ob1);
    this.list.push(ob2);
    this.startDate = this.monthStart;
    this.endDate = this.monthEnd;
    this.dateList = this.getDateList();
    this.getEmployee();
  }

  getEmployee() {
    var val = {
      limit: 20,
      offSet: 0
    }
    this.employeeService.getEmployeeSimpleList(val).subscribe(
      result => {
        this.listEmployeies = result
      }
    )
  }

  getDateList() {
    if (!this.startDate || !this.endDate) {
      return [];
    }
    var list = [];
    var oneDay = 1000 * 60 * 60 * 24;
    var days = (this.endDate.getTime() - this.startDate.getTime()) / oneDay;
    if (days > 30) {
      alert('Vui lòng xem tối đa 30 ngày để đảm bảo tốc độ phần mềm');
      return [];
    }
    for (var i = 0; i < days + 1; i++) {
      var date = new Date(this.startDate.toDateString());
      date.setDate(this.startDate.getDate() + i);
      list.push(date);
    }
    return list;
  }

  clickTimeSheet(empId, date) {
    const modalRef = this.modalService.open(TimeKeepingSetupDialogComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cài đặt';
    modalRef.componentInstance.employeeId = empId;
    modalRef.componentInstance.dateTime = date;
    modalRef.result.then(() => {

    });
  }

}
