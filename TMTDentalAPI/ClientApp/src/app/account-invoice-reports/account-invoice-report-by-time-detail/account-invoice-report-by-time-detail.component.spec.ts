import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceReportByTimeDetailComponent } from './account-invoice-report-by-time-detail.component';

describe('AccountInvoiceReportByTimeDetailComponent', () => {
  let component: AccountInvoiceReportByTimeDetailComponent;
  let fixture: ComponentFixture<AccountInvoiceReportByTimeDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceReportByTimeDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceReportByTimeDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
