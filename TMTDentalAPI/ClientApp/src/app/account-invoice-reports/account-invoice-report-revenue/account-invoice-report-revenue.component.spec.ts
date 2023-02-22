import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceReportRevenueComponent } from './account-invoice-report-revenue.component';

describe('AccountInvoiceReportRevenueComponent', () => {
  let component: AccountInvoiceReportRevenueComponent;
  let fixture: ComponentFixture<AccountInvoiceReportRevenueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceReportRevenueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceReportRevenueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
