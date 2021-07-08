import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentFullCalendarComponent } from './appointment-full-calendar.component';

describe('AppointmentFullCalendarComponent', () => {
  let component: AppointmentFullCalendarComponent;
  let fixture: ComponentFixture<AppointmentFullCalendarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentFullCalendarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentFullCalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
