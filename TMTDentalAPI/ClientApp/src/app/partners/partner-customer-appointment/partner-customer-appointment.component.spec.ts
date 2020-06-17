import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerAppointmentComponent } from './partner-customer-appointment.component';

describe('PartnerCustomerAppointmentComponent', () => {
  let component: PartnerCustomerAppointmentComponent;
  let fixture: ComponentFixture<PartnerCustomerAppointmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerAppointmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerAppointmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
