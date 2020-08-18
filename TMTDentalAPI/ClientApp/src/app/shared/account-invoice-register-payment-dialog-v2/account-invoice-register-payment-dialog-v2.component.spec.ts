import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceRegisterPaymentDialogV2Component } from './account-invoice-register-payment-dialog-v2.component';

describe('AccountInvoiceRegisterPaymentDialogV2Component', () => {
  let component: AccountInvoiceRegisterPaymentDialogV2Component;
  let fixture: ComponentFixture<AccountInvoiceRegisterPaymentDialogV2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceRegisterPaymentDialogV2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceRegisterPaymentDialogV2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
