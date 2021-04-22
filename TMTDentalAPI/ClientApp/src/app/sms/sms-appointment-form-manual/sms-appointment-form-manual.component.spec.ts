import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsAppointmentFormManualComponent } from './sms-appointment-form-manual.component';

describe('SmsAppointmentFormManualComponent', () => {
  let component: SmsAppointmentFormManualComponent;
  let fixture: ComponentFixture<SmsAppointmentFormManualComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsAppointmentFormManualComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsAppointmentFormManualComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
