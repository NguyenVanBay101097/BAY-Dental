import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceReportRevenuePartnerComponent } from './account-invoice-report-revenue-partner.component';

describe('AccountInvoiceReportRevenuePartnerComponent', () => {
  let component: AccountInvoiceReportRevenuePartnerComponent;
  let fixture: ComponentFixture<AccountInvoiceReportRevenuePartnerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceReportRevenuePartnerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceReportRevenuePartnerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
