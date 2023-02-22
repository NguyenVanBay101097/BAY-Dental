import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewAppointmentComponent } from './partner-overview-appointment.component';

describe('PartnerOverviewAppointmentComponent', () => {
  let component: PartnerOverviewAppointmentComponent;
  let fixture: ComponentFixture<PartnerOverviewAppointmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewAppointmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewAppointmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
