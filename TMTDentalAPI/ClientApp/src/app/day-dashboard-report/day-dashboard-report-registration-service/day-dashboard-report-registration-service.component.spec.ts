import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DayDashboardReportRegistrationServiceComponent } from './day-dashboard-report-registration-service.component';

describe('DayDashboardReportRegistrationServiceComponent', () => {
  let component: DayDashboardReportRegistrationServiceComponent;
  let fixture: ComponentFixture<DayDashboardReportRegistrationServiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DayDashboardReportRegistrationServiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DayDashboardReportRegistrationServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
