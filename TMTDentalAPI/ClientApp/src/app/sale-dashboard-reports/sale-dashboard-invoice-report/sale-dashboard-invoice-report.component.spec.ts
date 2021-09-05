import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleDashboardInvoiceReportComponent } from './sale-dashboard-invoice-report.component';

describe('SaleDashboardInvoiceReportComponent', () => {
  let component: SaleDashboardInvoiceReportComponent;
  let fixture: ComponentFixture<SaleDashboardInvoiceReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleDashboardInvoiceReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleDashboardInvoiceReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
