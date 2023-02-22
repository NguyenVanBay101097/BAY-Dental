import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardCashBankReportComponent } from './dashboard-cash-bank-report.component';

describe('DashboardCashBankReportComponent', () => {
  let component: DashboardCashBankReportComponent;
  let fixture: ComponentFixture<DashboardCashBankReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardCashBankReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardCashBankReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
