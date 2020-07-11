import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterAppointmentDayComponent } from './audience-filter-appointment-day.component';

describe('AudienceFilterAppointmentDayComponent', () => {
  let component: AudienceFilterAppointmentDayComponent;
  let fixture: ComponentFixture<AudienceFilterAppointmentDayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterAppointmentDayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterAppointmentDayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
