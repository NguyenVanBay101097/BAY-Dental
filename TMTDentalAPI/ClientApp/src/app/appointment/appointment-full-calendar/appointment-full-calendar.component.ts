import { Component, ElementRef, Input, IterableDiffers, OnInit } from '@angular/core';
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
  @Input() events;
  eventDiffer: any;

  constructor(private el: ElementRef, differs: IterableDiffers) {
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
    // this.calendar.next();
    console.log(this.calendar.view.activeStart);
    console.log(this.calendar.view.activeEnd);
    console.log(this.calendar.view.currentStart);
    this.eventDiffer = differs.find([]).create(null);
  }

 
  ngDoCheck() {
    const eventChanges = this.eventDiffer.diff(this.events);
    if (this.calendar && eventChanges) {
      this.calendar.removeAllEventSources();
      if (this.events) {
        this.calendar.addEventSource(this.events);
      }
    }
  }

  ngOnInit() {
  }

  handleDateSelect(event) {

  }

  handleEventClick(event) {
    console.log(event);
  }
}
