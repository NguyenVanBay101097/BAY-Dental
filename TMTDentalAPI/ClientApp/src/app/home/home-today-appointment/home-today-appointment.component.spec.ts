import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeTodayAppointmentComponent } from './home-today-appointment.component';

describe('HomeTodayAppointmentComponent', () => {
  let component: HomeTodayAppointmentComponent;
  let fixture: ComponentFixture<HomeTodayAppointmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HomeTodayAppointmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HomeTodayAppointmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
