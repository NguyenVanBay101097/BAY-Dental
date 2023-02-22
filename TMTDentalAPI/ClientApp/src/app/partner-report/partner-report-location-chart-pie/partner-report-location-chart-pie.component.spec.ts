import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportLocationChartPieComponent } from './partner-report-location-chart-pie.component';

describe('PartnerReportLocationChartPieComponent', () => {
  let component: PartnerReportLocationChartPieComponent;
  let fixture: ComponentFixture<PartnerReportLocationChartPieComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportLocationChartPieComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportLocationChartPieComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
