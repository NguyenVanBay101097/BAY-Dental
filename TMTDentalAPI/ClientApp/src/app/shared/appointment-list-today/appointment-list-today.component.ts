import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AppointmentPaged, DateFromTo } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { EmployeeService } from 'src/app/employees/employee.service';

@Component({
  selector: 'app-appointment-list-today',
  templateUrl: './appointment-list-today.component.html',
  styleUrls: ['./appointment-list-today.component.css']
})
export class AppointmentListTodayComponent implements OnInit {
  gridData: GridDataResult;
  dateFrom: Date;
  dateTo: Date;
  state: string;
  userId: string;
  limit = 1000;
  skip = 0;
  loading = false;
  opened = false;

  search: string;
  searchUpdate = new Subject<string>();
  public today: Date = new Date(new Date().toDateString());
  stateFilter: string;

  stateFilterOptions: any[] = [];

  constructor(private appointmentService: AppointmentService,
    private intlService: IntlService, private modalService: NgbModal,
    private notificationService: NotificationService, private router: Router, private employeeService: EmployeeService) { }


  ngOnInit() {
    
    this.loadDataFromApi();
    this.loadStateCount();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AppointmentPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || "";
    val.dateTimeFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.dateTimeTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    // val.state = this.stateFilter.join(',');
    val.state = this.stateFilter;

    this.appointmentService.loadAppointmentList(val).pipe(
      map((response) => <GridDataResult>{
        data: response.items,
        total: response.totalItems,
      }
      )
    )
      .subscribe(
        (res) => {
          this.gridData = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

// checkfilterState(state) {
//     let indexLocation = this.stateFilter.indexOf(state);
//     if (indexLocation >= 0) {
//       this.stateFilter = this.stateFilter.filter((i) => i !== state);
//     } else {
//       this.stateFilter.push(state);
//     }

//     this.loadDataFromApi();
// }

onChangeOverState(value){
  this.stateFilter = value;
  this.loadDataFromApi();
}

loadStateCount(){
  var val = new DateFromTo();
  val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
  val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
  this.appointmentService.getCountState(val).subscribe(
    (result:any) => {
      debugger
      this.stateFilterOptions = result;
    },
    error => {

    }
  );
}

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
    this.loadStateCount();
  }

  stateGet(state) {
    switch (state) {
      case 'wait':
        return 'Chờ khám';
      case 'examination':
        return 'Đang khám';
      case 'done':
        return 'Hoàn thành';
      case 'cancel':
        return 'Hủy hẹn';
      default:
        return 'Đang hẹn';
    }
  }


}
