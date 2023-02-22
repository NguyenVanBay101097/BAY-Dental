import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountReportGeneralLedgerMoveLineComponent } from './account-report-general-ledger-move-line.component';

describe('AccountReportGeneralLedgerMoveLineComponent', () => {
  let component: AccountReportGeneralLedgerMoveLineComponent;
  let fixture: ComponentFixture<AccountReportGeneralLedgerMoveLineComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountReportGeneralLedgerMoveLineComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountReportGeneralLedgerMoveLineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
