import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardRevenueTodayReportComponent } from './dashboard-revenue-today-report.component';

describe('DashboardRevenueTodayReportComponent', () => {
  let component: DashboardRevenueTodayReportComponent;
  let fixture: ComponentFixture<DashboardRevenueTodayReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardRevenueTodayReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardRevenueTodayReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
