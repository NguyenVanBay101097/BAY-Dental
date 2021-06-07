import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceReportRevenueServiceComponent } from './account-invoice-report-revenue-service.component';

describe('AccountInvoiceReportRevenueServiceComponent', () => {
  let component: AccountInvoiceReportRevenueServiceComponent;
  let fixture: ComponentFixture<AccountInvoiceReportRevenueServiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceReportRevenueServiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceReportRevenueServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
