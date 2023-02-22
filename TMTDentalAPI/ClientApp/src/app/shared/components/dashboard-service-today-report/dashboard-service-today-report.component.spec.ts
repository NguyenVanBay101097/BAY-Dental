import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardServiceTodayReportComponent } from './dashboard-service-today-report.component';

describe('DashboardServiceTodayReportComponent', () => {
  let component: DashboardServiceTodayReportComponent;
  let fixture: ComponentFixture<DashboardServiceTodayReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardServiceTodayReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardServiceTodayReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
