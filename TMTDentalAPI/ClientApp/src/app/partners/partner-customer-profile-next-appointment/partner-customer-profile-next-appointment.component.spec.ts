import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerProfileNextAppointmentComponent } from './partner-customer-profile-next-appointment.component';

describe('PartnerCustomerProfileNextAppointmentComponent', () => {
  let component: PartnerCustomerProfileNextAppointmentComponent;
  let fixture: ComponentFixture<PartnerCustomerProfileNextAppointmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PartnerCustomerProfileNextAppointmentComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerProfileNextAppointmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
