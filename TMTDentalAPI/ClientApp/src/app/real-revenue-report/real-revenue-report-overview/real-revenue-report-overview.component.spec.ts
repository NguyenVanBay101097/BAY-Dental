import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RealRevenueReportOverviewComponent } from './real-revenue-report-overview.component';

describe('RealRevenueReportOverviewComponent', () => {
  let component: RealRevenueReportOverviewComponent;
  let fixture: ComponentFixture<RealRevenueReportOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RealRevenueReportOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RealRevenueReportOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
