import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RealRevenueReportItemDetailComponent } from './real-revenue-report-item-detail.component';

describe('RealRevenueReportItemDetailComponent', () => {
  let component: RealRevenueReportItemDetailComponent;
  let fixture: ComponentFixture<RealRevenueReportItemDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RealRevenueReportItemDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RealRevenueReportItemDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
