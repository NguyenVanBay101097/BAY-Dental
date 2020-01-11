import { Component, OnInit, Input } from '@angular/core';
import { AppointmentVMService } from '../appointment-vm.service';
import { AppointmentService } from '../appointment.service';
import { forkJoin } from 'rxjs';
import { AppointmentSearchByDate, AppointmentBasic } from '../appointment';
import { IntlService } from '@progress/kendo-angular-intl';
import { PagedResult2 } from 'src/app/core/paged-result-2';
import * as _ from 'lodash';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AppointmentCreateUpdateComponent } from '../appointment-create-update/appointment-create-update.component';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-appointment-kanban',
  templateUrl: './appointment-kanban.component.html',
  styleUrls: ['./appointment-kanban.component.css']
})
export class AppointmentKanbanComponent implements OnInit {
  dateFrom: Date;
  dateTo: Date;
  state: string;
  search: string;
  dateList: Date[];
  today = new Date();
  appointmentByDate: { [id: string]: AppointmentBasic[]; } = {};
  constructor(private appointmentVMService: AppointmentVMService, private appointmentService: AppointmentService,
    private intlService: IntlService, private modalService: NgbModal) {
    this.appointmentVMService.dateRange$.subscribe(result => {
      this.dateFrom = new Date(result.dateFrom.toDateString());
      this.dateTo = new Date(result.dateTo.toDateString());
      this.dateList = this.getDateList();
      this.loadData();
    });

    this.appointmentVMService.state$.subscribe(state => {
      this.state = state;
      this.loadData();
    });

    this.appointmentVMService.apCreate$.subscribe(id => {
      this.addAppointmentById(id);
    });

    this.appointmentVMService.refresh$.subscribe(() => {
      this.loadData();
    });

    this.appointmentVMService.search$.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(search => {
      this.search = search;
      this.loadData();
    });
  }

  ngOnInit() {

  }

  loadData() {
    this.resetData();
    var obs = this.dateList.map(date => {
      var val = new AppointmentSearchByDate();
      val.limit = 1000;
      if (this.state) {
        val.state = this.state;
      }
      if (this.search) {
        val.search = this.search;
      }
      val.date = this.intlService.formatDate(date, 'd', 'en-US');
      return this.appointmentService.searchReadByDate(val);
    });

    forkJoin(obs).subscribe(result => {
      result.forEach(item => {
        this.addAppointments(item);
      });
    });
  }

  resetData() {
    this.appointmentByDate = Object.assign({});
  }

  deleteAppointment(appointment: AppointmentBasic) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa lịch hẹn';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa lịch hẹn này?';
    modalRef.result.then(() => {
      this.appointmentService.removeAppointment(appointment.id).subscribe(() => {
        var date = new Date(appointment.date);
        var key = date.toDateString();
        if (this.appointmentByDate[key]) {
          var index = _.findIndex(this.appointmentByDate[key], o => o.id == appointment.id);
          if (index != -1) {
            this.appointmentByDate[key].splice(index, 1);
          }
        }
      });
    });
  }

  editAppointment(appointment: AppointmentBasic) {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = appointment.id;
    modalRef.result.then(() => {
      this.appointmentService.getBasic(appointment.id).subscribe(item => {
        var date = new Date(item.date);
        var key = date.toDateString();
        if (this.appointmentByDate[key]) {
          var index = _.findIndex(this.appointmentByDate[key], o => o.id == item.id);
          if (index != -1) {
            this.appointmentByDate[key][index] = item;
          }
        }
      });
    });
  }

  addAppointmentById(id) {
    this.appointmentService.getBasic(id).subscribe(item => {
      var date = new Date(item.date);
      var key = date.toDateString();
      var index = _.findIndex(this.dateList, o => o.toDateString() == date.toDateString());
      if (index == -1) {
        this.dateList.push(date);
      }

      if (!this.appointmentByDate[key]) {
        this.appointmentByDate[key] = [];
      }

      this.appointmentByDate[key].unshift(item);
    });
  }

  addAppointments(paged: PagedResult2<AppointmentBasic>) {
    for (var i = 0; i < paged.items.length; i++) {
      var item = paged.items[i];
      var date = new Date(item.date);
      var key = date.toDateString();
      if (!this.appointmentByDate[key]) {
        this.appointmentByDate[key] = [];
      }

      this.appointmentByDate[key].push(item);
    }
  }

  getDateList() {
    if (!this.dateFrom || !this.dateTo) {
      return [];
    }
    var list = [];
    var oneDay = 1000 * 60 * 60 * 24;
    var days = (this.dateTo.getTime() - this.dateFrom.getTime()) / oneDay;
    if (days > 30) {
      alert('Vui lòng xem tối đa 30 ngày để đảm bảo tốc độ phần mềm');
      return [];
    }
    for (var i = 0; i < days + 1; i++) {
      var date = new Date(this.dateFrom.toDateString());
      date.setDate(this.dateFrom.getDate() + i);
      list.push(date);
    }
    return list;
  }

}
