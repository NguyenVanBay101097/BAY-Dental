import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HeaderAppointmentComponent } from './header-appointment.component';

describe('HeaderAppointmentComponent', () => {
  let component: HeaderAppointmentComponent;
  let fixture: ComponentFixture<HeaderAppointmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HeaderAppointmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HeaderAppointmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
