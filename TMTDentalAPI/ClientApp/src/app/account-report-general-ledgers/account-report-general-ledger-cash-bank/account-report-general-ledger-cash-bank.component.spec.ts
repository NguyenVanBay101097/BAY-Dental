import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountReportGeneralLedgerCashBankComponent } from './account-report-general-ledger-cash-bank.component';

describe('AccountReportGeneralLedgerCashBankComponent', () => {
  let component: AccountReportGeneralLedgerCashBankComponent;
  let fixture: ComponentFixture<AccountReportGeneralLedgerCashBankComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountReportGeneralLedgerCashBankComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountReportGeneralLedgerCashBankComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
