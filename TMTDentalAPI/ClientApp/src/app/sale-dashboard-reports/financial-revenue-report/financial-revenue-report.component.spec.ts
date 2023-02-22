import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FinancialRevenueReportComponent } from './financial-revenue-report.component';

describe('FinancialRevenueReportComponent', () => {
  let component: FinancialRevenueReportComponent;
  let fixture: ComponentFixture<FinancialRevenueReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FinancialRevenueReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinancialRevenueReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
