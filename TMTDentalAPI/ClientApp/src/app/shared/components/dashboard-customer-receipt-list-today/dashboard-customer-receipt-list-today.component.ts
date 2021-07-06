import { Component, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AppointmentGetCountVM } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';

@Component({
  selector: 'app-dashboard-customer-receipt-list-today',
  templateUrl: './dashboard-customer-receipt-list-today.component.html',
  styleUrls: ['./dashboard-customer-receipt-list-today.component.css']
})
export class DashboardCustomerReceiptListTodayComponent implements OnInit {
  public today: Date = new Date(new Date().toDateString());

  stateFilter: string = '';
  stateCount: any = {};
  states: any[] = [
    { value: '', text: 'Tất cả'},
    { value: 'waiting', text: 'Chờ khám'},
    { value: 'examination', text: 'Đang khám'},
    { value: 'done', text: 'Hoàn thành'},
  ]

  constructor(private intlService: IntlService,private appointmentService: AppointmentService,) { }

  ngOnInit() {
  }

  loadStateCount() {
    forkJoin(this.states.map(x => {
      var val = new AppointmentGetCountVM();
      val.state = x.value;
      val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
      val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
      return this.appointmentService.getCount(val).pipe(
        switchMap(count => of({state: x.value, count: count}))
      );
    })).subscribe((result) => {
      result.forEach(item => {
        this.stateCount[item.state] = item.count;
      });
    });
  }

  setStateFilter(state: any) {
    this.stateFilter = state;  
  }

}
