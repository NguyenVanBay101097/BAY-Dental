import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CashBookTabCashBankComponent } from './cash-book-tab-cash-bank.component';

describe('CashBookTabCashBankComponent', () => {
  let component: CashBookTabCashBankComponent;
  let fixture: ComponentFixture<CashBookTabCashBankComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CashBookTabCashBankComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CashBookTabCashBankComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
