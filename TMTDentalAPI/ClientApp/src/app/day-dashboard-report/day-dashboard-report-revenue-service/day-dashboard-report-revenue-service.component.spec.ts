import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DayDashboardReportRevenueServiceComponent } from './day-dashboard-report-revenue-service.component';

describe('DayDashboardReportRevenueServiceComponent', () => {
  let component: DayDashboardReportRevenueServiceComponent;
  let fixture: ComponentFixture<DayDashboardReportRevenueServiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DayDashboardReportRevenueServiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DayDashboardReportRevenueServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
