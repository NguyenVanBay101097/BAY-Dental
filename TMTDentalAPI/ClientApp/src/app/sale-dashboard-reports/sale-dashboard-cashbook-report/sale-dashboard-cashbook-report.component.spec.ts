import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleDashboardCashbookReportComponent } from './sale-dashboard-cashbook-report.component';

describe('SaleDashboardCashbookReportComponent', () => {
  let component: SaleDashboardCashbookReportComponent;
  let fixture: ComponentFixture<SaleDashboardCashbookReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleDashboardCashbookReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleDashboardCashbookReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
