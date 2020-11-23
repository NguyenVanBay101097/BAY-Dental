import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleDashboardReportChartFlowMonthComponent } from './sale-dashboard-report-chart-flow-month.component';

describe('SaleDashboardReportChartFlowMonthComponent', () => {
  let component: SaleDashboardReportChartFlowMonthComponent;
  let fixture: ComponentFixture<SaleDashboardReportChartFlowMonthComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleDashboardReportChartFlowMonthComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleDashboardReportChartFlowMonthComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
