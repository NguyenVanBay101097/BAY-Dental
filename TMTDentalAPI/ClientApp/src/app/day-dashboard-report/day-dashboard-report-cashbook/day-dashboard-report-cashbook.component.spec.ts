import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DayDashboardReportCashbookComponent } from './day-dashboard-report-cashbook.component';

describe('DayDashboardReportCashbookComponent', () => {
  let component: DayDashboardReportCashbookComponent;
  let fixture: ComponentFixture<DayDashboardReportCashbookComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DayDashboardReportCashbookComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DayDashboardReportCashbookComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
