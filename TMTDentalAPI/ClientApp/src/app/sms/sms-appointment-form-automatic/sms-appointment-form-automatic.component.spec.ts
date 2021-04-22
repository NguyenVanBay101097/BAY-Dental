import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsAppointmentFormAutomaticComponent } from './sms-appointment-form-automatic.component';

describe('SmsAppointmentFormAutomaticComponent', () => {
  let component: SmsAppointmentFormAutomaticComponent;
  let fixture: ComponentFixture<SmsAppointmentFormAutomaticComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsAppointmentFormAutomaticComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsAppointmentFormAutomaticComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
