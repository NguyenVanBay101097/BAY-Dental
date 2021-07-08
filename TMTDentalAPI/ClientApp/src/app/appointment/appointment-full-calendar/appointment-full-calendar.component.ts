import { Component, ElementRef, OnInit } from '@angular/core';
import { Calendar, EventApi } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid'; 
import timeGrigPlugin from '@fullcalendar/timegrid'; 
import interactionPlugin from '@fullcalendar/interaction'; 

@Component({
  selector: 'app-appointment-full-calendar',
  templateUrl: './appointment-full-calendar.component.html',
  styleUrls: ['./appointment-full-calendar.component.css']
})
export class AppointmentFullCalendarComponent implements OnInit {
  calendar: Calendar;
  events=[
    { title: 'event 1', date: '2021-07-09'},
    { title: 'event 2', date: '2020-07-08'},
  ];

  constructor(private el: ElementRef) {
    this.calendar = new Calendar(el.nativeElement, {
      plugins: [dayGridPlugin, timeGrigPlugin, interactionPlugin], 
      editable: true, 
      defaultView: "timeGridWeek", 
      header: {
        left: 'prev,next today',
        center: 'title',
        right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
      },
      height: "auto", 
      dir: "ltr", 
      events: this.events, 
      locale: "vi", 
      firstDay: 1, 
      eventClick: this.handleEventClick.bind(this)
    });
    this.calendar.render();
    // for 7
    this.calendar.next();
    console.log(this.calendar.view.activeStart);
    console.log(this.calendar.view.currentStart);

  }

  ngOnInit() {
  }

  handleDateSelect(event) {

  }

  handleEventClick(event) {
    console.log(event);
  }
}
