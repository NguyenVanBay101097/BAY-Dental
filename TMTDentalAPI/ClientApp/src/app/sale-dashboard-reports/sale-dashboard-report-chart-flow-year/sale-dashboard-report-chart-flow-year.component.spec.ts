import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleDashboardReportChartFlowYearComponent } from './sale-dashboard-report-chart-flow-year.component';

describe('SaleDashboardReportChartFlowYearComponent', () => {
  let component: SaleDashboardReportChartFlowYearComponent;
  let fixture: ComponentFixture<SaleDashboardReportChartFlowYearComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleDashboardReportChartFlowYearComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleDashboardReportChartFlowYearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
