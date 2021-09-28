import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DayDashboardReportRevenueComponent } from './day-dashboard-report-revenue.component';

describe('DayDashboardReportRevenueComponent', () => {
  let component: DayDashboardReportRevenueComponent;
  let fixture: ComponentFixture<DayDashboardReportRevenueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DayDashboardReportRevenueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DayDashboardReportRevenueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
