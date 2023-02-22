import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardPartnerCustomerReportComponent } from './dashboard-partner-customer-report.component';

describe('DashboardPartnerCustomerReportComponent', () => {
  let component: DashboardPartnerCustomerReportComponent;
  let fixture: ComponentFixture<DashboardPartnerCustomerReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardPartnerCustomerReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardPartnerCustomerReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
