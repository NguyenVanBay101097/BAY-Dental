import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceReportRevenueEmployeeComponent } from './account-invoice-report-revenue-employee.component';

describe('AccountInvoiceReportRevenueEmployeeComponent', () => {
  let component: AccountInvoiceReportRevenueEmployeeComponent;
  let fixture: ComponentFixture<AccountInvoiceReportRevenueEmployeeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceReportRevenueEmployeeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceReportRevenueEmployeeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
