import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DayDashboardReportServiceComponent } from './day-dashboard-report-service.component';

describe('DayDashboardReportServiceComponent', () => {
  let component: DayDashboardReportServiceComponent;
  let fixture: ComponentFixture<DayDashboardReportServiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DayDashboardReportServiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DayDashboardReportServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
