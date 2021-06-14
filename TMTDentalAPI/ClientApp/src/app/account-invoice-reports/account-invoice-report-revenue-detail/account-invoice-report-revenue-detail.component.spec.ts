import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceReportRevenueDetailComponent } from './account-invoice-report-revenue-detail.component';

describe('AccountInvoiceReportRevenueDetailComponent', () => {
  let component: AccountInvoiceReportRevenueDetailComponent;
  let fixture: ComponentFixture<AccountInvoiceReportRevenueDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceReportRevenueDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceReportRevenueDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
