import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsAppointmentFormComponent } from './sms-appointment-form.component';

describe('SmsAppointmentFormComponent', () => {
  let component: SmsAppointmentFormComponent;
  let fixture: ComponentFixture<SmsAppointmentFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsAppointmentFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsAppointmentFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
