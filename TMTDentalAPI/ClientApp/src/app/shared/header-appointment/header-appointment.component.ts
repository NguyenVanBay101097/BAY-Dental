import { Component, OnInit } from '@angular/core';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { interval } from 'rxjs';
import { flatMap } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-header-appointment',
  templateUrl: './header-appointment.component.html',
  styleUrls: ['./header-appointment.component.css']
})
export class HeaderAppointmentComponent implements OnInit {
  count = 0;
  constructor(private appointmentService: AppointmentService, private intlService: IntlService) { }

  ngOnInit() {
    interval(60 * 1000)
      .pipe(
        flatMap(() => this.appointmentService.checkForthcoming())
      )
      .subscribe(() => {
      });
  }
}
