import { Component, OnInit } from '@angular/core';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { AppointmentPaged } from 'src/app/appointment/appointment';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-home-today-appointment',
  templateUrl: './home-today-appointment.component.html',
  styleUrls: ['./home-today-appointment.component.css']
})
export class HomeTodayAppointmentComponent implements OnInit {
  public today: Date = new Date(new Date().toDateString());
  stateCount = {};
  constructor(private appointmentService: AppointmentService,
    private intlService: IntlService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    var states = ["confirmed", "done", "cancel"];
    var val = new AppointmentPaged();

    this.appointmentService.getPaged(val);

    var obs = states.map(state => {
      var val = new AppointmentPaged();
      val.dateTimeFrom = this.intlService.formatDate(this.today, 'd', 'en-US');
      val.dateTimeTo = this.intlService.formatDate(this.today, 'd', 'en-US');
      val.state = state;
      return this.appointmentService.getPaged(val);
    });

    forkJoin(obs).subscribe(result => {
      result.forEach(item => {
        if (item.items.length) {
          var state = item.items[0].state;
          this.stateCount[state] = item.totalItems;
        }
      });
    });
  }
}
