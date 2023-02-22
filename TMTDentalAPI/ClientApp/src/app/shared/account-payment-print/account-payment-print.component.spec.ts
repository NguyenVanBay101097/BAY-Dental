import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountPaymentPrintComponent } from './account-payment-print.component';

describe('AccountPaymentPrintComponent', () => {
  let component: AccountPaymentPrintComponent;
  let fixture: ComponentFixture<AccountPaymentPrintComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountPaymentPrintComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountPaymentPrintComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
