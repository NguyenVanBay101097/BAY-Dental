import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardSaleReportComponent } from './dashboard-sale-report.component';

describe('DashboardSaleReportComponent', () => {
  let component: DashboardSaleReportComponent;
  let fixture: ComponentFixture<DashboardSaleReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardSaleReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardSaleReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
