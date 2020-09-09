import { Component, OnInit, NgModuleRef } from '@angular/core';
import { EmployeeService } from 'src/app/employees/employee.service';
import { EmployeeSimple, EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { NgbModal, NgbPopoverConfig } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { DateInputModule } from '@progress/kendo-angular-dateinputs';
import { IntlService, load } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { offset } from '@progress/kendo-date-math';
import { Router } from '@angular/router';
import { TimeSheetEmployee, TimeKeepingService, EmployeeChamCongPaged, ChamCongBasic } from '../time-keeping.service';
import { TimeKeepingSettingDialogComponent } from '../time-keeping-setting-dialog/time-keeping-setting-dialog.component';
import { TimeKeepingSetupDialogComponent } from '../time-keeping-setup-dialog/time-keeping-setup-dialog.component';
import { PopupCloseEvent } from '@progress/kendo-angular-grid';
import { TimeKeepingForallDialogComponent } from '../time-keeping-forall-dialog/time-keeping-forall-dialog.component';

@Component({
  selector: 'app-time-keeping-view-calendar',
  templateUrl: './time-keeping-view-calendar.component.html',
  styleUrls: ['./time-keeping-view-calendar.component.css']
})
export class TimeKeepingViewCalendarComponent implements OnInit {

  title: string = "Bảng chấm công";
  listEmployeies: EmployeeSimple[] = [];
  flag = true;
  listTimeSheetByEmpId: { [id: string]: TimeSheetEmployee[] } = {};
  dateList: Date[];
  searchUpdate = new Subject<string>();
  search: string;


  public today: Date = new Date(new Date().toDateString());
  public filterMonth: Date = new Date(this.today);
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public weekStart: Date = new Date(new Date().setDate(new Date().getDate() - new Date().getDay() + (new Date().getDay() == 0 ? -6 : 1)));
  public weekEnd: Date = new Date(new Date().setDate(new Date().getDate() - new Date().getDay() + (new Date().getDay() == 0 ? -6 : 1) + 6));
  constructor(
    private intl: IntlService,
    private employeeService: EmployeeService,
    private modalService: NgbModal,
    private timeKeepingService: TimeKeepingService,
    private router: Router,
    config: NgbPopoverConfig,
    private notificationService: NotificationService
  ) {
    config.placement = 'bottom';
    config.triggers = 'hover';
    config.container = "body"
  }

  ngOnInit() {
    this.getDateMonthList();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadEmployee(value);
      });


  }

  loadEmployee(search?: string) {
    var paged = new EmployeePaged();
    paged.limit = 20;
    paged.offset = 0;
    paged.search = search ? search : '';
    this.employeeService.getEmployeeSimpleList(paged).subscribe(
      result => {
        this.listEmployeies = result;
        if (this.listEmployeies) {
          this.listEmployeies.forEach(emp => {
            this.loadData(emp.id)
          });
        }
      }
    )
  }

  loadData(empId) {
    var page = new EmployeeChamCongPaged();
    page.to = this.intl.formatDate(this.monthEnd, 'yyyy-MM-dd');
    page.from = this.intl.formatDate(this.monthStart, 'yyyy-MM-dd');
    page.employeeId = empId;
    this.timeKeepingService.getAllByEmpId(page).subscribe(
      result => {
        this.loadTimeSheet(empId, result);
      }
    )
  }

  loadTimeSheet(empId, vals: ChamCongBasic[]) {
    this.listTimeSheetByEmpId[empId] = [];
    this.dateList.forEach(date => {
      var value = new TimeSheetEmployee();
      if (!value.chamCongs) {
        value.chamCongs = [];
      }
      var cc = vals ? vals.filter(x => new Date(x.date).toDateString() == date.toDateString()) : null;
      if (cc) {
        value.chamCongs = cc;
        value.date = date;
      } else {
        value.date = date;
      }
      this.listTimeSheetByEmpId[empId].push(value);
    })
  }

  setupTimeKeeping() {
    const modalRef = this.modalService.open(TimeKeepingSettingDialogComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thiết lập chấm công';
    modalRef.result.then(() => {
      // this.loadData();
    });
  }

  exportFileTimeKeeping() {
    if (!this.monthStart && !this.monthEnd)
      return false;
    var page = new EmployeeChamCongPaged();
    page.from = this.intl.formatDate(this.monthStart, "yyyy-MM-dd");
    page.to = this.intl.formatDate(this.monthEnd, "yyyy-MM-dd");
    this.timeKeepingService.exportTimeKeeping(page).subscribe(
      rs => {
        let filename = "Danh sách chấm công";
        let newBlob = new Blob([rs], {
          type:
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        });

        let data = window.URL.createObjectURL(newBlob);
        let link = document.createElement("a");
        link.href = data;
        link.download = filename;
        link.click();
        setTimeout(() => {
          // For Firefox it is necessary to delay revoking the ObjectURL
          window.URL.revokeObjectURL(data);
        }, 100);
      }
    )
  }

  changeWeekMonth(value) {
    if (value == "weeks") {
      // this.getDateWeekList();
      this.flag = false;
    }
    else if (value == "months") {
      this.getDateMonthList();
      this.flag = true;
    }
  }

  getDateMonthList() {
    if (!this.monthStart || !this.monthEnd) {
      return [];
    }
    var list = [];
    var oneDay = 1000 * 60 * 60 * 24;
    var days = (this.monthEnd.getTime() - this.monthStart.getTime()) / oneDay;
    if (days > 30) {
      alert('Vui lòng xem tối đa 30 ngày để đảm bảo tốc độ phần mềm');
      return [];
    }
    for (var i = 0; i < days + 1; i++) {
      var date = new Date(this.monthStart.toDateString());
      date.setDate(this.monthStart.getDate() + i);
      list.push(date);
    }
    this.dateList = list;
    this.loadEmployee();
  }

  // getDateWeekList() {
  //   if (!this.weekStart || !this.weekEnd) {
  //     return [];
  //   }
  //   var list = [];
  //   for (var i = 0; i < 6 + 1; i++) {
  //     var date = new Date(this.weekStart.toDateString());
  //     date.setDate(this.weekStart.getDate() + i);
  //     list.push(date);
  //   }
  //   this.dateList = list;
  //   this.loadData();
  // }

  buttonFilterMonth(event) {
    if (event && event.dateFrom && event.dateTo) {
      this.monthStart = event.dateFrom;
      this.monthEnd = event.dateTo;
      this.filterMonth = event.dateFrom;
      this.getDateMonthList();
    }
  }

  clickTimeSheet(evt, id, date, employee) {
    // if (new Date().getDate() < date.getDate()) {
    //   this.notificationService.show({
    //     content: 'Chưa đến ngày bạn bạn không thể chấm công',
    //     hideAfter: 3000,
    //     position: { horizontal: 'right', vertical: 'bottom' },
    //     animation: { type: 'fade', duration: 400 },
    //     type: { style: 'error', icon: true }
    //   });
    //   return;
    // }
    evt.stopPropagation();
    const modalRef = this.modalService.open(TimeKeepingSetupDialogComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa chấm công';
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.employee = employee;
    modalRef.componentInstance.dateTime = date;
    modalRef.result.then(result => {
      if (result)
        this.loadData(result);
      this.notificationService.show({
        content: 'Cập nhật thành công',
        hideAfter: 3000,
        position: { horizontal: 'right', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    });
  }

  clickTimeSheetCreate(evt, date, employee) {
    // if (new Date().getDate() < date.getDate()) {
    //   this.notificationService.show({
    //     content: 'Chưa đến ngày bạn bạn không thể chấm công',
    //     hideAfter: 3000,
    //     position: { horizontal: 'right', vertical: 'bottom' },
    //     animation: { type: 'fade', duration: 400 },
    //     type: { style: 'error', icon: true }
    //   });
    //   return;
    // }
    evt.stopPropagation();
    const modalRef = this.modalService.open(TimeKeepingSetupDialogComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm 1 chấm công';
    modalRef.componentInstance.employee = employee;
    modalRef.componentInstance.dateTime = date;
    modalRef.result.then(result => {
      if (result)
        this.loadData(result)
      this.notificationService.show({
        content: 'Tạo mới thành công',
        hideAfter: 3000,
        position: { horizontal: 'right', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    });
  }

  setupWordEntryType() {
    this.router.navigateByUrl("time-keepings/types");
  }

  nextMonthFilter(myDate) {
    var nextMonth = new Date(myDate);
    nextMonth.setMonth(myDate.getMonth() + 1);
    this.filterMonth = nextMonth;
    this.monthStart = new Date(new Date(nextMonth.setDate(1)));
    this.monthEnd = new Date(this.monthStart.getFullYear(), this.monthStart.getMonth() + 1, 0);
    this.getDateMonthList();
  }

  previousMonthFilter(myDate) {
    var previous = new Date(myDate);
    previous.setMonth(myDate.getMonth() - 1);
    this.filterMonth = previous;
    this.monthStart = new Date(new Date(previous.setDate(1)));
    this.monthEnd = new Date(this.monthStart.getFullYear(), this.monthStart.getMonth() + 1, 0);
    this.getDateMonthList();
  }

  nextWeekFilter() {
    this.weekStart = new Date(new Date().setDate(this.weekStart.getDate() + 7));
    this.weekEnd = new Date(new Date().setDate(this.weekStart.getDate() + 6));
    // this.getDateWeekList();
  }

  previousWeekFilter() {
    this.weekStart = new Date(new Date().setDate(this.weekStart.getDate() - 7));
    this.weekEnd = new Date(new Date().setDate(this.weekStart.getDate() + 6));
    // this.getDateWeekList();
  }

  importFileExcell() {
    const modalRef = this.modalService.open(TimeKeepingSetupDialogComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });

    modalRef.result.then(res => {

    })
  }

  timeKeepingForAll() {
    const modalRef = this.modalService.open(TimeKeepingForallDialogComponent, { scrollable: true, size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title  = 'Chấm công nguyên ngày';
    modalRef.result.then(res => {
      this.timeKeepingService.timeKeepingForAll(res).subscribe(result => {
        this.notificationService.show({
          content: 'Thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.getDateMonthList();
      });
    });
  }

}
