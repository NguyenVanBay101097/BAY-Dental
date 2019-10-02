import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceReportByTimeComponent } from './account-invoice-report-by-time.component';

describe('AccountInvoiceReportByTimeComponent', () => {
  let component: AccountInvoiceReportByTimeComponent;
  let fixture: ComponentFixture<AccountInvoiceReportByTimeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceReportByTimeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceReportByTimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
