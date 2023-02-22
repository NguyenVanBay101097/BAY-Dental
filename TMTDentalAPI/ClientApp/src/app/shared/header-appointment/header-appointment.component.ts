import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-header-appointment',
  templateUrl: './header-appointment.component.html',
  styleUrls: ['./header-appointment.component.css']
})
export class HeaderAppointmentComponent implements OnInit {
  count = 0;
  constructor() { }

  ngOnInit() {
    // interval(60 * 1000)
    //   .pipe(
    //     flatMap(() => this.appointmentService.checkForthcoming())
    //   )
    //   .subscribe(() => {
    //   });
  }
}
