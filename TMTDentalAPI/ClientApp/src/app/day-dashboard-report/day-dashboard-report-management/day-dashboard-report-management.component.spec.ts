import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DayDashboardReportManagementComponent } from './day-dashboard-report-management.component';

describe('DayDashboardReportManagementComponent', () => {
  let component: DayDashboardReportManagementComponent;
  let fixture: ComponentFixture<DayDashboardReportManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DayDashboardReportManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DayDashboardReportManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
